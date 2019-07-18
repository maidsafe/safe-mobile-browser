using System;
using System.Collections.Generic;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.ViewModels;
using Xamarin.Forms;

namespace SafeMobileBrowser.Views
{
    public partial class HomePage : ContentPage
    {
        List<string> _websiteList;
        HomePageViewModel _viewModel;
        bool _isLogInitialised;

        public HomePage()
        {
            InitializeComponent();

            HybridWebViewControl.Navigating += (s, e) =>
            {
                _viewModel.WebViewNavigatingCommand.Execute(e);
            };

            HybridWebViewControl.Navigated += (s, e) =>
            {
                _viewModel.WebViewNavigatedCommand.Execute(e);
            };

            AddressBarEntry.Focused += EntryFocused;
            AddressBarEntry.Unfocused += EntryUnfocused;
            AddressBarEntry.TextChanged += TextChanged;
            AddressBarEntry.Completed += (s, e) =>
            {
                _viewModel.PageLoadCommand.Execute(null);
            };

            MessagingCenter.Subscribe<BookmarksModalPageViewModel, string>(
                this,
                MessageCenterConstants.BookmarkUrl,
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
               (sender) =>
               {
                   if (((UrlWebViewSource)HybridWebViewControl.Source).Url != _viewModel.WelcomePageUrl)
                   {
                       HybridWebViewControl.Source = _viewModel.WelcomePageUrl;
                   }
                   else
                   {
                       UpdateHTMLPageToShowError();
                       _viewModel.IsErrorState = false;
                       if (_viewModel.IsNavigating)
                           _viewModel.IsNavigating = false;
                   }
               });
            MessagingCenter.Subscribe<HomePageViewModel>(
               this,
               MessageCenterConstants.UpdateErrorMsg,
               (sender) =>
               {
                   UpdateHTMLPageToShowError();
               });
        }

        private void UpdateHTMLPageToShowError()
        {
            var errorData = GenerateErrorMessage(_viewModel.ErrorType);
            var jsToEvaluate = $"javascript: ChangePageContent (" +
            $"'{errorData.Item1}'," +
            $"'{errorData.Item2}'," +
            $" true)";

            Logger.Info(jsToEvaluate);
            HybridWebViewControl.Eval(jsToEvaluate);
        }

        private (string, string) GenerateErrorMessage(string errorType)
        {
            switch (errorType)
            {
                case ErrorConstants.InvalidUrl:
                    return (ErrorConstants.InvalidUrlTitle, ErrorConstants.InvalidUrlMsg);
                case ErrorConstants.SessionNotAvailable:
                    return (ErrorConstants.SessionNotAvailableTitle, ErrorConstants.SessionNotAvailableMsg);
                case ErrorConstants.NoInternetConnection:
                    return (ErrorConstants.NoInternetConnectionTitle, ErrorConstants.NoInternetConnectionMsg);
                default:
                    return (ErrorConstants.NoInternetConnectionTitle, ErrorConstants.NoInternetConnectionMsg);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!_isLogInitialised)
            {
                _isLogInitialised = await FileHelper.TransferAssetFilesAndInitLoggingAsync();
            }

            if (_viewModel == null)
            {
                _viewModel = new HomePageViewModel(Navigation);
                BindingContext = _viewModel;
            }

            if (App.AppSession == null)
                await _viewModel.InitilizeSessionAsync();

#if DEV_BUILD
            if (Device.RuntimePlatform == Device.Android)
                AddWebsiteList();
#endif
        }

        ~HomePage()
        {
            MessagingCenter.Unsubscribe<BookmarksModalPageViewModel, string>(
                this,
                MessageCenterConstants.BookmarkUrl);
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

        private void AddWebsiteList()
        {
            if (_websiteList == null)
                _websiteList = WebsiteList.GetWebsiteList();

            if (ToolbarItems.Count == 0)
            {
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
        }

        public void ClearAddressBar(object sender, EventArgs args)
        {
            AddressBarEntry.Unfocus();
            AddressBarEntry.Text = string.Empty;
            AddressBarEntry.Focus();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
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

        private async void EntryUnfocused(object sender, FocusEventArgs e)
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                AddressBarButton.IsVisible = false;
                SafeLabel.ScaleTo(1, 250, Easing.CubicOut);
                SafeLabel.FadeTo(100, 250);
                AddressBarEntry.TranslateTo(0, 0, 250, Easing.CubicOut);
                AddressBarEntry.WidthRequest -= SafeLabel.WidthRequest;
            });
            _viewModel.AddressBarUnfocusCommand.Execute(null);
        }

        private void EntryFocused(object sender, FocusEventArgs e)
        {
            SafeLabel.FadeTo(0, 250);
            SafeLabel.ScaleTo(0, 250, Easing.CubicIn);
            AddressBarEntry.TranslateTo(-SafeLabel.Width, 0, 250, Easing.CubicIn);
            if (AddressBarEntry.Text.Length > 0)
            {
                AddressBarEntry.SelectionLength = AddressBarEntry.Text.Length;
                AddressBarButton.IsVisible = true;
            }
        }
    }
}
