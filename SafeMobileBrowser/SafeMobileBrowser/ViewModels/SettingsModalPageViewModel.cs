using System.Windows.Input;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Themes;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class SettingsModalPageViewModel : BaseViewModel
    {
        public ICommand FaqCommand { get; }

        public ICommand PrivacyInfoCommand { get; }

        public string ApplicationVersion => AppInfo.VersionString;

        public ICommand GoBackCommand { get; set; }

        private INavigation _navigation;

        private bool _isDarkThemeEnabled;

        public bool IsDarkThemeEnabled
        {
            get => _isDarkThemeEnabled;

            set
            {
                SetProperty(ref _isDarkThemeEnabled, value);
                ThemeHelper.ChangeTheme(value ? AppThemeMode.Dark : AppThemeMode.Light);
            }
        }

        public SettingsModalPageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            GoBackCommand = new Command(GoBackToHomePage);
            FaqCommand = new Command(ShowNotImplementedDialog);
            PrivacyInfoCommand = new Command(ShowNotImplementedDialog);
            var currentTheme = ThemeHelper.CurrentTheme();
            switch (currentTheme)
            {
                case AppThemeMode.Light:
                    IsDarkThemeEnabled = false;
                    break;
                case AppThemeMode.Dark:
                    IsDarkThemeEnabled = true;
                    break;
                default:
                    break;
            }
        }

        private void ShowNotImplementedDialog()
        {
            Application.Current.MainPage.DisplayAlert("Feature", "This feature not available yet.", "OK");
        }

        private async void GoBackToHomePage()
        {
            await _navigation.PopModalAsync();
        }
    }
}
