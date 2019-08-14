using Foundation;
using SafeMobileBrowser.iOS.PlatformServices;
using SafeMobileBrowser.Themes;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(NativeThemeManager))]

namespace SafeMobileBrowser.iOS.PlatformServices
{
    public class NativeThemeManager : INativeThemeManager
    {
        private UIColor darkStatusBarColor = UIColor.FromRGB(33, 33, 33);
        private UIColor lightStatusBarColor = UIColor.White;
        private NSString statusBarKey = new NSString("statusBar");

        public void ChangeAppTheme(ThemeHelper.AppThemeMode theme, bool isAppLaunched)
        {
            switch (theme)
            {
                case ThemeHelper.AppThemeMode.Light:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UIView statusBar = UIApplication.SharedApplication.ValueForKey(statusBarKey) as UIView;
                        statusBar.BackgroundColor = lightStatusBarColor;
                    });
                    break;
                case ThemeHelper.AppThemeMode.Dark:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UIView statusBar = UIApplication.SharedApplication.ValueForKey(statusBarKey) as UIView;
                        statusBar.BackgroundColor = darkStatusBarColor;
                    });
                    break;
            }
        }
    }
}
