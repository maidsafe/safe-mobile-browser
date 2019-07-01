using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Rg.Plugins.Popup.Extensions;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using SafeMobileBrowser.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class MenuPopUpViewModel : BaseViewModel
    {
        BookmarksModalPage _bookmarksModalPage;
        SettingsModalPage _settingsModalPage;

        public INavigation Navigation { get; set; }

        private bool _checkIfBookmarked;

        public bool CheckIfAlreadyAvailableInBookmark
        {
            get => _checkIfBookmarked;
            set => SetProperty(ref _checkIfBookmarked, value);
        }

        public ICommand RefreshWebViewCommand { get; set; }

        public ICommand ManageBookmarkCommand { get; set; }

        private ObservableCollection<PopUpMenuItem> _popMenuItems;

        public ObservableCollection<PopUpMenuItem> PopMenuItems
        {
            get => _popMenuItems;

            set
            {
                SetProperty(ref _popMenuItems, value);
            }
        }

        private PopUpMenuItem _reloadMenuItem;

        public PopUpMenuItem ReloadMenuItem
        {
            get => _reloadMenuItem;

            set
            {
                SetProperty(ref _reloadMenuItem, value);
            }
        }

        private PopUpMenuItem _bookmarkMenuItem;

        public PopUpMenuItem BookmarkMenuItem
        {
            get => _bookmarkMenuItem;

            set
            {
                SetProperty(ref _bookmarkMenuItem, value);
            }
        }

        private PopUpMenuItem _selectedPopMenuItem;

        public PopUpMenuItem SelectedPopMenuItem
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
            ManageBookmarkCommand = new Command(AddOrRemoveBookmark);
            InitaliseMenuItems();
        }

        private void AddOrRemoveBookmark()
        {
            if (CheckIfAlreadyAvailableInBookmark)
            {
                RemoveBookmark();
            }
            else
            {
                AddBookmarkToSAFE();
            }
        }

        private void RemoveBookmark()
        {
            var currentUrl = HomePageViewModel.CurrentUrl.Replace("https", "safe");
            if (!string.IsNullOrWhiteSpace(currentUrl) && AppService.IsSessionAvailable)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        using (UserDialogs.Instance.Toast("Bookmark removed successfully"))
                        {
                            await BookmarkManager.DeleteBookmarks(currentUrl);
                            CheckIsBookmarkAvailable();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        UserDialogs.Instance.Toast("Failed to remove bookmark");
                    }
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
            if (!string.IsNullOrWhiteSpace(currentUrl) && AppService.IsSessionAvailable)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        using (UserDialogs.Instance.Toast("Bookmark added successfully"))
                        {
                            await BookmarkManager.AddBookmark(currentUrl.Replace("https", "safe"));
                            CheckIsBookmarkAvailable();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        UserDialogs.Instance.Toast("Failed to add bookmark");
                    }
                });
            }
            Task.Run(async () =>
            {
                await Navigation.PopPopupAsync();
            });
        }

        internal void UpdateMenuItemsStatus()
        {
            CheckIsBookmarkAvailable();
            UpdatePopMenuItemStates();
        }

        internal void UpdatePopMenuItemStates()
        {
            var shareMenuItem = PopMenuItems.FirstOrDefault(p => string.Equals(p.MenuItemTitle, "Share"));

            if (!string.IsNullOrWhiteSpace(HomePageViewModel.CurrentUrl))
            {
                shareMenuItem.IsEnabled = true;
                ReloadMenuItem.IsEnabled = true;
                BookmarkMenuItem.IsEnabled = true;
            }
            else
            {
                shareMenuItem.IsEnabled = false;
                ReloadMenuItem.IsEnabled = false;
                BookmarkMenuItem.IsEnabled = false;
            }

            BookmarkMenuItem.MenuItemIcon = CheckIfAlreadyAvailableInBookmark ? IconFont.Bookmark : IconFont.BookmarkOutline;
            BookmarkMenuItem.IsEnabled = AppService.IsSessionAvailable;

            var bookmarksMenuItem = PopMenuItems.FirstOrDefault(p => string.Equals(p.MenuItemTitle, "Bookmarks"));
            bookmarksMenuItem.IsEnabled = AppService.IsSessionAvailable;

            var authenticationMenuItem = PopMenuItems.FirstOrDefault(p => string.Equals(p.MenuItemTitle, "Authenticate"));
            authenticationMenuItem.IsEnabled = !AppService.IsSessionAvailable;
        }

        internal void CheckIsBookmarkAvailable()
        {
            if (AppService.IsSessionAvailable && !string.IsNullOrWhiteSpace(HomePageViewModel.CurrentUrl))
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
            PopMenuItems = new ObservableCollection<PopUpMenuItem>
            {
                new PopUpMenuItem { MenuItemTitle = "Settings", MenuItemIcon = IconFont.Settings, IsEnabled = true },
                new PopUpMenuItem { MenuItemTitle = "Bookmarks", MenuItemIcon = IconFont.BookmarkPlusOutline, },
                new PopUpMenuItem { MenuItemTitle = "Authenticate", MenuItemIcon = IconFont.Web, IsEnabled = true },
                new PopUpMenuItem { MenuItemTitle = "Share", MenuItemIcon = IconFont.ShareVariant }
            };

            ReloadMenuItem = new PopUpMenuItem { MenuItemTitle = "Reload", MenuItemIcon = IconFont.Reload };
            BookmarkMenuItem = new PopUpMenuItem { MenuItemTitle = "Bookmark", MenuItemIcon = IconFont.BookmarkOutline };
        }

        private async void OnPopupMenuSelection()
        {
            try
            {
                var selectedMenuItemTitle = SelectedPopMenuItem.MenuItemTitle;
                switch (selectedMenuItemTitle)
                {
                    case "Settings":
                        if (_settingsModalPage == null)
                            _settingsModalPage = new SettingsModalPage();
                        await Navigation.PushModalAsync(_settingsModalPage);
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
                        {
                            await AuthenticationService.RequestNonMockAuthenticationAsync();
                        }
                        break;
                    case "Share":
                        if (HomePageViewModel.CurrentTitle != null && !HomePageViewModel.CurrentTitle.StartsWith("file://"))
                        {
                            await Share.RequestAsync(new ShareTextRequest
                            {
                                Title = HomePageViewModel.CurrentTitle,
                                Uri = HomePageViewModel.CurrentUrl
                            });
                        }
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
                    ex.Message);
                Debug.WriteLine(ex);
            }

            // Todo: delay in popup. Needs refactoring
            await Navigation.PopPopupAsync();
            SelectedPopMenuItem = null;
        }
    }
}
