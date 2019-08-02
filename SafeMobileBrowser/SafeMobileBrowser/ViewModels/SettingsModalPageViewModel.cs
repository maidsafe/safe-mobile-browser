using System;
using System.Windows.Input;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using SafeMobileBrowser.Themes;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class SettingsModalPageViewModel : BaseViewModel
    {
        public IPlatformService OpenNativeBrowserService => DependencyService.Get<IPlatformService>();

        public ICommand FaqCommand { get; }

        public ICommand PrivacyInfoCommand { get; }

        public ICommand ToggleThemeCommand { get; }

        public string ApplicationVersion => AppInfo.VersionString;

        public ICommand GoBackCommand { get; set; }

        private readonly INavigation _navigation;

        private bool _appDarkMode;

        public bool AppDarkMode
        {
            get => _appDarkMode;
            set
            {
                var theme = Preferences.Get("CurrentAppTheme", 0) == 0 ? false : true;
                if (value != theme)
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
