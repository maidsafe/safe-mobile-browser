// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

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
                    var currentWebView = webView as HybridWebViewRenderer;
                    currentWebView.SetCurrentPageVersion(safeResponse.CurrentNrsVersion);
                    currentWebView.SetLatestPageVersion(safeResponse.LatestNrsVersion);
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
