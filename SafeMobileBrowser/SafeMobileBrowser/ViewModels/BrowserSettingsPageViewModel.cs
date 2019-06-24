using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class BrowserSettingsPageViewModel : BaseViewModel
    {
        public ICommand FaqCommand { get; }

        public ICommand PrivacyInfoCommand { get; }

        public string ApplicationVersion => AppInfo.VersionString;

        public ICommand GoBackCommand { get; set; }

        private INavigation _navigation;

        public BrowserSettingsPageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            GoBackCommand = new Command(GoBackToHomePage);
            FaqCommand = new Command(ShowNotImplementedDialog);

            PrivacyInfoCommand = new Command(ShowNotImplementedDialog);
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
