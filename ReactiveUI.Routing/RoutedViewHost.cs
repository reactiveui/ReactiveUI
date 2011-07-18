using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using ReactiveUI.Xaml;

namespace ReactiveUI.Routing
{
    public class RoutedViewHost : ContentControl
    {
        IDisposable _inner = null;

        public RoutingState Router {
            get { return (RoutingState)GetValue(RouterProperty); }
            set { SetValue(RouterProperty, value); }
        }
        public static readonly DependencyProperty RouterProperty =
            DependencyProperty.Register("Router", typeof(RoutingState), typeof(RoutedViewHost), new UIPropertyMetadata(null));

        public RoutedViewHost()
        {
            this.ObservableFromDP(x => x.Router)
                .Subscribe(x => {
                    if (_inner != null) {
                        _inner.Dispose();
                        _inner = null;
                    }

                    _inner = x.Value.CurrentViewModel.Subscribe(vm => {
                        if (vm == null) {
                            // XXX: Replace with default view
                            Content = null;
                            return;
                        }

                        var view = RxRouting.ResolveView(vm);
                        view.ViewModel = vm;
                        Content = view;
                    });
                });
        }
    }
}
