using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        public BookmarkManager BookmarkManager => DependencyService.Get<BookmarkManager>();

        public AppService AppService => DependencyService.Get<AppService>();

        public AuthenticationService AuthService => DependencyService.Get<AuthenticationService>();

        bool isBusy = false;

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
    }
}
