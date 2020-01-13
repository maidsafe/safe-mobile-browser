// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System.Windows.Input;
using Xamarin.Forms;

namespace SafeMobileBrowser.Controls
{
    public class AddressBarEntry : Entry
    {
        public static readonly BindableProperty AddressBarFocusCommandProperty = BindableProperty.Create(
            nameof(AddressBarFocusCommand),
            typeof(ICommand),
            typeof(AddressBarEntry),
            null,
            BindingMode.OneWayToSource);

        public ICommand AddressBarFocusCommand
        {
            get => (ICommand)GetValue(AddressBarFocusCommandProperty);
            set => SetValue(AddressBarFocusCommandProperty, value);
        }

        public AddressBarEntry()
        {
            AddressBarFocusCommand = new Command(() => Focus());
        }
    }
}
