using Android.Views;
using Plugin.CurrentActivity;
using SafeMobileBrowser.Droid.PlatformServices;
using SafeMobileBrowser.Themes;
using Xamarin.Forms;
using AColor = Android.Graphics.Color;

[assembly: Dependency(typeof(NativeThemeManager))]

namespace SafeMobileBrowser.Droid.PlatformServices
{
    public class NativeThemeManager : INativeThemeManager
    {
        private readonly AColor _lightStatusBarColor = AColor.White;
        private readonly AColor _darkStatusBarColor = AColor.ParseColor("#212121");

        public void ChangeAppTheme(ThemeHelper.AppThemeMode theme, bool isAppLaunched)
        {
            var currentWindow = GetCurrentWindow();
            switch (theme)
            {
                case ThemeHelper.AppThemeMode.Light:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        currentWindow.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                        currentWindow.SetStatusBarColor(_lightStatusBarColor);
                        if (!isAppLaunched)
                        {
                            CrossCurrentActivity.Current.Activity.SetTheme(Resource.Style.MainTheme);
                        }
                    });
                    break;
                case ThemeHelper.AppThemeMode.Dark:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        currentWindow.DecorView.SystemUiVisibility = 0;
                        currentWindow.SetStatusBarColor(_darkStatusBarColor);
                        if (!isAppLaunched)
                        {
                            CrossCurrentActivity.Current.Activity.SetTheme(Resource.Style.MainDarkTheme);
                        }
                    });
                    break;
            }
        }

        private static Window GetCurrentWindow()
        {
            var window = CrossCurrentActivity.Current.Activity.Window;

            // clear FLAG_TRANSLUCENT_STATUS flag:
            window.ClearFlags(WindowManagerFlags.TranslucentStatus);

            // add FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS flag to the window
            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            return window;
        }
    }
}
