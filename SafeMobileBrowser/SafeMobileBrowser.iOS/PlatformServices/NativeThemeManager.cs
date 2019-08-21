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
        private readonly UIColor _darkStatusBarColor = UIColor.FromRGB(33, 33, 33);
        private readonly UIColor _lightStatusBarColor = UIColor.White;
        private readonly NSString _statusBarKey = new NSString("statusBar");

        public void ChangeAppTheme(ThemeHelper.AppThemeMode theme, bool isAppLaunched)
        {
            switch (theme)
            {
                case ThemeHelper.AppThemeMode.Light:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (UIApplication.SharedApplication.ValueForKey(_statusBarKey) is UIView statusBar)
                            statusBar.BackgroundColor = _lightStatusBarColor;
                    });
                    break;
                case ThemeHelper.AppThemeMode.Dark:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (UIApplication.SharedApplication.ValueForKey(_statusBarKey) is UIView statusBar)
                            statusBar.BackgroundColor = _darkStatusBarColor;
                    });
                    break;
            }
        }
    }
}
