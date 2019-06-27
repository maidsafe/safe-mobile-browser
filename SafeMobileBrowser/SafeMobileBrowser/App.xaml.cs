using SafeApp;
using SafeMobileBrowser.Themes;
using SafeMobileBrowser.Views;
using Xamarin.Forms;

namespace SafeMobileBrowser
{
    public partial class App : Application
    {
        public static Session AppSession { get; set; }

        public App()
        {
            InitializeComponent();
            ThemeHelper.LoadTheme();
            MainPage = new NavigationPage(new HomePage()) { BarBackgroundColor = Color.White };
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
