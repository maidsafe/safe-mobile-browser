using System;
using Android.Webkit;
using SafeMobileBrowser.Controls;
using Xamarin.Forms.Platform.Android;

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class HybridWebViewChromeClient : FormsWebChromeClient
    {
        private readonly WeakReference<HybridWebViewRenderer> _renderer;

        public HybridWebViewChromeClient(HybridWebViewRenderer renderer)
        {
            _renderer = new WeakReference<HybridWebViewRenderer>(renderer);
        }

        public override void OnProgressChanged(WebView view, int newProgress)
        {
            base.OnProgressChanged(view, newProgress);

            _renderer.TryGetTarget(out var renderer);

            if (renderer != null)
            {
                ((HybridWebView)renderer.Element).ContentLoadProgress = (double)newProgress / 100;
            }
        }
    }
}
