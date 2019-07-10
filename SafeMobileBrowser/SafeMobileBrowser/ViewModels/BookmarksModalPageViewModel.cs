using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Models;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class BookmarksModalPageViewModel : BaseViewModel
    {
        public ICommand GoBackCommand { get; set; }

        public ICommand DeleteBookmarkCommand { get; set; }

        public INavigation Navigation { get; set; }

        private string _selectedBookmarkItem;

        public string SelectedBookmarkItem
        {
            get => _selectedBookmarkItem;

            set
            {
                SetProperty(ref _selectedBookmarkItem, value);
                OpenBookmarkedPage();
            }
        }

        private ObservableCollection<string> _bookmarks;

        public ObservableCollection<string> Bookmarks
        {
            get => _bookmarks;
            set => SetProperty(ref _bookmarks, value);
        }

        public BookmarksModalPageViewModel(INavigation navigation)
        {
            Navigation = navigation;
            GoBackCommand = new Command(GoBackToHomePage);
            DeleteBookmarkCommand = new Command(async (object obj) =>
            {
                await RemoveBookmark(obj);
            });
            if (Bookmarks == null)
                Bookmarks = new ObservableCollection<string>();
        }

        private async Task RemoveBookmark(object bookmark)
        {
            try
            {
                if (!App.IsConnectedToInternet)
                {
                    await App.Current.MainPage.DisplayAlert("No internet connection", "Please connect to the internet", "Ok");
                    return;
                }

                using (UserDialogs.Instance.Toast("Bookmark removed successfully"))
                {
                    await BookmarkManager.DeleteBookmarks(bookmark.ToString());
                    Bookmarks.Remove((string)bookmark);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                UserDialogs.Instance.Toast("Failed to remove bookmark");
            }
        }

        public async void OpenBookmarkedPage()
        {
            await Navigation.PopModalAsync();
            MessagingCenter.Send(
                this,
                MessageCenterConstants.BookmarkUrl,
                SelectedBookmarkItem.Replace("safe://", string.Empty));
        }

        public async Task GetBookmarks()
        {
            try
            {
                Bookmarks = new ObservableCollection<string>(BookmarkManager.RetrieveBookmarks());

                if (!App.IsConnectedToInternet)
                {
                    await App.Current.MainPage.DisplayAlert("No internet Connection", "Showing previously fetched bookmarks", "Ok");
                    return;
                }
                if (!AppService.IsAccessContainerMDataInfoAvailable)
                {
                    var mdInfo = await AppService.GetAccessContainerMdataInfoAsync();
                    BookmarkManager.SetMdInfo(mdInfo);
                }
                await BookmarkManager.FetchBookmarks();
                Bookmarks = new ObservableCollection<string>(BookmarkManager.RetrieveBookmarks());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                await App.Current.MainPage.DisplayAlert("Unable to fetch bookmarks", "Showing previously fetched bookmarks", "Ok");
            }
        }

        private async void GoBackToHomePage()
        {
            await Navigation.PopModalAsync();
        }
    }
}
