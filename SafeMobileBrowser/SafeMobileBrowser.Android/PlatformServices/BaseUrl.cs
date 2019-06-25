using SafeMobileBrowser.Droid.PlatformServices;
using SafeMobileBrowser.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]

namespace SafeMobileBrowser.Droid.PlatformServices
{
    public class BaseUrl : IBaseUrl
    {
        public string GetBaseUrl()
        {
            return "file:///android_asset/";
        }
    }
}
