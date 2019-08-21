using Android.Content;
using Android.Graphics.Drawables;
using Android.Text;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.Droid.ControlRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AddressBarEntry), typeof(CustomEntryRenderer))]

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            var gd = new GradientDrawable();
            gd.SetColor(global::Android.Graphics.Color.Transparent);
            Control.SetBackground(gd);
            Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
            Control.SetPadding(Control.PaddingLeft, 0, Control.PaddingRight, 0);
        }
    }
}
