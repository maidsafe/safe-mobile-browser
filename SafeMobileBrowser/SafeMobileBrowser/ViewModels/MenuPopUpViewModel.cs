using System;
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

        private bool _checkIfBookmarked;

        public bool CheckIfAlreadyAvailableInBookmark
        {
            get => _checkIfBookmarked;
            set => SetProperty(ref _checkIfBookmarked, value);
        }

        public ICommand RefreshWebViewCommand { get; set; }

        public ICommand AddBookmarkCommand { get; set; }

        public ICommand RemoveBookmarkCommand { get; set; }

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
                SetProperty(ref _selectedPopMenuItem, value);
                if (value != null)
                {
                    OnPopupMenuSelection();
                }
            }
        }

        public MenuPopUpViewModel(INavigation navigation)
        {
            Navigation = navigation;
            RefreshWebViewCommand = new Command(RefreshWebView);
            AddBookmarkCommand = new Command(AddBookmarkToSAFE);
            RemoveBookmarkCommand = new Command(RemoveBookmark);
            InitaliseMenuItems();
        }

        public void InitialiseBookmarkManager()
        {
            if (BookmarkManager == null)
                BookmarkManager = new BookmarkManager(AppService.Session);
        }

        private void RemoveBookmark()
        {
            var currentUrl = HomePageViewModel.CurrentUrl.Replace("https", "safe");
            if (!string.IsNullOrWhiteSpace(currentUrl) && BookmarkManager != null)
            {
                Task.Run(async () =>
                {
                    await BookmarkManager.DeleteBookmarks(currentUrl);
                    CheckIsBookmarkAvailable();
                });
            }
            Task.Run(async () =>
            {
                await Navigation.PopPopupAsync();
            });
        }

        private void AddBookmarkToSAFE()
        {
            var currentUrl = HomePageViewModel.CurrentUrl;
            InitialiseBookmarkManager();
            if (!string.IsNullOrWhiteSpace(currentUrl))
            {
                Task.Run(async () =>
                {
                    await BookmarkManager.AddBookmark(currentUrl.Replace("https", "safe"));
                    CheckIsBookmarkAvailable();
                });
            }
            Task.Run(async () =>
            {
                await Navigation.PopPopupAsync();
            });
        }

        internal void CheckIsBookmarkAvailable()
        {
            if (AppService.IsSessionAvailable && !string.IsNullOrWhiteSpace(HomePageViewModel.CurrentUrl) && BookmarkManager != null)
            {
                var currentUrl = HomePageViewModel.CurrentUrl.Replace("https", "safe");
                CheckIfAlreadyAvailableInBookmark = BookmarkManager.CheckIfBookmarkAvailable(currentUrl);
            }
            else
            {
                CheckIfAlreadyAvailableInBookmark = false;
            }
        }

        private void RefreshWebView(object obj)
        {
            var currentUrl = HomePageViewModel.CurrentUrl;
            if (!string.IsNullOrWhiteSpace(currentUrl))
            {
                MessagingCenter.Send(
                    this,
                    MessageCenterConstants.ReloadMessage);
            }
            Task.Run(async () =>
            {
                await Navigation.PopPopupAsync();
            });
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
                            InitialiseBookmarkManager();
                            await Navigation.PushModalAsync(_bookmarksModalPage);
                        }
                        else
                        {
                            throw new Exception("Please authenticate");
                        }
                        break;
                    case "Authenticate":
                        if (!AppService.IsSessionAvailable)
                        {
                            await AuthenticationService.RequestNonMockAuthenticationAsync();
                        }
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
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(
                    this,
                    MessageCenterConstants.DisplayAlertMessage,
                    "Unable to Authenticate");
                Debug.WriteLine(ex);
            }

            // Todo: delay in popup. Needs refactoring
            await Navigation.PopPopupAsync();
            SelectedPopMenuItem = null;
        }
    }
}
