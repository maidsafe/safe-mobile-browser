using System.Windows.Input;
using Xamarin.Forms;

namespace SafeMobileBrowser.Controls
{
    public class AddressBarEntry : Entry
    {
        public static readonly BindableProperty AddressBarFocusCommandProperty = BindableProperty.Create(
            nameof(AddressBarFocusCommand),
            typeof(ICommand),
            typeof(AddressBarEntry),
            null,
            BindingMode.OneWayToSource);

        public ICommand AddressBarFocusCommand
        {
            get => (ICommand)GetValue(AddressBarFocusCommandProperty);
            set => SetValue(AddressBarFocusCommandProperty, value);
        }

        public AddressBarEntry()
        {
            AddressBarFocusCommand = new Command(() => Focus());
        }
    }
}
