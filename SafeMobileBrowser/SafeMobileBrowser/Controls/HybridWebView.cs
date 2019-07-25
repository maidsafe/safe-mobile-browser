using System.Windows.Input;
using Xamarin.Forms;

namespace SafeMobileBrowser.Controls
{
    public class HybridWebView : WebView
    {
        public static readonly BindableProperty GoBackCommandProperty = BindableProperty.Create(
            nameof(GoBackCommand),
            typeof(ICommand),
            typeof(HybridWebView),
            null,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty GoForwardCommandProperty = BindableProperty.Create(
            nameof(GoForwardCommand),
            typeof(ICommand),
            typeof(HybridWebView),
            null,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty ReloadCommandProperty = BindableProperty.Create(
            nameof(ReloadCommand),
            typeof(ICommand),
            typeof(HybridWebView),
            null,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty NavigatingCommandProperty = BindableProperty.Create(
            nameof(NavigatingCommand),
            typeof(ICommand),
            typeof(HybridWebView),
            null,
            BindingMode.OneWayToSource);

        public static readonly BindableProperty NavigatedCommandProperty = BindableProperty.Create(
            nameof(NavigatedCommand),
            typeof(ICommand),
            typeof(HybridWebView),
            null,
            BindingMode.OneWayToSource);

        public ICommand GoBackCommand
        {
            get => (ICommand)GetValue(GoBackCommandProperty);
            set => SetValue(GoBackCommandProperty, value);
        }

        public ICommand GoForwardCommand
        {
            get => (ICommand)GetValue(GoForwardCommandProperty);
            set => SetValue(GoForwardCommandProperty, value);
        }

        public ICommand ReloadCommand
        {
            get => (ICommand)GetValue(ReloadCommandProperty);
            set => SetValue(ReloadCommandProperty, value);
        }

        public ICommand NavigatingCommand
        {
            get => (ICommand)GetValue(NavigatedCommandProperty);
            set => SetValue(NavigatedCommandProperty, value);
        }

        public ICommand NavigatedCommand
        {
            get => (ICommand)GetValue(NavigatedCommandProperty);
            set => SetValue(NavigatedCommandProperty, value);
        }

        public HybridWebView()
        {
            GoBackCommand = new Command(GoBack);
            GoForwardCommand = new Command(GoForward);
            ReloadCommand = new Command(Reload);
        }
    }
}
