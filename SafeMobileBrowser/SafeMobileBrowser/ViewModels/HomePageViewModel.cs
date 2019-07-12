using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Rg.Plugins.Popup.Extensions;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using SafeMobileBrowser.Views;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        private MenuPopUp _menuPopUp;

        public static string CurrentUrl { get; private set; }

        public static string CurrentTitle { get; private set; }

        public string WelcomePageUrl => $"{DependencyService.Get<IPlatformService>().BaseUrl}/index.html";

        public bool IsSessionAvailable => App.AppSession != null;

        public ICommand PageLoadCommand { get; private set; }

        public ICommand ToolbarItemCommand { get; private set; }

        public Command BottomNavbarTapCommand { get; set; }

        public ICommand GoBackCommand { get; set; }

        public ICommand GoForwardCommand { get; set; }

        public ICommand AddressBarFocusCommand { get; set; }

        public ICommand ReloadCommand { get; set; }

        public ICommand GoToHomePageCommand { get; set; }

        public ICommand WebViewNavigatingCommand { get; set; }

        public ICommand WebViewNavigatedCommand { get; set; }

        public ICommand MenuCommand { get; set; }

        public ICommand AddressBarUnfocusCommand { get; set; }

        private bool _canGoBack;

        public bool CanGoBack
        {
            get => _canGoBack;
            set => SetProperty(ref _canGoBack, value);
        }

        private bool _canGoForward;

        public bool CanGoForward
        {
            get => _canGoForward;
            set => SetProperty(ref _canGoForward, value);
        }

        private bool _isNavigating;

        public bool IsNavigating
        {
            get => _isNavigating;
            set => SetProperty(ref _isNavigating, value);
        }

        private bool _pageLoading;

        public bool IsPageLoading
        {
            get => _pageLoading;
            set => SetProperty(ref _pageLoading, value);
        }

        private WebViewSource _url;

        public WebViewSource Url
        {
            get => _url;
            set => SetProperty(ref _url, value);
        }

        private string _addressbarText;

        public string AddressbarText
        {
            get => _addressbarText;
            set
            {
                SetProperty(ref _addressbarText, value);
                if (string.IsNullOrWhiteSpace(value))
                {
                    CurrentUrl = CurrentTitle = value;
                    CanGoToHomePage = false;
                }
                else
                {
                    CurrentUrl = CurrentTitle = $"safe://{value}";
                    CanGoToHomePage = true;
                }
                OnPropertyChanged(nameof(CanGoToHomePage));
            }
        }

        public bool CanGoToHomePage { get; set; }

        public string ErrorType { get; private set; }

        public bool IsErrorState { get; set; }

        public bool IsAddressBarFocused { get; set; }

        public INavigation Navigation { get; set; }

        public HomePageViewModel(INavigation navigation)
        {
            Navigation = navigation;
            PageLoadCommand = new Command<string>(LoadUrl);
            BottomNavbarTapCommand = new Command<string>(OnTapped);
            WebViewNavigatingCommand = new Command<WebNavigatingEventArgs>(OnNavigating);
            WebViewNavigatedCommand = new Command<WebNavigatedEventArgs>(OnNavigated);
            GoToHomePageCommand = new Command(GoToHomePage);
            MenuCommand = new Command(ShowPopUpMenu);
            AddressBarUnfocusCommand = new Command(RestoreAddressBar);
        }

        private void GoToHomePage()
        {
            Url = WelcomePageUrl;
        }

        private async void ShowPopUpMenu()
        {
            try
            {
                if (_menuPopUp == null)
                    _menuPopUp = new MenuPopUp();
                await Navigation.PushPopupAsync(_menuPopUp);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnNavigated(WebNavigatedEventArgs obj)
        {
            if (IsErrorState)
            {
                MessagingCenter.Send(this, MessageCenterConstants.UpdateErrorMsg);
                IsErrorState = false;
            }

            IsNavigating = false;
        }

        private void OnNavigating(WebNavigatingEventArgs args)
        {
            try
            {
                string url = args.Url.ToString();
                SetAddressBarText(url);

                if (!IsNavigating)
                    IsNavigating = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        internal async Task InitilizeSessionAsync()
        {
            try
            {
                using (UserDialogs.Instance.Loading(Constants.ConnectingProgressText))
                {
                    await AuthService.ConnectUsingStoredSerialisedConfiguration();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                await App.Current.MainPage.DisplayAlert(
                   ErrorConstants.ConnectionFailedTitle,
                   ErrorConstants.ConnectionFailedMsg,
                   "OK");
            }
        }

        public void RestoreAddressBar()
        {
            if (string.IsNullOrWhiteSpace(AddressbarText) && !IsAddressBarFocused)
            {
                var currentSourceUrl = ((UrlWebViewSource)Url).Url;
                SetAddressBarText(currentSourceUrl);
            }
        }

        internal void SetAddressBarText(string url)
        {
            if (url.StartsWith("file://") && !IsErrorState)
            {
                AddressbarText = string.Empty;
            }
            else if (url.StartsWith("safe://"))
            {
                string newurlText = url.Replace("safe://", string.Empty).TrimEnd('/');
                AddressbarText = newurlText;
            }
            else if (url.StartsWith("https://"))
            {
                string newurlText = url.Replace("https://", string.Empty).TrimEnd('/');
                AddressbarText = newurlText;
            }
        }

        public void OnTapped(string navigationBarIconString)
        {
            switch (navigationBarIconString)
            {
                case "Back":
                    IsNavigating = true;
                    GoBackCommand.Execute(null);
                    break;
                case "Forward":
                    IsNavigating = true;
                    GoForwardCommand.Execute(null);
                    break;
                case "Focus":
                    AddressBarFocusCommand.Execute(null);
                    break;
                case "Home":
                    GoToHomePage();
                    break;
                case "Menu":
                    ShowPopUpMenu();
                    break;
                default:
                    break;
            }
        }

        public void LoadUrl(string url = null)
        {
            try
            {
                url = url?.Trim().ToLower() ?? AddressbarText.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(url))
                    return;
                else
                    AddressbarText = url;

                if (!App.IsConnectedToInternet)
                {
                    TriggerErrorState(ErrorConstants.NoInternetConnection);
                    return;
                }

                if (!IsSessionAvailable)
                {
                    TriggerErrorState(ErrorConstants.SessionNotAvailable);
                    return;
                }

                if (Device.RuntimePlatform == Device.iOS)
                    url = $"safe://{url}";
                else
                    url = $"https://{url}";

                if (!IsValidUri(url))
                    return;

                IsNavigating = true;

                Url = url;
            }
            catch (UriFormatException ex)
            {
                Logger.Error(ex);
                TriggerErrorState(ErrorConstants.InvalidUrl);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private bool IsValidUri(string url)
        {
            try
            {
                // Trying to generate a new Uri object from the string url.
                // If failed it will show an invalid url page.
                var uri = new Uri(url);
                return true;
            }
            catch (UriFormatException ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        private void TriggerErrorState(string errorType)
        {
            IsErrorState = true;
            ErrorType = errorType;
            MessagingCenter.Send(this, MessageCenterConstants.ShowErrorPage);
        }
    }
}
