namespace ReactiveUI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;

    /// <summary>
    /// An observable (INCC) read-only collection wrapper supporting filtering and sorting.
    /// </summary>
    public class ObservableCollectionView<T>
        : ObservableCollection<T>, IList<T>
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

            FetchItems();

            WireNotificationHandlers();
        }

        void FetchItems()
        {
            var items = source.Where(filter);

            if (order != null)
            {
                items = items.OrderBy(_ => _, order);
            }

            ClearItems();
            items.ForEach(AddItem);
        }

        void WireNotificationHandlers()
        {
            var changes = source.ObserveCollectionChanged();

            changes.Where(x => x.Action == NotifyCollectionChangedAction.Reset)
                .Subscribe(_ => FetchItems());

            changes.Where(HasItemsToAdd)
                .SelectMany(x => x.NewItems.Cast<T>())
                .Where(filter)
                .Subscribe(AddItem);

            changes.Where(HasItemsToRemove)
                .SelectMany(x => x.OldItems.Cast<T>())
                .Subscribe(RemoveItem);
        }

        void AddItem(T item)
        {
            InsertItem(GetNewIndexFor(item), item);
        }

        void RemoveItem(T item)
        {
            var index = IndexOf(item);
            if (index >= 0)
            {
                RemoveItem(index);
            }
        }

        int GetNewIndexFor(T item)
        {
            if (order == null)
            {
                return Count;
            }

            var match = Items.BinarySearch(item, order);
            return match < 0 ? Math.Abs(match + 1) : match;
        }

        static bool HasItemsToRemove(NotifyCollectionChangedEventArgs notification)
        {
            return notification.Action == NotifyCollectionChangedAction.Remove
                || notification.Action == NotifyCollectionChangedAction.Replace;
        }

        static bool HasItemsToAdd(NotifyCollectionChangedEventArgs notification)
        {
            return notification.Action == NotifyCollectionChangedAction.Add
                || notification.Action == NotifyCollectionChangedAction.Replace;
        }

        bool ICollection<T>.IsReadOnly
        {
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