// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Distribute;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppUpdateService))]

namespace SafeMobileBrowser.Services
{
    public class AppUpdateService
    {
        private const string _appCenterSecrets =
            "ios=_IOS_APP_CENTER_SECRET_;android=_ANDROID_APP_CENTER_SECRET_";

        public static void UpdateAppSettings(bool inAppUpdateEnabled) =>
            Preferences.Set(Constants.AppUpdatePreferenceKey, inAppUpdateEnabled);

        public static void UpdateAppCenterUpdateChecks(bool inAppUpdateEnabled) =>
            Distribute.SetEnabledAsync(inAppUpdateEnabled);

        public static bool CheckIfAppUpdateSettingsExists() =>
            Preferences.ContainsKey(Constants.AppUpdatePreferenceKey);

        public static bool GetAppUpdateSettings() =>
            Preferences.Get(Constants.AppUpdatePreferenceKey, false);

        public static async Task<bool> CheckIfUpdateServiceEnabledAsync() =>
            await Distribute.IsEnabledAsync();

        public static void CheckForNewVersion() => Distribute.CheckForUpdate();

        public static void RegisterUpdateService()
        {
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
                Distribute.UpdateTrack = UpdateTrack.Private;

            Distribute.ReleaseAvailable = OnReleaseAvailable;
            AppCenter.Start(_appCenterSecrets, typeof(Distribute));
        }

        private static bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            // Get public properties for the new release
            var versionName = releaseDetails.ShortVersion;
            var versionCodeOrBuildNumber = releaseDetails.Version;
            var releaseNotes = releaseDetails.ReleaseNotes;
            var releaseNotesUrl = releaseDetails.ReleaseNotesUrl;

            if (string.IsNullOrWhiteSpace(releaseNotes))
                releaseNotes = "A new version for this app is available.";

            // custom dialog
            var title = $"New version available {versionName} !";
            Task answer;

            answer = Application.Current.MainPage.DisplayAlert(title, releaseNotes, "Download and install now", "later");

            answer.ContinueWith((task) =>
            {
                // If mandatory or if answer was positive, then update the app
                if (releaseDetails.MandatoryUpdate || (task as Task<bool>).Result)
                {
                    Distribute.NotifyUpdateAction(UpdateAction.Update);
                }
                else
                {
                    // Postpone the update for 1 day.
                    // This method call is ignored by the SDK if the update is mandatory
                    Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                }
            });

            return true;
        }
    }
}
