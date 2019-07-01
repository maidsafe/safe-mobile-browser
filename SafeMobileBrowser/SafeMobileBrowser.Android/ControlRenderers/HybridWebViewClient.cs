using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Android.Webkit;
using SafeApp.Utilities;
using SafeMobileBrowser.Droid.PlatformServices;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.WebFetchImplementation;
using Xamarin.Forms.Platform.Android;
using AWeb = Android.Webkit;

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class HybridWebViewClient : FormsWebViewClient
    {
        readonly WeakReference<HybridWebViewRenderer> _renderer;

        public HybridWebViewClient(HybridWebViewRenderer renderer)
            : base(renderer)
        {
            _renderer = new WeakReference<HybridWebViewRenderer>(renderer);
        }

        public override WebResourceResponse ShouldInterceptRequest(AWeb.WebView view, IWebResourceRequest request)
        {
            try
            {
                Debug.WriteLine(request.Url.ToString());
                var headers = request.RequestHeaders;
                if (request.Url.Scheme.ToLower().Contains("http") && !request.Url.ToString().ToLower().Contains("favicon"))
                {
                    if (headers.ContainsKey("Range"))
                    {
                        var options = new WebFetchOptions
                        {
                            Range = RequestHelpers.RangeStringToArray(headers["Range"])
                        };
                        var saferesponse = WebFetchService.FetchResourceAsync(request.Url.ToString(), options).Result;
                        Stream stream = new MemoryStream(saferesponse.Data);

                        // var contentType = "video/mp4";
                        // var contentLength = saferesponse.Data.Length;
                        // var contentStart = options?.Range[0].Start;
                        // var contentEnd = options?.Range[0].End > 0 ? options?.Range[0].End : contentLength - 1;
                        WebResourceResponse response = new WebResourceResponse(saferesponse.MimeType, "UTF-8", stream);
                        response.SetStatusCodeAndReasonPhrase(206, "Partial Content");
                        response.ResponseHeaders = new Dictionary<string, string>
                        {
                            { "Accept-Ranges", "bytes" },
                            { "content-type", saferesponse.MimeType },
                            { "Content-Range", saferesponse.Headers["Content-Range"] },
                            { "Content-Length", saferesponse.Headers["Content-Length"] },
                        };
                        return response;
                    }
                    else
                    {
                        var saferesponse = WebFetchService.FetchResourceAsync(request.Url.ToString()).Result;
                        Stream stream = new MemoryStream(saferesponse.Data);
                        WebResourceResponse response = new WebResourceResponse(saferesponse.MimeType, "UTF-8", stream);
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);

                if (ex.InnerException != null)
                {
                    var htmlString = GetAssetsFileData.ReadHtmlFile("index.html");

                    if (ex.InnerException is WebFetchException)
                    {
                        var webFetchException = ex.InnerException as WebFetchException;
                        if (webFetchException.ErrorCode == WebFetchConstants.NoSuchData ||
                        webFetchException.ErrorCode == WebFetchConstants.NoSuchEntry ||
                        webFetchException.ErrorCode == WebFetchConstants.NoSuchPublicName ||
                        webFetchException.ErrorCode == WebFetchConstants.NoSuchServiceName)
                        {
                            htmlString = ReplaceHtmlStringContent(htmlString, "ErrorHeading", "Page not found");
                            htmlString = ReplaceHtmlStringContent(htmlString, "ErrorMessage", webFetchException.Message);
                        }
                    }
                    else if (ex.InnerException is FfiException)
                    {
                        var ffiException = ex.InnerException as FfiException;
                        if (ffiException.ErrorCode == -11 &&
                            Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
                        {
                            htmlString = ReplaceHtmlStringContent(htmlString, "ErrorHeading", "No internet access");
                            htmlString = ReplaceHtmlStringContent(htmlString, "ErrorMessage", "Please connect to the internet");
                        }
                        else
                        {
                            htmlString = ReplaceHtmlStringContent(htmlString, "ErrorHeading", "Error occured");
                            htmlString = ReplaceHtmlStringContent(htmlString, "ErrorMessage", ffiException.Message);
                        }
                    }

                    byte[] byteArray = Encoding.ASCII.GetBytes(htmlString);
                    MemoryStream stream = new MemoryStream(byteArray);
                    var response = new WebResourceResponse("text/html", "UTF-8", stream);
                    return response;
                }
            }
            return base.ShouldInterceptRequest(view, request);
        }

        string ReplaceHtmlStringContent(string htmlString, string find, string replaceString)
        {
            int index = htmlString.IndexOf(find);
            return htmlString.Remove(index, find.Length).Insert(index, replaceString);
        }
    }
}
