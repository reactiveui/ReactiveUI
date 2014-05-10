using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Splat;

namespace ReactiveUI
{
    public sealed class ViewModelActivator
    {
        readonly List<Func<IEnumerable<IDisposable>>> blocks;
        IDisposable activationHandle = Disposable.Empty;
        int refCount = 0;

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
            if (Interlocked.Increment(ref refCount) == 1) {
                var disp = new CompositeDisposable(blocks.SelectMany(x => x()));
                Interlocked.Exchange(ref activationHandle, disp).Dispose();
            }

            return Disposable.Create(() => Deactivate());
        }

        public void Deactivate(bool ignoreRefCount = false)
        {
            if (Interlocked.Decrement(ref refCount) == 0 || ignoreRefCount) {
                Interlocked.Exchange(ref activationHandle, Disposable.Empty).Dispose();
            }
        }
    }

    public static class ViewForMixins
    {
        static ViewForMixins()
        {
            RxApp.EnsureInitialized();
        }

        public static void WhenActivated(this ISupportsActivation This, Func<IEnumerable<IDisposable>> block)
        {
            This.Activator.addActivationBlock(block);
        }

        public static void WhenActivated(this ISupportsActivation This, Action<Action<IDisposable>> block)
        {
            This.Activator.addActivationBlock(() => {
                var ret = new List<IDisposable>();
                block(ret.Add);
                return ret;
            });
        }

        public static IDisposable WithActivation(this ISupportsActivation This)
        {
            This.Activator.Activate();
            return Disposable.Create(() => This.Activator.Deactivate());
        }

        public static IDisposable WhenActivated(this IActivatable This, Func<IEnumerable<IDisposable>> block)
        {
            var activationFetcher = activationFetcherCache.Get(This.GetType());
            if (activationFetcher == null) {
                var msg = "Don't know how to detect when {0} is activated/deactivated, you may need to implement IActivationForViewFetcher";
                throw new ArgumentException(String.Format(msg, This.GetType().FullName));
            }

            var activationEvents = activationFetcher.GetActivationForView(This);

            var vmDisposable = Disposable.Empty;
            if (This is IViewFor) {
                vmDisposable = handleViewModelActivation(This as IViewFor, activationEvents);
            }

            var viewDisposable = handleViewActivation(block, activationEvents);
            return new CompositeDisposable(vmDisposable, viewDisposable);
        }

        static IDisposable handleViewActivation(Func<IEnumerable<IDisposable>> block, Tuple<IObservable<Unit>, IObservable<Unit>> activation)
        {
            var viewDisposable = new SerialDisposable();

            return new CompositeDisposable(
                // Activation
                activation.Item1.Subscribe(_ => {
                    // NB: We need to make sure to respect ordering so that the cleanup
                    // happens before we invoke block again
                    viewDisposable.Disposable = Disposable.Empty;
                    viewDisposable.Disposable = new CompositeDisposable(block());
                }),
                // Deactivation
                activation.Item2.Subscribe(_ => {
                    viewDisposable.Disposable = Disposable.Empty;
                }),
                viewDisposable);
        }

        static IDisposable handleViewModelActivation(IViewFor view, Tuple<IObservable<Unit>, IObservable<Unit>> activation)
        {
            var vmDisposable = new SerialDisposable();

            return new CompositeDisposable(
                // Activation
                activation.Item1
                    .Select(_ => view.WhenAnyValue(x => x.ViewModel))
                    .Switch()
                    .Select(x => x as ISupportsActivation)
                    .Subscribe(x => {
                        // NB: We need to make sure to respect ordering so that the cleanup
                        // happens before we activate again
                        vmDisposable.Disposable = Disposable.Empty;
                        if(x != null) {
                            vmDisposable.Disposable = x.Activator.Activate();
                        }
                    }),
                // Deactivation
                activation.Item2.Subscribe(_ => {
                    vmDisposable.Disposable = Disposable.Empty;
                }),
                vmDisposable);
        }

        public static IDisposable WhenActivated(this IActivatable This, Action<Action<IDisposable>> block)
        {
            return This.WhenActivated(() => {
                var ret = new List<IDisposable>();
                block(ret.Add);
                return ret;
            });
        }

        static readonly MemoizingMRUCache<Type, IActivationForViewFetcher> activationFetcherCache = 
            new MemoizingMRUCache<Type, IActivationForViewFetcher>((t, _) => {
                return Locator.Current.GetServices<IActivationForViewFetcher>()
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
            return (typeof(ICanActivate).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())) ?
                10 : 0;
        }

        public Tuple<IObservable<Unit>, IObservable<Unit>> GetActivationForView(IActivatable view)
        {
            var ca = view as ICanActivate;
            return Tuple.Create(ca.Activated, ca.Deactivated);
        }
    }
}
