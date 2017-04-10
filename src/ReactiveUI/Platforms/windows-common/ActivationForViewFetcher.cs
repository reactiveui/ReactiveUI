using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Reflection;

#if NETFX_CORE
using Windows.UI.Xaml;
#endif

namespace ReactiveUI
{
    public class ActivationForViewFetcher : IActivationForViewFetcher
    {
        public int GetAffinityForView(Type view)
        {
            return (typeof(FrameworkElement).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())) ? 10 : 0;
        }

        public IObservable<bool> GetActivationForView(IActivatable view)
        {
            var fe = view as FrameworkElement;

            if (fe == null)
                return Observable<bool>.Empty;
#if WINDOWS_UWP
            var viewLoaded = WindowsObservable.FromEventPattern<FrameworkElement, object>(x => fe.Loading += x,
                x => fe.Loading -= x).Select(_ => true);
#else
            var viewLoaded = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(x => fe.Loaded += x,
                x => fe.Loaded -= x).Select(_ => true);
#endif

            var viewUnloaded = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(x => fe.Unloaded += x,
                x => fe.Unloaded -= x).Select(_ => false);

            return viewLoaded
                .Merge(viewUnloaded)
                .Select(b => b ? fe.WhenAnyValue(x => x.IsHitTestVisible).SkipWhile(x => !x) : Observables.False)
                .Switch()
                .DistinctUntilChanged();
        }
    }
}
