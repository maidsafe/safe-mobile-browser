using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using SafeMobileBrowser.Helpers;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class BookmarksModalPageViewModel : BaseViewModel
    {
        private readonly TimeSpan _toastTimeSpan = TimeSpan.FromSeconds(1.5);

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
                if (value != null)
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
            DeleteBookmarkCommand = new Command(async obj =>
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
                    await Application.Current.MainPage.DisplayAlert(
                        ErrorConstants.NoInternetConnectionTitle,
                        ErrorConstants.NoInternetConnectionMsg,
                        "Ok");
                    return;
                }

                await BookmarkManager.DeleteBookmarks(bookmark.ToString());
                Bookmarks.Remove((string)bookmark);
                UserDialogs.Instance.Toast(Constants.BookmarkRemovedSuccessfully);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                UserDialogs.Instance.Toast("Failed to remove bookmark", _toastTimeSpan);
            }
        }

        public async void OpenBookmarkedPage()
        {
            var urlToOpen = SelectedBookmarkItem.Replace("safe://", string.Empty);
            MessagingCenter.Send(this, MessageCenterConstants.BookmarkUrl, urlToOpen);
            SelectedBookmarkItem = null;
            await Navigation.PopModalAsync();
        }

        public async Task GetBookmarks()
        {
            try
            {
                Bookmarks = new ObservableCollection<string>(BookmarkManager.RetrieveBookmarks());

                if (!App.IsConnectedToInternet)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        ErrorConstants.NoInternetConnectionTitle,
                        ErrorConstants.BookmarkFetchFailedMsg,
                        "Ok");
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
                await Application.Current.MainPage.DisplayAlert(
                    ErrorConstants.BookmarkFetchFailedTitle,
                    ErrorConstants.BookmarkFetchFailedMsg,
                    "Ok");
            }
        }

        private async void GoBackToHomePage()
        {
            await Navigation.PopModalAsync();
        }
    }
}
