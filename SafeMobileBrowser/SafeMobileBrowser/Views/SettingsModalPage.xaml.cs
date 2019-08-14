using SafeMobileBrowser.Themes;
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
            AppThemeChangeSwitch.IsToggled = ThemeHelper.CurrentTheme() == ThemeHelper.AppThemeMode.Dark;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                _viewModel = new SettingsModalPageViewModel(Navigation);
                BindingContext = _viewModel;
            }
        }
    }
}
