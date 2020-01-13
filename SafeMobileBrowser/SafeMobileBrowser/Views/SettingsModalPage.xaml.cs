// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

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
