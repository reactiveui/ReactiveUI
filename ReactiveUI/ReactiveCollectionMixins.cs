using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Diagnostics.Contracts;
using System.Collections.Specialized;
using System.Reactive.Subjects;
using System.Globalization;
using System.Threading;
using System.Reactive.Disposables;

namespace ReactiveUI
{
    public sealed class ReactiveDerivedCollection<T> : ReactiveCollection<T>, IDisposable
    {
        IDisposable inner = null;

        public ReactiveDerivedCollection(IDisposable disposable) : base()
        {
            inner = disposable ?? Disposable.Empty;
        }

        public ReactiveDerivedCollection(IEnumerable<T> items, IDisposable disposable) : base(items)
        {
            inner = disposable;
        }

        public void Dispose()
        {
            var disp = Interlocked.Exchange(ref inner, null);
            if (disp == null) return;

            disp.Dispose();
        }
    }

    public static class ReactiveCollectionMixins
    {
        /// <summary>
        /// Creates a collection based on an an Observable by adding items
        /// provided until the Observable completes, optionally ensuring a
        /// delay. Note that if the Observable never completes and withDelay is
        /// set, this method will leak a Timer. This method also guarantees that
        /// items are always added via the UI thread.
        /// </summary>
        /// <param name="fromObservable">The Observable whose items will be put
        /// into the new collection.</param>
        /// <param name="onError">The handler for errors from the Observable. If
        /// not specified, an error will go to DefaultExceptionHandler.</param>
        /// <param name="withDelay">If set, items will be populated in the
        /// collection no faster than the delay provided.</param>
        /// <returns>A new collection which will be populated with the
        /// Observable.</returns>
        public static ReactiveDerivedCollection<T> CreateCollection<T>(
            this IObservable<T> fromObservable, 
            TimeSpan? withDelay = null,
            Action<Exception> onError = null)
        {
            var disp = new SingleAssignmentDisposable();
            var ret = new ReactiveDerivedCollection<T>(disp);

            onError = onError ?? (ex => RxApp.DefaultExceptionHandler.OnNext(ex));
            if (withDelay == null) {
                disp.Disposable = fromObservable.ObserveOn(RxApp.DeferredScheduler).Subscribe(ret.Add, onError);
                return ret;
            }

            // On a timer, dequeue items from queue if they are available
            var queue = new Queue<T>();
            var disconnect = Observable.Timer(withDelay.Value, withDelay.Value, RxApp.DeferredScheduler)
                .Subscribe(_ => {
                    if (queue.Count > 0) { 
                        ret.Add(queue.Dequeue());
                    }
                });

            disp.Disposable = disconnect;

            // When new items come in from the observable, stuff them in the queue.
            // Using the DeferredScheduler guarantees we'll always access the queue
            // from the same thread.
            fromObservable.ObserveOn(RxApp.DeferredScheduler).Subscribe(queue.Enqueue, onError);

            // This is a bit clever - keep a running count of the items actually 
            // added and compare them to the final count of items provided by the
            // Observable. Combine the two values, and when they're equal, 
            // disconnect the timer
            ret.ItemsAdded.Scan(0, ((acc, _) => acc+1)).Zip(fromObservable.Aggregate(0, (acc,_) => acc+1), 
                (l,r) => (l == r)).Where(x => x).Subscribe(_ => disconnect.Dispose());

            return ret;
        }

        /// <summary>
        /// Creates a collection based on an an Observable by adding items
        /// provided until the Observable completes, optionally ensuring a
        /// delay. Note that if the Observable never completes and withDelay is
        /// set, this method will leak a Timer. This method also guarantees that
        /// items are always added via the UI thread.
        /// </summary>
        /// <param name="fromObservable">The Observable whose items will be put
        /// into the new collection.</param>
        /// <param name="selector">A Select function that will be run on each
        /// item.</param>
        /// <param name="withDelay">If set, items will be populated in the
        /// collection no faster than the delay provided.</param>
        /// <returns>A new collection which will be populated with the
        /// Observable.</returns>
        public static ReactiveDerivedCollection<TRet> CreateCollection<T, TRet>(
            this IObservable<T> fromObservable, 
            Func<T, TRet> selector, 
            TimeSpan? withDelay = null)
        {
            Contract.Requires(selector != null);
            return fromObservable.Select(selector).CreateCollection(withDelay);
        }
    }

    public static class ObservableCollectionMixin
    {
        /// <summary>
        /// Creates a collection whose contents will "follow" another
        /// collection; this method is useful for creating ViewModel collections
        /// that are automatically updated when the respective Model collection
        /// is updated.
        ///
        /// Note that even though this method attaches itself to any 
        /// IEnumerable, it will only detect changes from objects implementing
        /// INotifyCollectionChanged (like ReactiveCollection). If your source
        /// collection doesn't implement this, signalReset is the way to signal
        /// the derived collection to reorder/refilter itself.
        /// </summary>
        /// <param name="selector">A Select function that will be run on each
        /// item.</param>
        /// <param name="filter">A filter to determine whether to exclude items 
        /// in the derived collection.</param>
        /// <param name="orderer">A comparator method to determine the ordering of
        /// the resulting collection.</param>
        /// <param name="signalReset">When this Observable is signalled, 
        /// the derived collection will be manually 
        /// reordered/refiltered.</param>
        /// <returns>A new collection whose items are equivalent to
        /// Collection.Select().Where().OrderBy() and will mirror changes 
        /// in the initial collection.</returns>
        public static ReactiveDerivedCollection<TNew> CreateDerivedCollection<T, TNew, TDontCare>(
            this IEnumerable<T> This,
            Func<T, TNew> selector,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null,
            IObservable<TDontCare> signalReset = null)
        {
            Contract.Requires(selector != null);

            var disp = new CompositeDisposable();
            var collChanged = new Subject<NotifyCollectionChangedEventArgs>();

            if (selector == null) {
                selector = (x => (TNew)Convert.ChangeType(x, typeof(TNew), CultureInfo.CurrentCulture));
            }

            var origEnum = This;
            origEnum = (filter != null ? origEnum.Where(filter) : origEnum);
            var enumerable = origEnum.Select(selector);
            enumerable = (orderer != null ? enumerable.OrderBy(x => x, new FuncComparator<TNew>(orderer)) : enumerable);

            var ret = new ReactiveDerivedCollection<TNew>(enumerable, disp);

            var incc = This as INotifyCollectionChanged;
            if (incc != null) {
                var connObs = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(x => incc.CollectionChanged += x, x => incc.CollectionChanged -= x)
                    .Select(x => x.EventArgs)
                    .Multicast(collChanged);

                disp.Add(connObs.Connect());
            }

            if (filter != null && orderer == null) {
                throw new Exception("If you specify a filter, you must also specify an ordering function");
            }

            disp.Add(signalReset.Subscribe(_ => collChanged.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))));

            disp.Add(collChanged.Subscribe(args => {
                if (args.Action == NotifyCollectionChangedAction.Reset) {
                    using(ret.SuppressChangeNotifications()) {
                        ret.Clear();
                        enumerable.ForEach(ret.Add);
                    }

                    return;
                }

                int oldIndex = (args.Action == NotifyCollectionChangedAction.Replace ?
                    args.NewStartingIndex : args.OldStartingIndex);

                if (args.OldItems != null) {
                    // NB: Tracking removes gets hard, because unless the items
                    // are objects, we have trouble telling them apart. This code
                    // is also tart, but it works.
                    foreach(T x in args.OldItems) {
                        if (filter != null && !filter(x)) {
                            continue;
                        }
                        if (orderer == null) {
                            ret.RemoveAt(oldIndex);
                            continue;
                        }
                        for(int i = 0; i < ret.Count; i++) {
                            if (orderer(ret[i], selector(x)) == 0) {
                                ret.RemoveAt(i);
                            }
                        }
                    }
                }

                if (args.NewItems != null) {
                    foreach(T x in args.NewItems) {
                        if (filter != null && !filter(x)) {
                            continue;
                        }
                        if (orderer == null) {
                            ret.Insert(args.NewStartingIndex, selector(x));
                            continue;
                        }

                        var toAdd = selector(x);
                        ret.Insert(positionForNewItem(ret, toAdd, orderer), toAdd);
                    }
                }
            }));

            return ret;
        }

        /// <summary>
        /// Creates a collection whose contents will "follow" another
        /// collection; this method is useful for creating ViewModel collections
        /// that are automatically updated when the respective Model collection
        /// is updated.
        /// 
        /// Be aware that this overload will result in a collection that *only* 
        /// updates if the source implements INotifyCollectionChanged. If your
        /// list changes but isn't a ReactiveCollection/ObservableCollection,
        /// you probably want to use the other overload.
        /// </summary>
        /// <param name="selector">A Select function that will be run on each
        /// item.</param>
        /// <param name="filter">A filter to determine whether to exclude items 
        /// in the derived collection.</param>
        /// <param name="orderer">A comparator method to determine the ordering of
        /// the resulting collection.</param>
        /// <returns>A new collection whose items are equivalent to
        /// Collection.Select().Where().OrderBy() and will mirror changes 
        /// in the initial collection.</returns>
        public static ReactiveDerivedCollection<TNew> CreateDerivedCollection<T, TNew>(
            this IEnumerable<T> This,
            Func<T, TNew> selector,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null)
        {
            return This.CreateDerivedCollection(selector, filter, orderer, Observable.Empty<Unit>());
        }

        static int positionForNewItem<T>(IList<T> list, T item, Func<T, T, int> orderer)
        {
            if (list.Count == 0) {
                return 0;
            }

            if (list.Count == 1) {
                return orderer(list[0], item) >= 0 ? 0 : 1;
            }

            if (orderer(list[0], item) >= 1) return 0;

            // NB: This is the most tart way to do this possible
            int? prevCmp = null;
            int cmp;

            for (int i = 0; i < list.Count; i++) {
                cmp = sign(orderer(list[i], item));
                if (prevCmp.HasValue && cmp != prevCmp) {
                    return i;
                }

                prevCmp = cmp;
            }

            return list.Count;
        }

        static int sign(int i)
        {
            return (i == 0 ? 0 : i / Math.Abs(i));
        }
        
        class FuncComparator<T> : IComparer<T>
        {
            Func<T, T, int> _inner;

            public FuncComparator(Func<T, T, int> comparer)
            {
                _inner = comparer;
            }

            public int Compare(T x, T y)
            {
                return _inner(x, y);
            }
        }
    }
}