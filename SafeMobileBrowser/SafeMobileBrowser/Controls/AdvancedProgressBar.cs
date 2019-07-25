using Xamarin.Forms;

namespace SafeMobileBrowser.Controls
{
    public class AdvancedProgressBar : ProgressBar
    {
        public static readonly BindableProperty IsRunningProperty = BindableProperty.Create(
            nameof(IsRunning),
            typeof(bool),
            typeof(AdvancedProgressBar),
            default(bool));

        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }
    }
}
