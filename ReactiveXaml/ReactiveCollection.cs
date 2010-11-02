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
using System.Threading;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Disposables;
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
            // XXX: Should this be RxApp.DeferredScheduler
            _BeforeItemsAdded = new Subject<T>();
            _BeforeItemsRemoved = new Subject<T>();
            _ItemPropertyChanging = new Subject<ObservedChange<T, object>>();
            aboutToClear = new Subject<int>();

            if (List != null) {
                foreach(var v in List) { this.Add(v); }
            }

            var coll_changed = Observable.FromEvent<NotifyCollectionChangedEventArgs>(this, "CollectionChanged");

            _ItemsAdded = coll_changed
                .Where(x => x.EventArgs.Action == NotifyCollectionChangedAction.Add || x.EventArgs.Action == NotifyCollectionChangedAction.Replace)
                .SelectMany(x => (x.EventArgs.NewItems != null ? x.EventArgs.NewItems.OfType<T>() : Enumerable.Empty<T>()).ToObservable());

            _ItemsRemoved = coll_changed
                .Where(x => x.EventArgs.Action == NotifyCollectionChangedAction.Remove || x.EventArgs.Action == NotifyCollectionChangedAction.Replace || x.EventArgs.Action == NotifyCollectionChangedAction.Reset)
                .SelectMany(x => (x.EventArgs.OldItems != null ? x.EventArgs.OldItems.OfType<T>() : Enumerable.Empty<T>()).ToObservable());

            _CollectionCountChanging = Observable.Merge(
                _BeforeItemsAdded.Select(_ => this.Count),
                _BeforeItemsRemoved.Select(_ => this.Count),
                aboutToClear
            );

            _CollectionCountChanged = coll_changed
                .Select(x => this.Count)
                .DistinctUntilChanged();

            _ItemPropertyChanged = new Subject<ObservedChange<T,object>>();

            _ItemsAdded.Subscribe(x => {
                if (propertyChangeWatchers == null)
                    return;
                var item = x as IReactiveNotifyPropertyChanged;
                if (item != null) {
                    var to_dispose = new[] {
                        item.BeforeChange.Subscribe(before_change =>
                            _ItemPropertyChanging.OnNext(new ObservedChange<T, object>() { Sender = x, PropertyName = before_change.PropertyName })),
                        item.Subscribe(change => 
                            _ItemPropertyChanged.OnNext(new ObservedChange<T,object>() { Sender = x, PropertyName = change.PropertyName })),
                    };

                    propertyChangeWatchers.Add(x, Disposable.Create(() => { to_dispose[0].Dispose(); to_dispose[1].Dispose(); }));
                        
                }
            });

            _ItemsRemoved.Subscribe(x => {
                if (propertyChangeWatchers == null || !propertyChangeWatchers.ContainsKey(x))
                    return;

                propertyChangeWatchers[x].Dispose();
                propertyChangeWatchers.Remove(x);
            });
        }

        [IgnoreDataMember]
        protected IObservable<T> _ItemsAdded;
        public IObservable<T> ItemsAdded {
            get { return _ItemsAdded.Where(_ => areChangeNotificationsEnabled); }
        }

        [IgnoreDataMember]
        protected Subject<T> _BeforeItemsAdded;
        public IObservable<T> BeforeItemsAdded {
            get { return _BeforeItemsAdded.Where(_ => areChangeNotificationsEnabled); }
        }

        [IgnoreDataMember]
        protected IObservable<T> _ItemsRemoved;
        public IObservable<T> ItemsRemoved {
            get { return _ItemsRemoved.Where(_ => areChangeNotificationsEnabled); }
        }

        [IgnoreDataMember]
        protected Subject<T> _BeforeItemsRemoved;
        public IObservable<T> BeforeItemsRemoved {
            get { return _BeforeItemsRemoved.Where(_ => areChangeNotificationsEnabled); }
        }

        [IgnoreDataMember]
        protected Subject<int> aboutToClear;

        [IgnoreDataMember]
        protected IObservable<int> _CollectionCountChanging;
        public IObservable<int> CollectionCountChanging {
            get { return _CollectionCountChanging.Where(_ => areChangeNotificationsEnabled); }
        }

        [IgnoreDataMember]
        protected IObservable<int> _CollectionCountChanged;
        public IObservable<int> CollectionCountChanged {
            get { return _CollectionCountChanged.Where(_ => areChangeNotificationsEnabled); }
        }

        [IgnoreDataMember]
        protected Subject<ObservedChange<T, object>> _ItemPropertyChanging;
        public IObservable<ObservedChange<T, object>> ItemPropertyChanging {
            get { return _ItemPropertyChanging.Where(_ => areChangeNotificationsEnabled); }
        }

        [IgnoreDataMember]
        protected Subject<ObservedChange<T, object>> _ItemPropertyChanged;
        public IObservable<ObservedChange<T, object>> ItemPropertyChanged {
            get { return _ItemPropertyChanged.Where(_ => areChangeNotificationsEnabled); }
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

        protected override void InsertItem(int index, T item)
        {
            _BeforeItemsAdded.OnNext(item);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            _BeforeItemsRemoved.OnNext(this[index]);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            _BeforeItemsRemoved.OnNext(this[index]);
            _BeforeItemsAdded.OnNext(item);
            base.SetItem(index, item);
        }

        protected override void ClearItems()
        {
            aboutToClear.OnNext(this.Count);

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
                    // NB: SL4 fills in OldStartingIndex with -1 on Replace :-/
                    int old_index = (x.EventArgs.Action == NotifyCollectionChangedAction.Replace ?
                        x.EventArgs.NewStartingIndex : x.EventArgs.OldStartingIndex);

                    if (x.EventArgs.OldItems != null) {
                        foreach(object _ in x.EventArgs.OldItems) {
                            ret.RemoveAt(old_index);
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

        [IgnoreDataMember]
        long changeNotificationsSuppressed = 0;

        public IDisposable SuppressChangeNotifications()
        {
            Interlocked.Increment(ref changeNotificationsSuppressed);
            return Disposable.Create(() =>
                Interlocked.Decrement(ref changeNotificationsSuppressed));
        }

        protected bool areChangeNotificationsEnabled {
            get { 
#if SILVERLIGHT
                // N.B. On most architectures, machine word aligned reads are 
                // guaranteed to be atomic - sorry WP7, you're out of luck
                return changeNotificationsSuppressed == 0;
#else
                return (Interlocked.Read(ref changeNotificationsSuppressed) == 0); 
#endif
            }
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

    public static class ReactiveCollectionMixins
    {
        public static ReactiveCollection<T> CreateCollection<T>(this IObservable<T> FromObservable, TimeSpan? WithDelay = null)
        {
            var ret = new ReactiveCollection<T>();
            if (WithDelay == null) {
                FromObservable.ObserveOn(RxApp.DeferredScheduler).Subscribe(ret.Add);
                return ret;
            }

            // On a timer, dequeue items from queue if they are available
            var queue = new Queue<T>();
            var disconnect = Observable.Timer(WithDelay.Value, WithDelay.Value)
                .ObserveOn(RxApp.DeferredScheduler).Subscribe(_ => {
                    if (queue.Count > 0) { 
                        ret.Add(queue.Dequeue());
                    }
                });

            // When new items come in from the observable, stuff them in the queue.
            // Using the DeferredScheduler guarantees we'll always access the queue
            // from the same thread.
            FromObservable.ObserveOn(RxApp.DeferredScheduler).Subscribe(queue.Enqueue);

            // This is a bit clever - keep a running count of the items actually 
            // added and compare them to the final count of items provided by the
            // Observable. Combine the two values, and when they're equal, 
            // disconnect the timer
            ret.ItemsAdded.Scan0(0, ((acc, _) => acc+1)).Zip(FromObservable.Aggregate(0, (acc,_) => acc+1), 
                (l,r) => (l == r)).Where(x => x).Subscribe(_ => disconnect.Dispose());

            return ret;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :