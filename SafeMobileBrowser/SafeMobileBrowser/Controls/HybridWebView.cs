// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

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
            BindingMode.Default);

        public static readonly BindableProperty NavigatedCommandProperty = BindableProperty.Create(
            nameof(NavigatedCommand),
            typeof(ICommand),
            typeof(HybridWebView),
            null,
            BindingMode.Default);

        public static readonly BindableProperty ContentLoadProgressProperty = BindableProperty.Create(
            nameof(ContentLoadProgress),
            typeof(double),
            typeof(HybridWebView),
            default(double),
            BindingMode.TwoWay);

        public double ContentLoadProgress
        {
            get => (double)GetValue(ContentLoadProgressProperty);
            set => SetValue(ContentLoadProgressProperty, value);
        }

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
            get => (ICommand)GetValue(NavigatingCommandProperty);
            set => SetValue(NavigatingCommandProperty, value);
        }

        public ICommand NavigatedCommand
        {
            get => (ICommand)GetValue(NavigatedCommandProperty);
            set => SetValue(NavigatedCommandProperty, value);
        }

        public ulong CurrentPageVersion
        {
            get => (ulong)GetValue(CurrentPageVersionProperty);
            set => SetValue(CurrentPageVersionProperty, value);
        }

        public static readonly BindableProperty CurrentPageVersionProperty = BindableProperty.Create(
            nameof(CurrentPageVersion),
            typeof(ulong),
            typeof(HybridWebView),
            0UL,
            BindingMode.TwoWay);

        public ulong LatestPageVersion
        {
            get => (ulong)GetValue(LatestPageVersionProperty);
            set => SetValue(LatestPageVersionProperty, value);
        }

        public static readonly BindableProperty LatestPageVersionProperty = BindableProperty.Create(
            nameof(LatestPageVersion),
            typeof(ulong),
            typeof(HybridWebView),
            0UL,
            BindingMode.TwoWay);

        public HybridWebView()
        {
            GoBackCommand = new Command(GoBack);
            GoForwardCommand = new Command(GoForward);
            ReloadCommand = new Command(Reload);
            Navigated += (s, e) =>
            {
                NavigatedCommand?.Execute(e);
            };
            Navigating += (s, e) =>
            {
                NavigatingCommand?.Execute(e);
            };
        }
    }
}
