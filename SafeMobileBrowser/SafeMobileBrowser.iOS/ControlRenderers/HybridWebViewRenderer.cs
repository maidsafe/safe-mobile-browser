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
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.iOS.ControlRenderers;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]

namespace SafeMobileBrowser.iOS.ControlRenderers
{
    public class HybridWebViewRenderer : WkWebViewRenderer
    {
        private IDisposable _progressObserver;

        public static WKWebViewConfiguration GetHybridWKWebViewConfiguration()
        {
            var config = new WKWebViewConfiguration();
            config.SetUrlSchemeHandler(new SafeSchemaHandler(), "safe");
            return config;
        }

        public HybridWebViewRenderer()
            : base(GetHybridWKWebViewConfiguration())
        {
        }

        public static string BaseUrl { get; set; } = NSBundle.MainBundle.BundlePath;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (NativeView != null)
            {
                ((WKWebView)NativeView).AllowsBackForwardNavigationGestures = true;
                _progressObserver = NativeView.AddObserver("estimatedProgress", NSKeyValueObservingOptions.New, ProgressObserverAction);
                SetSource();
            }
        }

        private void ProgressObserverAction(NSObservedChange obj)
        {
            ((HybridWebView)Element).ContentLoadProgress = ((WKWebView)NativeView).EstimatedProgress;
        }

        private void SetSource()
        {
            if (NativeView == null || Element == null)
                return;
            var path = Path.Combine(BaseUrl, "index.html");
            var nsFileUri = new NSUrl($"file://{path}");
            var nsBaseUri = new NSUrl($"file://{BaseUrl}");

            ((WKWebView)NativeView).LoadFileUrl(nsFileUri, nsBaseUri);
        }

        protected override void Dispose(bool disposing)
        {
            _progressObserver?.Dispose();
            base.Dispose(disposing);
        }

        public void SetCurrentPageVersion(ulong version)
        {
            if (NativeView != null && Element != null)
            {
                ((HybridWebView)Element).CurrentPageVersion = version;
            }
        }

        public void SetLatestPageVersion(ulong version)
        {
            if (NativeView != null && Element != null)
            {
                ((HybridWebView)Element).LatestPageVersion = version;
            }
        }
    }
}
