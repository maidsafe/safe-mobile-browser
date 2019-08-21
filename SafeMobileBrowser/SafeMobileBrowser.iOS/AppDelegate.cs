using System;
using Foundation;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Services;
using UIKit;
using Xamarin.Forms;

namespace SafeMobileBrowser.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private readonly AuthenticationService _authenticationService = new AuthenticationService();
        private NSUrl _launchUrl;

        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Forms.SetFlags("CollectionView_Experimental");
            Rg.Plugins.Popup.Popup.Init();
            XamEffects.iOS.Effects.Init();
            global::Xamarin.Forms.Forms.Init();

            if (launchOptions != null)
            {
                _launchUrl = launchOptions["UIApplicationLaunchOptionsURLKey"] as NSUrl;
            }

            LoadApplication(_launchUrl == null ? new App() : new App(_launchUrl.ToString()));

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            Device.BeginInvokeOnMainThread(
              async () =>
              {
                  try
                  {
                      var launchUrl = url.ToString();
                      if (launchUrl.ToLower().StartsWith("safe://"))
                      {
                          MessagingCenter.Send((App)Xamarin.Forms.Application.Current, MessageCenterConstants.LoadSafeWebsite, launchUrl);
                      }
                      else if (launchUrl.StartsWith(Constants.AppId))
                      {
                          await _authenticationService.ProcessAuthenticationResponseAsync(launchUrl);
                      }
                      Logger.Info("IPC Msg Handling Completed");
                  }
                  catch (Exception ex)
                  {
                      Logger.Error(ex);
                  }
              });
            return true;
        }
    }
}
