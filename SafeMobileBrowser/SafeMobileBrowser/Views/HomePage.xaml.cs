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
                    _viewModel.ReloadCommand.Execute(null);
                });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                _viewModel = new HomePageViewModel(Navigation);
                await _viewModel.InitilizeSessionAsync();
            }

            BindingContext = _viewModel;

            if (Device.RuntimePlatform == Device.Android)
                AddWebsiteList();

            AddressBarEntry.Completed += (s, e) =>
            {
                _viewModel.PageLoadCommand.Execute(null);
            };
        }

        ~HomePage()
        {
            MessagingCenter.Unsubscribe<BookmarksModalPageViewModel, string>(
                this,
                MessageCenterConstants.BookmarkUrl);
            MessagingCenter.Unsubscribe<MenuPopUpViewModel>(
                this,
                MessageCenterConstants.ReloadMessage);
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
                    item.SetBinding(Xamarin.Forms.MenuItem.CommandProperty, new Binding("ToolbarItemCommand"));
                    ToolbarItems.Add(item);
                }
            }
        }

        public void ClearAddressBar(object sender, EventArgs args)
        {
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

        private void EntryUnfocused(object sender, FocusEventArgs e)
        {
            AddressBarButton.IsVisible = false;
            AddressBarEntry.TranslateTo(0, 0, 250, Easing.CubicInOut);
            SafeLabel.TranslateTo(0, 0, 250, Easing.CubicInOut);
            SafeLabel.FadeTo(100, 250);
            AddressBarEntry.WidthRequest -= SafeLabel.WidthRequest;
        }

        private void EntryFocused(object sender, FocusEventArgs e)
        {
            SafeLabel.FadeTo(0, 250);
            SafeLabel.TranslateTo(-SafeLabel.Width, 0, 250, Easing.CubicIn);
            AddressBarEntry.TranslateTo(-SafeLabel.Width, 0, 250, Easing.CubicIn);
            AddressBarEntry.WidthRequest += SafeLabel.WidthRequest;
            if (AddressBarEntry.Text.Length > 0)
            {
                AddressBarEntry.SelectionLength = AddressBarEntry.Text.Length;
                AddressBarButton.IsVisible = true;
            }
        }
    }
}
