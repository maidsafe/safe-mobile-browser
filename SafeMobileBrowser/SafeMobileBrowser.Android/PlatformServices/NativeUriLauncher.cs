using System.Threading.Tasks;
using Android.Content;
using SafeMobileBrowser.Droid.PlatformServices;
using SafeMobileBrowser.Services;

[assembly: Xamarin.Forms.Dependency(typeof(NativeUriLauncher))]

namespace SafeMobileBrowser.Droid.PlatformServices
{
    class NativeUriLauncher : INativeUriLauncher
    {
        public Task<bool> LaunchApp(string uri)
        {
            bool result = false;

            try
            {
                var aUri = Android.Net.Uri.Parse(uri.ToString());
                var intent = new Intent(Intent.ActionView, aUri);
#pragma warning disable CS0618 // Type or member is obsolete
                Xamarin.Forms.Forms.Context.StartActivity(intent);
#pragma warning restore CS0618 // Type or member is obsolete
                result = true;
            }
            catch (ActivityNotFoundException)
            {
                result = false;
            }

            return Task.FromResult(result);
        }
    }
}
