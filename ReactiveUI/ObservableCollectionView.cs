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
        
        public ObservableCollectionView(
            IEnumerable<T> source = null,
            Func<T, bool> filter = null,
            IComparer<T> order = null)
        {
            this.source = source ?? Enumerable.Empty<T>();
            this.filter = filter ?? (_ => true);
            this.order = order;

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