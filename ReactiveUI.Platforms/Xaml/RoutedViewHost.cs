﻿using System;
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

namespace ReactiveUI.Xaml
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
            DependencyProperty.Register("Router", typeof(IRoutingState), typeof(RoutedViewHost), new PropertyMetadata(null));

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
            DependencyProperty.Register("ViewContractObservable", typeof(IObservable<string>), typeof(RoutedViewHost), new PropertyMetadata(Observable.Return(default(string))));

        public IViewLocator ViewLocator { get; set; }

        public RoutedViewHost()
        {
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;

            if (RxApp.InUnitTestRunner()) return;

            var platform = RxApp.DependencyResolver.GetService<IPlatformOperations>();
            if (platform == null) {
                throw new Exception("Couldn't find an IPlatformOperations. This should never happen, your dependency resolver is broken");
            }

            ViewContractObservable = Observable.FromEventPattern<SizeChangedEventHandler, SizeChangedEventArgs>(x => SizeChanged += x, x => SizeChanged -= x)
                .Select(_ => platform.GetOrientation())
                .DistinctUntilChanged()
                .StartWith(platform.GetOrientation())
                .Select(x => x.ToString());

            var vmAndContract = Observable.CombineLatest(
                this.WhenAny(x => x.Router, x => x.Value)
                    .Select(x => x.ViewModelObservable()).Switch(),
                this.WhenAnyObservable(x => x.ViewContractObservable),
                (vm, contract) => new { ViewModel = vm, Contract = contract, });

            vmAndContract.Subscribe(x => {
                if (x.ViewModel == null) {
                    Content = DefaultContent;
                    return;
                }

                var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                var view = viewLocator.ResolveView(x.ViewModel, x.Contract);
                view.ViewModel = x.ViewModel;
                Content = view;
            }, ex => RxApp.DefaultExceptionHandler.OnNext(ex));
        }
    }
}
