using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Extensions;
using SafeMobileBrowser.Helpers;
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
                if (!string.IsNullOrWhiteSpace(value))
                    CurrentUrl = CurrentTitle = $"safe://{value}";
                else
                    CurrentUrl = CurrentTitle = value;
                CanGoToHomePage = string.IsNullOrWhiteSpace(value) ? false : true;
                OnPropertyChanged(nameof(CanGoToHomePage));
            }
        }

        public bool CanGoToHomePage { get; set; }

        private MenuPopUp _menuPopUp;

        public INavigation Navigation { get; set; }

        public HomePageViewModel(INavigation navigation)
        {
            Navigation = navigation;
            PageLoadCommand = new Command(LoadUrl);
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
            if (string.Equals(Url, homePageUrl))
            {
                MessagingCenter.Send(this, MessageCenterConstants.ResetHomePage);
            }
            else
            {
                Url = homePageUrl;
            }
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
                    IsNavigating = true;
                    string newurlText = url.Remove(0, 8).TrimEnd('/');
                    AddressbarText = newurlText;
                }
                else if (url.StartsWith("http"))
                {
                    IsNavigating = true;
                    string newurlText = url.Remove(0, 7).TrimEnd('/');
                    AddressbarText = newurlText;
                }
                else
                {
                    AddressbarText = url;
                    IsNavigating = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        internal async Task InitilizeSessionAsync()
        {
            // TODO: Connect using hardcoded response, provide option to authenticate using Authenticator
            await AuthService.ConnectUsingHardcodedResponseAsync();
            AppService = new AppService();
        }

        public void OnTapped(string navigationBarIconString)
        {
            switch (navigationBarIconString)
            {
                case "Back":
                    GoBackCommand.Execute(null);
                    break;
                case "Forward":
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

        public void LoadUrl(string url)
        {
            AddressbarText = url;
            LoadUrl();
        }

        private void LoadUrl()
        {
            if (Device.RuntimePlatform == Device.iOS)
                Url = $"safe://{AddressbarText}";
            else
                Url = $"https://{AddressbarText}";
        }
    }
}
