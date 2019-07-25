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

        protected override void OnDisappearing()
        {
            // This will remove the existing items from the collection view.
            // Once we have next XF version we can remove this.
            BindingContext = null;
            Device.BeginInvokeOnMainThread(() =>
            {
                BindingContext = _viewModel;
            });
        }
    }
}
