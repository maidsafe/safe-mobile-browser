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
using System.IO;
using Android.Webkit;
using Nito.AsyncEx.Synchronous;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.WebFetchImplementation;
using Xamarin.Forms.Platform.Android;

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class HybridWebViewClient : FormsWebViewClient
    {
        private readonly WeakReference<HybridWebViewRenderer> _renderer;

        public HybridWebViewClient(HybridWebViewRenderer renderer)
            : base(renderer)
        {
            _renderer = new WeakReference<HybridWebViewRenderer>(renderer);
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            if (request.Url.ToString().Contains("safe"))
            {
                view.LoadUrl(request.Url.ToString().Replace("safe://", "https:"));
                return true;
            }

            return base.ShouldOverrideUrlLoading(view, request);
        }

        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            try
            {
                Logger.Info($"Requested Url: {request.Url.ToString()}");
                var urlToFetch = request.Url.ToString().Replace(Constants.BufferText, string.Empty);
                var isHttpRequest = request.Url.Scheme == "https";
                if (isHttpRequest && !urlToFetch.Contains("favicon"))
                {
                    var requestHeaders = request.RequestHeaders;

                    if (requestHeaders.ContainsKey("Range"))
                    {
                        var options = new WebFetchOptions
                        {
                            Range = RequestHelpers.RangeStringToArray(requestHeaders["Range"])
                        };

                        var task = WebFetchService.FetchResourceAsync(urlToFetch, options);
                        var safeResponse = task.WaitAndUnwrapException();

                        var stream = new MemoryStream(safeResponse.Data);
                        var response = new WebResourceResponse(safeResponse.MimeType, "UTF-8", stream);
                        response.SetStatusCodeAndReasonPhrase(206, "Partial Content");
                        response.ResponseHeaders = new Dictionary<string, string>
                        {
                            { "Accept-Ranges", "bytes" },
                            { "content-type", safeResponse.MimeType },
                            { "Content-Range", safeResponse.Headers["Content-Range"] },
                            { "Content-Length", safeResponse.Headers["Content-Length"] },
                        };

                        if (_renderer.TryGetTarget(out HybridWebViewRenderer webviewRenderer))
                        {
                            webviewRenderer.SetCurrentPageVersion(safeResponse.CurrentNrsVersion);
                            webviewRenderer.SetLatestPageVersion(safeResponse.LatestNrsVersion);
                        }

                        return response;
                    }
                    else
                    {
                        var safeResponse = WebFetchService.FetchResourceAsync(urlToFetch).Result;
                        var stream = new MemoryStream(safeResponse.Data);
                        var response = new WebResourceResponse(safeResponse.MimeType, "UTF-8", stream);
                        if (_renderer.TryGetTarget(out HybridWebViewRenderer webviewRenderer))
                        {
                            webviewRenderer.SetCurrentPageVersion(safeResponse.CurrentNrsVersion);
                            webviewRenderer.SetLatestPageVersion(safeResponse.LatestNrsVersion);
                        }

                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                if (ex.InnerException != null)
                {
                    var stream = new MemoryStream();
                    var response = new WebResourceResponse("text/html", "UTF-8", stream);
                    response.SetStatusCodeAndReasonPhrase(404, "Not Found");
                    return response;
                }
            }

            return base.ShouldInterceptRequest(view, request);
        }
    }
}
