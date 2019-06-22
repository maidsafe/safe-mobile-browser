using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;

namespace SafeMobileBrowser.Droid
{
    [Activity(
        Label = "SAFE Browser",
        Icon = "@mipmap/ic_launcher",
        RoundIcon = "@mipmap/ic_launcher_round",
        Theme = "@style/MyTheme.Splash",
        MainLauncher = true,
        NoHistory = true,
        LaunchMode = LaunchMode.SingleTask)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(typeof(MainActivity));
            OverridePendingTransition(Resource.Drawable.fade_in, Resource.Drawable.fade_out);
        }

        public override void OnBackPressed()
        {
        }
    }
}
