using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        public static BookmarkManager BookmarkManager { get; set; }

        public static AppService AppService { get; set; }

        public AuthenticationService AuthService => DependencyService.Get<AuthenticationService>();

        bool isBusy = false;

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
    }
}
