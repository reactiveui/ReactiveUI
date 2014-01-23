using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Splat;

namespace ReactiveUI
{
    public class ViewModelActivator
    {
        readonly Func<IEnumerable<IDisposable>> block;
        IDisposable activationHandle = Disposable.Empty;

        public ViewModelActivator(Func<IEnumerable<IDisposable>> block)
        {
            this.block = block;
        }

        public IDisposable Activate()
        {
            var disp = new CompositeDisposable(block());
            Interlocked.Exchange(ref activationHandle, disp).Dispose();

            return Disposable.Create(Deactivate);
        }

        public void Deactivate()
        {
            Interlocked.Exchange(ref activationHandle, Disposable.Empty).Dispose();
        }
    }

    public static class ViewForMixins
    {
        public static ViewModelActivator WhenActivated(this ISupportsActivation This, Func<IEnumerable<IDisposable>> block)
        {
            return new ViewModelActivator(block);
        }

        public static ViewModelActivator WhenActivated(this ISupportsActivation This, Action<Action<IDisposable>> block)
        {
            return new ViewModelActivator(() => {
                var ret = new List<IDisposable>();
                block(ret.Add);
                return ret;
            });
        }

        public static IDisposable WhenActivated(this IViewFor This, Func<IEnumerable<IDisposable>> block)
        {
            var activationFetcher = activationFetcherCache.Get(This.GetType());
            if (activationFetcher == null) {
                throw new ArgumentException(
                    String.Format("Don't know how to detect when {0} is activated/deactivated, you may need to implement IActivationForViewFetcher",
                        This.GetType().FullName));
            }

            var activationEvents = activationFetcher.GetActivationForView(This);

            var viewForDisp = new SerialDisposable();
            var currentDisp = new SerialDisposable();

            return new CompositeDisposable(
                Observable.CombineLatest(This.WhenAnyValue(x => x.ViewModel), activationEvents.Item1, (vm, _) => vm)
                    .Where(vm => vm is ISupportsActivation)
                    .Subscribe(vm => viewForDisp.Disposable = ((ISupportsActivation)vm).Activator.Activate()),
                activationEvents.Item1.Subscribe(_ => currentDisp.Disposable = new CompositeDisposable(block())),
                activationEvents.Item2.Subscribe(_ => currentDisp.Disposable = viewForDisp.Disposable = Disposable.Empty),
                currentDisp, viewForDisp);
        }

        public static IDisposable WhenActivated(this IViewFor This, Action<Action<IDisposable>> block)
        {
            return This.WhenActivated(() => {
                var ret = new List<IDisposable>();
                block(ret.Add);
                return ret;
            });
        }

        static readonly MemoizingMRUCache<Type, IActivationForViewFetcher> activationFetcherCache = 
            new MemoizingMRUCache<Type, IActivationForViewFetcher>((t, _) => {
                return RxApp.Locator.GetServices<IActivationForViewFetcher>()
                    .Aggregate(Tuple.Create(0, default(IActivationForViewFetcher)), (acc, x) => {
                        int score = x.GetAffinityForView(t);
                        return (score > acc.Item1) ? Tuple.Create(score, x) : acc;
                    }).Item2;
            }, RxApp.SmallCacheLimit);
    }
}