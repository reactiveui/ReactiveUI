using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace ReactiveUI
{
    /// <summary>
    /// An observable (INCC) read-only collection wrapper supporting filtering and sorting.
    /// </summary>
    public class ObservableCollectionView<T> : ObservableCollection<T>, IList<T>
    {
        readonly IEnumerable<T> source;
        readonly Func<T, bool> filter;
        readonly IComparer<T> order;
        readonly Func<IObservedChange<T, object>, bool> updateFilter;

        /// <summary>
        /// Creates a read only view that tracks a collection providing filtering and sorting
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">A filter to be applied to the source. Only items matching this filter will appear in this view.</param>
        /// <param name="order">A custom sort function to apply to the view.</param>
        /// <param name="updateFilter">A filter that gives control over which items will be re-filtered and re-sorted when child properties on those items change.</param>
        public ObservableCollectionView(
            IEnumerable<T> source = null,
            Func<T, bool> filter = null,
            IComparer<T> order = null,
            Func<IObservedChange<T, object>, bool> updateFilter = null)
        {
            this.source = source ?? Enumerable.Empty<T>();
            this.filter = filter ?? (_ => true);
            this.order = order;
            this.updateFilter = updateFilter ?? (_ => true);

            fetchItems();

            wireNotificationHandlers();
        }

        void fetchItems()
        {
            var items = source.Where(filter);

            if (order != null)
            {
                items = items.OrderBy(_ => _, order);
            }

            ClearItems();
            items.ToObservable(RxApp.DeferredScheduler).Subscribe(addItem);
        }

        void wireNotificationHandlers()
        {
            var changes = source.ObserveCollectionChanged();

            changes.Where(x => x.Action == NotifyCollectionChangedAction.Reset)
                .ObserveOn(RxApp.DeferredScheduler)
                .Subscribe(_ => fetchItems());

            changes.Where(hasItemsToAdd)
                .SelectMany(x => x.NewItems.Cast<T>())
                .Where(filter)
                .ObserveOn(RxApp.DeferredScheduler)
                .Subscribe(addItem);

            changes.Where(hasItemsToRemove)
                .SelectMany(x => x.OldItems.Cast<T>())
                .ObserveOn(RxApp.DeferredScheduler)
                .Subscribe(removeItem);

            source.ObserveCollectionItemChanged<T>()
                .Where(updateFilter)
                .Select(x => x.Sender)
                .Where(filter)
                .ObserveOn(RxApp.DeferredScheduler)
                .Subscribe(updateItem);
        }


        void updateItem(T item)
        {
            removeItem(item);
            addItem(item);
        }

        void addItem(T item)
        {
            InsertItem(getNewIndexFor(item), item);
        }

        void removeItem(T item)
        {
            var index = IndexOf(item);
            if (index >= 0)
            {
                RemoveItem(index);
            }
        }

        int getNewIndexFor(T item)
        {
            if (order == null)
            {
                return Count;
            }

            var match = Items.BinarySearch(item, order);
            return match < 0 ? Math.Abs(match + 1) : match;
        }

        static bool hasItemsToRemove(NotifyCollectionChangedEventArgs notification)
        {
            return notification.Action == NotifyCollectionChangedAction.Remove
                || notification.Action == NotifyCollectionChangedAction.Replace;
        }

        static bool hasItemsToAdd(NotifyCollectionChangedEventArgs notification)
        {
            return notification.Action == NotifyCollectionChangedAction.Add
                || notification.Action == NotifyCollectionChangedAction.Replace;
        }

        bool ICollection<T>.IsReadOnly {
            get { return true; }
        }

        T IList<T>.this[int index]
        {
            get { return this[index]; }
            set { throw new InvalidOperationException(); }
        }

        void ICollection<T>.Add(T item)
        {
            throw new InvalidOperationException();
        }

        void ICollection<T>.Clear()
        {
            throw new InvalidOperationException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new InvalidOperationException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new InvalidOperationException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new InvalidOperationException();
        }
    }
}