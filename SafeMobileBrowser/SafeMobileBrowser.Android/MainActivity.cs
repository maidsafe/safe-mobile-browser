// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Microsoft.AppCenter.Distribute;
using Plugin.CurrentActivity;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

namespace SafeMobileBrowser.Droid
{
    [Activity(
        LaunchMode = LaunchMode.SingleTask,
        Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(
            new[] { Intent.ActionView },
            Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
            DataScheme = Constants.AppId)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly AuthenticationService _authenticationService = new AuthenticationService();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            var theme = Xamarin.Essentials.Preferences.Get(Constants.AppThemePreferenceKey, false);
            SetTheme(theme ? Resource.Style.MainDarkTheme : Resource.Style.MainTheme);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            Distribute.SetEnabledForDebuggableBuild(true);
            global::Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            XamEffects.Droid.Effects.Init();
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);
            UserDialogs.Init(() => this);
            LoadApplication(new App());
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            if (intent?.Data != null)
            {
                HandleAppLaunch(intent.Data.ToString());
            }
        }

        private void HandleAppLaunch(string url)
        {
            Logger.Info($"Launched via: {url}");
            Device.BeginInvokeOnMainThread(
              async () =>
              {
                  try
                  {
                      await _authenticationService.ProcessAuthenticationResponseAsync(url);
                  }
                  catch (Exception ex)
                  {
                      Logger.Error(ex);
                  }
              });
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
