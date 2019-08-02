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

        public void OnToggled(object sender, ToggledEventArgs args)
        {
            _viewModel.ToggleThemeCommand.Execute(args.Value);
        }
    }
}
