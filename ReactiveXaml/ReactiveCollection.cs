using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Disposables;

namespace ReactiveXaml
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReactiveCollection<T> : ObservableCollection<T>, IReactiveCollection<T>, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public ReactiveCollection() { setupRx(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public ReactiveCollection(IEnumerable<T> list) { setupRx(list); }

        [OnDeserialized]
        void setupRx(StreamingContext _) { setupRx(); }

        void setupRx(IEnumerable<T> List = null)
        {
            _BeforeItemsAdded = new Subject<T>();
            _BeforeItemsRemoved = new Subject<T>();
            aboutToClear = new Subject<int>();

            if (List != null) {
                foreach(var v in List) { this.Add(v); }
            }

            var ocChangedEvent = Observable.FromEvent<NotifyCollectionChangedEventArgs>(this, "CollectionChanged");

            _ItemsAdded = ocChangedEvent
                .Where(x => x.EventArgs.Action == NotifyCollectionChangedAction.Add || x.EventArgs.Action == NotifyCollectionChangedAction.Replace)
                .SelectMany(x => (x.EventArgs.NewItems != null ? x.EventArgs.NewItems.OfType<T>() : Enumerable.Empty<T>()).ToObservable());

            _ItemsRemoved = ocChangedEvent
                .Where(x => x.EventArgs.Action == NotifyCollectionChangedAction.Remove || x.EventArgs.Action == NotifyCollectionChangedAction.Replace || x.EventArgs.Action == NotifyCollectionChangedAction.Reset)
                .SelectMany(x => (x.EventArgs.OldItems != null ? x.EventArgs.OldItems.OfType<T>() : Enumerable.Empty<T>()).ToObservable());

            _CollectionCountChanging = Observable.Merge(
                _BeforeItemsAdded.Select(_ => this.Count),
                _BeforeItemsRemoved.Select(_ => this.Count),
                aboutToClear
            );

            _CollectionCountChanged = ocChangedEvent
                .Select(x => this.Count)
                .DistinctUntilChanged();

            _ItemChanging = new Subject<IObservedChange<T, object>>();
            _ItemChanged = new Subject<IObservedChange<T,object>>();

            // TODO: Fix up this selector nonsense once SL/WP7 gets Covariance
            _Changing = Observable.Merge(
                _BeforeItemsAdded.Select<T, IObservedChange<object, object>>(x => new ObservedChange<object, object>() {PropertyName =  "Items", Sender = this, Value = this}),
                _BeforeItemsRemoved.Select<T, IObservedChange<object, object>>(x => new ObservedChange<object, object>() {PropertyName =  "Items", Sender = this, Value = this}),
                aboutToClear.Select<int, IObservedChange<object, object>>(x => new ObservedChange<object, object>() {PropertyName = "Items", Sender = this, Value = this}),
                _ItemChanging.Select<IObservedChange<T, object>, IObservedChange <object, object>>(x => new ObservedChange<object, object>() {PropertyName = x.PropertyName, Sender = x.Sender, Value = x.Value}));

            _Changed = Observable.Merge(
                _ItemsAdded.Select<T, IObservedChange<object, object>>(x => new ObservedChange<object, object>() {PropertyName = "It ems", Sender = this, Value = this}),
                _ItemsRemoved.Select<T, IObservedChange<object, object>>(x => new ObservedChange<object, object>() {PropertyName =  "Items", Sender = this, Value = this}),
                _ItemChanged.Select<IObservedChange<T, object>, IObservedChange<object, object>>(x => new ObservedChange<object, object>() {PropertyName = x.PropertyName, Sender = x.Sender, Value = x.Value}));

            _ItemsAdded.Subscribe(x => {
                this.Log().DebugFormat("Item Added to {0:X} - {1}", this.GetHashCode(), x);
                if (propertyChangeWatchers == null)
                    return;
                addItemToPropertyTracking(x);
            });

            _ItemsRemoved.Subscribe(x => {
                this.Log().DebugFormat("Item removed from {0:X} - {1}", this.GetHashCode(), x);
                if (propertyChangeWatchers == null || !propertyChangeWatchers.ContainsKey(x))
                    return;

                propertyChangeWatchers[x].Dispose();
                propertyChangeWatchers.Remove(x);
            });

#if DEBUG
            _ItemChanged.Subscribe(x => 
                this.Log().DebugFormat("Object {0} changed in collection {1:X}", x, this.GetHashCode()));
#endif
        }

        void addItemToPropertyTracking(T toTrack)
        {
            var item = toTrack as IReactiveNotifyPropertyChanged;
            if (item == null)
                return;

            var to_dispose = new[] {
                item.Changing.Subscribe(before_change =>
                    _ItemChanging.OnNext(new ObservedChange<T, object>() { Sender = toTrack, PropertyName = before_change.PropertyName })),
                item.Changed.Subscribe(change => 
                    _ItemChanged.OnNext(new ObservedChange<T,object>() { Sender = toTrack, PropertyName = change.PropertyName })),
            };

            propertyChangeWatchers.Add(toTrack, Disposable.Create(() => {
                to_dispose[0].Dispose(); to_dispose[1].Dispose();
            }));
        }

        [IgnoreDataMember]
        protected IObservable<T> _ItemsAdded;

        /// <summary>
        ///
        /// </summary>
        public IObservable<T> ItemsAdded {
            get { return _ItemsAdded.Where(_ => areChangeNotificationsEnabled); }
        }

        [IgnoreDataMember]
        protected Subject<T> _BeforeItemsAdded;

        /// <summary>
        /// 
        /// </summary>
        public IObservable<T> BeforeItemsAdded {
            get { return _BeforeItemsAdded.Where(_ => areChangeNotificationsEnabled); }
        }

        [IgnoreDataMember]
        protected IObservable<T> _ItemsRemoved;

        /// <summary>
        /// 
        /// </summary>
        public IObservable<T> ItemsRemoved {
            get { return _ItemsRemoved.Where(_ => areChangeNotificationsEnabled); }
        }

        [IgnoreDataMember]
        protected Subject<T> _BeforeItemsRemoved;

        /// <summary>
        /// 
        /// </summary>
        public IObservable<T> BeforeItemsRemoved {
            get { return _BeforeItemsRemoved.Where(_ => areChangeNotificationsEnabled); }
        }

        [IgnoreDataMember]
        protected Subject<int> aboutToClear;

        [IgnoreDataMember]
        protected IObservable<int> _CollectionCountChanging;

        /// <summary>
        /// 
        /// </summary>
        public IObservable<int> CollectionCountChanging {
            get { return _CollectionCountChanging.Where(_ => areChangeNotificationsEnabled); }
        }

        [IgnoreDataMember]
        protected IObservable<int> _CollectionCountChanged;

        /// <summary>
        /// 
        /// </summary>
        public IObservable<int> CollectionCountChanged {
            get { return _CollectionCountChanged.Where(_ => areChangeNotificationsEnabled); }
        }

        [IgnoreDataMember]
        protected Subject<IObservedChange<T, object>> _ItemChanging;

        /// <summary>
        /// 
        /// </summary>
        public IObservable<IObservedChange<T, object>> ItemChanging {
            get { return _ItemChanging.Where(_ => areChangeNotificationsEnabled); }
        }
        IObservable<IObservedChange<object, object>> IReactiveCollection.ItemChanging {
            get { return (IObservable<IObservedChange<object, object>>)ItemChanging; }
        }

        [IgnoreDataMember]
        protected Subject<IObservedChange<T, object>> _ItemChanged;

        /// <summary>
        /// 
        /// </summary>
        public IObservable<IObservedChange<T, object>> ItemChanged {
            get { return _ItemChanged.Where(_ => areChangeNotificationsEnabled); }
        }
        IObservable<IObservedChange<object, object>> IReactiveCollection.ItemChanged {
            get { return (IObservable<IObservedChange<object, object>>)ItemChanged; }
        }

        [IgnoreDataMember]
        protected IObservable<IObservedChange<object, object>> _Changing;

        /// <summary>
        /// 
        /// </summary>
        public IObservable<IObservedChange<object, object>> Changing {
            get { return _Changing.Where(_ => areChangeNotificationsEnabled);  }
        }

        [IgnoreDataMember]
        protected IObservable<IObservedChange<object, object>> _Changed;

        /// <summary>
        /// 
        /// </summary>
        public IObservable<IObservedChange<object, object>> Changed {
            get { return _Changed.Where(_ => areChangeNotificationsEnabled);  }
        }

        [field:IgnoreDataMember]
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        ///
        /// </summary>
        public bool ChangeTrackingEnabled {
            get { return (propertyChangeWatchers != null); }
            set {
                if ((propertyChangeWatchers != null) == value)
                    return;
                if (propertyChangeWatchers == null) {
                    propertyChangeWatchers = new Dictionary<object,IDisposable>();
                    foreach (var v in this) {
                        addItemToPropertyTracking(v);
                    }
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


        public void Dispose()
        {
            ChangeTrackingEnabled = false;
        }

        [IgnoreDataMember]
        long changeNotificationsSuppressed = 0;

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
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

        IObservable<object> IReactiveCollection.ItemsAdded {
            get { return ItemsAdded.Select(x => (object) x); }
        }
        IObservable<object> IReactiveCollection.BeforeItemsAdded {
            get { return BeforeItemsAdded.Select(x => (object) x); }
        }
        IObservable<object> IReactiveCollection.ItemsRemoved {
            get { return ItemsRemoved.Select(x => (object) x); }
        }
        IObservable<object> IReactiveCollection.BeforeItemsRemoved {
            get { return BeforeItemsRemoved.Select(x => (object)x); }
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
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromObservable"></param>
        /// <param name="withDelay"></param>
        /// <returns></returns>
        public static ReactiveCollection<T> CreateCollection<T>(this IObservable<T> fromObservable, TimeSpan? withDelay = null)
        {
            var ret = new ReactiveCollection<T>();
            if (withDelay == null) {
                fromObservable.ObserveOn(RxApp.DeferredScheduler).Subscribe(ret.Add);
                return ret;
            }

            // On a timer, dequeue items from queue if they are available
            var queue = new Queue<T>();
            var disconnect = Observable.Timer(withDelay.Value, withDelay.Value, RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.DeferredScheduler).Subscribe(_ => {
                    if (queue.Count > 0) { 
                        ret.Add(queue.Dequeue());
                    }
                });

            // When new items come in from the observable, stuff them in the queue.
            // Using the DeferredScheduler guarantees we'll always access the queue
            // from the same thread.
            fromObservable.ObserveOn(RxApp.DeferredScheduler).Subscribe(queue.Enqueue);

            // This is a bit clever - keep a running count of the items actually 
            // added and compare them to the final count of items provided by the
            // Observable. Combine the two values, and when they're equal, 
            // disconnect the timer
            ret.ItemsAdded.Scan0(0, ((acc, _) => acc+1)).Zip(fromObservable.Aggregate(0, (acc,_) => acc+1), 
                (l,r) => (l == r)).Where(x => x).Subscribe(_ => disconnect.Dispose());

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="fromObservable"></param>
        /// <param name="selector"></param>
        /// <param name="withDelay"></param>
        /// <returns></returns>
        public static ReactiveCollection<TRet> CreateCollection<T, TRet>(this IObservable<T> fromObservable, Func<T, TRet> selector, TimeSpan? withDelay = null)
        {
            Contract.Requires(selector != null);
            return fromObservable.Select(selector).CreateCollection(withDelay);
        }
    }

    public static class ObservableCollectionMixin
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TNew"></typeparam>
        /// <param name="This"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static ReactiveCollection<TNew> CreateDerivedCollection<T, TNew>(this ObservableCollection<T> This, Func<T, TNew> selector)
        {
            Contract.Requires(selector != null);
#if !IOS    // Contract.Result is borked in Mono
            Contract.Ensures(Contract.Result<ReactiveCollection<TNew>>().Count == This.Count);
#endif
            var ret = new ReactiveCollection<TNew>(This.Select(selector));
            var coll_changed = Observable.FromEvent<NotifyCollectionChangedEventArgs>(This, "CollectionChanged");

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
                            ret.Insert(x.EventArgs.NewStartingIndex, selector(item));
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
    }
}

// vim: tw=120 ts=4 sw=4 et :