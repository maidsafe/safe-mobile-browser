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
                SetSource();
            }
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

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            base.ObserveValue(keyPath, ofObject, change, context);

            if (keyPath == "estimatedProgress")
            {
                ((HybridWebView) Element).ContentLoadProgress = ((WKWebView)NativeView).EstimatedProgress;
            }
        }
    }
}
