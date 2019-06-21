using SafeMobileBrowser.ViewModels;
using Xamarin.Forms;

namespace SafeMobileBrowser.Views
{
    public partial class BookmarksModalPage : ContentPage
    {
        BookmarksModalPageViewModel _viewModel;

        public BookmarksModalPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                _viewModel = new BookmarksModalPageViewModel(Navigation);
            }
            await _viewModel.GetBookmarks();
            BindingContext = _viewModel;
        }
    }
}
