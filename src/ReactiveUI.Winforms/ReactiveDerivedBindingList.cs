// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

using ReactiveUI.Legacy;

namespace ReactiveUI.Winforms.Legacy
{
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    internal class ReactiveDerivedBindingList<TSource, TValue> : ReactiveDerivedCollection<TSource, TValue>, IReactiveDerivedBindingList<TValue>
    {
        private const string ReadonlyExceptionMessage = "Derived collections cannot be modified.";

        public ReactiveDerivedBindingList(
            IEnumerable<TSource> source,
            Func<TSource, TValue> selector,
            Func<TSource, bool> filter,
            Func<TValue, TValue, int> orderer,
            Action<TValue> removed,
            IObservable<Unit> signalReset)
            : base(source, selector, filter, orderer, removed, signalReset, Scheduler.Immediate)
        {
        }

        public event ListChangedEventHandler ListChanged;

        public bool AllowNew => false;

        public bool AllowEdit => false;

        public bool AllowRemove => false;

        public bool SupportsChangeNotification => true;

        public bool SupportsSearching => false;

        public bool SupportsSorting => false;

        public bool IsSorted => false;

        public PropertyDescriptor SortProperty => null;

        public ListSortDirection SortDirection => ListSortDirection.Ascending;

        public object AddNew()
        {
            throw new NotSupportedException(ReadonlyExceptionMessage);
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

        protected override void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.RaiseCollectionChanged(e);
            if (ListChanged != null)
            {
                e.AsListChangedEventArgs().ForEach(x => ListChanged(this, x));
            }
        }
    }
}
