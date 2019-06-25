using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using SafeMobileBrowser.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMobileBrowser.Views
{
    public partial class AuthenticationPage : ContentPage
    {
        AuthenticationPageViewMode _viewModel;

        public AuthenticationPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
                _viewModel = new AuthenticationPageViewMode();

            BindingContext = _viewModel;

            MessagingCenter.Subscribe<AuthenticationService, string>(
                this,
                MessageCenterConstants.Authenticated,
                async (sender, encodedResponse) =>
                {
                    _viewModel.ProgressText = "Authentication successful";
                    var storeResponse = await DisplayAlert(
                        "Store authentication response",
                        "Do you want to store auth response for future use.",
                        "Yes",
                        "No");
                    if (storeResponse)
                    {
                        await CredentialCacheService.Store(encodedResponse);
                    }
                    Application.Current.MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.White };
                });

            MessagingCenter.Subscribe<AuthenticationService>(
                this,
                MessageCenterConstants.Authenticated,
                (sender) => Application.Current.MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.White });

            MessagingCenter.Subscribe<AuthenticationPageViewMode>(
                this,
                MessageCenterConstants.Authenticated,
                (sender) => Application.Current.MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.White });

            MessagingCenter.Subscribe<AuthenticationService>(
                this,
                MessageCenterConstants.ProcessingAuthResponse,
                (sender) => _viewModel.ProgressText = "Process authentication response");

            MessagingCenter.Subscribe<AuthenticationService>(
                this,
                MessageCenterConstants.AuthenticationFailed,
                async (sender) =>
                {
                    await Application.Current.MainPage.DisplayAlert("Authentication Failed", "Unable to authenticate", "OK");
                    _viewModel.IsBusy = false;
                    _viewModel.AuthenticateCommand.ForceCanExecute();
                });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<AuthenticationService>(this, MessageCenterConstants.Authenticated);
            MessagingCenter.Unsubscribe<AuthenticationPageViewMode>(this, MessageCenterConstants.Authenticated);
            MessagingCenter.Unsubscribe<AuthenticationService>(this, MessageCenterConstants.ProcessingAuthResponse);
            MessagingCenter.Unsubscribe<AuthenticationService>(this, MessageCenterConstants.AuthenticationFailed);
        }
    }
}
