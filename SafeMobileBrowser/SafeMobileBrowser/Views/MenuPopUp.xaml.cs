using Rg.Plugins.Popup.Pages;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.ViewModels;
using Xamarin.Forms;

namespace SafeMobileBrowser.Views
{
    public partial class MenuPopUp : PopupPage
    {
        MenuPopUpViewModel _viewModel;

        public MenuPopUp()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<MenuPopUpViewModel, string>(
                this,
                MessageCenterConstants.DisplayAlertMessage,
                async (sender, args) =>
                {
                    await DisplayAlert("Selected", args, "Ok");
                });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                _viewModel = new MenuPopUpViewModel(Navigation);
                BindingContext = _viewModel;
            }
            _viewModel.CheckIsBookmarkAvailable();
        }

        ~MenuPopUp()
        {
            MessagingCenter.Unsubscribe<MenuPopUpViewModel, string>(this, MessageCenterConstants.DisplayAlertMessage);
        }
    }
}
