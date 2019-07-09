using System;
using System.Threading.Tasks;
using SafeApp;
using SafeMobileBrowser.CustomAsyncCommand;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;

namespace SafeMobileBrowser.ViewModels
{
    public class AuthenticationPageViewMode : BaseViewModel
    {
        public bool IsMock { get; set; }

        private string _progressText;

        public string ProgressText
        {
            get { return _progressText; }
            set { SetProperty(ref _progressText, value); }
        }

        public string StoredResponse { get; private set; }

        public string AuthenticationButtonText { get; set; }

        public IAsyncCommand AuthenticateCommand { get; private set; }

        public AuthenticationPageViewMode()
        {
            Task.Run(async () =>
            {
                IsBusy = true;
                ProgressText = "Checking cached response";
                StoredResponse = await CredentialCacheService.Retrieve();
                IsBusy = false;
            }).Wait();
            SetAuthenticationButtonText();
            CheckIfMock();
            AuthenticateCommand = new AsyncCommand(AuthenticateAppAsync, CanExecute);
        }

        private void SetAuthenticationButtonText()
        {
            if (StoredResponse == null)
                AuthenticationButtonText = "Authorise";
            else
                AuthenticationButtonText = "Connect";
        }

        private bool CanExecute()
        {
            return !IsBusy;
        }

        private async Task AuthenticateAppAsync()
        {
            try
            {
                IsBusy = true;
#if SAFE_APP_MOCK
                //TODO: Update code to transfer the mockvault file
                ProgressText = "Transferring assets";
                await DeviceServices.TransferAssetFiles();
                ProgressText = "Creating mock account";
                await AuthenticationService.CreateMockAccount();
                ProgressText = "Mock authentication in progress";
                await AuthenticationService.RequestMockNetworkAuthenticationAsync();
                MessagingCenter.Send(this, MessageCenterConstants.Authenticated);
                IsBusy = false;
#else
                if (StoredResponse == null)
                {
                    ProgressText = "Requesting authentication for authenticator app";
                    await AuthenticationService.RequestNonMockAuthenticationAsync(true);
                }
                else
                {
                    ProgressText = "Estaiblishing session using cached response";
                    await AuthService.ConnectUsingStoredSerialisedConfiguration(StoredResponse);
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                await App.Current.MainPage.DisplayAlert("Authentication Failed", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CheckIfMock()
        {
            IsBusy = true;
            IsMock = Session.IsMockBuild();
            IsBusy = false;
        }
    }
}
