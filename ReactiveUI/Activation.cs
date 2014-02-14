using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;

namespace ReactiveUI
{
    public sealed class ViewModelActivator
    {
        readonly List<Func<IEnumerable<IDisposable>>> blocks;
        IDisposable activationHandle = Disposable.Empty;

        public ViewModelActivator()
        {
            blocks = new List<Func<IEnumerable<IDisposable>>>();
        }

        internal void addActivationBlock(Func<IEnumerable<IDisposable>> block)
        {
            blocks.Add(block);
        }

        public IDisposable Activate()
        {
            var disp = new CompositeDisposable(blocks.SelectMany(x => x()));

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
        public static IDisposable WithActivation(this ISupportsActivation This)
        {
            This.Activator.Activate();
            return Disposable.Create(This.Activator.Deactivate);
        }

        public static void WhenActivated(this ISupportsActivation This,
            Func<IEnumerable<IDisposable>> block)
        {
            This.Activator.addActivationBlock(block);
        }

        public static void WhenActivated(this ISupportsActivation This, 
            Action<Action<IDisposable>> block)
        {
            This.Activator.addActivationBlock(() => {
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
                    String.Format(
                        "Don't know how to detect when {0} is activated/deactivated, you may need to implement IActivationForViewFetcher",
                        This.GetType().FullName));
            }

            var activationEvents = activationFetcher.GetActivationForView(This);

            var viewDisposable = new SerialDisposable();

            return new CompositeDisposable(
                activationEvents.Item1.Subscribe(_ => viewDisposable.Disposable = new CompositeDisposable(block())),
                activationEvents.Item2.Subscribe(_ => viewDisposable.Disposable = Disposable.Empty),
                handleViewModelActivation(This, activationEvents),
                viewDisposable);
        }

        static IDisposable handleViewModelActivation(IViewFor view,
            Tuple<IObservable<Unit>, IObservable<Unit>> activation)
        {
            var vm = view.ViewModel as ISupportsActivation;
            var disp = new SerialDisposable() {Disposable = (vm != null ? vm.Activator.Activate() : Disposable.Empty)};

            var latestVm = Observable.Merge(
                activation.Item1.Select(_ => view.WhenAnyValue(x => x.ViewModel)),
                activation.Item2.Select(_ => Observable.Never<object>().StartWith(default(object))))
                .Switch()
                .Select(x => x as ISupportsActivation);

            return new CompositeDisposable(
                disp,
                latestVm.Subscribe(x => disp.Disposable =
                    (x != null ? x.Activator.Activate() : Disposable.Empty)));
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
                return RxApp.MutableResolver.GetServices<IActivationForViewFetcher>()
                    .Aggregate(Tuple.Create(0, default(IActivationForViewFetcher)), (acc, x) => {
                        int score = x.GetAffinityForView(t);
                        return (score > acc.Item1) ? Tuple.Create(score, x) : acc;
                    }).Item2;
            }, RxApp.SmallCacheLimit);
    }

    public class CanActivateViewFetcher : IActivationForViewFetcher
    {
        public int GetAffinityForView(Type view)
        {
            return (typeof (ICanActivate).GetTypeInfo().IsAssignableFrom(view.GetType().GetTypeInfo())) ?
                10 : 0;
        }

        public Tuple<IObservable<Unit>, IObservable<Unit>> GetActivationForView(IViewFor view)
        {
            var ca = view as ICanActivate;
            return Tuple.Create(ca.Activated, ca.Deactivated);
        }
    }
}