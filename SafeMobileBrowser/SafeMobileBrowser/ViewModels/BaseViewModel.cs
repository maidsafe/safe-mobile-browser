using System;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class BaseViewModel : ObservableObject, IDisposable
    {
        public BookmarkManager BookmarkManager => DependencyService.Get<BookmarkManager>();

        public AppService AppService => DependencyService.Get<AppService>();

        public AuthenticationService AuthService => DependencyService.Get<AuthenticationService>();

        public INativeBrowserService OpenNativeBrowserService => DependencyService.Get<INativeBrowserService>();

        bool isBusy = false;

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public bool IsNotBusy => !IsBusy;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
