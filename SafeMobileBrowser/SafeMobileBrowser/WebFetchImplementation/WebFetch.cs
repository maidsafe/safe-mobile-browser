// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafeApp;
using SafeApp.Core;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;

namespace SafeMobileBrowser.WebFetchImplementation
{
    public class WebFetch
    {
        private static readonly string _filesContainerText = "FILESLIST";
        private static readonly string _defaultPage = "index.html";

        public async Task<WebFetchResponse> FetchAsync(string url, WebFetchOptions options = null)
        {
            try
            {
                var response = new WebFetchResponse();
                var fetchUrl = url.Replace("https://", "safe://");
                var data = await App
                                 .AppSession
                                 .Fetch
                                 .FetchAsync(fetchUrl);

                if (data is FilesContainer filesContainer)
                {
                    ulong nrsContainerVersion = filesContainer
                                                .ResolvedFrom
                                                .Version;
                    if (filesContainer.FilesMap.Files != null && filesContainer.FilesMap.Files.Count > 0)
                    {
                        var indexFileInfo = filesContainer
                                            .FilesMap
                                            .Files
                                            .FirstOrDefault(
                                                file => file.FileName == $"/{_defaultPage}" ||
                                                file.FileName == $"{_defaultPage}");

                        if (!indexFileInfo.Equals(default(FileInfo)))
                        {
                            var indexFileLink = indexFileInfo
                                                .FileMetaData
                                                .FirstOrDefault(meta => meta.MetaDataKey == "link")
                                                .MetaDataValue;

                            if (!string.IsNullOrEmpty(indexFileLink))
                            {
                                var fetchResponse = await FetchAsync(indexFileLink);
                                fetchResponse.CurrentNrsVersion = nrsContainerVersion;
                                fetchResponse.LatestNrsVersion = await GetLatestVersionAsync(fetchUrl);
                                return fetchResponse;
                            }
                        }
                        else
                        {
                            var content = await CreateFilesContainerPageAsync(filesContainer.FilesMap.Files);
                            response.LatestNrsVersion = await GetLatestVersionAsync(fetchUrl);
                            response.CurrentNrsVersion = filesContainer.ResolvedFrom.Version;
                            response.Data = content.ToUtfBytes();
                            response.MimeType = "text/html";
                            response.Headers.Add("Content-Type", "text/html");
                            return response;
                        }
                    }
                }
                else if (data is PublishedImmutableData fetchedData)
                {
                    response.LatestNrsVersion = await GetLatestVersionAsync(fetchUrl);
                    response.CurrentNrsVersion = fetchedData.ResolvedFrom.Version;
                    response.Data = fetchedData.Data;
                    response.MimeType = fetchedData.MediaType;
                    response.Headers.Add("Content-Type", fetchedData.MediaType);
                    return response;
                }
                else if (data is SafeDataFetchFailed fetchFailed)
                {
                    throw new WebFetchException(
                        WebFetchConstants.NoSuchData,
                        WebFetchConstants.NoSuchDataMessage);
                }

                throw new WebFetchException(
                        WebFetchConstants.NoSuchPublicName,
                        WebFetchConstants.NoSuchPublicNameMessage);
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

        private async Task<string> CreateFilesContainerPageAsync(List<FileInfo> files)
        {
            var htmlString = await FileHelper.ReadAssetFileContentAsync("FilesContainer.html");
            var filesContainerContent = new StringBuilder();
            foreach (var file in files)
            {
                filesContainerContent.Append($"<li>{file.FileName}</li>");
            }
            int index = htmlString.IndexOf(_filesContainerText, StringComparison.Ordinal);
            return htmlString
                .Remove(index, _filesContainerText.Length)
                .Insert(index, filesContainerContent.ToString());
        }

        public async Task<ulong> GetLatestVersionAsync(string url)
        {
            try
            {
                var fetchUrl = url.Replace("https://", "safe://").TrimEnd('/');
                if (url.Contains("?v="))
                {
                    var versionTextIndex = url.LastIndexOf("?v=", StringComparison.Ordinal);
                    fetchUrl = url.Replace("https://", "safe://").Substring(0, versionTextIndex);
                }

                var data = await App
                                 .AppSession
                                 .Fetch
                                 .InspectAsync(fetchUrl);

                if (data is FilesContainer filesContainer)
                {
                    return filesContainer.ResolvedFrom.Version;
                }
                else if (data is PublishedImmutableData fetchedData)
                {
                    return fetchedData.ResolvedFrom.Version;
                }

                throw new WebFetchException(
                        WebFetchConstants.NoSuchPublicName,
                        WebFetchConstants.NoSuchPublicNameMessage);
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
    }
}
