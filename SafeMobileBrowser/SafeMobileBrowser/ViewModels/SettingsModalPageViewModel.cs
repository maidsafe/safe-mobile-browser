using System.Windows.Input;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
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
        }

        private async void GoBackToHomePage()
        {
            await _navigation.PopModalAsync();
        }
    }
}
