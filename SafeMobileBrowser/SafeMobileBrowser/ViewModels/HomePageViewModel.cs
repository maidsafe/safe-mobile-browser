using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Rg.Plugins.Popup.Extensions;
using SafeMobileBrowser.Services;
using SafeMobileBrowser.Views;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        public static string CurrentUrl { get; private set; }

        public static string CurrentTitle { get; private set; }

        private string _baseUrl = DependencyService.Get<IBaseUrl>().GetBaseUrl();

        public bool IsSessionAvailable => App.AppSession != null ? true : false;

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

        private bool _canGoBack;

        public bool CanGoBack
        {
            get => _canGoBack;

            set
            {
                _canGoBack = value;
                OnPropertyChanged();
            }
        }

        private bool _canGoForward;

        public bool CanGoForward
        {
            get => _canGoForward;

            set
            {
                _canGoForward = value;
                OnPropertyChanged();
            }
        }

        private bool _isNavigating;

        public bool IsNavigating
        {
            get => _isNavigating;

            set
            {
                _isNavigating = value;
                OnPropertyChanged();
            }
        }

        private bool _pageLoading;

        public bool IsPageLoading
        {
            get { return _pageLoading; }
            set { SetProperty(ref _pageLoading, value); }
        }

        private string _url;

        public string Url
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
                var address = string.IsNullOrWhiteSpace(value);
                if (!address)
                {
                    CurrentUrl = CurrentTitle = $"safe://{value}";
                    CanGoToHomePage = true;
                }
                else
                {
                    CurrentUrl = CurrentTitle = value;
                    CanGoToHomePage = false;
                }
                OnPropertyChanged(nameof(CanGoToHomePage));
            }
        }

        public bool CanGoToHomePage { get; set; }

        private MenuPopUp _menuPopUp;

        public INavigation Navigation { get; set; }

        public HomePageViewModel(INavigation navigation)
        {
            Navigation = navigation;
            PageLoadCommand = new Command<string>(LoadUrl);
            ToolbarItemCommand = new Command<string>(LoadUrl);
            BottomNavbarTapCommand = new Command<string>(OnTapped);
            WebViewNavigatingCommand = new Command<WebNavigatingEventArgs>(OnNavigating);
            WebViewNavigatedCommand = new Command<WebNavigatedEventArgs>(OnNavigated);
            GoToHomePageCommand = new Command(GoToHomePage);
            MenuCommand = new Command(ShowPopUpMenu);
        }

        private void GoToHomePage()
        {
            var homePageUrl = $"{_baseUrl}index.html";
            Url = homePageUrl;
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
                Debug.WriteLine(ex);
            }
        }

        private void OnNavigated(WebNavigatedEventArgs obj)
        {
            IsNavigating = false;
        }

        private void OnNavigating(WebNavigatingEventArgs args)
        {
            try
            {
                string url = args.Url.ToString();
                if (url.StartsWith("file://"))
                {
                    AddressbarText = string.Empty;
                }
                else if (url.StartsWith("https"))
                {
                    string newurlText = url.Remove(0, 8).TrimEnd('/');
                    AddressbarText = newurlText;
                }
                else if (url.StartsWith("http"))
                {
                    string newurlText = url.Remove(0, 7).TrimEnd('/');
                    AddressbarText = newurlText;
                }
                else
                {
                    AddressbarText = url;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        internal async Task InitilizeSessionAsync()
        {
            try
            {
                using (UserDialogs.Instance.Loading("Connecting to SAFE Network"))
                {
                    await AuthService.ConnectUsingHardcodedResponseAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await App.Current.MainPage.DisplayAlert("Connection failed", "Unable to connect to the SAFE Network. Try updating your IP address on invite server.", "OK");
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

        public async void LoadUrl(string url = null)
        {
            if (!App.IsConnectedToInternet)
            {
                await App.Current.MainPage.DisplayAlert("No internet connection", "Please connect to the internet", "Ok");
                return;
            }
            if (App.AppSession == null)
            {
                await InitilizeSessionAsync();
            }
            else
            {
                if (url != null)
                    AddressbarText = url;

                IsNavigating = true;

                if (Device.RuntimePlatform == Device.iOS)
                    Url = $"safe://{AddressbarText}";
                else
                    Url = $"https://{AddressbarText}";
            }
        }
    }
}
