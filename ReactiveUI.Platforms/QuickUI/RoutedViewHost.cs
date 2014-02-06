using System;
using System.Reactive.Linq;
using Xamarin.QuickUI;
using ReactiveUI;
using Splat;

namespace ReactiveUI.QuickUI
{
    /// <summary>
    /// This control hosts the View associated with a Router, and will display
    /// the View and wire up the ViewModel whenever a new ViewModel is
    /// navigated to. Put this control as the only control in your Window.
    /// </summary>
    public class RoutedViewHost : StackLayout, IActivatable
    {
        IDisposable _inner = null;

        /// <summary>
        /// The Router associated with this View Host.
        /// </summary>
        public IRoutingState Router {
            get { return (IRoutingState)GetValue(RouterProperty); }
            set { SetValue(RouterProperty, value); }
        }
        public static readonly BindableProperty RouterProperty =
            BindableProperty.Create<RoutedViewHost, IRoutingState>(x => x.Router, null, BindingMode.OneWay);

        /// <summary>
        /// This content is displayed whenever there is no page currently
        /// routed.
        /// </summary>
        public View DefaultContent {
            get { return (View)GetValue(DefaultContentProperty); }
            set { SetValue(DefaultContentProperty, value); }
        }
        public static readonly BindableProperty DefaultContentProperty =
            BindableProperty.Create<RoutedViewHost, View>(x => x.DefaultContent, null, BindingMode.OneWay);

        public IObservable<string> ViewContractObservable {
            get { return (IObservable<string>)GetValue(ViewContractObservableProperty); }
            set { SetValue(ViewContractObservableProperty, value); }
        }
        public static readonly BindableProperty ViewContractObservableProperty =
            BindableProperty.Create<RoutedViewHost, IObservable<string>>(x => x.ViewContractObservable, Observable.Never<string>(), BindingMode.OneWay);

        public IViewLocator ViewLocator { get; set; }

        public RoutedViewHost()
        {
            if (ModeDetector.InUnitTestRunner()) {
                ViewContractObservable = Observable.Never<string>();
                return;
            }

            var platform = RxApp.Locator.GetService<IPlatformOperations>();
            if (platform == null) {
                throw new Exception("Couldn't find an IPlatformOperations. This should never happen, your dependency resolver is broken");
            }

            ViewContractObservable = Observable.FromEventPattern<EventHandler, EventArgs>(x => SizeChanged += x, x => SizeChanged -= x)
                .Select(_ => platform.GetOrientation())
                .DistinctUntilChanged()
                .StartWith(platform.GetOrientation())
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
                        foreach (View v in this.Children) this.Remove(v);
                        if (DefaultContent != null) this.Add(DefaultContent);
                        return;
                    }

                    var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                    var view = viewLocator.ResolveView(x.Item1, x.Item2) ?? viewLocator.ResolveView(x.Item1, null);

                    if (view == null) {
                        throw new Exception(String.Format("Couldn't find view for '{0}'.", x.Item1));
                    }

                    view.ViewModel = x.Item1;

                    foreach (View v in this.Children) this.Remove(v);
                    if (view != null) this.Add(view as View);
                }, ex => RxApp.DefaultExceptionHandler.OnNext(ex)));
            });
        }
    }
}
