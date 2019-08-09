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
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var currentWindow = GetCurrentWindow();
                            currentWindow.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                            currentWindow.SetStatusBarColor(Android.Graphics.Color.White);
                        });
                    }
                    break;
                case ThemeHelper.AppThemeMode.Dark:
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var currentWindow = GetCurrentWindow();
                            currentWindow.DecorView.SystemUiVisibility = 0;
                            currentWindow.SetStatusBarColor(Android.Graphics.Color.ParseColor("#212121"));
                        });
                    }
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
