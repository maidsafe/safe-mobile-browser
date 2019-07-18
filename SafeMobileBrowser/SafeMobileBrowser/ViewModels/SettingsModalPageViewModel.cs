using System.Windows.Input;
using SafeMobileBrowser.Helpers;
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

        private readonly INavigation _navigation;

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
        }

        private async void GoBackToHomePage()
        {
            await _navigation.PopModalAsync();
        }
    }
}
