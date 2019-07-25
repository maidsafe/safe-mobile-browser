using System.ComponentModel;
using Android.Content;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.Droid.ControlRenderers;
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

        protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.ScaleY = 4;
                UpdateProgressBar();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == AdvancedProgressBar.IsRunningProperty.PropertyName)
            {
                UpdateProgressBar();
            }
        }

        private void UpdateProgressBar()
        {
            var progressbar = (AdvancedProgressBar)Element;
            Control.Indeterminate = progressbar.IsRunning;
        }
    }
}
