using System;
using System.Text;
using System.Threading.Tasks;
using SafeApp.Utilities;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using Xamarin.Essentials;

namespace SafeMobileBrowser.WebFetchImplementation
{
    public static class WebFetchService
    {
        private static readonly WebFetch webFetch = new WebFetch(App.AppSession);

        public static async Task<WebFetchResponse> FetchResourceAsync(string url, WebFetchOptions options = null)
        {
            try
            {
                return await webFetch.FetchAsync(url, options);
            }
            catch (WebFetchException ex)
            {
                Logger.Error(ex);

                var htmlString = await FileHelper.ReadAssetFileContentAsync("index.html");
                if (ex.ErrorCode == WebFetchConstants.NoSuchData ||
                ex.ErrorCode == WebFetchConstants.NoSuchEntry ||
                ex.ErrorCode == WebFetchConstants.NoSuchPublicName ||
                ex.ErrorCode == WebFetchConstants.NoSuchServiceName ||
                ex.ErrorCode == WebFetchConstants.FileNotFound)
                {
                    htmlString = ReplaceHtmlStringContent(htmlString, ErrorConstants.ErrorHeadingText, ErrorConstants.PageNotFound);
                    htmlString = ReplaceHtmlStringContent(htmlString, ErrorConstants.ErrorMessageText, ex.Message);
                }
                else
                {
                    htmlString = ReplaceHtmlStringContent(htmlString, ErrorConstants.ErrorHeadingText, ErrorConstants.ErrorOccured);
                    htmlString = ReplaceHtmlStringContent(htmlString, ErrorConstants.ErrorMessageText, ex.Message);
                }

                return new WebFetchResponse
                {
                    Data = Encoding.ASCII.GetBytes(htmlString),
                    MimeType = ErrorConstants.ErrorPageMimeType
                };
            }
            catch (FfiException ex)
            {
                Logger.Error(ex);

                var htmlString = await FileHelper.ReadAssetFileContentAsync("index.html");
                if (ex.ErrorCode == -11 &&
                    Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    htmlString = ReplaceHtmlStringContent(htmlString, ErrorConstants.ErrorHeadingText, ErrorConstants.NoInternetConnectionTitle);
                    htmlString = ReplaceHtmlStringContent(htmlString, ErrorConstants.ErrorMessageText, ErrorConstants.NoInternetConnectionMsg);
                }
                else
                {
                    htmlString = ReplaceHtmlStringContent(htmlString, ErrorConstants.ErrorHeadingText, ErrorConstants.ErrorOccured);
                    htmlString = ReplaceHtmlStringContent(htmlString, ErrorConstants.ErrorMessageText, ErrorConstants.UnableToFetchDataMsg);
                }
                return new WebFetchResponse
                {
                    Data = Encoding.ASCII.GetBytes(htmlString),
                    MimeType = ErrorConstants.ErrorPageMimeType
                };
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public static string ReplaceHtmlStringContent(string htmlString, string find, string replaceString)
        {
            int index = htmlString.IndexOf(find);
            return htmlString.Remove(index, find.Length).Insert(index, replaceString);
        }
    }
}
