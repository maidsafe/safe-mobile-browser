using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SafeApp;
using SafeApp.Core;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;

namespace SafeMobileBrowser.WebFetchImplementation
{
    public class FileMapItem
    {
        public DateTimeOffset Created { get; set; }

        public string Link { get; set; }

        public DateTimeOffset Modified { get; set; }

        public long Size { get; set; }

        public string Type { get; set; }
    }

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
                var fetchUrl = url.Replace("https://", "safe://");
                var data = await _session.Fetch.FetchAsync(fetchUrl);

                if (data is FilesContainer filesContainer)
                {
                    ulong nrsContainerVersion = filesContainer.ResolvedFrom.Version;
                    if (!string.IsNullOrWhiteSpace(filesContainer.FilesMap))
                    {
                        var filesMap = JsonConvert.DeserializeObject<JObject>(filesContainer.FilesMap);
                        var indexFileItem = filesMap["/index.html"] as JObject;
                        if (indexFileItem != null)
                        {
                            var indexFileLink = (string)indexFileItem["link"];
                            if (!string.IsNullOrWhiteSpace(indexFileLink))
                            {
                                var fetchResponse = await FetchAsync(indexFileLink);
                                fetchResponse.CurrentNrsVersion = nrsContainerVersion;
                                fetchResponse.LatestNrsVersion = await GetLatestVersionAsync(fetchUrl);
                                return fetchResponse;
                            }
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

                throw new WebFetchException(
                        WebFetchConstants.NoSuchPublicName,
                        WebFetchConstants.NoSuchPublicNameMessage);

                // else if (data is SafeDataFetchFailed)
                // {
                //    throw new WebFetchException(
                //        WebFetchConstants.NoSuchPublicName,
                //        WebFetchConstants.NoSuchPublicNameMessage);
                // }
                // else if (data is Wallet || data is SafeKey)
                // {
                //    throw new WebFetchException(
                //        WebFetchConstants.NoSuchPublicName,
                //        WebFetchConstants.NoSuchPublicNameMessage);
                // }
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

        public async Task<ulong> GetLatestVersionAsync(string url)
        {
            try
            {
                var fetchUrl = url.Replace("https://", "safe://").TrimEnd('/');
                if (url.Contains("?v="))
                {
                    var versionTextIndex = url.LastIndexOf("?v=");
                    fetchUrl = url.Replace("https://", "safe://").Substring(0, versionTextIndex);
                }

                var data = await _session.Fetch.InspectAsync(fetchUrl);

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
