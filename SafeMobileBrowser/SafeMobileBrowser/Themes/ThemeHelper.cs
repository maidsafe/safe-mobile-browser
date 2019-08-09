using SafeMobileBrowser.Helpers;
using Xamarin.Forms;

namespace SafeMobileBrowser.Themes
{
    public class ThemeHelper
    {
        public enum AppThemeMode
        {
            Light,
            Dark
        }

        /// <summary>
        /// Changes the theme of the app.
        /// Add additional switch cases for more themes you add to the app.
        /// This also updates the local key storage value for the selected theme.
        /// </summary>
        /// <param name="theme"></param>
        public static void ToggleTheme(AppThemeMode theme)
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();

                // Update local key value with the new theme you select.
                Xamarin.Essentials.Preferences.Set("CurrentAppTheme", (int)theme);

                switch (theme)
                {
                    case AppThemeMode.Light:
                        mergedDictionaries.Add(new LightTheme());
                        break;
                    case AppThemeMode.Dark:
                        mergedDictionaries.Add(new DarkTheme());
                        break;
                    default:
                        mergedDictionaries.Add(new LightTheme());
                        break;
                }

                DependencyService.Get<INativeThemeManager>().ChangeAppTheme(theme);
                MessagingCenter.Send((App)Application.Current, MessageCenterConstants.UpdateWelcomePageTheme);
            }
        }

        private static void ManuallyCopyThemes(ResourceDictionary fromResource, ResourceDictionary toResource)
        {
            foreach (var item in fromResource.Keys)
            {
                toResource[item] = fromResource[item];
            }
        }

        /// <summary>
        /// Reads current theme id from the local storage and loads it.
        /// </summary>
        public static void LoadTheme()
        {
            var currentTheme = CurrentTheme();
            ToggleTheme(currentTheme);
        }

        /// <summary>
        /// Gives current/last selected theme from the local storage.
        /// </summary>
        /// <returns></returns>
        public static AppThemeMode CurrentTheme()
        {
            return (AppThemeMode)Xamarin.Essentials.Preferences.Get("CurrentAppTheme", (int)AppThemeMode.Light);
        }
    }
}
