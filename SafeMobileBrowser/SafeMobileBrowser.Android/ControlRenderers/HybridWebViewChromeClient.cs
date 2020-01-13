// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

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
