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

                if (data is SafeDataFetchFailed)
                {
                    throw new WebFetchException(
                        WebFetchConstants.NoSuchPublicName,
                        WebFetchConstants.NoSuchPublicNameMessage);
                }

                if (data is PublishedImmutableData fetchedData)
                {
                    response.Data = fetchedData.Data;
                    response.MimeType = fetchedData.MediaType;
                    response.Headers.Add("Content-Type", fetchedData.MediaType);
                    return response;
                }

                if (data is FilesContainer filesContainer)
                {
                    if (!string.IsNullOrWhiteSpace(filesContainer.FilesMap))
                    {
                        var filesMap = JsonConvert.DeserializeObject<JObject>(filesContainer.FilesMap);
                        var indexFileItem = filesMap["/index.html"] as JObject;
                        if (indexFileItem != null)
                        {
                            var indexFileLink = (string)indexFileItem["link"];
                            if (!string.IsNullOrWhiteSpace(indexFileLink))
                                await FetchAsync(indexFileLink);
                        }
                    }
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
    }
}
