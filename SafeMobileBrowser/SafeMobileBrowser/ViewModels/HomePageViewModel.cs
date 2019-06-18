using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Extensions;
using SafeMobileBrowser.Views;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        public bool IsSessionAvailable => App.AppSession != null ? true : false;

        public ICommand PageLoadCommand { get; private set; }

        public ICommand ToolbarItemCommand { get; private set; }

        public Command BottomNavbarTapCommand { get; set; }

        public ICommand GoBackCommand { get; set; }

        public ICommand GoForwardCommand { get; set; }

        public ICommand AddressBarFocusCommand { get; set; }

        public ICommand ReloadCommand { get; set; }

        public ICommand WebViewNavigatingCommand { get; set; }

        public ICommand WebViewNavigatedCommand { get; set; }

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
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        private string _addressbarText;

        public string AddressbarText
        {
            get { return _addressbarText; }
            set { SetProperty(ref _addressbarText, value); }
        }

        private MenuPopUp _menuPopUp;

        public INavigation Navigation { get; set; }

        public HomePageViewModel(INavigation navigation)
        {
            PageLoadCommand = new Command(LoadUrl);
            ToolbarItemCommand = new Command<string>(LoadUrl);
            BottomNavbarTapCommand = new Command<string>(OnTapped);
            WebViewNavigatingCommand = new Command<WebNavigatingEventArgs>(OnNavigating);
            WebViewNavigatedCommand = new Command<WebNavigatedEventArgs>(OnNavigated);
        }

        private async void ShowCollectionView()
        {
            // if (bookMarkManager == null)
            // {
            //    if(AppService.fetchSession() == null)
            //         AuthService.ProcessNonMockAuthentication();
            //    if(AppService.fetchSession() !=  null)
            //    {
            //        bookMarkManager = new BookmarkManager(AppService.fetchSession());
            //        bookMarkManager.SetMdInfo(await AppService.GetMdInfoAsync());
            //        await GetWebsiteBookmark();
            //    }

            // }
            // else
            // {
            //    await GetWebsiteBookmark();
            // }
            GetWebsiteBookmark();
        }

        private async Task GetWebsiteBookmark()
        {
            try
            {
                if (_menuPopUp == null)
                    _menuPopUp = new MenuPopUp();
                await Navigation.PushPopupAsync(_menuPopUp);

                // var modelPage = new ModelPage();
                // await Navigation.PushModalAsync(modelPage);
            }
            catch (Exception ex)
            {
            }
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
                    string newurlText = url.Remove(0, 8);
                    AddressbarText = newurlText;
                }
                else if (url.StartsWith("http"))
                {
                    IsNavigating = true;
                    string newurlText = url.Remove(0, 7);
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
            await AuthService.ConnectUsingHardcodedResponse();
        }

        public void OnTapped(string navigationBarIconString)
        {
            switch (navigationBarIconString)
            {
                case "Back":
                    if (CanGoBack)
                        GoBackCommand.Execute(null);
                    break;
                case "Forward":
                    if (CanGoForward)
                        GoForwardCommand.Execute(null);
                    break;
                case "Focus":
                    AddressBarFocusCommand.Execute(null);
                    break;
                case "Refresh":
                    ReloadCommand.Execute(null);
                    break;
                default:
                    break;
            }
        }

        private void LoadUrl(string url)
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
