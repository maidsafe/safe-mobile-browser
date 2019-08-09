using SafeMobileBrowser.iOS.PlatformServices;
using SafeMobileBrowser.Themes;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(NativeThemeManager))]

namespace SafeMobileBrowser.iOS.PlatformServices
{
    public class NativeThemeManager : INativeThemeManager
    {
        public void ChangeAppTheme(ThemeHelper.AppThemeMode theme)
        {
            switch (theme)
            {
                case ThemeHelper.AppThemeMode.Light:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.Default, false);
                        GetCurrentViewController().SetNeedsStatusBarAppearanceUpdate();
                    });
                    break;
                case ThemeHelper.AppThemeMode.Dark:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
                        GetCurrentViewController().SetNeedsStatusBarAppearanceUpdate();
                    });
                    break;
            }
        }

        UIViewController GetCurrentViewController()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
                vc = vc.PresentedViewController;
            return vc;
        }
    }
}
