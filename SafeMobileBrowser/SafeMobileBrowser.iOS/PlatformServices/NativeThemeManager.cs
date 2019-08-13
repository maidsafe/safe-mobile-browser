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
        public void ChangeAppTheme(ThemeHelper.AppThemeMode theme, bool isAppLaunched)
        {
            var neroColor = UIColor.FromRGB(33, 33, 33);
            var nsStatusbar = new NSString("statusBar");
            switch (theme)
            {
                case ThemeHelper.AppThemeMode.Light:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UIView statusBar = UIApplication.SharedApplication.ValueForKey(nsStatusbar) as UIView;
                        statusBar.BackgroundColor = UIColor.White;
                    });
                    break;
                case ThemeHelper.AppThemeMode.Dark:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UIView statusBar = UIApplication.SharedApplication.ValueForKey(nsStatusbar) as UIView;
                        statusBar.BackgroundColor = neroColor;
                    });
                    break;
            }
        }
    }
}
