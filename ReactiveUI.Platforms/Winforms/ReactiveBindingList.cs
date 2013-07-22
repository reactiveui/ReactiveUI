namespace ReactiveUI.Winforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;

    using ReactiveUI;

    public class ReactiveBindingList<T> : ReactiveList<T>,  IList<T>, ICollection<T>, IEnumerable<T>, ICollection, IEnumerable, IList, IBindingList, ICancelAddNew, IRaiseItemChangedEvents
    {
        public ReactiveBindingList()
            : this(null)
        {}

        #region Implementation of ICancelAddNew

        public void CancelNew(int itemIndex)
        {
            //throw new NotImplementedException();
        }

        public void EndNew(int itemIndex)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IRaiseItemChangedEvents

        public bool RaisesItemChangedEvents {get
        {
            return base.ChangeTrackingEnabled;
        }}

        #endregion

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
            this.transformAndRaise(e);
        }
       
        /// <summary>
        /// Transforms NotifyCollectionChangedEventArgs into 1 or more ListChangedEventsArgs
        /// and raises them if there are any attached handlers
        /// </summary>
        /// <param name="ea"></param>
        void transformAndRaise(NotifyCollectionChangedEventArgs ea)
        {
            if (this.ListChanged == null) return;

            var events = new List<ListChangedEventArgs>();

            switch (ea.Action){
            case NotifyCollectionChangedAction.Reset:
                events.Add(new ListChangedEventArgs(ListChangedType.Reset, -1));
                break;
            case NotifyCollectionChangedAction.Replace:
                events.Add(new ListChangedEventArgs(ListChangedType.ItemChanged, ea.NewStartingIndex));
                break;
            case NotifyCollectionChangedAction.Remove:
                events.AddRange(
                    Enumerable.Range(ea.OldStartingIndex, ea.OldItems.Count)
                    .Select(index => new ListChangedEventArgs(ListChangedType.ItemDeleted, index)));
                break;
            case NotifyCollectionChangedAction.Add:
                events.AddRange(
                    Enumerable.Range(ea.NewStartingIndex, ea.NewItems.Count)
                    .Select(index => new ListChangedEventArgs(ListChangedType.ItemAdded, index)));
                break;
            case NotifyCollectionChangedAction.Move:
                //this one is actually not supported by the default BindingList<T> implementation
                //maybe we should do a reset instead?
                events.Add(
                    new ListChangedEventArgs(ListChangedType.ItemMoved, ea.NewStartingIndex, ea.OldStartingIndex));
                break;
            }

            events.ForEach(x=>this.ListChanged(this, x));
        }


        #region Implementation of IBindingList

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

        public bool AllowNew {
            get { return true; }
        }

        public bool AllowEdit {
            get { return true; }
        }

        public bool AllowRemove {
            get { return true; }
        }

        public bool SupportsChangeNotification {
            get { return true; }
        }

        public bool SupportsSearching {
            get { return false; }
        }

        public bool SupportsSorting {
            get { return false; }
        }

        public bool IsSorted {
            get{ return false; }
        }

        public PropertyDescriptor SortProperty {
            get{ return null; }
        }

        public ListSortDirection SortDirection {
            get { return ListSortDirection.Ascending; }
        }

        public event ListChangedEventHandler ListChanged;

        #endregion
    }
}
