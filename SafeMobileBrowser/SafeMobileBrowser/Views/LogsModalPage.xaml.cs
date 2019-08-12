using SafeMobileBrowser.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeMobileBrowser.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogsModalPage : ContentPage
    {
        private LogsModalPageViewModel _viewModel;

        public LogsModalPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
                _viewModel = new LogsModalPageViewModel(Navigation);

            BindingContext = _viewModel;
        }
    }
}
