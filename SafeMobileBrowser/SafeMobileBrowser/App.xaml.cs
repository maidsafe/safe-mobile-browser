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

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.White };
            IsConnectedToInternet = Connectivity.NetworkAccess == NetworkAccess.Internet;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                IsConnectedToInternet = true;
                if (App.AppSession == null)
                    MessagingCenter.Send(this, MessageCenterConstants.InitialiseSession);
                else
                    AppSession.ReconnectAsync();
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
        }
    }
}
