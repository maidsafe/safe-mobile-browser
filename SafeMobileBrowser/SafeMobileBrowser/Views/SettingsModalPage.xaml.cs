using SafeMobileBrowser.Themes;
using SafeMobileBrowser.ViewModels;
using Xamarin.Forms;

namespace SafeMobileBrowser.Views
{
    public partial class SettingsModalPage : ContentPage
    {
        private SettingsModalPageViewModel _viewModel;

        public SettingsModalPage()
        {
            InitializeComponent();
            AppThemeChangeSwitch.IsToggled = ThemeHelper.CurrentTheme() == ThemeHelper.AppThemeMode.Dark;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel != null)
                return;

            _viewModel = new SettingsModalPageViewModel(Navigation);
            BindingContext = _viewModel;
        }
    }
}
