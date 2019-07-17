using System.ComponentModel;
using SafeMobileBrowser.Controls;
using SafeMobileBrowser.iOS.ControlRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AdvancedProgressBar), typeof(AdvancedProgressBarRenderer))]

namespace SafeMobileBrowser.iOS.ControlRenderers
{
    public class AdvancedProgressBarRenderer : ViewRenderer<AdvancedProgressBar, UIView>
    {
        LinearProgressBar _linearProgressBarView;

        protected override void OnElementChanged(ElementChangedEventArgs<AdvancedProgressBar> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && Control == null)
            {
                _linearProgressBarView = new LinearProgressBar(Frame);
                SetNativeControl(_linearProgressBarView);

                UpdateProgressBar();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == AdvancedProgressBar.IsRunningProperty.PropertyName)
                UpdateProgressBar();
        }

        private void UpdateProgressBar()
        {
            if (Element.IsRunning)
                _linearProgressBarView.StartAnimation();
            else
                _linearProgressBarView.StopAnimation();
        }
    }
}
