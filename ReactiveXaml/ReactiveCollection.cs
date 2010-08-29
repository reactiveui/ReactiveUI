using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;

namespace ReactiveXaml
{
    [Serializable]
    public class ReactiveCollection<T> : ObservableCollection<T>, IReactiveCollection<T>, INotifyPropertyChanged, IDisposable
    {
        public ReactiveCollection() { setupRx(); }
        public ReactiveCollection(IEnumerable<T> List) : base(List) { setupRx(); }

        [OnDeserialized]
        void setupRx(StreamingContext _) { setupRx(); }

        void setupRx()
        {
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

        [NonSerialized]
        IObservable<T> _ItemsAdded;
        public IObservable<T> ItemsAdded {
            get { return _ItemsAdded; }
            protected set { _ItemsAdded = value; }
        }

        [NonSerialized]
        IObservable<T> _ItemsRemoved;
        public IObservable<T> ItemsRemoved {
            get { return _ItemsRemoved; }
            set { _ItemsRemoved = value; }
        }

        [NonSerialized]
        IObservable<int> _CollectionCountChanged;
        public IObservable<int> CollectionCountChanged { 
            get { return _CollectionCountChanged; }
            set { _CollectionCountChanged = value; }
        }

        [NonSerialized]
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

        [NonSerialized]
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
            propertyChangeWatchers.Values.Run(x => x.Dispose());
            propertyChangeWatchers.Clear();
        }

        protected override void ClearItems()
        {
            // N.B: Reset doesn't give us the items that were cleared out,
            // we have to release the watchers or else we leak them.
            releasePropChangeWatchers();
            base.ClearItems();
        }

        public void Dispose()
        {
            ChangeTrackingEnabled = false;
        }

        [field: NonSerialized]
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        [field: NonSerialized]
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

    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :