// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI.Legacy;

namespace ReactiveUI.Winforms.Legacy
{
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
#pragma warning disable SA1600 // Elements should be documented
    public class ReactiveBindingList<T> : ReactiveList<T>,
        IList<T>, ICollection<T>, IEnumerable<T>,
        ICollection, IEnumerable, IList, IBindingList,
        ICancelAddNew, IRaiseItemChangedEvents
    {
        public ReactiveBindingList()
            : this(null)
        {
        }

        /// <inheritdoc/>
        public void CancelNew(int itemIndex)
        {
            // throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void EndNew(int itemIndex)
        {
            // throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool RaisesItemChangedEvents => ChangeTrackingEnabled;

        /// <summary>
        /// ReactiveBindingList constructor.
        /// </summary>
        /// <param name="items">The items.</param>
        public ReactiveBindingList(IEnumerable<T> items)
            : base(items)
        {
        }

        /// <inheritdoc/>
        protected override void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.RaiseCollectionChanged(e);
            if (ListChanged != null)
            {
                e.AsListChangedEventArgs().ForEach(x => ListChanged(this, x));
            }
        }

        /// <inheritdoc/>
        public object AddNew()
        {
            return Activator.CreateInstance<T>();
        }

        /// <inheritdoc/>
        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void RemoveSort()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public bool AllowNew => true;

        /// <inheritdoc/>
        public bool AllowEdit => true;

        /// <inheritdoc/>
        public bool AllowRemove => true;

        /// <inheritdoc/>
        public bool SupportsChangeNotification => true;

        /// <inheritdoc/>
        public bool SupportsSearching => false;

        /// <inheritdoc/>
        public bool SupportsSorting => false;

        /// <inheritdoc/>
        public bool IsSorted => false;

        /// <inheritdoc/>
        public PropertyDescriptor SortProperty => null;

        /// <inheritdoc/>
        public ListSortDirection SortDirection => ListSortDirection.Ascending;

        /// <inheritdoc/>
        public event ListChangedEventHandler ListChanged;
    }
#pragma warning restore SA1600 // Elements should be documented
}
