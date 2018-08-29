// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

#pragma warning disable SA1600 // Elements should be documented -- not used for legacy
#pragma warning disable SA1201 // Ordering -- not used for legacy
#pragma warning disable SA1202 // Ordering -- not used for legacy
#pragma warning disable SA1124 // Do not use regions -- not used for legacy
#pragma warning disable RCS1165 // Unconstrained null check -- not used for legacy
#pragma warning disable CA1001 // Undisposed type -- not used for legacy
#pragma warning disable CA1822 // Mark member static -- not used for legacy
#pragma warning disable SA1100 // Do not use prefix -- not used for legacy
#pragma warning disable SA1407 // Expression should declare precedence -- not used for legacy
#pragma warning disable SA1402 // File many contain only single type -- not used for legacy

namespace ReactiveUI.Legacy
{
    /// <summary>
    /// This class represents a change-notifying Collection which is derived from
    /// a source collection, via CreateDerivedCollection or via another method.
    /// It is read-only, and any attempts to change items in the collection will
    /// fail.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    internal class ReactiveDerivedCollection<TSource, TValue> : ReactiveDerivedCollection<TValue>, IDisposable
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TValue> _selector;
        private readonly Func<TSource, bool> _filter;
        private readonly Func<TValue, TValue, int> _orderer;
        private readonly Action<TValue> _onRemoved;
        private readonly IObservable<Unit> _signalReset;
        private readonly IScheduler _scheduler;

        // This list maps indices in this collection to their corresponding indices in the source collection.
        private readonly List<int> _indexToSourceIndexMap;
        private readonly List<TSource> _sourceCopy;
        private CompositeDisposable _inner;

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
            {
                filter = x => true;
            }

            _source = source;
            _selector = selector;
            _filter = filter;
            _orderer = orderer;
            _onRemoved = onRemoved ?? (_ => { });
            _signalReset = signalReset;
            _scheduler = scheduler;

            _inner = new CompositeDisposable();
            _indexToSourceIndexMap = new List<int>();
            _sourceCopy = new List<TSource>();

            _inner.Add(Disposable.Create(() =>
            {
                foreach (var item in this)
                {
                    _onRemoved(item);
                }
            }));

            AddAllItemsFromSourceCollection();
            WireUpChangeNotifications();
        }

        private static readonly Dictionary<Type, bool> hasWarned = new Dictionary<Type, bool>();

        private void WireUpChangeNotifications()
        {
            var incc = _source as INotifyCollectionChanged;

            if (incc == null)
            {
                var type = _source.GetType();

                lock (hasWarned)
                {
                    if (!hasWarned.ContainsKey(type))
                    {
                        this.Log().Warn(
                            "{0} doesn't implement INotifyCollectionChanged, derived collection will only update " +
                            "when the Reset() method is invoked manually or the reset observable is signalled.",
                            type.FullName);
                        hasWarned.Add(type, true);
                    }
                }
            }
            else
            {
                var irncc = _source as IReactiveNotifyCollectionChanged<TSource>;
                var eventObs = irncc != null
                    ? irncc.Changed
                    : Observable
                        .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                            x => incc.CollectionChanged += x,
                            x => incc.CollectionChanged -= x)
                        .Select(x => x.EventArgs);
                _inner.Add(eventObs.ObserveOn(_scheduler).Subscribe(OnSourceCollectionChanged));
            }

            var irc = _source as IReactiveCollection<TSource>;

            if (irc != null)
            {
                _inner.Add(irc.ItemChanged.Select(x => x.Sender).ObserveOn(_scheduler).Subscribe(OnItemChanged));
            }

            if (_signalReset != null)
            {
                _inner.Add(_signalReset.ObserveOn(_scheduler).Subscribe(x => Reset()));
            }
        }

        private void OnItemChanged(TSource changedItem)
        {
            // If you've implemented INotifyPropertyChanged on a struct then you're doing it wrong(TM) and change
            // tracking won't work in derived collections (change tracking for value types makes no sense any way)
            // NB: It's possible the sender exists in multiple places in the source collection.
            var sourceIndices = IndexOfAll(_sourceCopy, changedItem, ReferenceEqualityComparer<TSource>.Default);

            var shouldBeIncluded = _filter(changedItem);

            foreach (int sourceIndex in sourceIndices)
            {
                int currentDestinationIndex = GetIndexFromSourceIndex(sourceIndex);
                bool isIncluded = currentDestinationIndex >= 0;

                if (isIncluded && !shouldBeIncluded)
                {
                    InternalRemoveAt(currentDestinationIndex);
                }
                else if (!isIncluded && shouldBeIncluded)
                {
                    InternalInsertAndMap(sourceIndex, _selector(changedItem));
                }
                else if (isIncluded && shouldBeIncluded)
                {
                    // The item is already included and it should stay there but it's possible that the change that
                    // caused this event affects the ordering. This gets a little tricky so let's be verbose.
                    TValue newItem = _selector(changedItem);

                    if (_orderer == null)
                    {
                        // We don't have an orderer so we're currently using the source collection index for sorting
                        // meaning that no item change will affect ordering. Look at our current item and see if it's
                        // the exact (reference-wise) same object. If it is then we're done, if it's not (for example
                        // if it's an integer) we'll issue a replace event so that subscribers get the new value.
                        if (!ReferenceEquals(newItem, this[currentDestinationIndex]))
                        {
                            InternalReplace(currentDestinationIndex, newItem);
                        }
                    }
                    else
                    {
                        // Don't be tempted to just use the orderer to compare the new item with the previous since
                        // they'll almost certainly be equal (for reference types). We need to test whether or not the
                        // new item can stay in the same position that the current item is in without comparing them.
                        if (CanItemStayAtPosition(newItem, currentDestinationIndex))
                        {
                            // The new item should be in the same position as the current but there's no need to signal
                            // that in case they are the same object.
                            if (!ReferenceEquals(newItem, this[currentDestinationIndex]))
                            {
                                InternalReplace(currentDestinationIndex, newItem);
                            }
                        }
                        else
                        {
                            // The change is forcing us to reorder. We'll use a move operation if the item hasn't
                            // changed (ie it's the same object) and we'll implement it as a remove and add if the
                            // object has changed (ie the selector is not an identity function).
                            if (ReferenceEquals(newItem, this[currentDestinationIndex]))
                            {
                                int newDestinationIndex = NewPositionForExistingItem(
                                    sourceIndex, currentDestinationIndex, newItem);

                                Debug.Assert(newDestinationIndex != currentDestinationIndex, "This can't be, canItemStayAtPosition said it this couldn't happen");

                                _indexToSourceIndexMap.RemoveAt(currentDestinationIndex);
                                _indexToSourceIndexMap.Insert(newDestinationIndex, sourceIndex);

                                InternalMove(currentDestinationIndex, newDestinationIndex);
                            }
                            else
                            {
                                InternalRemoveAt(currentDestinationIndex);
                                InternalInsertAndMap(sourceIndex, newItem);
                            }
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
        /// <param name="item">The item.</param>
        /// <param name="currentIndex">The current index.</param>
        private bool CanItemStayAtPosition(TValue item, int currentIndex)
        {
            bool hasPrecedingItem = currentIndex > 0;

            if (hasPrecedingItem)
            {
                bool isGreaterThanOrEqualToPrecedingItem = _orderer(item, this[currentIndex - 1]) >= 0;
                if (!isGreaterThanOrEqualToPrecedingItem)
                {
                    return false;
                }
            }

            bool hasSucceedingItem = currentIndex < Count - 1;

            if (hasSucceedingItem)
            {
                bool isLessThanOrEqualToSucceedingItem = _orderer(item, this[currentIndex + 1]) <= 0;
                if (!isLessThanOrEqualToSucceedingItem)
                {
                    return false;
                }
            }

            return true;
        }

        private void InternalReplace(int destinationIndex, TValue newItem)
        {
            var item = this[destinationIndex];
            SetItem(destinationIndex, newItem);
            _onRemoved(item);
        }

        /// <summary>
        /// Gets the index of the dervived item based on it's originating element index in the source collection.
        /// </summary>
        /// <param name="sourceIndex">The source index.</param>
        private int GetIndexFromSourceIndex(int sourceIndex)
        {
            return _indexToSourceIndexMap.IndexOf(sourceIndex);
        }

        /// <summary>
        /// Returns one or more positions in the source collection where the given item is found based on the
        /// provided equality comparer.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="item">The item.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        private List<int> IndexOfAll(
            IEnumerable<TSource> source,
            TSource item,
            IEqualityComparer<TSource> equalityComparer)
        {
            var indices = new List<int>(1);
            int sourceIndex = 0;
            foreach (var x in source)
            {
                if (equalityComparer.Equals(x, item))
                {
                    indices.Add(sourceIndex);
                }

                sourceIndex++;
            }

            return indices;
        }

        private void OnSourceCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                Reset();
                return;
            }

            if (args.Action == NotifyCollectionChangedAction.Move)
            {
                Debug.Assert(args.OldItems.Count == args.NewItems.Count);

                if (args.OldItems.Count > 1 || args.NewItems.Count > 1)
                {
                    throw new NotSupportedException("Derived collections doesn't support multi-item moves");
                }

                // Yeah apparently this can happen. ObservableCollection triggers this notification on Move(0,0)
                if (args.OldStartingIndex == args.NewStartingIndex)
                {
                    return;
                }

                int oldSourceIndex = args.OldStartingIndex;
                int newSourceIndex = args.NewStartingIndex;

                _sourceCopy.RemoveAt(oldSourceIndex);
                _sourceCopy.Insert(newSourceIndex, (TSource)args.NewItems[0]);

                int currentDestinationIndex = GetIndexFromSourceIndex(oldSourceIndex);

                MoveSourceIndexInMap(oldSourceIndex, newSourceIndex);

                if (currentDestinationIndex == -1)
                {
                    return;
                }

                TValue value = base[currentDestinationIndex];

                if (_orderer == null)
                {
                    // We mirror the order of the source collection so we'll perform the same move operation
                    // as the source. As is the case with when we have an orderer we don't test whether or not
                    // the item should be included or not here. If it has been included at some point it'll
                    // stay included until onItemChanged picks up a change which filters it.
                    int newDestinationIndex = NewPositionForExistingItem(
                        _indexToSourceIndexMap, newSourceIndex, currentDestinationIndex);

                    if (newDestinationIndex != currentDestinationIndex)
                    {
                        _indexToSourceIndexMap.RemoveAt(currentDestinationIndex);
                        _indexToSourceIndexMap.Insert(newDestinationIndex, newSourceIndex);

                        InternalMove(currentDestinationIndex, newDestinationIndex);
                    }
                    else
                    {
                        _indexToSourceIndexMap[currentDestinationIndex] = newSourceIndex;
                    }
                }
                else
                {
                    // TODO: Conceptually I feel like we shouldn't concern ourselves with ordering when we
                    // receive a Move notification. If it affects ordering it should be picked up by the
                    // onItemChange and resorted there instead.
                    _indexToSourceIndexMap[currentDestinationIndex] = newSourceIndex;
                }

                return;
            }

            if (args.OldItems != null)
            {
                _sourceCopy.RemoveRange(args.OldStartingIndex, args.OldItems.Count);

                for (int i = 0; i < args.OldItems.Count; i++)
                {
                    int destinationIndex = GetIndexFromSourceIndex(args.OldStartingIndex + i);
                    if (destinationIndex != -1)
                    {
                        InternalRemoveAt(destinationIndex);
                    }
                }

                int removedCount = args.OldItems.Count;
                ShiftIndicesAtOrOverThreshold(args.OldStartingIndex + removedCount, -removedCount);
            }

            if (args.NewItems != null)
            {
                ShiftIndicesAtOrOverThreshold(args.NewStartingIndex, args.NewItems.Count);

                for (int i = 0; i < args.NewItems.Count; i++)
                {
                    var sourceItem = (TSource)args.NewItems[i];
                    _sourceCopy.Insert(args.NewStartingIndex + i, sourceItem);

                    if (!_filter(sourceItem))
                    {
                        continue;
                    }

                    var destinationItem = _selector(sourceItem);
                    InternalInsertAndMap(args.NewStartingIndex + i, destinationItem);
                }
            }
        }

        /// <summary>
        /// Increases (or decreases depending on move direction) all source indices between the source and destination
        /// move indices.
        /// </summary>
        /// <param name="oldSourceIndex">The old source index.</param>
        /// <param name="newSourceIndex">The new source index.</param>
        private void MoveSourceIndexInMap(int oldSourceIndex, int newSourceIndex)
        {
            if (newSourceIndex > oldSourceIndex)
            {
                // Item is moving towards the end of the list, everything between its current position and its
                // new position needs to be shifted down one index
                ShiftSourceIndicesInRange(oldSourceIndex + 1, newSourceIndex + 1, -1);
            }
            else
            {
                // Item is moving towards the front of the list, everything between its current position and its
                // new position needs to be shifted up one index
                ShiftSourceIndicesInRange(newSourceIndex, oldSourceIndex, 1);
            }
        }

        /// <summary>
        /// Increases (or decreases) all source indices equal to or higher than the threshold. Represents an
        /// insert or remove of one or more items in the source list thus causing all subsequent items to shift
        /// up or down.
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        /// <param name="value">The value.</param>
        private void ShiftIndicesAtOrOverThreshold(int threshold, int value)
        {
            for (int i = 0; i < _indexToSourceIndexMap.Count; i++)
            {
                if (_indexToSourceIndexMap[i] >= threshold)
                {
                    _indexToSourceIndexMap[i] += value;
                }
            }
        }

        /// <summary>
        /// Increases (or decreases) all source indices within the range (lower inclusive, upper exclusive).
        /// </summary>
        /// <param name="rangeStart">The start of the range.</param>
        /// <param name="rangeStop">The end of the range.</param>
        /// <param name="value">The value.</param>
        private void ShiftSourceIndicesInRange(int rangeStart, int rangeStop, int value)
        {
            for (int i = 0; i < _indexToSourceIndexMap.Count; i++)
            {
                int sourceIndex = _indexToSourceIndexMap[i];
                if (sourceIndex >= rangeStart && sourceIndex < rangeStop)
                {
                    _indexToSourceIndexMap[i] += value;
                }
            }
        }

        public override void Reset()
        {
            using (SuppressChangeNotifications())
            {
                InternalClear();
                AddAllItemsFromSourceCollection();
            }
        }

        private void AddAllItemsFromSourceCollection()
        {
            Debug.Assert(_sourceCopy.Count == 0, "Expceted source copy to be empty");

            int sourceIndex = 0;

            foreach (TSource sourceItem in _source)
            {
                _sourceCopy.Add(sourceItem);

                if (_filter(sourceItem))
                {
                    var destinationItem = _selector(sourceItem);
                    InternalInsertAndMap(sourceIndex, destinationItem);
                }

                sourceIndex++;
            }
        }

        protected override void InternalClear()
        {
            _indexToSourceIndexMap.Clear();
            _sourceCopy.Clear();
            var items = this.ToArray();

            base.InternalClear();

            foreach (var item in items)
            {
                _onRemoved(item);
            }
        }

        private void InternalInsertAndMap(int sourceIndex, TValue value)
        {
            int destinationIndex = PositionForNewItem(sourceIndex, value);

            _indexToSourceIndexMap.Insert(destinationIndex, sourceIndex);
            InternalInsert(destinationIndex, value);
        }

        protected override void InternalRemoveAt(int destinationIndex)
        {
            _indexToSourceIndexMap.RemoveAt(destinationIndex);
            var item = this[destinationIndex];
            base.InternalRemoveAt(destinationIndex);
            _onRemoved(item);
        }

        /// <summary>
        /// Internal equality comparer used for looking up the source object of a property change notification in
        /// the source list.
        /// </summary>
        /// <typeparam name="T">The type for comparison.</typeparam>
        private class ReferenceEqualityComparer<T> : IEqualityComparer<T>
        {
            public static readonly ReferenceEqualityComparer<T> Default = new ReferenceEqualityComparer<T>();

            public bool Equals(T x, T y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(T obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }

        private int PositionForNewItem(int sourceIndex, TValue value)
        {
            // If we haven't got an orderer we'll simply match our items to that of the source collection.
            return _orderer == null
                ? PositionForNewItem(_indexToSourceIndexMap, sourceIndex, Comparer<int>.Default.Compare)
                : PositionForNewItem(this, 0, Count, value, _orderer);
        }

        internal static int PositionForNewItem<T>(IList<T> list, T item, Func<T, T, int> orderer)
        {
            return PositionForNewItem(list, 0, list.Count, item, orderer);
        }

        internal static int PositionForNewItem<T>(
            IList<T> list, int index, int count, T item, Func<T, T, int> orderer)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(count >= 0);
            Debug.Assert(list.Count - index >= count);

            if (count == 0)
            {
                return index;
            }

            if (count == 1)
            {
                return orderer(list[index], item) >= 0 ? index : index + 1;
            }

            if (orderer(list[index], item) >= 1)
            {
                return index;
            }

            int low = index, hi = index + count - 1;
            int mid, cmp;

            while (low <= hi)
            {
                mid = low + (hi - low) / 2;
                cmp = orderer(list[mid], item);

                if (cmp == 0)
                {
                    return mid;
                }

                if (cmp < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }

            return low;
        }

        /// <summary>
        /// Calculates a new destination for an updated item that's already in the list.
        /// </summary>
        /// <param name="sourceIndex">The source index.</param>
        /// <param name="currentIndex">The current index.</param>
        /// <param name="item">The item.</param>
        private int NewPositionForExistingItem(int sourceIndex, int currentIndex, TValue item)
        {
            // If we haven't got an orderer we'll simply match our items to that of the source collection.
            return _orderer == null
                ? NewPositionForExistingItem(_indexToSourceIndexMap, sourceIndex, currentIndex)
                : NewPositionForExistingItem(this, item, currentIndex, _orderer);
        }

        /// <summary>
        /// Calculates a new destination for an updated item that's already in the list.
        /// </summary>
        /// <typeparam name="T">The list type.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="item">The item.</param>
        /// <param name="currentIndex">The current index.</param>
        /// <param name="orderer">The order.</param>
        internal static int NewPositionForExistingItem<T>(
            IList<T> list, T item, int currentIndex, Func<T, T, int> orderer = null)
        {
            // Since the item changed is most likely a value type we must refrain from ever comparing it to itself.
            // We do this by figuring out how the updated item compares to its neighbors. By knowing if it's
            // less than or greater than either one of its neighbors we can limit the search range to a range exlusive
            // of the current index.
            Debug.Assert(list.Count > 0);

            if (list.Count == 1)
            {
                return 0;
            }

            int precedingIndex = currentIndex - 1;
            int succeedingIndex = currentIndex + 1;

            // The item on the preceding or succeeding index relative to currentIndex.
            T comparand = list[precedingIndex >= 0 ? precedingIndex : succeedingIndex];

            if (orderer == null)
            {
                orderer = Comparer<T>.Default.Compare;
            }

            // Compare that to the (potentially) new value.
            int cmp = orderer(item, comparand);

            int min = 0;
            int max = list.Count;

            if (cmp == 0)
            {
                // The new value is equal to the preceding or succeeding item, it may stay at the current position
                return currentIndex;
            }
            else if (cmp > 0)
            {
                // The new value is greater than the preceding or succeeding item, limit the search to indices after
                // the succeeding item.
                min = succeedingIndex;
            }
            else
            {
                // The new value is less than the preceding or succeeding item, limit the search to indices before
                // the preceding item.
                max = precedingIndex;
            }

            // Bail if the search range is invalid.
            if (min == list.Count || max < 0)
            {
                return currentIndex;
            }

            int ix = PositionForNewItem(list, min, max - min, item, orderer);

            // If the item moves 'forward' in the collection we have to account for the index where
            // the item currently resides getting removed first.
            return ix >= currentIndex ? ix - 1 : ix;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disp = Interlocked.Exchange(ref _inner, null);
                if (disp == null)
                {
                    return;
                }

                disp.Dispose();
            }
        }
    }

    /// <summary>
    /// This class represents a change-notifying Collection which is derived from
    /// a source collection, via CreateDerivedCollection or via another method.
    /// It is read-only, and any attempts to change items in the collection will
    /// fail.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    internal abstract class ReactiveDerivedCollection<TValue> : ReactiveList<TValue>, IReactiveDerivedList<TValue>, IDisposable
    {
        private const string ReadonlyExceptionMessage = "Derived collections cannot be modified.";

        public override bool IsReadOnly => true;

        public override TValue this[int index]
        {
            get => base[index];
            set => throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        public override void Add(TValue item)
        {
            throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        protected virtual void InternalAdd(TValue item)
        {
            base.Add(item);
        }

        public override void AddRange(IEnumerable<TValue> collection)
        {
            throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        protected virtual void InternalAddRange(IEnumerable<TValue> collection)
        {
            base.AddRange(collection);
        }

        public override void Clear()
        {
            throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        protected virtual void InternalClear()
        {
            base.Clear();
        }

        public override void Insert(int index, TValue item)
        {
            throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        protected virtual void InternalInsert(int index, TValue item)
        {
            base.Insert(index, item);
        }

        public override void InsertRange(int index, IEnumerable<TValue> collection)
        {
            throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        protected virtual void InternalInsertRange(int index, IEnumerable<TValue> collection)
        {
            base.InsertRange(index, collection);
        }

        public override bool Remove(TValue item)
        {
            throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        protected virtual bool InternalRemove(TValue item)
        {
            return base.Remove(item);
        }

        public override void RemoveAll(IEnumerable<TValue> items)
        {
            throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        protected virtual void InternalRemoveAll(IEnumerable<TValue> items)
        {
            base.RemoveAll(items);
        }

        public override void RemoveAt(int index)
        {
            throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        protected virtual void InternalRemoveAt(int index)
        {
            base.RemoveAt(index);
        }

        public override void Move(int oldIndex, int newIndex)
        {
            throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        protected virtual void InternalMove(int oldIndex, int newIndex)
        {
            base.Move(oldIndex, newIndex);
        }

        public override void RemoveRange(int index, int count)
        {
            throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        protected virtual void InternalRemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
        }

        public override void Sort(Comparison<TValue> comparison)
        {
            throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        protected virtual void InternalSort(Comparison<TValue> comparison)
        {
            base.Sort(comparison);
        }

        public override void Sort(IComparer<TValue> comparer = null)
        {
            throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        protected virtual void InternalSort(IComparer<TValue> comparer = null)
        {
            base.Sort(comparer);
        }

        public override void Sort(int index, int count, IComparer<TValue> comparer)
        {
            throw new InvalidOperationException(ReadonlyExceptionMessage);
        }

        protected virtual void InternalSort(int index, int count, IComparer<TValue> comparer)
        {
            base.Sort(index, count, comparer);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
