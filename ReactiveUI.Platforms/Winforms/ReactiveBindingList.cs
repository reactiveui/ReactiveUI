using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace ReactiveUI.Winforms
{
    public class ReactiveBindingList<T> : ReactiveList<T>,
        IList<T>, ICollection<T>, IEnumerable<T>,
        ICollection, IEnumerable, IList, IBindingList,
        ICancelAddNew, IRaiseItemChangedEvents
    {
        public ReactiveBindingList() : this(null) {}

        public void CancelNew(int itemIndex)
        {
            //throw new NotImplementedException();
        }

        public void EndNew(int itemIndex)
        {
            //throw new NotImplementedException();
        }

        public bool RaisesItemChangedEvents { get { return base.ChangeTrackingEnabled; } }

        /// <summary>
        /// ReactiveBindingList constructor
        /// </summary>
        /// <param name="items"></param>
        public ReactiveBindingList(IEnumerable<T> items)
            : base(items)
        {}

        protected override void raiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.raiseCollectionChanged(e);
            if (this.ListChanged != null) {
                e.AsListChangedEventArgs().ForEach(x => this.ListChanged(this, x));
            }
        }

        public object AddNew()
        {
            return Activator.CreateInstance<T>();
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

        public bool AllowNew { get { return true; } }

        public bool AllowEdit { get { return true; } }

        public bool AllowRemove { get { return true; } }

        public bool SupportsChangeNotification { get { return true; } }

        public bool SupportsSearching { get { return false; } }

        public bool SupportsSorting { get { return false; } }

        public bool IsSorted { get { return false; } }

        public PropertyDescriptor SortProperty { get { return null; } }

        public ListSortDirection SortDirection { get { return ListSortDirection.Ascending; } }

        public event ListChangedEventHandler ListChanged;
    }
}
