using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ReactiveUI
{
    /// <summary>
    /// This is a helper interface that aggregates information
    /// on how to create a derived collection.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source collection.</typeparam>
    /// <typeparam name="TTarget">The type of the elements exposed in this collection.</typeparam>
    /// <typeparam name="TComparer">The input type of the comparer.</typeparam>
    public interface IReactiveDerivedCollectionAggregator<TSource, out TTarget, TComparer> : IEnumerable<TTarget>
    {
        IEnumerable<TSource> BaseCollection { get; }
        Func<TSource, bool> Filter { get; }
        Func<TSource, TTarget> Selector { get; }
        Func<TSource, TComparer> ComparerSelector { get; }
        Func<TComparer, TComparer, int> Orderer { get; }
        IObservable<Unit> SignalReset { get; } 
    }

    public abstract class ReactiveDerivedCollection<TValue> : ReactiveList<TValue>, IDisposable
    {
        const string readonlyExceptionMessage = "Derived collections cannot be modified.";

        public override bool IsReadOnly { get { return true; } }

        public override TValue this[int index]
        {
            get { return base[index]; }
            set { throw new InvalidOperationException(readonlyExceptionMessage); }
        }

        public override void Add(TValue item)
        {
            throw new InvalidOperationException(readonlyExceptionMessage);
        }

        protected virtual void internalAdd(TValue item)
        {
            base.Add(item);
        }

        public override void AddRange(IEnumerable<TValue> collection)
        {
            throw new InvalidOperationException(readonlyExceptionMessage);
        }

        protected virtual void internalAddRange(IEnumerable<TValue> collection)
        {
            base.AddRange(collection);
        }

        public override void Clear()
        {
            throw new InvalidOperationException(readonlyExceptionMessage);
        }

        protected virtual void internalClear()
        {
            base.Clear();
        }

        public override void Insert(int index, TValue item)
        {
            throw new InvalidOperationException(readonlyExceptionMessage);
        }

        protected virtual void internalInsert(int index, TValue item)
        {
            base.Insert(index, item);
        }

        public override void InsertRange(int index, IEnumerable<TValue> collection)
        {
            throw new InvalidOperationException(readonlyExceptionMessage);
        }

        protected virtual void internalInsertRange(int index, IEnumerable<TValue> collection)
        {
            base.InsertRange(index, collection);
        }

        public override bool Remove(TValue item)
        {
            throw new InvalidOperationException(readonlyExceptionMessage);
        }

        protected virtual bool internalRemove(TValue item)
        {
            return base.Remove(item);
        }

        public override void RemoveAll(IEnumerable<TValue> items)
        {
            throw new InvalidOperationException(readonlyExceptionMessage);
        }

        protected virtual void internalRemoveAll(IEnumerable<TValue> items)
        {
            base.RemoveAll(items);
        }

        public override void RemoveAt(int index)
        {
            throw new InvalidOperationException(readonlyExceptionMessage);
        }

        protected virtual void internalRemoveAt(int index)
        {
            base.RemoveAt(index);
        }

        public override void RemoveRange(int index, int count)
        {
            throw new InvalidOperationException(readonlyExceptionMessage);
        }

        protected virtual void internalRemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
        }

        public override void Sort(Comparison<TValue> comparison)
        {
            throw new InvalidOperationException(readonlyExceptionMessage);
        }

        protected virtual void internalSort(Comparison<TValue> comparison)
        {
            base.Sort(comparison);
        }

        public override void Sort(IComparer<TValue> comparer = null)
        {
            throw new InvalidOperationException(readonlyExceptionMessage);
        }

        protected virtual void internalSort(IComparer<TValue> comparer = null)
        {
            base.Sort(comparer);
        }

        public override void Sort(int index, int count, IComparer<TValue> comparer)
        {
            throw new InvalidOperationException(readonlyExceptionMessage);
        }

        protected virtual void internalSort(int index, int count, IComparer<TValue> comparer)
        {
            base.Sort(index, count, comparer);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public virtual void Dispose(bool disposing) { }
    }

    public sealed class ReactiveDerivedCollection<TSource, TValue> : ReactiveDerivedCollection<TValue>, IDisposable, IReactiveDerivedCollectionAggregator<TSource, TValue, TValue>
    {
        readonly IEnumerable<TSource> source;
        readonly Func<TSource, TValue> selector;
        readonly Func<TSource, bool> filter;
        readonly Func<TValue, TValue, int> orderer;
        readonly IObservable<Unit> signalReset;

        // This list maps indices in this collection to their corresponding indices in the source collection.
        List<int> indexToSourceIndexMap;
        CompositeDisposable inner;

        public ReactiveDerivedCollection(
            IEnumerable<TSource> source,
            Func<TSource, TValue> selector,
            Func<TSource, bool> filter,
            Func<TValue, TValue, int> orderer,
            IObservable<Unit> signalReset)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);

            if (filter == null)
                filter = x => true;

            this.source = source;
            this.selector = selector;
            this.filter = filter;
            this.orderer = orderer;
            this.signalReset = signalReset;

            this.inner = new CompositeDisposable();
            this.indexToSourceIndexMap = new List<int>();

            this.addAllItemsFromSourceCollection();
            this.wireUpChangeNotifications();
        }

        static readonly Dictionary<Type, bool> hasWarned = new Dictionary<Type, bool>();

        void wireUpChangeNotifications()
        {
            var incc = source as INotifyCollectionChanged;

            if (incc == null) {
                var type = source.GetType();

                lock (hasWarned) {
                    if (!hasWarned.ContainsKey(type)) {
                        this.Log().Warn(
                            "{0} doesn't implement INotifyCollectionChanged, derived collection will only update " +
                            "when the Reset() method is invoked manually or the reset observable is signalled.",
                            type.FullName);
                        hasWarned.Add(type, true);
                    }
                }
            } else {
                var collChanged = new Subject<NotifyCollectionChangedEventArgs>();

                var connObs = Observable
                    .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        x => incc.CollectionChanged += x,
                        x => incc.CollectionChanged -= x)
                    .Select(x => x.EventArgs)
                    .Multicast(collChanged);

                inner.Add(collChanged.Subscribe(onSourceCollectionChanged));
                inner.Add(connObs.Connect());
            }

            var irc = source as IReactiveCollection;

            if (irc != null) {
                inner.Add(irc.ItemChanged.Select(x => (TSource)x.Sender).Subscribe(onItemChanged));
            }

            if (signalReset != null) {
                inner.Add(signalReset.Subscribe(x => this.Reset()));
            }
        }

        void onItemChanged(TSource changedItem)
        {
            // If you've implemented INotifyPropertyChanged on a struct then you're doing it wrong(TM) and change
            // tracking won't work in derived collections (change tracking for value types makes no sense any way)
            // NB: It's possible the sender exists in multiple places in the source collection.
            var sourceIndices = indexOfAll(source, changedItem, ReferenceEqualityComparer<TSource>.Default);

            var shouldBeIncluded = filter(changedItem);

            foreach (int sourceIndex in sourceIndices) {

                int destinationIndex = getIndexFromSourceIndex(sourceIndex);
                bool isIncluded = destinationIndex >= 0;

                if (isIncluded && !shouldBeIncluded) {
                    internalRemoveAt(destinationIndex);
                } else if (!isIncluded && shouldBeIncluded) {
                    internalInsertAndMap(sourceIndex, selector(changedItem));
                } else if (isIncluded && shouldBeIncluded) {
                    // The item is already included and it should stay there but it's possible that the change that
                    // caused this event affects the ordering. This gets a little tricky so let's be verbose.

                    TValue newItem = selector(changedItem);

                    if (orderer == null) {
                        // We don't have an orderer so we're currently using the source collection index for sorting 
                        // meaning that no item change will affect ordering. Look at our current item and see if it's
                        // the exact (reference-wise) same object. If it is then we're done, if it's not (for example 
                        // if it's an integer) we'll issue a replace event so that subscribers get the new value.
                        if (!object.ReferenceEquals(newItem, this[destinationIndex])) {
                            internalReplace(destinationIndex, newItem);
                        }
                    } else {
                        // Don't be tempted to just use the orderer to compare the new item with the previous since
                        // they'll almost certainly be equal (for reference types). We need to test whether or not the
                        // new item can stay in the same position that the current item is in without comparing them.
                        if (canItemStayAtPosition(newItem, destinationIndex)) {
                            // The new item should be in the same position as the current but there's no need to signal
                            // that in case they are the same object.
                            if (!object.ReferenceEquals(newItem, this[destinationIndex])) {
                                internalReplace(destinationIndex, newItem);
                            }
                        } else {
                            // The change is forcing us to reorder, implemented as a remove and insert.
                            internalRemoveAt(destinationIndex);
                            internalInsertAndMap(sourceIndex, newItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the item fits (sort-wise) at the provided index. The determination
        /// is made by checking whether or not it's considered larger than or equal to the preceeding item and if
        /// it's less than or equal to the succeeding item.
        /// </summary>
        bool canItemStayAtPosition(TValue item, int currentIndex)
        {
            bool hasPrecedingItem = currentIndex > 0;

            if (hasPrecedingItem) {
                bool isGreaterThanOrEqualToPrecedingItem = orderer(item, this[currentIndex - 1]) >= 0;
                if (!isGreaterThanOrEqualToPrecedingItem) {
                    return false;
                }
            }

            bool hasSucceedingItem = currentIndex < this.Count - 1;

            if (hasSucceedingItem) {
                bool isLessThanOrEqualToSucceedingItem = orderer(item, this[currentIndex + 1]) <= 0;
                if (!isLessThanOrEqualToSucceedingItem) {
                    return false;
                }
            }

            return true;
        }

        void internalReplace(int destinationIndex, TValue newItem)
        {
            base.SetItem(destinationIndex, newItem);
        }

        /// <summary>
        /// Gets the index of the dervived item based on it's originating element index in the source collection.
        /// </summary>
        int getIndexFromSourceIndex(int sourceIndex)
        {
            return this.indexToSourceIndexMap.IndexOf(sourceIndex);
        }

        /// <summary>
        /// Returns one or more positions in the source collection where the given item is found based on the
        /// provided equality comparer.
        /// </summary>
        IEnumerable<int> indexOfAll(IEnumerable<TSource> source, TSource item,
            IEqualityComparer<TSource> equalityComparer)
        {
            int sourceIndex = 0;
            foreach (var x in source) {

                if (equalityComparer.Equals(x, item)) {
                    yield return sourceIndex;
                }

                sourceIndex++;
            }
        }

        void onSourceCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset) {
                this.Reset();
                return;
            }

            if (args.OldItems != null) {
                for (int i = 0; i < args.OldItems.Count; i++) {
                    int destinationIndex = getIndexFromSourceIndex(args.OldStartingIndex + i);
                    if (destinationIndex != -1) {
                        internalRemoveAt(destinationIndex);
                    }
                }

                int removedCount = args.OldItems.Count;
                shiftIndicesAtOrOverThreshold(args.OldStartingIndex + removedCount, -removedCount);
            }

            if (args.NewItems != null) {
                shiftIndicesAtOrOverThreshold(args.NewStartingIndex, args.NewItems.Count);

                for (int i = 0; i < args.NewItems.Count; i++) {
                    var sourceItem = (TSource)args.NewItems[i];

                    if (!filter(sourceItem)) {
                        continue;
                    }

                    var destinationItem = selector(sourceItem);
                    internalInsertAndMap(args.NewStartingIndex + i, destinationItem);
                }
            }
        }

        /// <summary>
        /// Increases (or decreases) all source indices equal to or higher than the threshold. Represents an
        /// insert or remove of one or more items in the source list thus causing all subsequent items to shift
        /// up or down.
        /// </summary>
        void shiftIndicesAtOrOverThreshold(int threshold, int value)
        {
            for (int i = 0; i < indexToSourceIndexMap.Count; i++) {
                if (indexToSourceIndexMap[i] >= threshold) {
                    indexToSourceIndexMap[i] += value;
                }
            }
        }

        public override void Reset()
        {
            using (base.SuppressChangeNotifications()) {
                if (this.Count > 0)
                    internalClear();

                addAllItemsFromSourceCollection();
            }
        }

        void addAllItemsFromSourceCollection()
        {
            int sourceIndex = 0;

            foreach (TSource sourceItem in source) {
                if (filter(sourceItem)) {
                    var destinationItem = selector(sourceItem);
                    internalInsertAndMap(sourceIndex, destinationItem);
                }

                sourceIndex++;
            }
        }

        protected override void internalClear()
        {
            indexToSourceIndexMap.Clear();
            base.internalClear();
        }

        void internalInsertAndMap(int sourceIndex, TValue value)
        {
            int destinationIndex = positionForNewItem(sourceIndex, value);

            indexToSourceIndexMap.Insert(destinationIndex, sourceIndex);
            base.internalInsert(destinationIndex, value);
        }

        protected override void internalRemoveAt(int destinationIndex)
        {
            indexToSourceIndexMap.RemoveAt(destinationIndex);
            base.internalRemoveAt(destinationIndex);
        }

        /// <summary>
        /// Internal equality comparer used for looking up the source object of a property change notification in
        /// the source list.
        /// </summary>
        class ReferenceEqualityComparer<T> : IEqualityComparer<T>
        {
            public static readonly ReferenceEqualityComparer<T> Default = new ReferenceEqualityComparer<T>();

            public bool Equals(T x, T y)
            {
                return object.ReferenceEquals(x, y);
            }

            public int GetHashCode(T obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }

        int positionForNewItem(int sourceIndex, TValue value)
        {
            // If we haven't got an orderer we'll simply match our items to that of the source collection.
            int destinationIndex = orderer == null
                ? positionForNewItem(this.indexToSourceIndexMap, sourceIndex, (x, y) => x.CompareTo(y))
                : positionForNewItem(this, value, orderer);

            return destinationIndex;
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

        public override void Dispose(bool disposing)
        {
            if (disposing) {
                var disp = Interlocked.Exchange(ref inner, null);
                if (disp == null) return;

                disp.Dispose();
            }
        }

        IEnumerable<TSource> IReactiveDerivedCollectionAggregator<TSource, TValue, TValue>.BaseCollection
        {
            get { return source; }
        }

        Func<TSource, bool> IReactiveDerivedCollectionAggregator<TSource, TValue, TValue>.Filter
        {
            get { return filter; }
        }

        Func<TSource, TValue> IReactiveDerivedCollectionAggregator<TSource, TValue, TValue>.Selector
        {
            get { return selector; }
        }

        Func<TSource, TValue> IReactiveDerivedCollectionAggregator<TSource, TValue, TValue>.ComparerSelector
        {
            get { return selector; }
        }

        Func<TValue, TValue, int> IReactiveDerivedCollectionAggregator<TSource, TValue, TValue>.Orderer
        {
            get { return orderer; }
        }

        IObservable<Unit> IReactiveDerivedCollectionAggregator<TSource, TValue, TValue>.SignalReset
        {
            get { return signalReset; }
        }
    }

    internal class ReactiveDerivedCollectionFromObservable<T>: ReactiveDerivedCollection<T>
    {
        SingleAssignmentDisposable inner;

        public ReactiveDerivedCollectionFromObservable(
            IObservable<T> observable,
            TimeSpan? withDelay = null,
            Action<Exception> onError = null)
        {
            this.inner = new SingleAssignmentDisposable();

            onError = onError ?? (ex => RxApp.DefaultExceptionHandler.OnNext(ex));
            if (withDelay == null) {
                inner.Disposable = observable.ObserveOn(RxApp.MainThreadScheduler).Subscribe(internalAdd, onError);
                return;
            }

            // On a timer, dequeue items from queue if they are available
            var queue = new Queue<T>();
            var disconnect = Observable.Timer(withDelay.Value, withDelay.Value, RxApp.MainThreadScheduler)
                .Subscribe(_ => {
                    if (queue.Count > 0) { 
                        this.internalAdd(queue.Dequeue());
                    }
                });

            inner.Disposable = disconnect;

            // When new items come in from the observable, stuff them in the queue.
            // Using the MainThreadScheduler guarantees we'll always access the queue
            // from the same thread.
            observable.ObserveOn(RxApp.MainThreadScheduler).Subscribe(queue.Enqueue, onError);

            // This is a bit clever - keep a running count of the items actually 
            // added and compare them to the final count of items provided by the
            // Observable. Combine the two values, and when they're equal, 
            // disconnect the timer
            this.ItemsAdded.Scan(0, ((acc, _) => acc + 1)).Zip(observable.Aggregate(0, (acc, _) => acc + 1),
                (l,r) => (l == r)).Where(x => x).Subscribe(_ => disconnect.Dispose());
        }

        public override void Dispose(bool disposing)
        {
            if (disposing) {
                var disp = Interlocked.Exchange(ref inner, null);
                if (disp == null) return;

                disp.Dispose();
            }
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
            return new ReactiveDerivedCollectionFromObservable<T>(fromObservable, withDelay, onError);
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

    class ReactiveDerivedCollectionAggregator<TSource, TTarget, TComparer> : IReactiveDerivedCollectionAggregator<TSource, TTarget, TComparer>
    {
        readonly IEnumerable<TTarget> _collection; 
        readonly IEnumerable<TSource> _baseCollection;
        readonly Func<TSource, bool> _filter;
        readonly Func<TSource, TTarget> _selector;
        readonly Func<TComparer, TComparer, int> _orderer;
        readonly IObservable<Unit> _signalReset;
        readonly Func<TSource, TComparer> _comparerSelector;

        public ReactiveDerivedCollectionAggregator(
            IEnumerable<TSource> baseCollection,
            Func<TSource, bool> filter,
            Func<TSource, TTarget> selector,
            Func<TComparer, TComparer, int> orderer,
            Func<TSource, TComparer> comparerSelector,
            IObservable<Unit> signalReset,
            IEnumerable<TTarget> collection)
        {
            if (baseCollection == null) {
                throw new ArgumentNullException("baseCollection");
            }
            if (selector == null) throw new ArgumentNullException("selector");
            if (orderer != null && comparerSelector == null) throw new ArgumentNullException("comparerSelector");

            if (collection == null) {
                throw new ArgumentNullException("collection");
            }

            _baseCollection = baseCollection;
            _filter = filter;
            _selector = selector;
            _orderer = orderer;
            _signalReset = signalReset;
            _collection = collection;
            _comparerSelector = comparerSelector;
        }

        public IEnumerable<TSource> BaseCollection
        {
            get { return _baseCollection; }
        }

        public Func<TSource, bool> Filter
        {
            get { return _filter; }
        }

        public Func<TSource, TTarget> Selector
        {
            get { return _selector; }
        }

        public Func<TSource, TComparer> ComparerSelector
        {
            get { return _comparerSelector; }
        }

        public Func<TComparer, TComparer, int> Orderer
        {
            get { return _orderer; }
        }

        public IObservable<Unit> SignalReset
        {
            get { return _signalReset; }
        }

        public IEnumerator<TTarget> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class ObservableCollectionMixin
    {
        /// <summary>
        /// Interprets a given sequence as a reactive sequence, which enables LinQ operations with change notification.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="sequence">A sequence containing the elements.</param>
        /// <param name="signalReset">An observable that, when signaled, indicates that the whole sequence should be queried again.</param>
        /// <returns>An instance of <see cref="IReactiveDerivedCollectionAggregator{T, T, T}"/> representing the given <paramref name="sequence"/>.</returns>
        public static IReactiveDerivedCollectionAggregator<T, T, T> AsReactive<T>(this IEnumerable<T> sequence, IObservable<Unit> signalReset = null)
        {
            if (sequence == null) throw new ArgumentNullException("sequence");

            return new ReactiveDerivedCollectionAggregator<T, T, T>(sequence, null, x => x, null, null, signalReset, sequence);
        }

        /// <summary>
        /// Materializes the given instance of <see cref="IReactiveDerivedCollectionAggregator{TSource, T, TComparer}"/> into an instance
        /// of <see cref="ReactiveDerivedCollection{TSource, T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <typeparam name="TComparer">The input type of the comparer used for ordering the sequence.</typeparam>
        /// <param name="This">An instance of <see cref="IReactiveDerivedCollectionAggregator{TSource, T, TComparer}"/> to materialize.</param>
        /// <returns>
        /// A new instance of <see cref="IReactiveDerivedCollectionAggregator{TSource, T, TComparer}"/> which contains the elements
        /// appropriately filtered, transformed and ordered.
        /// </returns>
        public static ReactiveDerivedCollection<TSource, T> ToDerivedCollection<TSource, T, TComparer>(
            this IReactiveDerivedCollectionAggregator<TSource, T, TComparer> This)
        {
            if (This == null) throw new ArgumentNullException("This");

            Func<T, T, int> comparer;

            if (This.Orderer == null) {
                comparer = null;
            }
            else if(typeof(T) == typeof(TComparer)) {
                comparer = (Func<T, T, int>) (object) This.Orderer;
            }
            else {
                throw new NotSupportedException("If the sequence is ordered, it needs to be the last operation on the sequence.");
            }

            return CreateDerivedCollection(This.BaseCollection, This.Selector, This.Filter, comparer, This.SignalReset);
        }

        /// <summary>
        /// Creates a collection whose contents will "follow" another
        /// collection; this method is useful for creating ViewModel collections
        /// that are automatically updated when the respective Model collection
        /// is updated.
        ///
        /// Note that even though this method attaches itself to any 
        /// IEnumerable, it will only detect changes from objects implementing
        /// INotifyCollectionChanged (like ReactiveList). If your source
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
        public static ReactiveDerivedCollection<T, TNew> CreateDerivedCollection<T, TNew, TDontCare>(
            this IEnumerable<T> This,
            Func<T, TNew> selector,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null,
            IObservable<TDontCare> signalReset = null)
        {
            Contract.Requires(selector != null);

            IObservable<Unit> reset = null;

            if (signalReset != null) {
                reset = signalReset.Select(_ => Unit.Default);
            }

            return new ReactiveDerivedCollection<T, TNew>(This, selector, filter, orderer, reset);
        }

        /// <summary>
        /// Creates a collection whose contents will "follow" another
        /// collection; this method is useful for creating ViewModel collections
        /// that are automatically updated when the respective Model collection
        /// is updated.
        /// 
        /// Be aware that this overload will result in a collection that *only* 
        /// updates if the source implements INotifyCollectionChanged. If your
        /// list changes but isn't a ReactiveList/ObservableCollection,
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
        public static ReactiveDerivedCollection<T, TNew> CreateDerivedCollection<T, TNew>(
            this IEnumerable<T> This,
            Func<T, TNew> selector,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null)
        {
            return This.CreateDerivedCollection(selector, filter, orderer, (IObservable<Unit>) null);
        }

        public static IReactiveDerivedCollectionAggregator<TSource, T, TComparer> Where<T, TSource, TComparer>(
            this IReactiveDerivedCollectionAggregator<TSource, T, TComparer> This,
            Func<T, bool> predicate)
        {
            if (This == null) throw new ArgumentNullException("This");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return new ReactiveDerivedCollectionAggregator<TSource, T, TComparer>(
                This.BaseCollection,
                x => (This.Filter == null || This.Filter(x)) && predicate(This.Selector(x)),
                This.Selector,
                This.Orderer,
                This.ComparerSelector,
                This.SignalReset,
                Enumerable.Where(This, predicate));
        }

        public static IReactiveDerivedCollectionAggregator<TSource, TTarget, TComparer> Select
            <TSource, T, TTarget, TComparer>(
            this IReactiveDerivedCollectionAggregator<TSource, T, TComparer> This,
            Func<T, TTarget> selector)
        {
            if (This == null) throw new ArgumentNullException("This");
            if (selector == null) throw new ArgumentNullException("selector");

            return new ReactiveDerivedCollectionAggregator<TSource, TTarget, TComparer>(
                This.BaseCollection,
                This.Filter,
                x => selector(This.Selector(x)),
                This.Orderer,
                This.ComparerSelector,
                This.SignalReset,
                Enumerable.Select(This, selector));
        }

        public static IReactiveDerivedCollectionAggregator<TSource, T, TKey> OrderBy<TSource, T, TKey, TDontCare>(
            this IReactiveDerivedCollectionAggregator<TSource, T, TDontCare> This,
            Func<T, TKey> keySelector)
        {
            return OrderBy(This, keySelector, Comparer<TKey>.Default);
        }

        public static IReactiveDerivedCollectionAggregator<TSource, T, TKey> OrderBy<TSource, T, TKey, TDontCare>(
            this IReactiveDerivedCollectionAggregator<TSource, T, TDontCare> This,
            Func<T, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            if (This == null) throw new ArgumentNullException("This");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (comparer == null) throw new ArgumentNullException("comparer");


            return new ReactiveDerivedCollectionAggregator<TSource, T, TKey>(
                This.BaseCollection,
                This.Filter,
                This.Selector,
                comparer.Compare,
                x => keySelector(This.Selector(x)),
                This.SignalReset,
                Enumerable.OrderBy(This, keySelector, comparer));
        }
    }
}