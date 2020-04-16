// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class BaseViewModel : ObservableObject, IDisposable
    {
        public AppService AppService =>
            DependencyService.Get<AppService>();

        public AuthenticationService AuthService =>
            DependencyService.Get<AuthenticationService>();

        public AppUpdateService AppUpdateService =>
            DependencyService.Get<AppUpdateService>();

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
