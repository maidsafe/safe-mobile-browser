using SafeMobileBrowser.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMobileBrowser.Views
{
    public partial class BrowserSettingsPage : ContentPage
    {
        BrowserSettingsPageViewModel _viewModel;

        public BrowserSettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                _viewModel = new BrowserSettingsPageViewModel(Navigation);
            }

            BindingContext = _viewModel;
        }
    }
}
