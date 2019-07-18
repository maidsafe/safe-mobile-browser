using Foundation;
using SafariServices;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.iOS.PlatformServices;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IOSNativeBrowserService))]

namespace SafeMobileBrowser.iOS.PlatformServices
{
    public class IOSNativeBrowserService : INativeBrowserService
    {
        public void LaunchNativeEmbeddedBrowser(string url)
        {
            var destination = new NSUrl(url);
            var sfViewController = new SFSafariViewController(destination);

            var window = UIApplication.SharedApplication.KeyWindow;
            var controller = window.RootViewController;
            controller.PresentViewController(sfViewController, true, null);
        }
    }
}
