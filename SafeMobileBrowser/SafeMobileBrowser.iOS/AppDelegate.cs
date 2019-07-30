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

        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Forms.SetFlags("CollectionView_Experimental");
            Rg.Plugins.Popup.Popup.Init();
            XamEffects.iOS.Effects.Init();
            global::Xamarin.Forms.Forms.Init();

            if (launchOptions != null)
            {
                var url = launchOptions["UIApplicationLaunchOptionsURLKey"] as NSUrl;
                LoadApplication(new App(url.ToString()));
            }
            else
            {
                LoadApplication(new App());
            }

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            Device.BeginInvokeOnMainThread(
              async () =>
              {
                  try
                  {
                      var opendUrl = url.ToString();
                      if (opendUrl.ToLower().StartsWith("safe://"))
                      {
                          MessagingCenter.Send((App)Xamarin.Forms.Application.Current, MessageCenterConstants.LoadSafeWebsite, opendUrl);
                      }
                      else if (opendUrl.StartsWith(Constants.AppId))
                      {
                          await _authenticationService.ProcessAuthenticationResponseAsync(opendUrl);
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
