using SafeMobileBrowser.ViewModels;
using Xamarin.Forms;

namespace SafeMobileBrowser.Views
{
    public partial class SettingsModalPage : ContentPage
    {
        SettingsModalPageViewModel _viewModel;

        public SettingsModalPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                _viewModel = new SettingsModalPageViewModel(Navigation);
            }

            BindingContext = _viewModel;
        }

        public void OnDarkThemeToggled(object sender, ToggledEventArgs toggledEventArgs)
        {
            if (toggledEventArgs.Value)
            {
                DisplayAlert("Dark Mode", "Coming soon", "Ok");
                AppThemeChangeSwitch.IsToggled = false;
            }
        }
    }
}
