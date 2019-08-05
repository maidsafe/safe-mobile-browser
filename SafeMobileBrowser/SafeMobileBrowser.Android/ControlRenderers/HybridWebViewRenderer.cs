using System.ComponentModel;
using Android.Content;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.Droid.ControlRenderers;
using SafeMobileBrowser.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class HybridWebViewRenderer : WebViewRenderer
    {
        HybridWebViewClient _webViewClient;
        HybridWebViewChromeClient _webViewChromeClient;

        public HybridWebViewRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                _webViewClient = GetHybridWebViewClient();
                Control.SetWebViewClient(_webViewClient);
                _webViewChromeClient = GetHybridWebViewChromeClient();
                Control.SetWebChromeClient(_webViewChromeClient);
                Control.Settings.SetSupportZoom(true);
                if (Control.Settings.SupportZoom())
                {
                    Control.Settings.BuiltInZoomControls = true;
                    Control.Settings.DisplayZoomControls = false;
                }
                Control.LoadUrl($"{AssetBaseUrl}index.html");
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == HybridWebView.ContentLoadProgressProperty.PropertyName)
            {
                Logger.Info($"Content Load progress: {((HybridWebView)Element).ContentLoadProgress}");
            }
        }

        private HybridWebViewClient GetHybridWebViewClient()
        {
            return new HybridWebViewClient(this);
        }

        private HybridWebViewChromeClient GetHybridWebViewChromeClient()
        {
            return new HybridWebViewChromeClient(this);
        }

        protected override void Dispose(bool disposing)
        {
            _webViewClient?.Dispose();
            _webViewChromeClient?.Dispose();
            base.Dispose(disposing);
        }
    }
}
