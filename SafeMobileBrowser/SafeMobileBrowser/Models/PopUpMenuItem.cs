// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

namespace SafeMobileBrowser.Models
{
    public class PopUpMenuItem : ObservableObject
    {
        public string MenuItemTitle { get; set; }

        private string _menuItemIcon;

        public string MenuItemIcon
        {
            get => _menuItemIcon;
            set => SetProperty(ref _menuItemIcon, value);
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;

            set => SetProperty(ref _isEnabled, value);
        }
    }
}
