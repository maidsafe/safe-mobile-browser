using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeMapping;
using SafeApp;
using SafeApp.Utilities;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;

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
                var filedata = await ReadContentFromFile(serviceMd, file, options);
                response.Data = filedata.Item1.ToArray();
                response.MimeType = file.MimeType;
                response.Headers.Add("Content-Type", file.MimeType);
                if (options != null)
                {
                    response.Headers.Add("Content-Range", $"bytes {filedata.Item2}-{filedata.Item3}/{filedata.Item4}");
                    response.Headers.Add("Content-Length", $"{filedata.Item4}");
                }
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

            Logger.Info($"Input path: {path}");
            Logger.Info($"final path: {finalPath}");

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

            var parsedUrl = new Uri(url);
            var hostname = parsedUrl.Host;

            var hostparts = hostname.Split('.').ToList();
            var publicName = hostparts.Last();
            hostparts.Remove(publicName);
            var serviceName = string.Join(".", hostparts);

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
            (List<byte>, ulong) serviceInfo = (default(List<byte>), default(ulong));
            try
            {
                // Fetch mdata entry value for service
                var address = await SafeApp.Misc.Crypto.Sha3HashAsync(pubName.ToUtfBytes());
                MDataInfo servicesContainer = new MDataInfo
                {
                    TypeTag = WebFetchConstants.DNSTagType,
                    Name = address.ToArray()
                };
                serviceInfo = await _session.MData.GetValueAsync(
                    servicesContainer,
                    (string.IsNullOrWhiteSpace(serviceName) ? WebFetchConstants.DefaultSerive : serviceName).ToUtfBytes());
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
                    TypeTag = WebFetchConstants.WWWTagType,
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
            void HandleNFSFetchException(FfiException exception)
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
                    HandleNFSFetchException((FfiException)ex);
            }

            if (file == null && initialPath.StartsWith("/"))
            {
                try
                {
                    filePath = initialPath.Replace("/", string.Empty);
                    (file, _) = await _session.NFS.DirFetchFileAsync(fileMdInfo, filePath);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(FfiException))
                        HandleNFSFetchException((FfiException)ex);
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
                        HandleNFSFetchException((FfiException)ex);
                }
            }
            if (file == null)
            {
                try
                {
                    filePath = $"{initialPath}/{WebFetchConstants.IndexFileName}".Replace("/", string.Empty);
                    (file, _) = await _session.NFS.DirFetchFileAsync(fileMdInfo, filePath);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(FfiException))
                        HandleNFSFetchException((FfiException)ex);

                    throw new WebFetchException(WebFetchConstants.FileNotFound, WebFetchConstants.FileNotFoundMessage);
                }
            }

            string extension = filePath.Substring(filePath.LastIndexOf('.') + 1);
            string mimeType = MimeUtility.GetMimeMapping(extension);
            return new WebFile { File = file.Value, MimeType = mimeType };
        }

        /// <summary>
        /// Helper function to read the file's content, and return an
        /// http compliant response based on the mime-type and options provided
        /// </summary>
        /// <param name="fileMdInfo">Opened file's MdInfo</param>
        /// <param name="openedFile">Open file</param>
        /// <param name="options">WebFetch options (ex. range to read)</param>
        /// <returns>Data as List<byte>, startIndex, endIndex, and size</returns>
        public async Task<(List<byte>, ulong, ulong, ulong)> ReadContentFromFile(
            MDataInfo fileMdInfo,
            WebFile openedFile,
            WebFetchOptions options = null)
        {
            try
            {
                var filehandle = await _session.NFS.FileOpenAsync(fileMdInfo, openedFile.File, SafeApp.Misc.NFS.OpenMode.Read);
                var filesize = await _session.NFS.FileSizeAsync(filehandle);

                if (options == null)
                {
                    var filedata = await _session.NFS.FileReadAsync(filehandle, 0, filesize - 1);
                    return (filedata, 0, filesize - 1, filesize);
                }
                else
                {
                    var partStartIndex = (ulong)(options?.Range[0].Start > 0 ? options?.Range[0].Start : 0);
                    var partendIndex = 0UL;

                    if (options?.Range[0].End > 0)
                    {
                        partendIndex = (ulong)options?.Range[0].End;
                    }
                    else
                    {
                        partendIndex = filesize - 1;
                    }

                    var partSize = partendIndex - partStartIndex + 1;
                    var filedata = await _session.NFS.FileReadAsync(filehandle, partStartIndex, partSize);
                    return (filedata, partStartIndex, partendIndex, partSize);
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
