// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using Xamarin.Forms;

namespace SafeMobileBrowser.Controls
{
    public enum ShadowType
    {
        None = 0,
        Top,
        Bottom
    }

    public class ShadowBoxView : BoxView
    {
        public static readonly BindableProperty ShadowTypeProperty = BindableProperty.Create(
            nameof(ShadowType),
            typeof(ShadowType),
            typeof(ShadowBoxView));

        public ShadowType ShadowType
        {
            get => (ShadowType)GetValue(ShadowTypeProperty);
            set => SetValue(ShadowTypeProperty, value);
        }
    }
}
