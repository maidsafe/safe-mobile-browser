// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SafeMobileBrowser.Views
{
    public partial class HomePage : ContentPage
    {
        private List<string> _websiteList;
        private HomePageViewModel _viewModel;
        private bool _isLogInitialised;
        private string _launchUrl;

        public HomePage([Optional]string url)
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS && !string.IsNullOrWhiteSpace(url) && url.StartsWith("safe://"))
                _launchUrl = url;

            AddressBarEntry.Focused += AddressBarEntryFocused;
            AddressBarEntry.Unfocused += AddressBarEntryUnfocused;
            AddressBarEntry.TextChanged += AddressBarEntryTextChanged;
            AddressBarEntry.Completed += AddressBarTextChangeCompleted;

            MessagingCenter.Subscribe<App, string>(
                this,
                MessageCenterConstants.LoadSafeWebsite,
                (sender, args) =>
                {
                    _viewModel.LoadUrl(args);
                });
            MessagingCenter.Subscribe<MenuPopUpViewModel>(
                this,
                MessageCenterConstants.ReloadMessage,
                (sender) =>
                {
                    _viewModel.IsNavigating = true;
                    _viewModel.ReloadCommand.Execute(null);
                });
            MessagingCenter.Subscribe<HomePageViewModel>(
               this,
               MessageCenterConstants.ShowErrorPage,
               async (sender) =>
               {
                   if (((UrlWebViewSource)HybridWebViewControl.Source).Url != _viewModel.WelcomePageUrl)
                   {
                       HybridWebViewControl.Source = _viewModel.WelcomePageUrl;
                   }
                   else
                   {
                       await UpdateHTMLPageToShowErrorAsync();
                       _viewModel.IsErrorState = false;
                       if (_viewModel.IsNavigating)
                           _viewModel.IsNavigating = false;
                   }
               });
            MessagingCenter.Subscribe<HomePageViewModel>(
               this,
               MessageCenterConstants.UpdateErrorMsg,
               async (sender) =>
               {
                   await UpdateHTMLPageToShowErrorAsync();
               });
            MessagingCenter.Subscribe<App>(
               this,
               MessageCenterConstants.InitialiseSession,
               async (sender) =>
               {
                   await _viewModel.InitilizeSessionAsync();
               });
            MessagingCenter.Subscribe<App>(
                this,
                MessageCenterConstants.SessionReconnect,
                async (sender) =>
                {
                    if (App.AppSession.IsDisconnected)
                        await ReconnectSessionAsync();
                });
            MessagingCenter.Subscribe<App>(
                this,
                MessageCenterConstants.UpdateWelcomePageTheme,
                async (sender) =>
                {
                    var theme = Xamarin.Essentials.Preferences.Get(Constants.AppThemePreferenceKey, false);
                    var jsToEvaluate = "ChangeBackgroundColor (" + $"'{theme.ToString()}'" + ")";
                    await HybridWebViewControl.EvaluateJavaScriptAsync(jsToEvaluate);
                });

            HybridWebViewControl.Navigated += HybridWebViewControl_NavigatedAsync;
        }

        private async void HybridWebViewControl_NavigatedAsync(object sender, WebNavigatedEventArgs e)
        {
            var jsToEvaluate = "isStaticPage()";
            var response = await HybridWebViewControl.EvaluateJavaScriptAsync(jsToEvaluate);
            if (_viewModel != null && bool.TryParse(response, out _))
                _viewModel.HideVersionChangeControls();
        }

        private async Task UpdateHTMLPageToShowErrorAsync()
        {
            var errorData = GenerateErrorMessage(_viewModel.ErrorType);
            var jsToEvaluate = "ChangePageContent (" +
            $"'{errorData.Item1}'," +
            $"'{errorData.Item2}'," +
            " true)";

            await HybridWebViewControl.EvaluateJavaScriptAsync(jsToEvaluate);
        }

        private (string, string) GenerateErrorMessage(string errorType)
        {
            switch (errorType)
            {
                case ErrorConstants.InvalidUrl:
                    return (ErrorConstants.InvalidUrlTitle, ErrorConstants.InvalidUrlMsg);
                default:
                    return (ErrorConstants.ErrorOccured, ErrorConstants.UnableToFetchDataMsg);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (Device.RuntimePlatform == Device.iOS)
            {
                var safeInsets = On<iOS>().SafeAreaInsets();
                safeInsets.Bottom = 10;
                Padding = safeInsets;
            }

            if (!_isLogInitialised)
            {
                _isLogInitialised = await FileHelper.TransferAssetFilesAndInitLoggingAsync();
            }

            if (_viewModel == null)
            {
                _viewModel = new HomePageViewModel(Navigation);
                BindingContext = _viewModel;
            }

            if (App.AppSession == null && !App.PendingRequest)
            {
                await _viewModel.InitilizeSessionAsync();

                if (!string.IsNullOrWhiteSpace(_launchUrl))
                {
                    _viewModel.LoadUrl(_launchUrl);
                    _launchUrl = null;
                }
            }

            // Todo: Enable this later when we have disconnect handler
            // if (App.AppSession != null && App.AppSession.IsDisconnected)
            //    await ReconnectSessionAsync();

#if DEV_BUILD
            // Todo: Enable this later when we have new preview network.
            // if (Device.RuntimePlatform == Device.Android)
            // AddWebsiteList();
#endif
        }

        private async Task ReconnectSessionAsync()
        {
            try
            {
                using (Acr.UserDialogs.UserDialogs.Instance.Loading(Constants.ConnectingProgressText))
                {
                    // await App.AppSession.ReconnectAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                await DisplayAlert(ErrorConstants.ConnectionFailedTitle, ErrorConstants.ConnectionFailedMsg, "OK");
            }
        }

        private void AddWebsiteList()
        {
            if (ToolbarItems.Count != 0)
                return;

            if (_websiteList == null)
                _websiteList = WebsiteList.GetWebsiteList();

            foreach (var url in _websiteList)
            {
                var item = new ToolbarItem
                {
                    Order = ToolbarItemOrder.Secondary,
                    Text = url,
                    CommandParameter = url
                };
                item.SetBinding(MenuItem.CommandProperty, new Binding(nameof(_viewModel.PageLoadCommand)));
                ToolbarItems.Add(item);
            }
        }

        public void ClearAddressBar(object sender, EventArgs args)
        {
            _viewModel.IsAddressBarFocused = true;
            AddressBarEntry.Unfocus();
            AddressBarEntry.Text = string.Empty;
            AddressBarEntry.Focus();
        }

        private void AddressBarEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 0 && AddressBarEntry.IsFocused)
            {
                AddressBarButton.IsVisible = true;
            }
            else
            {
                AddressBarButton.IsVisible = false;
            }
        }

        private async void AddressBarEntryUnfocused(object sender, FocusEventArgs e)
        {
            _viewModel.AddressBarUnfocusCommand.Execute(null);
            await Device.InvokeOnMainThreadAsync(() =>
            {
                AddressBarButton.IsVisible = false;
                SafeLabel.ScaleTo(1, 250, Easing.CubicOut);
                SafeLabel.FadeTo(100);
                AddressBarEntry.TranslateTo(0, 0, 250, Easing.CubicOut);
                AddressBarEntry.WidthRequest -= SafeLabel.WidthRequest;
            });
            _viewModel.IsAddressBarFocused = false;
        }

        private void AddressBarEntryFocused(object sender, FocusEventArgs e)
        {
            SafeLabel.FadeTo(0);
            SafeLabel.ScaleTo(0, 250, Easing.CubicIn);
            AddressBarEntry.TranslateTo(-SafeLabel.Width, 0, 250, Easing.CubicIn);
            if (AddressBarEntry.Text?.Length > 0)
            {
                AddressBarEntry.SelectionLength = AddressBarEntry.Text.Length;
                AddressBarButton.IsVisible = true;
            }
        }

        private void AddressBarTextChangeCompleted(object sender, EventArgs e)
        {
            _viewModel.PageLoadCommand.Execute(null);
        }

        ~HomePage()
        {
            MessagingCenter.Unsubscribe<App>(
               this,
               MessageCenterConstants.UpdateWelcomePageTheme);
            MessagingCenter.Unsubscribe<App>(
                this,
                MessageCenterConstants.InitialiseSession);
            MessagingCenter.Unsubscribe<App>(
                this,
                MessageCenterConstants.SessionReconnect);
            MessagingCenter.Unsubscribe<App>(
                this,
                MessageCenterConstants.LoadSafeWebsite);
            MessagingCenter.Unsubscribe<MenuPopUpViewModel>(
                this,
                MessageCenterConstants.ReloadMessage);
            MessagingCenter.Unsubscribe<HomePageViewModel>(
                this,
                MessageCenterConstants.ShowErrorPage);
            MessagingCenter.Unsubscribe<HomePageViewModel>(
                this,
                MessageCenterConstants.UpdateErrorMsg);
        }
    }
}
