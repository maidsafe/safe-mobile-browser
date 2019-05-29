﻿using Android.Webkit;
using SafeMobileBrowser.Droid.PlatformServices;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.WebFetchImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms.Platform.Android;
using AWeb = Android.Webkit;

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class HybridWebViewClient : FormsWebViewClient
    {
        public HybridWebViewClient(HybridWebViewRenderer renderer) : base(renderer)
        {
        }

        public override WebResourceResponse ShouldInterceptRequest(AWeb.WebView view, IWebResourceRequest request)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(request.Url.ToString());
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
                        //var contentType = "video/mp4";
                        //var contentLength = saferesponse.Data.Length;
                        //var contentStart = options?.Range[0].Start;
                        //var contentEnd = options?.Range[0].End > 0 ? options?.Range[0].End : contentLength - 1;
                        WebResourceResponse response = new WebResourceResponse(saferesponse.MimeType, "UTF-8", stream);
                        response.SetStatusCodeAndReasonPhrase(206, "Partial Content");
                        response.ResponseHeaders = new Dictionary<string, string>
                        {
                            { "Accept-Ranges", "bytes"},
                            { "content-type", saferesponse.MimeType },
                            { "Content-Range", saferesponse.Headers["Content-Range"]},
                            { "Content-Length", saferesponse.Headers["Content-Length"]},
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
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);

                if (ex.InnerException != null)
                {
                    var exception = ex.InnerException as WebFetchException;
                    System.Diagnostics.Debug.WriteLine("Error Code: " + exception.ErrorCode);
                    System.Diagnostics.Debug.WriteLine("Error Message: " + exception.Message);

                    if (exception.ErrorCode == WebFetchConstants.NoSuchData ||
                        exception.ErrorCode == WebFetchConstants.NoSuchEntry ||
                        exception.ErrorCode == WebFetchConstants.NoSuchPublicName ||
                        exception.ErrorCode == WebFetchConstants.NoSuchServiceName)
                    {
                        var htmlString = GetAssetsFileData.ReadHtmlFile("notfound.html");
                        byte[] byteArray = Encoding.ASCII.GetBytes(htmlString);
                        MemoryStream stream = new MemoryStream(byteArray);
                        var response = new WebResourceResponse("text/html", "UTF-8", stream);
                        return response;
                    }
                }
            }
            return base.ShouldInterceptRequest(view, request);
        }
    }
}
