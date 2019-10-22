using System;
using System.Threading.Tasks;
using SafeApp;
using SafeApp.Core;
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
                var data = await _session.Fetch.FetchAsync(url);
                var safedatatype = data.GetType();
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("type", safedatatype.ToString(), "ok");
                if (data is PublishedImmutableData)
                {
                    var fetchedData = (PublishedImmutableData)data;
                    response.Data = fetchedData.Data;
                    response.MimeType = fetchedData.MediaType;
                    response.Headers.Add("Content-Type", fetchedData.MediaType);
                    return response;
                }

                if (data is FilesContainer)
                {
                    System.Diagnostics.Debug.WriteLine(((FilesContainer)data).FilesMap);
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
