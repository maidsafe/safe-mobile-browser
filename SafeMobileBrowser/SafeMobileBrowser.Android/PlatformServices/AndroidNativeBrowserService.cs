using Android.App;
using Android.Content;
using Android.Support.CustomTabs;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.Droid.PlatformServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidNativeBrowserService))]

namespace SafeMobileBrowser.Droid.PlatformServices
{
    public class AndroidNativeBrowserService : INativeBrowserService
    {
        public void LaunchNativeEmbeddedBrowser(string url)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var activity = Forms.Context as Activity;
#pragma warning restore CS0618 // Type or member is obsolete

            if (activity == null)
            {
                return;
            }

            var customTabsActivityManager = new CustomTabsActivityManager(activity);

            customTabsActivityManager.CustomTabsServiceConnected += (name, client) =>
            {
                customTabsActivityManager.LaunchUrl(url);
            };

            if (!customTabsActivityManager.BindService())
            {
                var uri = Android.Net.Uri.Parse(url);
                var intent = new Intent(Intent.ActionView, uri);
                activity.StartActivity(intent);
            }
        }
    }
}
