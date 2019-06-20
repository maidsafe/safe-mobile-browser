﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Extensions;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using SafeMobileBrowser.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using MenuItem = SafeMobileBrowser.Models.MenuItem;

namespace SafeMobileBrowser.ViewModels
{
    public class MenuPopUpViewModel : BaseViewModel
    {
        BookmarksModalPage _bookmarksModalPage;

        public INavigation Navigation { get; set; }

        public ICommand RefreshWebViewCommand { get; set; }

        private List<MenuItem> _popMenuItems;

        public List<MenuItem> PopMenuItems
        {
            get => _popMenuItems;

            set
            {
                SetProperty(ref _popMenuItems, value);
            }
        }

        private MenuItem _selectedPopMenuItem;

        public MenuItem SelectedPopMenuItem
        {
            get => _selectedPopMenuItem;

            set
            {
                if (value != null)
                {
                    SetProperty(ref _selectedPopMenuItem, value);
                    OnPopupMenuSelection();
                }
            }
        }

        public MenuPopUpViewModel(INavigation navigation)
        {
            Navigation = navigation;
            RefreshWebViewCommand = new Command(RefreshWebView);
            InitaliseMenuItems();
        }

        private void RefreshWebView(object obj)
        {
            throw new NotImplementedException();
        }

        internal void InitaliseMenuItems()
        {
            PopMenuItems = new List<MenuItem>
            {
                new MenuItem { MenuItemTitle = "Settings", MenuItemIcon = IconFont.Settings },
                new MenuItem { MenuItemTitle = "Bookmarks", MenuItemIcon = IconFont.BookmarkPlusOutline },
                new MenuItem { MenuItemTitle = "Authenticate", MenuItemIcon = IconFont.Web },
                new MenuItem { MenuItemTitle = "Share", MenuItemIcon = IconFont.ShareVariant }
            };
        }

        private async void OnPopupMenuSelection()
        {
            try
            {
                var selectedMenuItemTitle = SelectedPopMenuItem.MenuItemTitle;
                switch (selectedMenuItemTitle)
                {
                    case "Settings":
                        var browserSettingsPage = new BrowserSettingsPage();
                        await Navigation.PushModalAsync(browserSettingsPage);
                        break;
                    case "Bookmarks":
                        if (AppService.IsSessionAvailable)
                        {
                            if (_bookmarksModalPage == null)
                                _bookmarksModalPage = new BookmarksModalPage();
                            await Navigation.PushModalAsync(_bookmarksModalPage);
                        }
                        else
                        {
                            throw new Exception("Please authenticate");
                        }
                        break;
                    case "Authenticate":
                        if (!AppService.IsSessionAvailable)
                            await AuthenticationService.RequestNonMockAuthenticationAsync();
                        break;
                    case "Share":
                        await Share.RequestAsync(new ShareTextRequest
                        {
                            Title = "Some Title",
                            Uri = "Page Uri"
                        });
                        break;
                    default:
                        break;
                }
                await Navigation.PopPopupAsync();
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(
                    this,
                    MessageCenterConstants.DisplayAlertMessage,
                    "Unable to Authenticate");
                Debug.WriteLine(ex);
            }
            SelectedPopMenuItem = null;
        }
    }
}