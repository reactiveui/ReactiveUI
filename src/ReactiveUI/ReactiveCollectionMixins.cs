using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// This class represents a change-notifying Collection which is derived from
    /// a source collection, via CreateDerivedCollection or via another method. 
    /// It is read-only, and any attempts to change items in the collection will
    /// fail.
    /// </summary>
    internal abstract class ReactiveDerivedCollection<TValue> : ReactiveList<TValue>, IReactiveDerivedList<TValue>, IDisposable
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

#if !SILVERLIGHT
        public override void Move(int oldIndex, int newIndex)
        {
            throw new InvalidOperationException(readonlyExceptionMessage);
        }

        protected virtual void internalMove(int oldIndex, int newIndex)
        {
            base.Move(oldIndex, newIndex);
        }
#endif

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

    /// <summary>
    /// This class represents a change-notifying Collection which is derived from
    /// a source collection, via CreateDerivedCollection or via another method. 
    /// It is read-only, and any attempts to change items in the collection will
    /// fail.
    /// </summary>
    internal class ReactiveDerivedCollection<TSource, TValue> : ReactiveDerivedCollection<TValue>, IDisposable
    {
        readonly IEnumerable<TSource> source;
        readonly Func<TSource, TValue> selector;
        readonly Func<TSource, bool> filter;
        readonly Func<TValue, TValue, int> orderer;
        readonly Action<TValue> onRemoved;
        readonly IObservable<Unit> signalReset;
        readonly IScheduler scheduler;

        // This list maps indices in this collection to their corresponding indices in the source collection.
        List<int> indexToSourceIndexMap;
        List<TSource> sourceCopy;
        CompositeDisposable inner;

        public ReactiveDerivedCollection(
            IEnumerable<TSource> source,
            Func<TSource, TValue> selector,
            Func<TSource, bool> filter,
            Func<TValue, TValue, int> orderer,
            Action<TValue> onRemoved,
            IObservable<Unit> signalReset,
            IScheduler scheduler)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);

            if (filter == null)
                filter = x => true;

            this.source = source;
            this.selector = selector;
            this.filter = filter;
            this.orderer = orderer;
            this.onRemoved = onRemoved ?? (_ => { });
            this.signalReset = signalReset;
            this.scheduler = scheduler;

            this.inner = new CompositeDisposable();
            this.indexToSourceIndexMap = new List<int>();
            this.sourceCopy = new List<TSource>();

            this.inner.Add(Disposable.Create(() => {
                foreach (var item in this) { this.onRemoved(item); }
            }));

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
                var irncc = source as IReactiveNotifyCollectionChanged<TSource>;
                var eventObs = irncc != null
                    ? irncc.Changed
                    : Observable
                        .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                            x => incc.CollectionChanged += x,
                            x => incc.CollectionChanged -= x)
                        .Select(x => x.EventArgs);
                inner.Add(eventObs.ObserveOn(scheduler).Subscribe(onSourceCollectionChanged));
            }

            var irc = source as IReactiveCollection<TSource>;

            if (irc != null) {
                inner.Add(irc.ItemChanged.Select(x => x.Sender).ObserveOn(scheduler).Subscribe(onItemChanged));
            }

            if (signalReset != null) {
                inner.Add(signalReset.ObserveOn(scheduler).Subscribe(x => this.Reset()));
            }
        }

        void onItemChanged(TSource changedItem)
        {
            // If you've implemented INotifyPropertyChanged on a struct then you're doing it wrong(TM) and change
            // tracking won't work in derived collections (change tracking for value types makes no sense any way)
            // NB: It's possible the sender exists in multiple places in the source collection.
            var sourceIndices = indexOfAll(sourceCopy, changedItem, ReferenceEqualityComparer<TSource>.Default);

            var shouldBeIncluded = filter(changedItem);

            foreach (int sourceIndex in sourceIndices) {

                int currentDestinationIndex = getIndexFromSourceIndex(sourceIndex);
                bool isIncluded = currentDestinationIndex >= 0;

                if (isIncluded && !shouldBeIncluded) {
                    internalRemoveAt(currentDestinationIndex);
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
                        if (!object.ReferenceEquals(newItem, this[currentDestinationIndex])) {
                            internalReplace(currentDestinationIndex, newItem);
                        }
                    } else {
                        // Don't be tempted to just use the orderer to compare the new item with the previous since
                        // they'll almost certainly be equal (for reference types). We need to test whether or not the
                        // new item can stay in the same position that the current item is in without comparing them.
                        if (canItemStayAtPosition(newItem, currentDestinationIndex)) {
                            // The new item should be in the same position as the current but there's no need to signal
                            // that in case they are the same object.
                            if (!object.ReferenceEquals(newItem, this[currentDestinationIndex])) {
                                internalReplace(currentDestinationIndex, newItem);
                            }
                        } else {
#if !SILVERLIGHT
                            // The change is forcing us to reorder. We'll use a move operation if the item hasn't 
                            // changed (ie it's the same object) and we'll implement it as a remove and add if the
                            // object has changed (ie the selector is not an identity function).
                            if (object.ReferenceEquals(newItem, this[currentDestinationIndex])) {

                                int newDestinationIndex = newPositionForExistingItem(
                                    sourceIndex, currentDestinationIndex, newItem);

                                Debug.Assert(newDestinationIndex != currentDestinationIndex,
                                    "This can't be, canItemStayAtPosition said it this couldn't happen");

                                indexToSourceIndexMap.RemoveAt(currentDestinationIndex);
                                indexToSourceIndexMap.Insert(newDestinationIndex, sourceIndex);

                                base.internalMove(currentDestinationIndex, newDestinationIndex);

                            } else {
                                internalRemoveAt(currentDestinationIndex);
                                internalInsertAndMap(sourceIndex, newItem);
                            }
#else
                            internalRemoveAt(currentDestinationIndex);
                            internalInsertAndMap(sourceIndex, newItem);
#endif
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
            var item = this[destinationIndex];
            base.SetItem(destinationIndex, newItem);
            onRemoved(item);
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
        List<int> indexOfAll(IEnumerable<TSource> source, TSource item,
            IEqualityComparer<TSource> equalityComparer)
        {
            var indices = new List<int>(1);
            int sourceIndex = 0;
            foreach (var x in source) {

                if (equalityComparer.Equals(x, item)) {
                    indices.Add(sourceIndex);
                }

                sourceIndex++;
            }

            return indices;
        }

        void onSourceCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset) {
                this.Reset();
                return;
            }

#if !SILVERLIGHT
            if (args.Action == NotifyCollectionChangedAction.Move) {

                Debug.Assert(args.OldItems.Count == args.NewItems.Count);

                if (args.OldItems.Count > 1 || args.NewItems.Count > 1) {
                    throw new NotSupportedException("Derived collections doesn't support multi-item moves");
                }

                // Yeah apparently this can happen. ObservableCollection triggers this notification on Move(0,0)
                if (args.OldStartingIndex == args.NewStartingIndex) {
                    return;
                }

                int oldSourceIndex = args.OldStartingIndex;
                int newSourceIndex = args.NewStartingIndex;

                sourceCopy.RemoveAt(oldSourceIndex);
                sourceCopy.Insert(newSourceIndex, (TSource)args.NewItems[0]);

                int currentDestinationIndex = getIndexFromSourceIndex(oldSourceIndex);

                moveSourceIndexInMap(oldSourceIndex, newSourceIndex);

                if (currentDestinationIndex == -1) {
                    return;
                }

                TValue value = base[currentDestinationIndex];

                if (orderer == null) {
                    // We mirror the order of the source collection so we'll perform the same move operation
                    // as the source. As is the case with when we have an orderer we don't test whether or not
                    // the item should be included or not here. If it has been included at some point it'll
                    // stay included until onItemChanged picks up a change which filters it.
                    int newDestinationIndex = newPositionForExistingItem(
                        indexToSourceIndexMap, newSourceIndex, currentDestinationIndex);

                    if (newDestinationIndex != currentDestinationIndex) {
                        indexToSourceIndexMap.RemoveAt(currentDestinationIndex);
                        indexToSourceIndexMap.Insert(newDestinationIndex, newSourceIndex);

                        base.internalMove(currentDestinationIndex, newDestinationIndex);
                    } else {
                        indexToSourceIndexMap[currentDestinationIndex] = newSourceIndex;
                    }
                } else {
                    // TODO: Conceptually I feel like we shouldn't concern ourselves with ordering when we 
                    // receive a Move notification. If it affects ordering it should be picked up by the
                    // onItemChange and resorted there instead.
                    indexToSourceIndexMap[currentDestinationIndex] = newSourceIndex;
                }

                return;
            }
#endif

            if (args.OldItems != null) {

                sourceCopy.RemoveRange(args.OldStartingIndex, args.OldItems.Count);

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
                    sourceCopy.Insert(args.NewStartingIndex + i, sourceItem);

                    if (!filter(sourceItem)) {
                        continue;
                    }

                    var destinationItem = selector(sourceItem);
                    internalInsertAndMap(args.NewStartingIndex + i, destinationItem);
                }
            }
        }

        /// <summary>
        /// Increases (or decreases depending on move direction) all source indices between the source and destination
        /// move indices.
        /// </summary>
        void moveSourceIndexInMap(int oldSourceIndex, int newSourceIndex)
        {
            if (newSourceIndex > oldSourceIndex) {
                // Item is moving towards the end of the list, everything between its current position and its 
                // new position needs to be shifted down one index
                shiftSourceIndicesInRange(oldSourceIndex + 1, newSourceIndex + 1, -1);
            } else {
                // Item is moving towards the front of the list, everything between its current position and its
                // new position needs to be shifted up one index
                shiftSourceIndicesInRange(newSourceIndex, oldSourceIndex, 1);
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

        /// <summary>
        /// Increases (or decreases) all source indices within the range (lower inclusive, upper exclusive). 
        /// </summary>
        void shiftSourceIndicesInRange(int rangeStart, int rangeStop, int value)
        {
            for (int i = 0; i < indexToSourceIndexMap.Count; i++) {
                int sourceIndex = indexToSourceIndexMap[i];
                if (sourceIndex >= rangeStart && sourceIndex < rangeStop) {
                    indexToSourceIndexMap[i] += value;
                }
            }
        }


        public override void Reset()
        {
            using (base.SuppressChangeNotifications()) {
                internalClear();
                addAllItemsFromSourceCollection();
            }
        }

        void addAllItemsFromSourceCollection()
        {
            Debug.Assert(sourceCopy.Count == 0, "Expceted source copy to be empty");

            int sourceIndex = 0;

            foreach (TSource sourceItem in source) {

                sourceCopy.Add(sourceItem);

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
            sourceCopy.Clear();
            var items = this.ToArray();

            base.internalClear();

            foreach (var item in items) { onRemoved(item); }
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
            var item = this[destinationIndex];
            base.internalRemoveAt(destinationIndex);
            onRemoved(item);
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
            return orderer == null
                ? positionForNewItem(indexToSourceIndexMap, sourceIndex, Comparer<int>.Default.Compare)
                : positionForNewItem(this, 0, this.Count, value, orderer);
        }

        internal static int positionForNewItem<T>(IList<T> list, T item, Func<T, T, int> orderer)
        {
            return positionForNewItem(list, 0, list.Count, item, orderer);
        }

        internal static int positionForNewItem<T>(
            IList<T> list, int index, int count, T item, Func<T, T, int> orderer)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(count >= 0);
            Debug.Assert((list.Count - index) >= count);

            if (count == 0) {
                return index;
            }

            if (count == 1) {
                return orderer(list[index], item) >= 0 ? index : index + 1;
            }

            if (orderer(list[index], item) >= 1) return index;

            int low = index, hi = index + count - 1;
            int mid, cmp;

            while (low <= hi) {
                mid = low + (hi - low) / 2;
                cmp = orderer(list[mid], item);

                if (cmp == 0) {
                    return mid;
                }

                if (cmp < 0) {
                    low = mid + 1;
                } else {
                    hi = mid - 1;
                }
            }

            return low;
        }

        /// <summary>
        /// Calculates a new destination for an updated item that's already in the list.
        /// </summary>
        int newPositionForExistingItem(int sourceIndex, int currentIndex, TValue item)
        {
            // If we haven't got an orderer we'll simply match our items to that of the source collection.
            return orderer == null
                ? newPositionForExistingItem(indexToSourceIndexMap, sourceIndex, currentIndex)
                : newPositionForExistingItem(this, item, currentIndex, orderer);
        }

        /// <summary>
        /// Calculates a new destination for an updated item that's already in the list.
        /// </summary>
        internal static int newPositionForExistingItem<T>(
            IList<T> list, T item, int currentIndex, Func<T, T, int> orderer = null)
        {
            // Since the item changed is most likely a value type we must refrain from ever comparing it to itself.
            // We do this by figuring out how the updated item compares to its neighbors. By knowing if it's
            // less than or greater than either one of its neighbors we can limit the search range to a range exlusive
            // of the current index.

            Debug.Assert(list.Count > 0);

            if (list.Count == 1) {
                return 0;
            }

            int precedingIndex = currentIndex - 1;
            int succeedingIndex = currentIndex + 1;

            // The item on the preceding or succeeding index relative to currentIndex.
            T comparand = list[precedingIndex >= 0 ? precedingIndex : succeedingIndex];

            if (orderer == null) {
                orderer = Comparer<T>.Default.Compare;
            }

            // Compare that to the (potentially) new value.
            int cmp = orderer(item, comparand);

            int min = 0;
            int max = list.Count;

            if (cmp == 0) {
                // The new value is equal to the preceding or succeeding item, it may stay at the current position
                return currentIndex;
            } else if (cmp > 0) {
                // The new value is greater than the preceding or succeeding item, limit the search to indices after
                // the succeeding item.
                min = succeedingIndex;
            } else {
                // The new value is less than the preceding or succeeding item, limit the search to indices before
                // the preceding item.
                max = precedingIndex;
            }

            // Bail if the search range is invalid.
            if (min == list.Count || max < 0) {
                return currentIndex;
            }

            int ix = positionForNewItem(list, min, max - min, item, orderer);

            // If the item moves 'forward' in the collection we have to account for the index where
            // the item currently resides getting removed first.
            return ix >= currentIndex ? ix - 1 : ix;
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

    internal class ReactiveDerivedCollectionFromObservable<T> : ReactiveDerivedCollection<T>
    {
        SingleAssignmentDisposable inner;

        public ReactiveDerivedCollectionFromObservable(
            IObservable<T> observable,
            TimeSpan? withDelay = null,
            Action<Exception> onError = null,
            IScheduler scheduler = null)
        {
            scheduler = scheduler ?? RxApp.MainThreadScheduler;
            this.inner = new SingleAssignmentDisposable();

            onError = onError ?? (ex => RxApp.DefaultExceptionHandler.OnNext(ex));
            if (withDelay == null) {
                inner.Disposable = observable.ObserveOn(scheduler).Subscribe(internalAdd, onError);
                return;
            }

            // On a timer, dequeue items from queue if they are available
            var queue = new Queue<T>();
            var disconnect = Observable.Timer(withDelay.Value, withDelay.Value, scheduler)
                .Subscribe(_ => {
                    if (queue.Count > 0) {
                        this.internalAdd(queue.Dequeue());
                    }
                });

            inner.Disposable = disconnect;

            // When new items come in from the observable, stuff them in the queue.
            observable.ObserveOn(scheduler).Subscribe(queue.Enqueue, onError);

            // This is a bit clever - keep a running count of the items actually 
            // added and compare them to the final count of items provided by the
            // Observable. Combine the two values, and when they're equal, 
            // disconnect the timer
            this.ItemsAdded.Scan(0, ((acc, _) => acc + 1)).Zip(observable.Aggregate(0, (acc, _) => acc + 1),
                (l, r) => (l == r)).Where(x => x).Subscribe(_ => disconnect.Dispose());
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

    /// <summary>
    /// Extension methods to create collections from observables
    /// </summary>
    public static class ReactiveCollectionMixins
    {
        /// <summary>
        /// Creates a collection based on an an Observable by adding items
        /// provided until the Observable completes. This method guarantees that
        /// items are always added in the context of the provided scheduler.
        /// </summary>
        /// <param name="fromObservable">
        /// The Observable whose items will be put into the new collection.
        /// </param>
        /// <param name="scheduler">
        /// Optionally specifies the scheduler on which
        /// the collection will be populated. Defaults to the main scheduler.
        /// </param>
        /// <returns>
        /// A new collection which will be populated with the Observable.
        /// </returns>
        public static IReactiveDerivedList<T> CreateCollection<T>(
            this IObservable<T> fromObservable,
            IScheduler scheduler)
        {
            return new ReactiveDerivedCollectionFromObservable<T>(fromObservable, scheduler: scheduler);
        }

        /// <summary>
        /// Creates a collection based on an an Observable by adding items
        /// provided until the Observable completes, optionally ensuring a
        /// delay. Note that if the Observable never completes and withDelay is
        /// set, this method will leak a Timer. This method also guarantees that
        /// items are always added in the context of the provided scheduler.
        /// </summary>
        /// <param name="fromObservable">
        /// The Observable whose items will be put into the new collection.
        /// </param>
        /// <param name="onError">
        /// The handler for errors from the Observable. If not specified, 
        /// an error will go to DefaultExceptionHandler.
        /// </param>
        /// <param name="withDelay">
        /// If set, items will be populated in the collection no faster than the delay provided.
        /// </param>
        /// <param name="scheduler">
        /// Optionally specifies the scheduler on which the collection will be populated. 
        /// Defaults to the main scheduler.
        /// </param>
        /// <returns>
        /// A new collection which will be populated with the Observable.
        /// </returns>
        public static IReactiveDerivedList<T> CreateCollection<T>(
            this IObservable<T> fromObservable,
            TimeSpan? withDelay = null,
            Action<Exception> onError = null,
            IScheduler scheduler = null)
        {
            return new ReactiveDerivedCollectionFromObservable<T>(fromObservable, withDelay, onError, scheduler);
        }
    }

    /// <summary>
    /// Extension methods to create collections that "follow" other collections.
    /// </summary>
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
        /// <see cref="INotifyCollectionChanged"/> (like <see cref="ReactiveList{T}"/>). 
        /// If your source collection doesn't implement this, <paramref name="signalReset"/> 
        /// is the way to signal the derived collection to reorder/refilter itself.
        /// </summary>
        /// <param name="This">
        /// The source <see cref="IEnumerable{T}"/> to track.
        /// </param>
        /// <param name="selector">
        /// A Select function that will be run on each item.
        /// </param>
        /// <param name="onRemoved">
        /// An action that is called on each item when it is removed.
        /// </param>
        /// <param name="filter">
        /// A filter to determine whether to exclude items in the derived collection.
        /// </param>
        /// <param name="orderer">
        /// A comparator method to determine the ordering of the resulting collection.
        /// </param>
        /// <param name="signalReset">
        /// When this Observable is signalled, the derived collection will be manually 
        /// reordered/refiltered.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler used to dispatch change notifications.
        /// </param>
        /// <returns>
        /// A new collection whose items are equivalent to
        /// <c>Collection.Select().Where().OrderBy()</c> and will mirror changes 
        /// in the initial collection.
        /// </returns>
        public static IReactiveDerivedList<TNew> CreateDerivedCollection<T, TNew, TDontCare>(
            this IEnumerable<T> This,
            Func<T, TNew> selector,
            Action<TNew> onRemoved,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null,
            IObservable<TDontCare> signalReset = null,
            IScheduler scheduler = null)
        {
            Contract.Requires(selector != null);

            IObservable<Unit> reset = null;

            if (signalReset != null) {
                reset = signalReset.Select(_ => Unit.Default);
            }

            if (scheduler == null) {
                scheduler = Scheduler.Immediate;
            }

            return new ReactiveDerivedCollection<T, TNew>(This, selector, filter, orderer, onRemoved, reset, scheduler);
        }

        /// <summary>
        /// Creates a collection whose contents will "follow" another
        /// collection; this method is useful for creating ViewModel collections
        /// that are automatically updated when the respective Model collection
        /// is updated.
        ///
        /// Note that even though this method attaches itself to any 
        /// IEnumerable, it will only detect changes from objects implementing
        /// <see cref="INotifyCollectionChanged"/> (like <see cref="ReactiveList{T}"/>). 
        /// If your source collection doesn't implement this, <paramref name="signalReset"/> 
        /// is the way to signal the derived collection to reorder/refilter itself.
        /// </summary>
        /// <param name="This">
        /// The source <see cref="IEnumerable{T}"/> to track.
        /// </param>
        /// <param name="selector">
        /// A Select function that will be run on each item.
        /// </param>
        /// <param name="filter">
        /// A filter to determine whether to exclude items in the derived collection.
        /// </param>
        /// <param name="orderer">
        /// A comparator method to determine the ordering of the resulting collection.
        /// </param>
        /// <param name="signalReset">
        /// When this Observable is signalled, the derived collection will be manually 
        /// reordered/refiltered.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler used to dispatch change notifications.
        /// </param>
        /// <returns>
        /// A new collection whose items are equivalent to
        /// <c>Collection.Select().Where().OrderBy()</c> and will mirror changes 
        /// in the initial collection.
        /// </returns>
        public static IReactiveDerivedList<TNew> CreateDerivedCollection<T, TNew, TDontCare>(
            this IEnumerable<T> This,
            Func<T, TNew> selector,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null,
            IObservable<TDontCare> signalReset = null,
            IScheduler scheduler = null)
        {
            return This.CreateDerivedCollection(selector, (Action<TNew>)null, filter, orderer, signalReset, scheduler);
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
        /// <param name="This">
        /// The source <see cref="IEnumerable{T}"/> to track.
        /// </param>
        /// <param name="selector">
        /// A Select function that will be run on each item.
        /// </param>
        /// <param name="onRemoved">
        /// An action that is called on each item when it is removed.
        /// </param>
        /// <param name="filter">
        /// A filter to determine whether to exclude items in the derived collection.
        /// </param>
        /// <param name="orderer">
        /// A comparator method to determine the ordering of the resulting collection.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler used to dispatch change notifications.
        /// </param>
        /// <returns>
        /// A new collection whose items are equivalent to
        /// <c>Collection.Select().Where().OrderBy()</c> and will mirror changes 
        /// in the initial collection.
        /// </returns>
        public static IReactiveDerivedList<TNew> CreateDerivedCollection<T, TNew>(
            this IEnumerable<T> This,
            Func<T, TNew> selector,
            Action<TNew> onRemoved,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null,
            IScheduler scheduler = null)
        {
            return This.CreateDerivedCollection(selector, onRemoved, filter, orderer, (IObservable<Unit>)null, scheduler);
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
        /// <param name="This">
        /// The source <see cref="IEnumerable{T}"/> to track.
        /// </param>
        /// <param name="selector">
        /// A Select function that will be run on each item.
        /// </param>
        /// <param name="filter">
        /// A filter to determine whether to exclude items in the derived collection.
        /// </param>
        /// <param name="orderer">
        /// A comparator method to determine the ordering of the resulting collection.
        /// </param>
        /// <param name="scheduler">
        /// An optional scheduler used to dispatch change notifications.
        /// </param>
        /// <returns>
        /// A new collection whose items are equivalent to
        /// <c>Collection.Select().Where().OrderBy()</c> and will mirror changes 
        /// in the initial collection.
        /// </returns>
        public static IReactiveDerivedList<TNew> CreateDerivedCollection<T, TNew>(
            this IEnumerable<T> This,
            Func<T, TNew> selector,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null,
            IScheduler scheduler = null)
        {
            return This.CreateDerivedCollection(selector, default(Action<TNew>), filter, orderer, (IObservable<Unit>)null, scheduler);
        }

    }
}
