namespace ReactiveUI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reactive.Linq;

    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns an observable sequence of the source collection change notifications.
        /// Returns Observable.Never for collections not implementing INCC.
        /// </summary>
        /// <param name="source">Collection to observe.</param>
        /// <returns>Observable sequence.</returns>
        public static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChanged(
            this IEnumerable source)
        {
            var notifying = source as INotifyCollectionChanged;
            if (notifying != null) {
                return Observable.FromEventPattern<
                    NotifyCollectionChangedEventHandler,
                    NotifyCollectionChangedEventArgs>(
                        ev => notifying.CollectionChanged += ev,
                        ev => notifying.CollectionChanged -= ev)
                    .Select(x => x.EventArgs);
            }

            return Observable.Never<NotifyCollectionChangedEventArgs>();
        }

        /// <summary>
        /// Sorts the specified list in place using the comparer.
        /// </summary>
        /// <param name="list">List to sort.</param>
        /// <param name="comparer">Comparer to use. If null, default comparer is used.</param>
        public static void Sort<T>(
            this IList<T> list,
            IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            var array = new T[list.Count];
            list.CopyTo(array, 0);
            Array.Sort(array, comparer);
            for (var i = 0; i < array.Length; i++) {
                list[i] = array[i];
            }
        }

        /// <summary>
        /// Finds an index of the specified value in the specified collection.
        /// </summary>
        public static int BinarySearch<T>(
            this ICollection<T> collection,
            T value,
            IComparer<T> comparer = null)
        {
            var array = new T[collection.Count];
            collection.CopyTo(array, 0);
            return Array.BinarySearch(array, value, comparer);
        }
    }
}