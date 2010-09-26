using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Diagnostics.Contracts;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace ReactiveXaml
{
    public class ReactiveCollection<T> : ObservableCollection<T>, IReactiveCollection<T>, INotifyPropertyChanged, IDisposable
    {
        public ReactiveCollection() { setupRx(); }
        public ReactiveCollection(IEnumerable<T> List) { setupRx(List); }

        [OnDeserialized]
        void setupRx(StreamingContext _) { setupRx(); }

        void setupRx(IEnumerable<T> List = null)
        {
            if (List != null) {
                foreach(var v in List) { this.Add(v); }
            }

            var coll_changed = Observable.FromEvent<NotifyCollectionChangedEventArgs>(this, "CollectionChanged");

            ItemsAdded = coll_changed
                .Where(x => x.EventArgs.Action == NotifyCollectionChangedAction.Add || x.EventArgs.Action == NotifyCollectionChangedAction.Replace)
                .SelectMany(x => (x.EventArgs.NewItems != null ? x.EventArgs.NewItems.OfType<T>() : Enumerable.Empty<T>()).ToObservable());

            ItemsRemoved = coll_changed
                .Where(x => x.EventArgs.Action == NotifyCollectionChangedAction.Remove || x.EventArgs.Action == NotifyCollectionChangedAction.Replace || x.EventArgs.Action == NotifyCollectionChangedAction.Reset)
                .SelectMany(x => (x.EventArgs.OldItems != null ? x.EventArgs.OldItems.OfType<T>() : Enumerable.Empty<T>()).ToObservable());

            CollectionCountChanged = coll_changed
                .Select(x => this.Count)
                .DistinctUntilChanged();

            _ItemPropertyChanged = new Subject<ObservedChange<T,object>>();

            ItemsAdded.Subscribe(x => {
                if (propertyChangeWatchers == null)
                    return;
                var item = x as IReactiveNotifyPropertyChanged;
                if (item != null) {
                    propertyChangeWatchers.Add(x, item.Subscribe(change => 
                        _ItemPropertyChanged.OnNext(new ObservedChange<T,object>() { Sender = x, PropertyName = change.PropertyName })));
                    return;
                }
            });

            ItemsRemoved.Subscribe(x => {
                if (propertyChangeWatchers == null)
                    return;
                if (propertyChangeWatchers.ContainsKey(x)) {
                    propertyChangeWatchers[x].Dispose();
                    propertyChangeWatchers.Remove(x);
                }
            });
        }

        [IgnoreDataMember]
        IObservable<T> _ItemsAdded;
        public IObservable<T> ItemsAdded {
            get { return _ItemsAdded; }
            protected set { _ItemsAdded = value; }
        }

        [IgnoreDataMember]
        IObservable<T> _ItemsRemoved;
        public IObservable<T> ItemsRemoved {
            get { return _ItemsRemoved; }
            set { _ItemsRemoved = value; }
        }

        [IgnoreDataMember]
        IObservable<int> _CollectionCountChanged;
        public IObservable<int> CollectionCountChanged { 
            get { return _CollectionCountChanged; }
            set { _CollectionCountChanged = value; }
        }

        [IgnoreDataMember]
        Subject<ObservedChange<T, object>> _ItemPropertyChanged;
        public IObservable<ObservedChange<T, object>> ItemPropertyChanged {
            get { return _ItemPropertyChanged; }
        }

        public bool ChangeTrackingEnabled {
            get { return (propertyChangeWatchers != null); }
            set {
                if ((propertyChangeWatchers != null) == value)
                    return;
                if (propertyChangeWatchers == null) {
                    propertyChangeWatchers = new Dictionary<object,IDisposable>();
                } else {
                    releasePropChangeWatchers();
                    propertyChangeWatchers = null;
                }
            }
        }

        [IgnoreDataMember]
        Dictionary<object, IDisposable> _propertyChangeWatchers;
        Dictionary<object, IDisposable> propertyChangeWatchers {
            get { return _propertyChangeWatchers; }
            set { _propertyChangeWatchers = value; }
        }

        protected void releasePropChangeWatchers()
        {
            if (propertyChangeWatchers == null) {
                return;
            }

            foreach(IDisposable x in propertyChangeWatchers.Values) { x.Dispose(); }
            propertyChangeWatchers.Clear();
        }

        protected override void ClearItems()
        {
            // N.B: Reset doesn't give us the items that were cleared out,
            // we have to release the watchers or else we leak them.
            releasePropChangeWatchers();
            base.ClearItems();
        }

        public ReactiveCollection<TNew> CreateDerivedCollection<TNew>(Func<T, TNew> Selector)
        {
            Contract.Requires(Selector != null);
            Contract.Ensures(Contract.Result<ReactiveCollection<TNew>>().Count == this.Count);

            var ret = new ReactiveCollection<TNew>(this.Select(Selector));
            var coll_changed = Observable.FromEvent<NotifyCollectionChangedEventArgs>(this, "CollectionChanged");

            coll_changed.Subscribe(x => {
                switch(x.EventArgs.Action) {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    if (x.EventArgs.OldItems != null) {
                        foreach(object _ in x.EventArgs.OldItems) {
                            ret.RemoveAt(x.EventArgs.OldStartingIndex);
                        }
                    }
                    if (x.EventArgs.NewItems != null) {
                        foreach(T item in x.EventArgs.NewItems.Cast<T>()) {
                            ret.Insert(x.EventArgs.NewStartingIndex, Selector(item));
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ret.Clear();
                    break;
                default:
                    break;
                }
            });

            return ret;
        }

        public void Dispose()
        {
            ChangeTrackingEnabled = false;
        }

#if !SILVERLIGHT
        //
        // N.B: This is a hack to make sure that the ObservableCollection bits 
        // don't end up in the serialized output.
        //

        [field:IgnoreDataMember]
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        [IgnoreDataMember]
        private PropertyChangedEventHandler _propertyChangedEventHandler;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
            add {
                _propertyChangedEventHandler = Delegate.Combine(_propertyChangedEventHandler, value) as PropertyChangedEventHandler;
            }
            remove {
                _propertyChangedEventHandler = Delegate.Remove(_propertyChangedEventHandler, value) as PropertyChangedEventHandler;
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handler = CollectionChanged;

            if (handler != null) {
                handler(this, e);
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = _propertyChangedEventHandler;

            if (handler != null) {
                handler(this, e);
            }
        }
#endif
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :