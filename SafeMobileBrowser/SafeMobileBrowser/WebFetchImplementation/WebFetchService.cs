using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SafeMobileBrowser.Models;

namespace SafeMobileBrowser.WebFetchImplementation
{
    public class WebFetchService
    {
        private static readonly WebFetch webFetch = new WebFetch();

        public static async Task<WebFetchResponse> FetchResourceAsync(string url, WebFetchOptions options = null)
        {
            try
            {
                return await webFetch.FetchAsync(url, options);
            }
            catch (WebFetchException ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
    }
}
