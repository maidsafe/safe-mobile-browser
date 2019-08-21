using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using SafariServices;
using SafeMobileBrowser.iOS.PlatformServices;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformService))]

namespace SafeMobileBrowser.iOS.PlatformServices
{
    public class PlatformService : IPlatformService
    {
        public string ConfigFilesPath => Environment.GetFolderPath(Environment.SpecialFolder.Resources);

        public string BaseUrl => NSBundle.MainBundle.BundlePath;

        public Task<bool> OpenUri(string uri)
        {
            var canOpen = UIApplication.SharedApplication.CanOpenUrl(new NSUrl(uri));

            return Task.FromResult(canOpen && UIApplication.SharedApplication.OpenUrl(new NSUrl(uri)));
        }

        public async Task TransferAssetsAsync(List<AssetFileTransferModel> fileList)
        {
            foreach (var file in fileList)
            {
                using (var reader = new StreamReader(Path.Combine(".", file.FileName)))
                {
                    using (var writer = new StreamWriter(Path.Combine(file.TransferLocation, file.FileName)))
                    {
                        await writer.WriteAsync(await reader.ReadToEndAsync());
                        writer.Close();
                    }
                    reader.Close();
                }
            }
        }

        public void LaunchNativeEmbeddedBrowser(string url)
        {
            var destination = new NSUrl(url);
            var sfViewController = new SFSafariViewController(destination);

            var controller = GetVisibleViewController();
            controller.PresentViewController(sfViewController, true, null);
        }

        private static UIViewController GetVisibleViewController()
        {
            var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            switch (rootController.PresentedViewController)
            {
                case null:
                    return rootController;
                case UINavigationController controller:
                    return controller.VisibleViewController;
                case UITabBarController controller:
                    return controller.SelectedViewController;
                default:
                    return rootController.PresentedViewController;
            }
        }
    }
}
