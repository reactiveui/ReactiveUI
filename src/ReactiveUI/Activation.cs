using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// ViewModelActivator is a helper class that you instantiate in your
    /// ViewModel classes in order to help with Activation. Views will internally
    /// call this class when the corresponding View comes on screen. This means
    /// you can set up resources such as subscriptions to global objects that
    /// should be cleaned up on exit. Once you instantiate this class, use the
    /// WhenActivated method to register what to do when activated.
    ///
    /// View Activation is  **not** the same as being loaded / unloaded; Views
    /// are Activated when they *enter* the Visual Tree, and are Deactivated when
    /// they *leave* the Visual Tree. This is a critical difference when it comes
    /// to views that are recycled, such as UITableViews or Virtualizing
    /// ScrollViews.
    ///
    /// Create this class solely in the **Base Class** of any classes that inherit
    /// from this class (i.e. if you create a FooViewModel that supports activation,
    /// the instance should be protected and a child BarViewModel should use the
    /// existing ViewModelActivator).
    ///
    /// NOTE: You **must** set up Activation in the corresponding View when using
    /// ViewModel Activation.
    /// </summary>
    public sealed class ViewModelActivator
    {
        readonly List<Func<IEnumerable<IDisposable>>> blocks;
        readonly Subject<Unit> activated;
        readonly Subject<Unit> deactivated;

        IDisposable activationHandle = Disposable.Empty;
        int refCount = 0;

        /// <summary>
        /// Activated observable will tick every time the Activator is activated.
        /// </summary>
        /// <value>The activated.</value>
        public IObservable<Unit> Activated { get { return activated; } }

        /// <summary>
        /// Deactivated observable will tick every time the Activator is deactivated.
        /// </summary>
        /// <value>The deactivated.</value>
        public IObservable<Unit> Deactivated { get { return deactivated; } }

        /// <summary>
        /// Constructs a new ViewModelActivator
        /// </summary>
        public ViewModelActivator()
        {
            blocks = new List<Func<IEnumerable<IDisposable>>>();
            activated = new Subject<Unit>();
            deactivated = new Subject<Unit>();
        }

        internal void addActivationBlock(Func<IEnumerable<IDisposable>> block)
        {
            blocks.Add(block);
        }

        /// <summary>
        /// This method is called by the framework when the corresponding View
        /// is activated. Call this method in unit tests to simulate a ViewModel
        /// being activated.
        /// </summary>
        /// <returns>A Disposable that calls Deactivate when disposed.</returns>
        public IDisposable Activate()
        {
            if (Interlocked.Increment(ref refCount) == 1) {
                var disp = new CompositeDisposable(blocks.SelectMany(x => x()));
                Interlocked.Exchange(ref activationHandle, disp).Dispose();
                activated.OnNext(Unit.Default);
            }

            return Disposable.Create(() => Deactivate());
        }

        /// <summary>
        /// This method is called by the framework when the corresponding View
        /// is deactivated.
        /// </summary>
        /// <param name="ignoreRefCount">
        /// Force the VM to be deactivated, even
        /// if more than one person called Activate.
        /// </param>
        public void Deactivate(bool ignoreRefCount = false)
        {
            if (Interlocked.Decrement(ref refCount) == 0 || ignoreRefCount) {
                Interlocked.Exchange(ref activationHandle, Disposable.Empty).Dispose();
                deactivated.OnNext(Unit.Default);
            }
        }
    }

    /// <summary>
    /// A set of extension methods to help wire up View and ViewModel activation
    /// </summary>
    public static class ViewForMixins
    {
        static ViewForMixins()
        {
            RxApp.EnsureInitialized();
        }

        /// <summary>
        /// WhenActivated allows you to register a Func to be called when a
        /// ViewModel's View is Activated.
        /// </summary>
        /// <param name="This">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding
        /// View is activated. It returns a list of Disposables that will be
        /// cleaned up when the View is deactivated.
        /// </param>
        public static void WhenActivated(this ISupportsActivation This, Func<IEnumerable<IDisposable>> block)
        {
            This.Activator.addActivationBlock(block);
        }

        /// <summary>
        /// WhenActivated allows you to register a Func to be called when a
        /// ViewModel's View is Activated.
        /// </summary>
        /// <param name="This">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding
        /// View is activated. The Action parameter (usually called 'd') allows
        /// you to register Disposables to be cleaned up when the View is
        /// deactivated (i.e. "d(someObservable.Subscribe());")
        /// </param>
        public static void WhenActivated(this ISupportsActivation This, Action<Action<IDisposable>> block)
        {
            This.Activator.addActivationBlock(() => {
                var ret = new List<IDisposable>();
                block(ret.Add);
                return ret;
            });
        }

        /// <summary>
        /// WhenActivated allows you to register a Func to be called when a
        /// ViewModel's View is Activated.
        /// </summary>
        /// <param name="This">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding
        /// View is activated. The Action parameter (usually called 'disposables') allows
        /// you to collate all the disposables to be cleaned up during deactivation.
        /// </param>
        public static void WhenActivated(this ISupportsActivation This, Action<CompositeDisposable> block)
        {
            This.Activator.addActivationBlock(() => {
                var d = new CompositeDisposable();
                block(d);
                return new[] { d };
            });
        }

        /// <summary>
        /// WhenActivated allows you to register a Func to be called when a
        /// View is Activated.
        /// </summary>
        /// <param name="This">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding
        /// View is activated. It returns a list of Disposables that will be
        /// cleaned up when the View is deactivated.
        /// </param>
        /// <returns>A Disposable that deactivates this registration.</returns>
        public static IDisposable WhenActivated(this IActivatable This, Func<IEnumerable<IDisposable>> block)
        {
            return This.WhenActivated(block, null);
        }

        /// <summary>
        /// WhenActivated allows you to register a Func to be called when a
        /// View is Activated.
        /// </summary>
        /// <param name="This">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding
        /// View is activated. It returns a list of Disposables that will be
        /// cleaned up when the View is deactivated.
        /// </param>
        /// <param name="view">
        /// The IActivatable will ordinarily also host the View
        /// Model, but in the event it is not, a class implementing <see cref="IViewFor" />
        /// can be supplied here.
        /// </param>
        /// <returns>A Disposable that deactivates this registration.</returns>
        public static IDisposable WhenActivated(this IActivatable This, Func<IEnumerable<IDisposable>> block, IViewFor view)
        {
            var activationFetcher = activationFetcherCache.Get(This.GetType());
            if (activationFetcher == null) {
                const string msg = "Don't know how to detect when {0} is activated/deactivated, you may need to implement IActivationForViewFetcher";
                throw new ArgumentException(String.Format(msg, This.GetType().FullName));
            }

            var activationEvents = activationFetcher.GetActivationForView(This);

            var vmDisposable = Disposable.Empty;
            var v = (view ?? This) as IViewFor;
            if (v != null) {
                vmDisposable = handleViewModelActivation(v, activationEvents);
            }

            var viewDisposable = handleViewActivation(block, activationEvents);
            return new CompositeDisposable(vmDisposable, viewDisposable);
        }

        /// <summary>
        /// WhenActivated allows you to register a Func to be called when a
        /// View is Activated.
        /// </summary>
        /// <param name="This">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding
        /// View is activated. The Action parameter (usually called 'd') allows
        /// you to register Disposables to be cleaned up when the View is
        /// deactivated (i.e. "d(someObservable.Subscribe());")
        /// </param>
        /// <returns>A Disposable that deactivates this registration.</returns>
        public static IDisposable WhenActivated(this IActivatable This, Action<Action<IDisposable>> block)
        {
            return This.WhenActivated(block, null);
        }

        /// <summary>
        /// WhenActivated allows you to register a Func to be called when a
        /// View is Activated.
        /// </summary>
        /// <param name="This">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding
        /// View is activated. The Action parameter (usually called 'd') allows
        /// you to register Disposables to be cleaned up when the View is
        /// deactivated (i.e. "d(someObservable.Subscribe());")
        /// </param>
        /// <param name="view">
        /// The IActivatable will ordinarily also host the View
        /// Model, but in the event it is not, a class implementing <see cref="IViewFor" />
        /// can be supplied here.
        /// </param>
        /// <returns>A Disposable that deactivates this registration.</returns>
        public static IDisposable WhenActivated(this IActivatable This, Action<Action<IDisposable>> block, IViewFor view)
        {
            return This.WhenActivated(() => {
                var ret = new List<IDisposable>();
                block(ret.Add);
                return ret;
            }, view);
        }

        /// <summary>
        /// WhenActivated allows you to register a Func to be called when a
        /// View is Activated.
        /// </summary>
        /// <param name="This">Object that supports activation.</param>
        /// <param name="block">
        /// The method to be called when the corresponding
        /// View is activated. The Action parameter (usually called 'disposables') allows
        /// you to collate all disposables that should be cleaned up during deactivation.
        /// </param>
        /// <param name="view">
        /// The IActivatable will ordinarily also host the View
        /// Model, but in the event it is not, a class implementing <see cref="IViewFor" />
        /// can be supplied here.
        /// </param>
        /// <returns>A Disposable that deactivates this registration.</returns>
        public static IDisposable WhenActivated(this IActivatable This, Action<CompositeDisposable> block, IViewFor view = null)
        {
            return This.WhenActivated(() => {
                var d = new CompositeDisposable();
                block(d);
                return new[] { d };
            }, view);
        }

        static IDisposable handleViewActivation(Func<IEnumerable<IDisposable>> block, IObservable<bool> activation)
        {
            var viewDisposable = new SerialDisposable();

            return new CompositeDisposable(
                activation.Subscribe(activated => {
                    // NB: We need to make sure to respect ordering so that the cleanup
                    // happens before we invoke block again
                    viewDisposable.Disposable = Disposable.Empty;
                    if(activated) {
                        viewDisposable.Disposable = new CompositeDisposable(block());
                    }
                }),
                viewDisposable);
        }

        static IDisposable handleViewModelActivation(IViewFor view, IObservable<bool> activation)
        {
            var vmDisposable = new SerialDisposable();
            var viewVmDisposable = new SerialDisposable();

            return new CompositeDisposable(
                activation.Subscribe(activated => {
                    if (activated) {
                        viewVmDisposable.Disposable = view.WhenAnyValue(x => x.ViewModel)
                            .Select(x => x as ISupportsActivation)
                            .Subscribe(x =>
                            {
                                // NB: We need to make sure to respect ordering so that the cleanup
                                // happens before we activate again
                                vmDisposable.Disposable = Disposable.Empty;
                                if (x != null) {
                                    vmDisposable.Disposable = x.Activator.Activate();
                                }
                            });
                    } else {
                        viewVmDisposable.Disposable = Disposable.Empty;
                        vmDisposable.Disposable = Disposable.Empty;
                    }
                }),
                vmDisposable,
                viewVmDisposable);
        }

        static readonly MemoizingMRUCache<Type, IActivationForViewFetcher> activationFetcherCache =
            new MemoizingMRUCache<Type, IActivationForViewFetcher>((t, _) =>
                Locator.Current
                       .GetServices<IActivationForViewFetcher>()
                       .Aggregate(Tuple.Create(0, default(IActivationForViewFetcher)), (acc, x) => {
                            int score = x.GetAffinityForView(t);
                            return (score > acc.Item1) ? Tuple.Create(score, x) : acc;
                        }).Item2, RxApp.SmallCacheLimit);
    }

    /// <summary>
    /// This class implements View Activation for classes that explicitly describe
    /// their activation via <see cref="ICanActivate"/>. This class is used by the framework.
    /// </summary>
    public class CanActivateViewFetcher : IActivationForViewFetcher
    {
        /// <summary>
        /// Returns a positive integer for derivates of the <see cref="ICanActivate"/> interface.
        /// </summary>
        /// <param name="view">The source type to check</param>
        /// <returns>
        /// A positive integer if <see cref="GetActivationForView(IActivatable)"/> is supported, 
        /// zero otherwise
        /// </returns>
        public int GetAffinityForView(Type view)
        {
            return (typeof(ICanActivate).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo())) ?
                10 : 0;
        }

        /// <summary>
        /// Get an observable defining whether the view is active
        /// </summary>
        /// <param name="view">The view to observe</param>
        /// <returns>An observable tracking whether the view is active</returns>
        public IObservable<bool> GetActivationForView(IActivatable view)
        {
            var ca = view as ICanActivate;
            return ca.Activated.Select(_ => true).Merge(ca.Deactivated.Select(_ => false));
        }
    }
}
