using System;
using System.IO;
using Foundation;
using Nito.AsyncEx;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.WebFetchImplementation;
using WebKit;

namespace SafeMobileBrowser.iOS.ControlRenderers
{
    public class SafeSchemaHandler : NSObject, IWKUrlSchemeHandler
    {
        [Export("webView:startURLSchemeTask:")]
        public void StartUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
        {
            try
            {
                var urlToFetch = urlSchemeTask.Request.Url.ToString();
                if (!urlToFetch.Contains("favicon"))
                {
                    var safeResponse = AsyncContext.Run(() => WebFetchService.FetchResourceAsync(urlToFetch));
                    var stream = new MemoryStream(safeResponse.Data);
                    var response = new NSUrlResponse(urlSchemeTask.Request.Url, safeResponse.MimeType, (nint)stream.Length, null);
                    urlSchemeTask.DidReceiveResponse(response);
                    urlSchemeTask.DidReceiveData(NSData.FromStream(stream));
                    urlSchemeTask.DidFinish();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                if (ex.InnerException != null)
                {
                    var stream = new MemoryStream();
                    var response = new NSUrlResponse(urlSchemeTask.Request.Url, "text/html", 0, null);
                    urlSchemeTask.DidReceiveResponse(response);
                    urlSchemeTask.DidReceiveData(NSData.FromStream(stream));
                    urlSchemeTask.DidFinish();
                }
            }
        }

        [Export("webView:stopURLSchemeTask:")]
        public void StopUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
        {
            throw new NotImplementedException();
        }
    }
}
