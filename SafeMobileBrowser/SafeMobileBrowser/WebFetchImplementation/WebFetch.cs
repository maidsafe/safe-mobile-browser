using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MimeMapping;
using SafeApp;
using SafeApp.Utilities;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using Xamarin.Forms;

namespace SafeMobileBrowser.WebFetchImplementation
{
    public class WebFetch
    {
        private readonly Session _session;

        public WebFetch(Session session)
        {
            _session = session;
        }

        public async Task<WebFetchResponse> FetchAsync(string url, WebFetchOptions options = null)
        {
            try
            {
                var response = new WebFetchResponse();
                var (serviceMd, parsedPath) = await WebFetchHelper(url);
                var path = CreateFinalPath(parsedPath);
                var file = await TryDifferentPaths(serviceMd, path);
                var fileData = await ReadContentFromFile(serviceMd, file, options);
                response.Data = fileData.Item1.ToArray();
                response.MimeType = file.MimeType;
                response.Headers.Add("Content-Type", file.MimeType);
                if (options == null)
                    return response;

                response.Headers.Add("Content-Range", $"bytes {fileData.Item2}-{fileData.Item3}/{fileData.Item4}");
                response.Headers.Add("Content-Length", $"{fileData.Item4}");
                return response;
            }
            catch (WebFetchException ex)
            {
                Logger.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Generate path to fetch resource from the SAFE Network.
        /// </summary>
        /// <param name="path">path from URL</param>
        /// <returns>Final path to get resource</returns>
        private static string CreateFinalPath(string path)
        {
            string finalPath;
            if (string.IsNullOrWhiteSpace(path))
                finalPath = "/index.html";
            else if (path.StartsWith("/") && path.EndsWith("/"))
                finalPath = path + "index.html";
            else
                finalPath = path;

            return finalPath;
        }

        /// <summary>
        /// WebFetch helper function to fetch a resource from the network.
        /// </summary>
        /// <param name="url">Requested resource URL</param>
        /// <returns>MdInfo and Path</returns>
        public async Task<(MDataInfo, string)> WebFetchHelper(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new WebFetchException(WebFetchConstants.NullUrl, WebFetchConstants.NullUrlMessage);

            if (
                Device.RuntimePlatform == Device.Android
                && Regex.IsMatch(url.Replace("https://", string.Empty).Replace("/", string.Empty), @"^\d+$"))
            {
                url = url.Replace("https://", "https://www.");
            }

            var parsedUrl = new Uri(url);
            var hostname = parsedUrl.Host;

            var hostParts = hostname.Split('.').ToList();
            var publicName = hostParts.Last();
            hostParts.Remove(publicName);
            var serviceName = string.Join(".", hostParts);

            // let's decompose and normalise the path
            var path = parsedUrl.AbsolutePath == "/" ? string.Empty : parsedUrl.AbsolutePath;
            var parsedPath = string.IsNullOrEmpty(path) ? string.Empty : System.Web.HttpUtility.UrlDecode(path);

            var mdataInfo = await GetContainerFromPublicId(publicName, serviceName);
            return (mdataInfo, parsedPath);
        }

        /// <summary>
        /// Helper function to fetch the Container
        /// from a public ID and service name provided
        /// </summary>
        /// <param name="pubName">Requested public name</param>
        /// <param name="serviceName">Requested service name</param>
        /// <returns>MDataInfo for the service MData</returns>
        public async Task<MDataInfo> GetContainerFromPublicId(string pubName, string serviceName)
        {
            (List<byte>, ulong) serviceInfo;
            try
            {
                // Fetch mdata entry value for service
                var address = await SafeApp.Misc.Crypto.Sha3HashAsync(pubName.ToUtfBytes());
                var servicesContainer = new MDataInfo
                {
                    TypeTag = WebFetchConstants.DnsTagType,
                    Name = address.ToArray()
                };
                serviceInfo = await _session.MData.GetValueAsync(
                    servicesContainer,
                    (string.IsNullOrWhiteSpace(serviceName) ? WebFetchConstants.DefaultService : serviceName).ToUtfBytes());
            }
            catch (FfiException ex)
            {
                Logger.Error(ex);
                switch (ex.ErrorCode)
                {
                    // there is no container stored at the location
                    case WebFetchConstants.NoSuchData:
                        throw new WebFetchException(WebFetchConstants.NoSuchData, WebFetchConstants.NoSuchDataMessage);
                    case WebFetchConstants.NoSuchEntry:
                        throw new WebFetchException(WebFetchConstants.NoSuchEntry, WebFetchConstants.NoSuchEntryMessage);
                    default:
                        throw;
                }
            }

            // the matching service name was soft-deleted
            if (serviceInfo.Item1.Count == 0)
            {
                throw new WebFetchException(WebFetchConstants.ServiceNotFound, WebFetchConstants.ServiceNotFoundMessage);
            }

            // Try parsing the serive mdinfo
            MDataInfo serviceMd;
            try
            {
                serviceMd = await _session.MDataInfoActions.DeserialiseAsync(serviceInfo.Item1);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                serviceMd = new MDataInfo
                {
                    TypeTag = WebFetchConstants.WwwTagType,
                    Name = serviceInfo.Item1.ToArray()
                };
            }
            return serviceMd;
        }

        /// <summary>
        /// Helper function to try private different paths private to find and
        /// fetch the index file from a web site container.
        /// </summary>
        /// <param name="fileMdInfo">File MdInfo</param>
        /// <param name="initialPath">Path</param>
        /// <returns>WebFile</returns>
        public async Task<WebFile> TryDifferentPaths(MDataInfo fileMdInfo, string initialPath)
        {
            void HandleNfsFetchException(FfiException exception)
            {
                Logger.Error(exception);
                if (exception.ErrorCode != WebFetchConstants.FileNotFound)
                    throw exception;
            }

            File? file = null;
            var filePath = string.Empty;
            try
            {
                filePath = initialPath;
                (file, _) = await _session.NFS.DirFetchFileAsync(fileMdInfo, filePath);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(FfiException))
                    HandleNfsFetchException((FfiException)ex);
            }

            if (file == null && initialPath.StartsWith("/"))
            {
                try
                {
                    filePath = initialPath.Substring(1, initialPath.Length - 1);
                    (file, _) = await _session.NFS.DirFetchFileAsync(fileMdInfo, filePath);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(FfiException))
                        HandleNfsFetchException((FfiException)ex);
                }
            }

            if (file == null && initialPath.StartsWith("/"))
            {
                try
                {
                    filePath = $"{initialPath}/{WebFetchConstants.IndexFileName}";
                    (file, _) = await _session.NFS.DirFetchFileAsync(fileMdInfo, filePath);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(FfiException))
                        HandleNfsFetchException((FfiException)ex);
                }
            }

            if (file == null)
            {
                try
                {
                    filePath = $"{initialPath}/{WebFetchConstants.IndexFileName}";
                    filePath = filePath.Substring(1, filePath.Length - 1);
                    (file, _) = await _session.NFS.DirFetchFileAsync(fileMdInfo, filePath);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(FfiException))
                        HandleNfsFetchException((FfiException)ex);

                    throw new WebFetchException(WebFetchConstants.FileNotFound, WebFetchConstants.FileNotFoundMessage);
                }
            }

            var extension = filePath.Substring(filePath.LastIndexOf('.') + 1);
            var mimeType = MimeUtility.GetMimeMapping(extension);
            return new WebFile { File = file.Value, MimeType = mimeType };
        }

        /// <summary>
        /// Helper function to read the file's content, and return an
        /// http compliant response based on the mime-type and options provided
        /// </summary>
        /// <param name="fileMdInfo">Opened file's MdInfo</param>
        /// <param name="openedFile">Open file</param>
        /// <param name="options">WebFetch options (ex. range to read)</param>
        /// <returns>Data as List &lt; byte &gt;, startIndex, endIndex, and size</returns>
        public async Task<(List<byte>, ulong, ulong, ulong)> ReadContentFromFile(
            MDataInfo fileMdInfo,
            WebFile openedFile,
            WebFetchOptions options = null)
        {
            try
            {
                var fileHandle = await _session.NFS.FileOpenAsync(fileMdInfo, openedFile.File, SafeApp.Misc.NFS.OpenMode.Read);
                var fileSize = await _session.NFS.FileSizeAsync(fileHandle);

                if (options == null)
                {
                    var fileData = await _session.NFS.FileReadAsync(fileHandle, 0, fileSize - 1);
                    return (fileData, 0, fileSize - 1, fileSize);
                }
                else
                {
                    var partStartIndex = options.Range[0].Start > 0 ? options.Range[0].Start : 0;
                    ulong partEndIndex;

                    if (options.Range[0].End > 0)
                    {
                        partEndIndex = options.Range[0].End;
                    }
                    else
                    {
                        partEndIndex = fileSize - 1;
                    }

                    var partSize = partEndIndex - partStartIndex + 1;
                    var fileData = await _session.NFS.FileReadAsync(fileHandle, partStartIndex, partSize);
                    return (fileData, partStartIndex, partEndIndex, partSize);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return (null, 0, 0, 0);
        }
    }
}
