using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Windows;
using ReactiveUI;
using Splat;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// This control hosts the View associated with a Router, and will display
    /// the View and wire up the ViewModel whenever a new ViewModel is
    /// navigated to. Put this control as the only control in your Window.
    /// </summary>
    public class RoutedViewHost : TransitioningContentControl, IActivatable, IEnableLogger
    {
        /// <summary>
        /// The Router associated with this View Host.
        /// </summary>
        public RoutingState Router {
            get { return (RoutingState)GetValue(RouterProperty); }
            set { SetValue(RouterProperty, value); }
        }
        public static readonly DependencyProperty RouterProperty =
            DependencyProperty.Register("Router", typeof(RoutingState), typeof(RoutedViewHost), new PropertyMetadata(null));

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

        public IObservable<string> ViewContractObservable {
            get { return (IObservable<string>)GetValue(ViewContractObservableProperty); }
            set { SetValue(ViewContractObservableProperty, value); }
        }
        public static readonly DependencyProperty ViewContractObservableProperty =
            DependencyProperty.Register("ViewContractObservable", typeof(IObservable<string>), typeof(RoutedViewHost), new PropertyMetadata(Observable<string>.Default));

        public IViewLocator ViewLocator { get; set; }

        public RoutedViewHost()
        {
#if NETFX_CORE
            this.DefaultStyleKey = typeof(RoutedViewHost);
#endif
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;

            if (ModeDetector.InUnitTestRunner()) {
                ViewContractObservable = Observable<string>.Never;
                return;
            }

            var platform = Locator.Current.GetService<IPlatformOperations>();
            Func<string> platformGetter = () => default(string);

            if (platform == null) {
                // NB: This used to be an error but WPF design mode can't read
                // good or do other stuff good.
                this.Log().Error("Couldn't find an IPlatformOperations. This should never happen, your dependency resolver is broken");
            } else {
                platformGetter = () => platform.GetOrientation();
            }

            ViewContractObservable = Observable.FromEventPattern<SizeChangedEventHandler, SizeChangedEventArgs>(x => SizeChanged += x, x => SizeChanged -= x)
                .Select(_ => platformGetter())
                .DistinctUntilChanged()
                .StartWith(platformGetter())
                .Select(x => x != null ? x.ToString() : default(string));

            var vmAndContract = Observable.CombineLatest(
                this.WhenAnyObservable(x => x.Router.CurrentViewModel),
                this.WhenAnyObservable(x => x.ViewContractObservable),
                (vm, contract) => Tuple.Create(vm, contract));

            this.WhenActivated(d => {
                // NB: The DistinctUntilChanged is useful because most views in 
                // WinRT will end up getting here twice - once for configuring
                // the RoutedViewHost's ViewModel, and once on load via SizeChanged
                d(vmAndContract.DistinctUntilChanged().Subscribe(x => {
                    if (x.Item1 == null) {
                        Content = DefaultContent;
                        return;
                    }

                    var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                    var view = viewLocator.ResolveView(x.Item1, x.Item2) ?? viewLocator.ResolveView(x.Item1, null);

                    if (view == null) {
                        throw new Exception(String.Format("Couldn't find view for '{0}'.", x.Item1));
                    }

                    view.ViewModel = x.Item1;
                    Content = view;
                }, ex => RxApp.DefaultExceptionHandler.OnNext(ex)));
            });
        }
    }
}
