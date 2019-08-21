using System;
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

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
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
