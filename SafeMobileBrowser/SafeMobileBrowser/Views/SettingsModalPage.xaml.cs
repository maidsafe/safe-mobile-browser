using SafeMobileBrowser.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMobileBrowser.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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
    }
}
