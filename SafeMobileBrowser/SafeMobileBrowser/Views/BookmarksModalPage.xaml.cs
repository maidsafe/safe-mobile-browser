using System;
using System.Collections.Generic;
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
            BindingContext = _viewModel;

            _viewModel.SetBookmarks(await _viewModel.GetBookmarks());
        }
    }
}
