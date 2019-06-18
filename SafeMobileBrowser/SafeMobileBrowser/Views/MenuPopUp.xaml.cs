using System.Collections.Generic;
using System.Linq;
using Rg.Plugins.Popup.Pages;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.ViewModels;

namespace SafeMobileBrowser.Views
{
    public partial class MenuPopUp : PopupPage
    {
        MenuPopUpViewModel _viewModel;
        List<MenuItem> menuItems;

        public MenuPopUp()
        {
            InitializeComponent();
            InitilizeMenu();
            MenuPopUpCollection.SelectionChanged += MenuItemSelected;
        }

        private void InitilizeMenu()
        {
            if (menuItems == null)
            {
                menuItems = new List<MenuItem>();
                menuItems.Add(new MenuItem { MenuItemTitle = "Settings", MenuItemIcon = "settings" });
                menuItems.Add(new MenuItem { MenuItemTitle = "Bookmarks", MenuItemIcon = "bookmark" });
                menuItems.Add(new MenuItem { MenuItemTitle = "Authenticate", MenuItemIcon = "authenticate" });
                menuItems.Add(new MenuItem { MenuItemTitle = "Share", MenuItemIcon = "share" });
                MenuPopUpCollection.ItemsSource = menuItems;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                _viewModel = new MenuPopUpViewModel(Navigation);
            }
            BindingContext = _viewModel;
        }

        private async void MenuItemSelected(object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            var activity = (e.CurrentSelection.FirstOrDefault() as MenuItem)?.MenuItemTitle;

            switch (activity)
            {
                case "Settings":
                    // do something
                    await DisplayAlert("Selected", "Settings", "OK");
                    break;
                case "Bookmarks":
                    // open bookmark page
                    // DisplayAlert("Selected", "Bookmarks", "OK");
                    bool isRegisteredSession = await _viewModel.OpenBookmarkPage();
                    if (!isRegisteredSession)
                        await DisplayAlert("Selected", "Please Authenticate", "OK");
                    break;
                case "Authenticate":
                    // authenticate the browser
                    // DisplayAlert("Selected", "Authenticate", "OK");
                    _viewModel.MakeRegisteredConnection();

                    break;
                case "Share":
                    // do something
                    await DisplayAlert("Selected", "Share", "OK");
                    break;
            }
        }
    }
}
