using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
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
            set
            {
                SetProperty(ref _bookmarks, value);
            }
        }

        public BookmarksModalPageViewModel(INavigation navigation)
        {
            Navigation = navigation;
            GoBackCommand = new Command(GoBackToHomePage);
            DeleteBookmarkCommand = new Command(async (object obj) =>
            {
                await RemoveBookmark(obj);
            });
            BookmarkManager = new BookmarkManager();
        }

        private async Task RemoveBookmark(object bookmark)
        {
            await BookmarkManager.DeleteBookmarks(bookmark.ToString());
        }

        public async void OpenBookmarkedPage()
        {
            await Navigation.PopModalAsync();
            MessagingCenter.Send(this, MessageCenterConstants.BookmarkUrl, SelectedBookmarkItem.Replace("safe://", string.Empty));
        }

        public void SetBookmarks(List<string> bookmarks)
        {
            Bookmarks = new ObservableCollection<string>(bookmarks);
        }

        public async Task<List<string>> GetBookmarks()
        {
            BookmarkManager.SetSession(AppService.Session);
            BookmarkManager.SetMdInfo(await AppService.GetMdInfoAsync());
            return await BookmarkManager.FetchBookmarks();
        }

        private async void GoBackToHomePage()
        {
            await Navigation.PopModalAsync();
        }
    }
}
