using System.Runtime.InteropServices;
using SafeApp;
using SafeMobileBrowser.Helpers;
using SafeMobileBrowser.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeMobileBrowser
{
    public partial class App : Application
    {
        public static Session AppSession { get; set; }

        public static bool IsConnectedToInternet { get; set; }

        public App([Optional]string url)
        {
            InitializeComponent();

            MainPage = new NavigationPage(new HomePage(url));
            IsConnectedToInternet = Connectivity.NetworkAccess == NetworkAccess.Internet;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                IsConnectedToInternet = true;
                MessagingCenter.Send(
                    this,
                    AppSession == null ? MessageCenterConstants.InitialiseSession : MessageCenterConstants.SessionReconnect);
            }
            else
            {
                IsConnectedToInternet = false;
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            if (Device.RuntimePlatform == Device.iOS)
            {
                MessagingCenter.Send(
                    this,
                    AppSession == null ? MessageCenterConstants.InitialiseSession : MessageCenterConstants.SessionReconnect);
            }
        }
    }
}
