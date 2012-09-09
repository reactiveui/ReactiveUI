using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Windows;
using ReactiveUI;
using ReactiveUI.Xaml;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

namespace ReactiveUI.Routing
{
    /// <summary>
    /// This control hosts the View associated with a Router, and will display
    /// the View and wire up the ViewModel whenever a new ViewModel is
    /// navigated to. Put this control as the only control in your Window.
    /// </summary>
    public class RoutedViewHost : TransitioningContentControl
    {
        IDisposable _inner = null;

        /// <summary>
        /// The Router associated with this View Host.
        /// </summary>
        public IRoutingState Router {
            get { return (IRoutingState)GetValue(RouterProperty); }
            set { SetValue(RouterProperty, value); }
        }
        public static readonly DependencyProperty RouterProperty =
            DependencyProperty.Register("Router", typeof(IRoutingState), typeof(RoutedViewHost), new PropertyMetadata(null, routerChanged));

        readonly Subject<IRoutingState> routerChange;

        /// <summary>
        /// This content is displayed whenever there is no page currently
	    /// routed.
        /// </summary>
        public object DefaultContent {
            get { return (object)GetValue(DefaultContentProperty); }
            set { SetValue(DefaultContentProperty, value); }
        }
        public static readonly DependencyProperty DefaultContentProperty =
            DependencyProperty.Register("DefaultContent", typeof(object), typeof(RoutedViewHost), new PropertyMetadata(null));

        public RoutedViewHost()
        {
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
            routerChange = new Subject<IRoutingState>();

            routerChange.Subscribe(x => {
                if (_inner != null) {
                    _inner.Dispose();
                    _inner = null;
                }

                _inner = x.ViewModelObservable().Subscribe(vm => {
                    if (vm == null) {
                        Content = DefaultContent;
                        return;
                    }

                    var view = RxRouting.ResolveView(vm);
                    view.ViewModel = vm;
                    Content = view;
                });
            });
        }

        static void routerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // XXX: This is to work around the fact that ObservableFromDP 
            // is broken in WinRT
            var This = ((RoutedViewHost) dependencyObject);
            This.routerChange.OnNext(This.Router);
        }

    }
}
