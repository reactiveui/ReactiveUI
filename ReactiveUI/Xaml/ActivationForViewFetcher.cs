using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;

#if WINRT
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
                return Observable.Empty<bool>();

            var viewLoaded = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(x => fe.Loaded += x,
                x => fe.Loaded -= x).Select(_ => true);

            var viewUnloaded = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(x => fe.Unloaded += x,
                x => fe.Unloaded -= x).Select(_ => false);

            return viewLoaded
                .Merge(viewUnloaded)
                .Select(b => b ? fe.WhenAnyValue(x => x.IsHitTestVisible).Select(hv => hv && b).SkipWhile(x => !x) : Observable.Return(b))
                .Switch()
                .DistinctUntilChanged();
        }
    }
}
