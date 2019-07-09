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
        readonly AuthenticationService authenticationService = new AuthenticationService();

        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Forms.SetFlags("CollectionView_Experimental");
            Rg.Plugins.Popup.Popup.Init();
            XamEffects.iOS.Effects.Init();
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());
            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            Device.BeginInvokeOnMainThread(
              async () =>
              {
                  try
                  {
                      await authenticationService.ProcessAuthenticationResponseAsync(url.ToString());
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
