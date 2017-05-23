using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
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
    /// This content control will automatically load the View associated with
    /// the ViewModel property and display it. This control is very useful
    /// inside a DataTemplate to display the View associated with a ViewModel.
    /// </summary>
    public class ViewModelViewHost : TransitioningContentControl, IViewFor, IEnableLogger
    {
        /// <summary>
        /// The ViewModel to display
        /// </summary>
        public object ViewModel {
            get { return GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty = 
            DependencyProperty.Register("ViewModel", typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null, somethingChanged));

        readonly Subject<Unit> updateViewModel = new Subject<Unit>();

        /// <summary>
        /// If no ViewModel is displayed, this content (i.e. a control) will be displayed.
        /// </summary>
        public object DefaultContent {
            get { return GetValue(DefaultContentProperty); }
            set { SetValue(DefaultContentProperty, value); }
        }
        public static readonly DependencyProperty DefaultContentProperty =
            DependencyProperty.Register("DefaultContent", typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null, somethingChanged));

        public IObservable<string> ViewContractObservable {
            get { return (IObservable<string>)GetValue(ViewContractObservableProperty); }
            set { SetValue(ViewContractObservableProperty, value); }
        }
        public static readonly DependencyProperty ViewContractObservableProperty =
            DependencyProperty.Register("ViewContractObservable", typeof(IObservable<string>), typeof(ViewModelViewHost), new PropertyMetadata(Observable<string>.Default));

        private string viewContract;

        public string ViewContract
        {
            get { return this.viewContract; }
            set { ViewContractObservable = Observable.Return(value); }
        }

        public IViewLocator ViewLocator { get; set; }

        public ViewModelViewHost()
        {
#if NETFX_CORE
            this.DefaultStyleKey = typeof(ViewModelViewHost);
#endif

            // NB: InUnitTestRunner also returns true in Design Mode
            if (ModeDetector.InUnitTestRunner()) {
                ViewContractObservable = Observable<string>.Never;
                return;
            }

            var vmAndContract = Observable.CombineLatest(
                this.WhenAnyValue(x => x.ViewModel),
                this.WhenAnyObservable(x => x.ViewContractObservable),
                (vm, contract) => new { ViewModel = vm, Contract = contract, });

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
                .StartWith(platformGetter())
                .DistinctUntilChanged();

            this.WhenActivated(d => {
                d(vmAndContract.Subscribe(x => {
                    if (x.ViewModel == null) {
                        Content = DefaultContent;
                        return;
                    }

                    var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                    var view = viewLocator.ResolveView(x.ViewModel, x.Contract) ?? viewLocator.ResolveView(x.ViewModel, null);

                    if (view == null) {
                        throw new Exception($"Couldn't find view for '{x.ViewModel}'.");
                    }

                    view.ViewModel = x.ViewModel;
                    Content = view;
                }));

                d(this.WhenAnyObservable(x => x.ViewContractObservable)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(x => viewContract = x));
            });
        }

        static void somethingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((ViewModelViewHost)dependencyObject).updateViewModel.OnNext(Unit.Default);
        }
    }
}
