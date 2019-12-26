using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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
        private SettingsModalPage _settingsModalPage;

        public INavigation Navigation { get; set; }

        public ICommand RefreshWebViewCommand { get; set; }

        public ICommand PopupMenuItemTappedCommand { get; set; }

        private ObservableCollection<PopUpMenuItem> _popMenuItems;

        public ObservableCollection<PopUpMenuItem> PopMenuItems
        {
            get => _popMenuItems;
            set => SetProperty(ref _popMenuItems, value);
        }

        private PopUpMenuItem _reloadMenuItem;

        public PopUpMenuItem ReloadMenuItem
        {
            get => _reloadMenuItem;
            set => SetProperty(ref _reloadMenuItem, value);
        }

        public MenuPopUpViewModel(INavigation navigation)
        {
            Navigation = navigation;
            RefreshWebViewCommand = new Command(RefreshWebView);
            PopupMenuItemTappedCommand = new Command<string>(OnPopupMenuSelection);
            InitaliseMenuItems();
        }

        internal void UpdateMenuItemsStatus()
        {
            UpdatePopMenuItemStates();
        }

        internal void UpdatePopMenuItemStates()
        {
            var shareMenuItem = PopMenuItems.First(p => string.Equals(p.MenuItemTitle, "Share"));

            if (!string.IsNullOrWhiteSpace(HomePageViewModel.CurrentUrl))
            {
                shareMenuItem.IsEnabled = true;
                ReloadMenuItem.IsEnabled = true;
            }
            else
            {
                shareMenuItem.IsEnabled = false;
                ReloadMenuItem.IsEnabled = false;
            }

            // var bookmarksMenuItem = PopMenuItems.First(p => string.Equals(p.MenuItemTitle, "Bookmarks"));
            // bookmarksMenuItem.IsEnabled = AppService.IsSessionAvailable;

            var authenticationMenuItem = PopMenuItems.First(p => string.Equals(p.MenuItemTitle, "Authenticate"));
            authenticationMenuItem.IsEnabled = !AppService.IsSessionAvailable;
        }

        private void RefreshWebView(object obj)
        {
            if (!App.IsConnectedToInternet)
            {
                Application.Current.MainPage.DisplayAlert(
                    ErrorConstants.NoInternetConnectionTitle,
                    ErrorConstants.NoInternetConnectionMsg,
                    "Ok");
                return;
            }
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

                // new PopUpMenuItem { MenuItemTitle = "Bookmarks", MenuItemIcon = IconFont.BookmarkPlusOutline, },
                new PopUpMenuItem { MenuItemTitle = "Authenticate", MenuItemIcon = IconFont.Web, IsEnabled = true },
                new PopUpMenuItem { MenuItemTitle = "Share", MenuItemIcon = IconFont.ShareVariant }
            };

            ReloadMenuItem = new PopUpMenuItem { MenuItemTitle = "Reload", MenuItemIcon = IconFont.Reload };
        }

        private async void OnPopupMenuSelection(string selectedMenuItemTitle)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await OnPopupMenuItemSelection(selectedMenuItemTitle);
            });

            if (Device.RuntimePlatform == Device.Android || selectedMenuItemTitle != "Share")
                await Navigation.PopPopupAsync(false);
        }

        private async Task OnPopupMenuItemSelection(string selectedMenuItemTitle)
        {
            try
            {
                switch (selectedMenuItemTitle)
                {
                    case "Settings":
                        if (_settingsModalPage == null)
                            _settingsModalPage = new SettingsModalPage();
                        await Navigation.PushModalAsync(_settingsModalPage);
                        break;
                    case "Authenticate":
                        if (!AppService.IsSessionAvailable)
                        {
                            await AuthenticationService.RequestAuthenticationAsync(true);
                        }
                        break;
                    case "Share":
                        if (!string.IsNullOrWhiteSpace(HomePageViewModel.CurrentUrl) && HomePageViewModel.CurrentUrl.StartsWith("safe://"))
                        {
                            await Share.RequestAsync(new ShareTextRequest
                            {
                                Title = HomePageViewModel.CurrentTitle,
                                Uri = HomePageViewModel.CurrentUrl
                            });
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MessagingCenter.Send(
                    this,
                    MessageCenterConstants.DisplayAlertMessage,
                    ex.Message);
            }
        }
    }
}
