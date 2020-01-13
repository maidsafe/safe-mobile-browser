// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using Foundation;
using SafeMobileBrowser.iOS.PlatformServices;
using SafeMobileBrowser.Themes;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(NativeThemeManager))]

namespace SafeMobileBrowser.iOS.PlatformServices
{
    public class NativeThemeManager : INativeThemeManager
    {
        private readonly UIColor _darkStatusBarColor = UIColor.FromRGB(33, 33, 33);
        private readonly UIColor _lightStatusBarColor = UIColor.White;
        private readonly NSString _statusBarKey = new NSString("statusBar");

        public void ChangeAppTheme(ThemeHelper.AppThemeMode theme, bool isAppLaunched)
        {
            // Implementation
            // switch (theme)
            // {
            //    case ThemeHelper.AppThemeMode.Light:
            //        Device.BeginInvokeOnMainThread(() =>
            //        {
            //            if (UIApplication.SharedApplication.ValueForKey(_statusBarKey) is UIView statusBar)
            //                statusBar.BackgroundColor = _lightStatusBarColor;
            //        });
            //        break;
            //    case ThemeHelper.AppThemeMode.Dark:
            //        Device.BeginInvokeOnMainThread(() =>
            //        {
            //            if (UIApplication.SharedApplication.ValueForKey(_statusBarKey) is UIView statusBar)
            //                statusBar.BackgroundColor = _darkStatusBarColor;
            //        });
            //        break;
            // }
        }
    }
}
