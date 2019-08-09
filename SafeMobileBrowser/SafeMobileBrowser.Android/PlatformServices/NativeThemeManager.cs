using Android.OS;
using Android.Views;
using Plugin.CurrentActivity;
using SafeMobileBrowser.Droid.PlatformServices;
using SafeMobileBrowser.Themes;
using Xamarin.Forms;

[assembly: Dependency(typeof(NativeThemeManager))]

namespace SafeMobileBrowser.Droid.PlatformServices
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
                        var currentWindow = GetCurrentWindow();
                        currentWindow.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                        currentWindow.SetStatusBarColor(Android.Graphics.Color.White);
                        if (!Xamarin.Essentials.Preferences.Get("AppOpenedNow", false))
                        {
                            CrossCurrentActivity.Current.Activity.SetTheme(Resource.Style.MainTheme);
                        }
                        Xamarin.Essentials.Preferences.Set("AppOpenedNow", false);
                    });
                    break;
                case ThemeHelper.AppThemeMode.Dark:
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var currentWindow = GetCurrentWindow();
                        currentWindow.DecorView.SystemUiVisibility = 0;
                        currentWindow.SetStatusBarColor(Android.Graphics.Color.ParseColor("#212121"));
                        if (!Xamarin.Essentials.Preferences.Get("AppOpenedNow", false))
                        {
                            CrossCurrentActivity.Current.Activity.SetTheme(Resource.Style.MainDarkTheme);
                        }
                        Xamarin.Essentials.Preferences.Set("AppOpenedNow", false);
                    });
                    break;
            }
        }

        Window GetCurrentWindow()
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
