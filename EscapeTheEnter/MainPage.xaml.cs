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

            _subscriptions.Add(keys.Where(x => x.VirtualKey == VirtualKey.Enter).Subscribe(_ => Accept()));
            _subscriptions.Add(keys.Where(x => x.VirtualKey == VirtualKey.Escape).Subscribe(_ => Cancel()));
        }

        private void Accept()
        {
            KeyBlock.Text = "Enter -> Accept";
        }

        private void Cancel()
        {
            KeyBlock.Text = "Escape -> Cancel";
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
