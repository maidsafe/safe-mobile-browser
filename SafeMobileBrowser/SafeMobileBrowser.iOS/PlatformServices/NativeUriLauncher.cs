using System.Threading.Tasks;
using Foundation;
using SafeMobileBrowser.iOS.PlatformServices;
using SafeMobileBrowser.Services;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(NativeUriLauncher))]

namespace SafeMobileBrowser.iOS.PlatformServices
{
    class NativeUriLauncher : INativeUriLauncher
    {
        public Task<bool> LaunchApp(string uri)
        {
            var canOpen = UIApplication.SharedApplication.CanOpenUrl(new NSUrl(uri));

            if (!canOpen)
                return Task.FromResult(false);

            return Task.FromResult(UIApplication.SharedApplication.OpenUrl(new NSUrl(uri)));
        }
    }
}
