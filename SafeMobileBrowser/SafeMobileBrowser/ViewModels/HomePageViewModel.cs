﻿// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Text.RegularExpressions;
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
        private MenuPopUp _menuPopUp;

        public static string CurrentUrl { get; private set; }

        public static string CurrentTitle { get; private set; }

        public string WelcomePageUrl => $"{DependencyService.Get<IPlatformService>().BaseUrl}/index.html";

        public ICommand PageLoadCommand { get; }

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

        public ICommand FetchPreviousVersionCommand { get; set; }

        public ICommand FetchNextVersionCommand { get; set; }

        public ICommand AuthenticateBrowserCommand { get; set; }

        private int _isAuthenticated;

        public int IsAuthenticated
        {
            get => _isAuthenticated;
            set => SetProperty(ref _isAuthenticated, value);
        }

        private bool _canFetchPreviousVersion;

        public bool CanFetchPreviousVersion
        {
            get => _canFetchPreviousVersion;
            set => SetProperty(ref _canFetchPreviousVersion, value);
        }

        private bool _canFetchNextVersion;

        public bool CanFetchNextVersion
        {
            get => _canFetchNextVersion;
            set => SetProperty(ref _canFetchNextVersion, value);
        }

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

        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        private bool _showVersionChangeControls;

        public bool ShowVersionChangeControls
        {
            get => _showVersionChangeControls;
            set => SetProperty(ref _showVersionChangeControls, value);
        }

        private ulong _currentWebPageVersion;

        public ulong CurrentWebPageVersion
        {
            get => _currentWebPageVersion;
            set => SetProperty(ref _currentWebPageVersion, value);
        }

        private ulong _latestWebPageVersion;

        public ulong LatestWebPageVersion
        {
            get => _latestWebPageVersion;
            set => SetProperty(ref _latestWebPageVersion, value);
        }

        internal void HideVersionChangeControls()
        {
            if (ShowVersionChangeControls)
                ShowVersionChangeControls = false;
        }

        public string ErrorType { get; private set; }

        public bool IsErrorState { get; set; }

        public bool IsAddressBarFocused { get; set; }

        public INavigation Navigation { get; set; }

        public HomePageViewModel(INavigation navigation)
        {
            ShowVersionChangeControls = false;
            Navigation = navigation;
            PageLoadCommand = new Command<string>(LoadUrl);
            BottomNavbarTapCommand = new Command<string>(OnTapped);
            WebViewNavigatingCommand = new Command<WebNavigatingEventArgs>(OnNavigating);
            WebViewNavigatedCommand = new Command<WebNavigatedEventArgs>(OnNavigated);
            GoToHomePageCommand = new Command(GoToHomePage);
            MenuCommand = new Command(ShowPopUpMenu);
            AddressBarUnfocusCommand = new Command(RestoreAddressBar);
            FetchPreviousVersionCommand = new Command(FetchPreviousVersion);
            FetchNextVersionCommand = new Command(FetchNextVersion);
            AuthenticateBrowserCommand = new Command(AuthenticateBrowser);
        }

        internal void UpdateAuthenticationState(bool? noInternet = null)
        {
            if (noInternet.HasValue)
                IsAuthenticated = -1;

            IsAuthenticated = App.AppSession != null ? 1 : 0;
        }

        private async void AuthenticateBrowser()
        {
            await AuthenticationService.RequestAuthenticationAsync(true);
        }

        private void FetchPreviousVersion()
        {
            if (!CanFetchPreviousVersion)
                return;

            var currentUrl = ((UrlWebViewSource)Url).Url.TrimEnd('/');
            if (currentUrl.Contains("?v="))
            {
                var versionText = "?v=";
                var versionStringIndex = currentUrl.LastIndexOf(versionText);
                var urlSubString = currentUrl.Substring(0, versionStringIndex);
                LoadUrl($"{urlSubString}?v={--CurrentWebPageVersion}");
            }
            else
            {
                LoadUrl($"{currentUrl}?v={--CurrentWebPageVersion}");
            }
        }

        private void FetchNextVersion()
        {
            if (!CanFetchNextVersion)
                return;

            var currentUrl = ((UrlWebViewSource)Url).Url.TrimEnd('/');
            if (currentUrl.Contains("?v="))
            {
                var versionText = "?v=";
                var versionStringIndex = currentUrl.LastIndexOf(versionText);
                var urlSubString = currentUrl.Substring(0, versionStringIndex);
                LoadUrl($"{urlSubString}?v={++CurrentWebPageVersion}");
            }
            else
            {
                LoadUrl($"{currentUrl}?v={++CurrentWebPageVersion}");
            }
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

            MessagingCenter.Send((App)Application.Current, MessageCenterConstants.UpdateWelcomePageTheme);
            if (obj.NavigationEvent == WebNavigationEvent.Refresh)
                IsRefreshing = false;

            IsNavigating = false;

            if (!IsErrorState && CanGoToHomePage)
                ShowVersionChangeControls = true;

            CanFetchPreviousVersion = CurrentWebPageVersion > 0;
            CanFetchNextVersion = CurrentWebPageVersion < LatestWebPageVersion;
        }

        private void OnNavigating(WebNavigatingEventArgs args)
        {
            try
            {
                ShowVersionChangeControls = false;

                var url = args.Url;
                SetAddressBarText(url);

                if (!IsNavigating)
                    IsNavigating = true;

                if (args.NavigationEvent == WebNavigationEvent.Refresh && CurrentUrl != args.Url.Replace("https://", "safe://"))
                    IsRefreshing = true;
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
                var result = await Application.Current.MainPage.DisplayAlert(
                                "Authentication",
                                "You can connect to a section of your choice using the Authenticator app, or you can connect to the MaidSafe hosted shared section.",
                                "Use authenticator app",
                                "Use MaidSafe shared section");
                if (result)
                    await AuthenticationService.RequestAuthenticationAsync(true);
                else
                    await AuthenticationService.DownloadMaidSafeSharedSectionVault();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                await Application.Current.MainPage.DisplayAlert(
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
            var newUrlText = url;

            if (url.StartsWith("file://") && !IsErrorState)
            {
                AddressbarText = string.Empty;
                return;
            }

            if (url.Contains(Constants.BufferText))
            {
                url = url.Replace(Constants.BufferText, string.Empty);
            }

            if (url.StartsWith("safe://"))
            {
                newUrlText = url.Replace("safe://", string.Empty).TrimEnd('/');
            }
            else if (url.StartsWith("https://"))
            {
                newUrlText = url.Replace("https://", string.Empty).TrimEnd('/');
            }
            else if (url.StartsWith("http://"))
            {
                newUrlText = url.Replace("http://", string.Empty).TrimEnd('/');
            }
            AddressbarText = newUrlText;
        }

        public void OnTapped(string navigationBarIconString)
        {
            switch (navigationBarIconString)
            {
                case "Back":
                    if (CanGoBack)
                    {
                        IsNavigating = true;
                        GoBackCommand.Execute(null);
                    }
                    break;
                case "Forward":
                    if (CanGoForward)
                    {
                        IsNavigating = true;
                        GoForwardCommand.Execute(null);
                    }
                    break;
                case "Focus":
                    AddressBarFocusCommand.Execute(null);
                    break;
                case "Home":
                    if (CanGoToHomePage)
                    {
                        GoToHomePage();
                    }
                    break;
                case "Menu":
                    ShowPopUpMenu();
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

                SetAddressBarText(url);

                url = Device.RuntimePlatform == Device.iOS ? $"safe://{AddressbarText}" : $"https://{AddressbarText}";

                if (!IsValidUri(url))
                    return;

                IsNavigating = true;

                string[] hostNames = AddressbarText.Split('.');

                if (Device.RuntimePlatform == Device.Android && Regex.IsMatch(hostNames[hostNames.Length - 1], @"^\d+$"))
                {
                    url = $"{url}buffer-text";
                }

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
