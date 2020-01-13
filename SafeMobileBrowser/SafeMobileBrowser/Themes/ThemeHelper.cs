// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using SafeMobileBrowser.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser.Themes
{
    public static class ThemeHelper
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
        /// <param name="theme">AppThemeMode enum value</param>
        /// <param name="isAppLaunched">Send true when called from app start method</param>
        public static void ToggleTheme(AppThemeMode theme, bool isAppLaunched = false)
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries == null)
                return;

            mergedDictionaries.Clear();

            // Update local key value with the new theme you select.
            Preferences.Set(Constants.AppThemePreferenceKey, theme == AppThemeMode.Dark);

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

            DependencyService.Get<INativeThemeManager>().ChangeAppTheme(theme, isAppLaunched);
            MessagingCenter.Send((App)Application.Current, MessageCenterConstants.UpdateWelcomePageTheme);
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
            ToggleTheme(currentTheme, true);
        }

        /// <summary>
        /// Gives current/last selected theme from the local storage.
        /// </summary>
        /// <returns></returns>
        public static AppThemeMode CurrentTheme()
        {
            var isDarkTheme = Preferences.Get(Constants.AppThemePreferenceKey, false);
            return isDarkTheme ? AppThemeMode.Dark : AppThemeMode.Light;
        }
    }
}
