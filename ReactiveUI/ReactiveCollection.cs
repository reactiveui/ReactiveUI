using System;
using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Reactive.Disposables;
using System.Globalization;
using System.Diagnostics;

namespace ReactiveUI
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    public class ReactiveCollection<T> : IList<T>, IList, IReactiveCollection<T>, INotifyPropertyChanging, INotifyPropertyChanged, IEnableLogger
    {
        public event NotifyCollectionChangedEventHandler CollectionChanging;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        [field: IgnoreDataMember]
        bool rxObjectsSetup = false;
        [IgnoreDataMember] Subject<NotifyCollectionChangedEventArgs> _changing;
        [IgnoreDataMember] Subject<NotifyCollectionChangedEventArgs> _changed;
        
        [DataMember] List<T> _inner;

        [IgnoreDataMember] int _suppressionRefCount = 0;

        [IgnoreDataMember] Lazy<Subject<T>> _beforeItemsAdded;
        [IgnoreDataMember] Lazy<Subject<T>> _itemsAdded;
        [IgnoreDataMember] Lazy<Subject<T>> _beforeItemsRemoved;
        [IgnoreDataMember] Lazy<Subject<T>> _itemsRemoved;
        [IgnoreDataMember] Lazy<Subject<IObservedChange<T, object>>> _itemChanging;
        [IgnoreDataMember] Lazy<Subject<IObservedChange<T, object>>> _itemChanged;

        [IgnoreDataMember] Dictionary<object, RefcountDisposeWrapper> _propertyChangeWatchers = null;

        [IgnoreDataMember] int _resetSubCount = 0;
        static bool _hasWhinedAboutNoResetSub = false;

        // NB: This exists so the serializer doesn't whine
        //
        // 2nd NB: VB.NET doesn't deal well with default parameters, create 
        // some overloads so people can continue to make bad life choices instead
        // of using C#
        public ReactiveCollection() { setupRx(); }
        public ReactiveCollection(IEnumerable<T> initialContents) { setupRx(initialContents); }

        public ReactiveCollection(IEnumerable<T> initialContents = null, IScheduler scheduler = null, double resetChangeThreshold = 0.3)
        {
            setupRx(initialContents, scheduler, resetChangeThreshold);
        }

        [OnDeserialized]
#if WP7
		public
#endif
        void setupRx(StreamingContext _) { setupRx(); }

        void setupRx(IEnumerable<T> initialContents = null, IScheduler scheduler = null, double resetChangeThreshold = 0.3)
        {
            if (rxObjectsSetup) return; 

            scheduler = scheduler ?? RxApp.DeferredScheduler;
            _inner = _inner ?? new List<T>();

            _changing = new Subject<NotifyCollectionChangedEventArgs>();
            _changing.Where(_ => CollectionChanging != null && _suppressionRefCount == 0).Subscribe(x => CollectionChanging(this, x));

            _changed = new Subject<NotifyCollectionChangedEventArgs>();
            _changed.Where(_ => CollectionChanged != null && _suppressionRefCount == 0).Subscribe(x => CollectionChanged(this, x));

            ResetChangeThreshold = resetChangeThreshold;

            _beforeItemsAdded = new Lazy<Subject<T>>(() => new Subject<T>());
            _itemsAdded = new Lazy<Subject<T>>(() => new Subject<T>());
            _beforeItemsRemoved = new Lazy<Subject<T>>(() => new Subject<T>());
            _itemsRemoved = new Lazy<Subject<T>>(() => new Subject<T>());
            _itemChanging = new Lazy<Subject<IObservedChange<T, object>>>(() => new Subject<IObservedChange<T, object>>());
            _itemChanged = new Lazy<Subject<IObservedChange<T, object>>>(() => new Subject<IObservedChange<T, object>>());

            // NB: We have to do this instead of initializing _inner so that
            // Collection<T>'s accounting is correct
            foreach (var item in initialContents ?? Enumerable.Empty<T>()) { Add(item); }

            // NB: ObservableCollection has a Secret Handshake with WPF where 
            // they fire an INPC notification with the token "Item[]". Emulate 
            // it here
            CollectionCountChanging.Subscribe(_ => {
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("Count"));
            });

            CollectionCountChanged.Subscribe(_ => {
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Count"));
            });

            Changing.Subscribe(_ => {
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("Item[]"));
            });

            Changed.Subscribe(_ => {
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Item[]"));
            });

            rxObjectsSetup = true;
        }


        /*
         * Collection<T> core methods
         */

        protected void InsertItem(int index, T item)
        {
            if (_suppressionRefCount > 0) {
                _inner.Insert(index, item);
            
                if (ChangeTrackingEnabled) addItemToPropertyTracking(item);
                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);

            _changing.OnNext(ea);
            if (_beforeItemsAdded.IsValueCreated) _beforeItemsAdded.Value.OnNext(item);

            _inner.Insert(index, item);

            _changed.OnNext(ea);
            if (_itemsAdded.IsValueCreated) _itemsAdded.Value.OnNext(item);

            if (ChangeTrackingEnabled) addItemToPropertyTracking(item);
        }

        protected void RemoveItem(int index)
        {
            var item = _inner[index];

            if (_suppressionRefCount > 0) {
                _inner.RemoveAt(index);
            
                if (ChangeTrackingEnabled) removeItemFromPropertyTracking(item);
                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);

            _changing.OnNext(ea);
            if (_beforeItemsRemoved.IsValueCreated) _beforeItemsRemoved.Value.OnNext(item);

            _inner.RemoveAt(index);

            _changed.OnNext(ea);
            if (_itemsRemoved.IsValueCreated) _itemsRemoved.Value.OnNext(item);
            if (ChangeTrackingEnabled) removeItemFromPropertyTracking(item);
        }

        protected void SetItem(int index, T item)
        {
            if (_suppressionRefCount > 0) {
                _inner[index] = item;

                if (ChangeTrackingEnabled) {
                    removeItemFromPropertyTracking(_inner[index]);
                    addItemToPropertyTracking(item);
                }

                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, _inner[index], index);

            _changing.OnNext(ea);

            if (ChangeTrackingEnabled) {
                removeItemFromPropertyTracking(_inner[index]);
                addItemToPropertyTracking(item);
            }

            _inner[index] = item;
            _changed.OnNext(ea);
        }

        protected void ClearItems()
        {
            if (_suppressionRefCount > 0) {
                _inner.Clear();
            
                if (ChangeTrackingEnabled) clearAllPropertyChangeWatchers();
                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

            _changing.OnNext(ea);
            _inner.Clear();
            _changed.OnNext(ea);

            if (ChangeTrackingEnabled) clearAllPropertyChangeWatchers();
        }


        /*
         * List<T> methods we can make faster by possibly sending ShouldReset 
         * notifications instead of thrashing the UI by readding items
         * one at a time
         */

        public double ResetChangeThreshold { get; set; }

        public virtual void AddRange(IEnumerable<T> collection)
        {
            InsertRange(_inner.Count, collection);
        }

        public virtual void InsertRange(int index, IEnumerable<T> collection)
        {
            var arr = collection.ToArray();
            var disp = isLengthAboveResetThreshold(arr.Length) ?
                SuppressChangeNotifications() : Disposable.Empty;

            using (disp) {
                // NB: If we don't do this, we'll break Collection<T>'s 
                // accounting of the length
                for (int i = arr.Length - 1; i >= 0; i--) {
                    InsertItem(index, arr[i]);
                }
            }
        }

        public virtual void RemoveRange(int index, int count)
        {
            var disp = isLengthAboveResetThreshold(count) ?
                SuppressChangeNotifications() : Disposable.Empty;

            using (disp) {
                // NB: If we don't do this, we'll break Collection<T>'s 
                // accounting of the length
                for (int i = count; i > 0; i--) {
                    RemoveItem(index);
                }
            }
        }

        public virtual void RemoveAll(IEnumerable<T> items)
        {
            Contract.Requires(items != null);

            var disp = isLengthAboveResetThreshold(items.Count()) ?
                SuppressChangeNotifications() : Disposable.Empty;

            using (disp) {
                // NB: If we don't do this, we'll break Collection<T>'s
                // accounting of the length
                foreach (var v in items) {
                    Remove(v);
                }
            }
        }

        public virtual void Sort(int index, int count, IComparer<T> comparer)
        {
            _inner.Sort(index, count, comparer);
            Reset();
        }

        public virtual void Sort(Comparison<T> comparison)
        {
            _inner.Sort(comparison);
            Reset();
        }

        public virtual void Sort(IComparer<T> comparer = null)
        {
            _inner.Sort(comparer ?? Comparer<T>.Default);
            Reset();
        }

        public virtual void Reset()
        {
            publishResetNotification();
        }


        protected virtual void publishResetNotification()
        {
            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            _changing.OnNext(ea);
            _changed.OnNext(ea);
        }

        bool isLengthAboveResetThreshold(int toChangeLength)
        {
            return (double) toChangeLength/_inner.Count > ResetChangeThreshold &&
                toChangeLength > 10;
        }


        /*
         * IReactiveCollection<T>
         */

        public bool ChangeTrackingEnabled {
            get { return _propertyChangeWatchers != null; }
            set {
                if (_propertyChangeWatchers != null && value) return;
                if (_propertyChangeWatchers == null && !value) return;

                if (value) {
                    _propertyChangeWatchers = new Dictionary<object, RefcountDisposeWrapper>();
                    foreach (var item in _inner) { addItemToPropertyTracking(item); }
                } else {
                    clearAllPropertyChangeWatchers();
                    _propertyChangeWatchers = null;
                }
            }
        }

        public IDisposable SuppressChangeNotifications()
        {
            Interlocked.Increment(ref _suppressionRefCount);

            if (!_hasWhinedAboutNoResetSub && _resetSubCount == 0 && CollectionChanged == null) {
                LogHost.Default.Warn("SuppressChangeNotifications was called (perhaps via AddRange), yet you do not");
                LogHost.Default.Warn("have a subscription to ShouldReset. This probably isn't what you want, as ItemsAdded");
                LogHost.Default.Warn("and friends will appear to 'miss' items");
                _hasWhinedAboutNoResetSub = true;
            }

            return Disposable.Create(() => {
                if (Interlocked.Decrement(ref _suppressionRefCount) == 0) {
                    publishResetNotification();
                }
            });
        }
 
        public IObservable<T> BeforeItemsAdded { get { return _beforeItemsAdded.Value; } }
        public IObservable<T> ItemsAdded { get { return _itemsAdded.Value; } }

        public IObservable<T> BeforeItemsRemoved { get { return _beforeItemsRemoved.Value; } }
        public IObservable<T> ItemsRemoved { get { return _itemsRemoved.Value; } }

        public IObservable<IObservedChange<T, object>> ItemChanging { get { return _itemChanging.Value; } }
        public IObservable<IObservedChange<T, object>> ItemChanged { get { return _itemChanged.Value; } }

        IObservable<object> IReactiveCollection.BeforeItemsAdded { get { return BeforeItemsAdded.Select(x => (object) x); } }
        IObservable<object> IReactiveCollection.ItemsAdded { get { return ItemsAdded.Select(x => (object) x); } }

        IObservable<object> IReactiveCollection.BeforeItemsRemoved { get { return BeforeItemsRemoved.Select(x => (object) x); } }
        IObservable<object> IReactiveCollection.ItemsRemoved { get { return ItemsRemoved.Select(x => (object) x); } }

        IObservable<IObservedChange<object, object>> IReactiveCollection.ItemChanging {
            get {
                return _itemChanging.Value.Select(x => (IObservedChange<object, object>) new ObservedChange<object, object>() {
                    Sender = x.Sender,
                    PropertyName = x.PropertyName,
                    Value = x.Value,
                });
            }
        }

        IObservable<IObservedChange<object, object>> IReactiveCollection.ItemChanged {
            get {
                return _itemChanged.Value.Select(x => (IObservedChange<object, object>) new ObservedChange<object, object>() {
                    Sender = x.Sender,
                    PropertyName = x.PropertyName,
                    Value = x.Value,
                });
            }
        }

        public IObservable<int> CollectionCountChanging {
            get { return _changing.Select(_ => _inner.Count).DistinctUntilChanged(); }
        }

        public IObservable<int> CollectionCountChanged {
            get { return _changed.Select(_ => _inner.Count).DistinctUntilChanged(); }
        }

        public IObservable<bool> IsEmpty {
            get { return _changed.Select(_ => _inner.Count == 0).DistinctUntilChanged(); }
        }

        public IObservable<NotifyCollectionChangedEventArgs> Changing {
            get { return _changing; }
        }

        public IObservable<NotifyCollectionChangedEventArgs> Changed {
            get { return _changed; }
        }

        public IObservable<Unit> ShouldReset {
            get {
                return refcountSubscribers(_changed.SelectMany(x => 
                    x.Action != NotifyCollectionChangedAction.Reset ?
                        Observable.Empty<Unit>() :
                        Observable.Return(Unit.Default)), x => _resetSubCount += x);
            }
        }


        /*
         * Property Change Tracking
         */

        void addItemToPropertyTracking(T toTrack)
        {
            if (_propertyChangeWatchers.ContainsKey(toTrack)) {
                _propertyChangeWatchers[toTrack].AddRef();
                return;
            }

            var changing = Observable.Never<IObservedChange<object, object>>();
            var changed = Observable.Never<IObservedChange<object, object>>();

            var irnpc = toTrack as IReactiveNotifyPropertyChanged;
            if (irnpc != null) {
                changing = irnpc.Changing;
                changed = irnpc.Changed;
                goto isSetup;
            }

            var inpc = toTrack as INotifyPropertyChanged;
            if (inpc != null) {
                changed = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(x => inpc.PropertyChanged += x, x => inpc.PropertyChanged -= x)
                    .Select(x => (IObservedChange<object, object>)
                        new ObservedChange<object, object>() { PropertyName = x.EventArgs.PropertyName, Sender = inpc });
                goto isSetup;
            }

            this.Log().Warn("Property change notifications are enabled and type {0} isn't INotifyPropertyChanged or ReactiveObject", typeof(T));

        isSetup:
            var toDispose = new[] {
                changing.Where(_ => _suppressionRefCount == 0).Subscribe(beforeChange =>
                    _itemChanging.Value.OnNext(new ObservedChange<T, object>() { 
                        Sender = toTrack, PropertyName = beforeChange.PropertyName })),
                changed.Where(_ => _suppressionRefCount == 0).Subscribe(change => 
                    _itemChanged.Value.OnNext(new ObservedChange<T,object>() { 
                        Sender = toTrack, PropertyName = change.PropertyName })),
            };

            _propertyChangeWatchers.Add(toTrack, 
                new RefcountDisposeWrapper(Disposable.Create(() => {
                    toDispose[0].Dispose(); toDispose[1].Dispose();
                    _propertyChangeWatchers.Remove(toTrack);
            })));
        }

        void removeItemFromPropertyTracking(T toUntrack)
        {
            _propertyChangeWatchers[toUntrack].Release();
        }

        void clearAllPropertyChangeWatchers()
        {
            while (_propertyChangeWatchers.Count > 0) _propertyChangeWatchers.Values.First().Release();
        }

        static IObservable<TObs> refcountSubscribers<TObs>(IObservable<TObs> input, Action<int> block)
        {
            return Observable.Create<TObs>(subj => {
                block(1);

                return new CompositeDisposable(
                    input.Subscribe(subj),
                    Disposable.Create(() => block(-1)));
            });
        }

        #region Super Boring IList crap
        public IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual void Add(T item)
        {
            InsertItem(_inner.Count, item);
        }

        public virtual void Clear()
        {
            ClearItems();
        }

        public bool Contains(T item)
        {
            return _inner.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _inner.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(T item)
        {
            int index = _inner.IndexOf(item);
            if (index < 0) return false;

            RemoveItem(index);
            return true;
        }

        public int Count { get { return _inner.Count; } }

        public virtual bool IsReadOnly { get { return false; } }

        public int IndexOf(T item)
        {
            return _inner.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            InsertItem(index, item);
        }

        public virtual void RemoveAt(int index)
        {
            RemoveItem(index);
        }

        public virtual T this[int index] {
            get { return _inner[index]; }
            set { SetItem(index, value); }
        }

        public int Add(object value)
        {
            Add((T)value);
            return Count - 1;
        }

        public bool Contains(object value)
        {
            return IsCompatibleObject(value) && Contains((T)value);
        }

        public int IndexOf(object value)
        {
            return IsCompatibleObject(value) ? IndexOf((T)value) : -1;
        }

        public void Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        public bool IsFixedSize { get { return false; } }

        public void Remove(object value)
        {
            if (IsCompatibleObject(value)) Remove((T)value);
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (T)value; }
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)_inner).CopyTo(array, index);
        }

        public bool IsSynchronized { get { return false; } }

        public object SyncRoot { get { return this; } }

        private static bool IsCompatibleObject(object value)
        {
            return ((value is T) || ((value == null) && (default(T) == null)));
        }
        #endregion
    }


}

// vim: tw=120 ts=4 sw=4 et :
