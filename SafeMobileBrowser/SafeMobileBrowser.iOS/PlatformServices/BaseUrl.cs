using Foundation;
using SafeMobileBrowser.iOS.PlatformServices;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]

namespace SafeMobileBrowser.iOS.PlatformServices
{
    public class BaseUrl : IBaseUrl
    {
        public string GetBaseUrl()
        {
            return NSBundle.MainBundle.BundlePath;
        }
    }
}
