using Android.App;
using Android.Support.V7.App;

namespace SafeMobileBrowser.Droid
{
    [Activity(
        Label = "SAFE Browser",
        Icon = "@mipmap/ic_launcher",
        RoundIcon = "@mipmap/ic_launcher_round",
        Theme = "@style/MyTheme.Splash",
        MainLauncher = true,
        NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
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
