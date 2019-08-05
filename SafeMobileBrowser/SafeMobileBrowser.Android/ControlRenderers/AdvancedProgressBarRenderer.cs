using System.ComponentModel;
using Android.Content;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.Droid.ControlRenderers;
using SafeMobileBrowser.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdvancedProgressBar), typeof(AdvancedProgressBarRenderer))]

namespace SafeMobileBrowser.Droid.ControlRenderers
{
    public class AdvancedProgressBarRenderer : ProgressBarRenderer
    {
        public AdvancedProgressBarRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ProgressBar.ProgressProperty.PropertyName)
            {
                Logger.Info($"Load Progress: {Element.Progress}");
            }
        }
    }
}
