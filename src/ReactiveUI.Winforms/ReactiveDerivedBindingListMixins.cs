using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace ReactiveUI.Winforms
{
    /// <summary>
    /// IReactiveDerivedList represents a bindinglist whose contents will "follow" another
    /// collection; this method is useful for creating ViewModel collections
    /// that are automatically updated when the respective Model collection is updated.
    /// </summary>
    public interface IReactiveDerivedBindingList<T> : IReactiveDerivedList<T>, IBindingList {}

    class ReactiveDerivedBindingList<TSource, TValue> : 
	    ReactiveDerivedCollection<TSource, TValue>, IReactiveDerivedBindingList<TValue>
    {
        public ReactiveDerivedBindingList(
            IEnumerable<TSource> source,
            Func<TSource, TValue> selector,
            Func<TSource, bool> filter,
            Func<TValue, TValue, int> orderer,
            Action<TValue> removed,
            IObservable<Unit> signalReset)
            : base(source, selector, filter, orderer, removed, signalReset, Scheduler.Immediate) {}

        protected override void raiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.raiseCollectionChanged(e);
            if (this.ListChanged != null) {
                e.AsListChangedEventArgs().ForEach(x => this.ListChanged(this, x));
            }
        }

        const string readonlyExceptionMessage = "Derived collections cannot be modified.";

        public object AddNew()
        {
            throw new NotSupportedException(readonlyExceptionMessage);
        }

        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotSupportedException();
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotSupportedException();
        }

        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotSupportedException();
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotSupportedException();
        }

        public void RemoveSort()
        {
            throw new NotSupportedException();
        }

        public bool AllowNew { get { return false; } }

        public bool AllowEdit { get { return false; } }

        public bool AllowRemove { get { return false; } }

        public bool SupportsChangeNotification { get { return true; } }

        public bool SupportsSearching { get { return false; } }

        public bool SupportsSorting { get { return false; } }

        public bool IsSorted { get { return false; } }

        public PropertyDescriptor SortProperty { get { return null; } }

        public ListSortDirection SortDirection { get { return ListSortDirection.Ascending; } }

        public event ListChangedEventHandler ListChanged;
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
        /// INotifyCollectionChanged (like ReactiveList). If your source
        /// collection doesn't implement this, signalReset is the way to signal
        /// the derived collection to reorder/refilter itself.
        /// </summary>
        /// <param name="selector">A Select function that will be run on each
        /// item.</param>
        /// <param name="onRemoved">An action that is called on each item when
        /// it is removed.</param>
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
        public static IReactiveDerivedBindingList<TNew> CreateDerivedBindingList<T, TNew, TDontCare>(
            this IEnumerable<T> This,
            Func<T, TNew> selector,
            Action<TNew> removed,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null,
            IObservable<TDontCare> signalReset = null)
        {
            Contract.Requires(selector != null);

            IObservable<Unit> reset = null;

            if (signalReset != null) {
                reset = signalReset.Select(_ => Unit.Default);
            }

            return new ReactiveDerivedBindingList<T, TNew>(This, selector, filter, orderer, removed, reset);
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
        public static IReactiveDerivedBindingList<TNew> CreateDerivedBindingList<T, TNew, TDontCare>(
            this IEnumerable<T> This,
            Func<T, TNew> selector,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null,
            IObservable<TDontCare> signalReset = null)
        {
            return This.CreateDerivedBindingList(selector, null, filter, orderer, signalReset);
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
        /// /// <param name="onRemoved">An action that is called on each item when
        /// it is removed.</param>
        /// <param name="filter">A filter to determine whether to exclude items 
        /// in the derived collection.</param>
        /// <param name="orderer">A comparator method to determine the ordering of
        /// the resulting collection.</param>
        /// <returns>A new collection whose items are equivalent to
        /// Collection.Select().Where().OrderBy() and will mirror changes 
        /// in the initial collection.</returns>
        public static IReactiveDerivedBindingList<TNew> CreateDerivedBindingList<T, TNew>(
            this IEnumerable<T> This,
            Func<T, TNew> selector,
            Action<TNew> removed,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null)
        {
            return This.CreateDerivedBindingList(selector, removed, filter, orderer, (IObservable<Unit>)null);
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
        public static IReactiveDerivedBindingList<TNew> CreateDerivedBindingList<T, TNew>(
            this IEnumerable<T> This,
            Func<T, TNew> selector,
            Func<T, bool> filter = null,
            Func<TNew, TNew, int> orderer = null)
        {
            return This.CreateDerivedBindingList(selector, null, filter, orderer, (IObservable<Unit>)null);
        }
    }
}
