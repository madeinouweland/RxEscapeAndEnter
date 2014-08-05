using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace EscapeTheEnter
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var keys = Observable.FromEventPattern<TypedEventHandler<CoreWindow, KeyEventArgs>, KeyEventArgs>(
                h => Window.Current.CoreWindow.KeyDown += h, h => Window.Current.CoreWindow.KeyDown -= h)
                .Select(pattern => pattern.EventArgs);

            var okClicks = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => ButtonOK.Click += h, h => ButtonOK.Click -= h);
            var cancelClicks = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => ButtonCancel.Click += h, h => ButtonCancel.Click -= h);

            _subscriptions.Add(keys.Where(x => x.VirtualKey == VirtualKey.Enter).Select(_ => true).Merge(okClicks.Select(_ => true)).Subscribe(Close));
            _subscriptions.Add(keys.Where(x => x.VirtualKey == VirtualKey.Escape).Select(_ => false).Merge(cancelClicks.Select(_ => false)).Subscribe(Close));
        }

        private void Close(bool result)
        {
            Info.Text = "Window close action: " + result;
            Info.Focus(FocusState.Programmatic);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            foreach (var sub in _subscriptions)
            {
                sub.Dispose();
            }
            _subscriptions.Clear();
        }
    }
}
