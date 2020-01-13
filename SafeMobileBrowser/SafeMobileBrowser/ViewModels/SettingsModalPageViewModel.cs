// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System.Windows.Input;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using SafeMobileBrowser.Themes;
using SafeMobileBrowser.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class SettingsModalPageViewModel : BaseViewModel
    {
        public IPlatformService OpenNativeBrowserService => DependencyService.Get<IPlatformService>();

        public ICommand FaqCommand { get; }

        public ICommand PrivacyInfoCommand { get; }

        public ICommand OpenLogsPageCommand { get; }

        public string ApplicationVersion => AppInfo.VersionString;

        public ICommand GoBackCommand { get; }

        private readonly INavigation _navigation;

        private bool _appDarkMode;

        public bool AppDarkMode
        {
            get => _appDarkMode;
            set
            {
                if (value != _appDarkMode)
                {
                    SetProperty(ref _appDarkMode, value);
                    ThemeHelper.ToggleTheme(value ? ThemeHelper.AppThemeMode.Dark : ThemeHelper.AppThemeMode.Light);
                }
            }
        }

        public SettingsModalPageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            GoBackCommand = new Command(GoBackToHomePage);
            FaqCommand = new Command(() =>
            {
                OpenNativeBrowserService.LaunchNativeEmbeddedBrowser(Constants.FaqUrl);
            });
            PrivacyInfoCommand = new Command(() =>
            {
                OpenNativeBrowserService.LaunchNativeEmbeddedBrowser(Constants.PrivacyInfoUrl);
            });

            OpenLogsPageCommand = new Command(() => { navigation.PushModalAsync(new LogsModalPage()); });

            var currentTheme = ThemeHelper.CurrentTheme();
            switch (currentTheme)
            {
                case ThemeHelper.AppThemeMode.Light:
                    AppDarkMode = false;
                    break;
                case ThemeHelper.AppThemeMode.Dark:
                    AppDarkMode = true;
                    break;
            }
        }

        private async void GoBackToHomePage()
        {
            await _navigation.PopModalAsync();
        }
    }
}
