using System;
using System.Collections.Specialized;
using System.Reactive.Linq;

namespace Xamarin.Forms
{
    public class TableSectionBaseEvents
    {
        TableSectionBase<TableSection> This;

        public TableSectionBaseEvents(TableSectionBase<TableSection> This)
        {
            this.This = This;
        }

        public IObservable<NotifyCollectionChangedEventArgs> SectionCollectionChanged {
            get { return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(x => This.CollectionChanged += x, x => This.CollectionChanged -= x).Select(x => x.EventArgs); }
        }
    }
}

