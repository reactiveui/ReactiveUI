using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using System.Threading;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Reactive List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ReactiveUI.IReactiveList{T}"/>
    /// <seealso cref="ReactiveUI.IReadOnlyReactiveList{T}"/>
    /// <seealso cref="System.Collections.IList"/>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    public class ReactiveList<T> : IReactiveList<T>, IReadOnlyReactiveList<T>, IList
    {
#if NET_45
        public event NotifyCollectionChangedEventHandler CollectionChanging;

        protected virtual void raiseCollectionChanging(NotifyCollectionChangedEventArgs args)
        {
            var handler = this.CollectionChanging;
            if (handler != null) {
                handler(this, args);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void raiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            var handler = this.CollectionChanged;
            if (handler != null) {
                handler(this, args);
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            var handler = this.PropertyChanging;
            if (handler != null) {
                handler(this, args);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = this.PropertyChanged;
            if (handler != null) {
                handler(this, args);
            }
        }
#else

        /// <summary>
        /// Occurs when [collection changing].
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanging
        {
            add { CollectionChangingEventManager.AddHandler(this, value); }
            remove { CollectionChangingEventManager.RemoveHandler(this, value); }
        }

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { CollectionChangedEventManager.AddHandler(this, value); }
            remove { CollectionChangedEventManager.RemoveHandler(this, value); }
        }

        /// <summary>
        /// Raises the collection changing.
        /// </summary>
        /// <param name="e">
        /// The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void raiseCollectionChanging(NotifyCollectionChangedEventArgs e)
        {
            CollectionChangingEventManager.DeliverEvent(this, e);
        }

        /// <summary>
        /// Raises the collection changed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void raiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChangedEventManager.DeliverEvent(this, e);
        }

        /// <summary>
        /// Occurs when [property changing].
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add { PropertyChangingEventManager.AddHandler(this, value); }
            remove { PropertyChangingEventManager.RemoveHandler(this, value); }
        }

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { PropertyChangedEventManager.AddHandler(this, value); }
            remove { PropertyChangedEventManager.RemoveHandler(this, value); }
        }

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

#endif

        [IgnoreDataMember] private Subject<NotifyCollectionChangedEventArgs> _changing;
        [IgnoreDataMember] private Subject<NotifyCollectionChangedEventArgs> _changed;

        [DataMember] private List<T> _inner;

        [IgnoreDataMember] private Lazy<Subject<T>> _beforeItemsAdded;
        [IgnoreDataMember] private Lazy<Subject<T>> _itemsAdded;
        [IgnoreDataMember] private Lazy<Subject<T>> _beforeItemsRemoved;
        [IgnoreDataMember] private Lazy<Subject<T>> _itemsRemoved;
        [IgnoreDataMember] private Lazy<ISubject<IReactivePropertyChangedEventArgs<T>>> _itemChanging;
        [IgnoreDataMember] private Lazy<ISubject<IReactivePropertyChangedEventArgs<T>>> _itemChanged;
        [IgnoreDataMember] private Lazy<Subject<IMoveInfo<T>>> _beforeItemsMoved;
        [IgnoreDataMember] private Lazy<Subject<IMoveInfo<T>>> _itemsMoved;

        [IgnoreDataMember] private Dictionary<object, RefcountDisposeWrapper> _propertyChangeWatchers = null;

        [IgnoreDataMember] private int _resetSubCount = 0;
        [IgnoreDataMember] private int _resetNotificationCount = 0;
        private static bool _hasWhinedAboutNoResetSub = false;

        /// <summary>
        /// Gets or sets the reset change threshold.
        /// </summary>
        /// <value>The reset change threshold.</value>
        [IgnoreDataMember]
        public double ResetChangeThreshold { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveList{T}"/> class.
        /// NB: This exists so the serializer doesn't whine // 2nd NB: VB.NET doesn't deal well with
        ///     default parameters, create some overloads so people can continue to make bad life
        /// choices instead of using C#
        /// </summary>
        public ReactiveList() { setupRx(); }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveList{T}"/> class.
        /// </summary>
        /// <param name="initialContents">The initial contents.</param>
        public ReactiveList(IEnumerable<T> initialContents)
        {
            setupRx(initialContents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveList{T}"/> class.
        /// </summary>
        /// <param name="initialContents">The initial contents.</param>
        /// <param name="resetChangeThreshold">The reset change threshold.</param>
        /// <param name="scheduler">The scheduler.</param>
        public ReactiveList(IEnumerable<T> initialContents = null, double resetChangeThreshold = 0.3, IScheduler scheduler = null)
        {
            setupRx(initialContents, resetChangeThreshold, scheduler);
        }

        [OnDeserialized]
        private void setupRx(StreamingContext _)
        { setupRx(); }

        private void setupRx(IEnumerable<T> initialContents = null, double resetChangeThreshold = 0.3, IScheduler scheduler = null)
        {
            scheduler = scheduler ?? RxApp.MainThreadScheduler;

            this._inner = this._inner ?? new List<T>();

            this._changing = new Subject<NotifyCollectionChangedEventArgs>();
            this._changing.Where(_ => this.areChangeNotificationsEnabled()).Subscribe(this.raiseCollectionChanging);

            this._changed = new Subject<NotifyCollectionChangedEventArgs>();
            this._changed.Where(_ => this.areChangeNotificationsEnabled()).Subscribe(this.raiseCollectionChanged);

            this.ResetChangeThreshold = resetChangeThreshold;

            this._beforeItemsAdded = new Lazy<Subject<T>>(() => new Subject<T>());
            this._itemsAdded = new Lazy<Subject<T>>(() => new Subject<T>());
            this._beforeItemsRemoved = new Lazy<Subject<T>>(() => new Subject<T>());
            this._itemsRemoved = new Lazy<Subject<T>>(() => new Subject<T>());
            this._itemChanging = new Lazy<ISubject<IReactivePropertyChangedEventArgs<T>>>(() => new ScheduledSubject<IReactivePropertyChangedEventArgs<T>>(scheduler));
            this._itemChanged = new Lazy<ISubject<IReactivePropertyChangedEventArgs<T>>>(() => new ScheduledSubject<IReactivePropertyChangedEventArgs<T>>(scheduler));
            this._beforeItemsMoved = new Lazy<Subject<IMoveInfo<T>>>(() => new Subject<IMoveInfo<T>>());
            this._itemsMoved = new Lazy<Subject<IMoveInfo<T>>>(() => new Subject<IMoveInfo<T>>());

            // NB: We have to do this instead of initializing _inner so that Collection<T>'s
            // accounting is correct
            foreach (var item in initialContents ?? Enumerable.Empty<T>()) { Add(item); }

            // NB: ObservableCollection has a Secret Handshake with WPF where they fire an INPC
            // notification with the token "Item[]". Emulate it here
            this.CountChanging.Where(_ => this.areChangeNotificationsEnabled()).Subscribe(_ => this.RaisePropertyChanging("Count"));

            this.CountChanged.Where(_ => this.areChangeNotificationsEnabled()).Subscribe(_ => this.RaisePropertyChanged("Count"));

            this.IsEmptyChanged.Where(_ => this.areChangeNotificationsEnabled()).Subscribe(_ => this.RaisePropertyChanged("IsEmpty"));

            this.Changing.Where(_ => this.areChangeNotificationsEnabled()).Subscribe(_ => this.RaisePropertyChanging("Item[]"));

            this.Changed.Where(_ => this.areChangeNotificationsEnabled()).Subscribe(_ => this.RaisePropertyChanged("Item[]"));
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get { return this.Count == 0; }
        }

        /*
         * Collection<T> core methods
         */

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        protected void InsertItem(int index, T item)
        {
            if (!this.areChangeNotificationsEnabled()) {
                this._inner.Insert(index, item);

                if (this.ChangeTrackingEnabled) addItemToPropertyTracking(item);
                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);

            this._changing.OnNext(ea);
            if (this._beforeItemsAdded.IsValueCreated) this._beforeItemsAdded.Value.OnNext(item);

            this._inner.Insert(index, item);

            this._changed.OnNext(ea);
            if (this._itemsAdded.IsValueCreated) this._itemsAdded.Value.OnNext(item);

            if (this.ChangeTrackingEnabled) addItemToPropertyTracking(item);
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="index">The index.</param>
        protected void RemoveItem(int index)
        {
            var item = this._inner[index];

            if (!this.areChangeNotificationsEnabled()) {
                this._inner.RemoveAt(index);

                if (this.ChangeTrackingEnabled) removeItemFromPropertyTracking(item);
                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);

            this._changing.OnNext(ea);
            if (this._beforeItemsRemoved.IsValueCreated) this._beforeItemsRemoved.Value.OnNext(item);

            this._inner.RemoveAt(index);

            this._changed.OnNext(ea);
            if (this._itemsRemoved.IsValueCreated) this._itemsRemoved.Value.OnNext(item);
            if (this.ChangeTrackingEnabled) removeItemFromPropertyTracking(item);
        }

        /// <summary>
        /// Moves the item.
        /// </summary>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        protected void MoveItem(int oldIndex, int newIndex)
        {
            var item = this._inner[oldIndex];

            if (!this.areChangeNotificationsEnabled()) {
                this._inner.RemoveAt(oldIndex);
                this._inner.Insert(newIndex, item);

                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
            var mi = new MoveInfo<T>(new[] { item }, oldIndex, newIndex);

            this._changing.OnNext(ea);
            if (this._beforeItemsMoved.IsValueCreated) this._beforeItemsMoved.Value.OnNext(mi);

            this._inner.RemoveAt(oldIndex);
            this._inner.Insert(newIndex, item);

            this._changed.OnNext(ea);
            if (this._itemsMoved.IsValueCreated) this._itemsMoved.Value.OnNext(mi);
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        protected void SetItem(int index, T item)
        {
            if (!this.areChangeNotificationsEnabled()) {
                if (this.ChangeTrackingEnabled) {
                    removeItemFromPropertyTracking(this._inner[index]);
                    addItemToPropertyTracking(item);
                }
                this._inner[index] = item;

                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, this._inner[index], index);

            this._changing.OnNext(ea);

            if (this.ChangeTrackingEnabled) {
                removeItemFromPropertyTracking(this._inner[index]);
                addItemToPropertyTracking(item);
            }

            this._inner[index] = item;
            this._changed.OnNext(ea);
        }

        /// <summary>
        /// Clears the items.
        /// </summary>
        protected void ClearItems()
        {
            if (!this.areChangeNotificationsEnabled()) {
                this._inner.Clear();

                if (this.ChangeTrackingEnabled) clearAllPropertyChangeWatchers();
                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

            this._changing.OnNext(ea);
            this._inner.Clear();
            this._changed.OnNext(ea);

            if (this.ChangeTrackingEnabled) clearAllPropertyChangeWatchers();
        }

        /*
         * List<T> methods we can make faster by possibly sending ShouldReset
         * notifications instead of thrashing the UI by readding items
         * one at a time
         */

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <exception cref="System.ArgumentNullException">collection</exception>
        public virtual void AddRange(IEnumerable<T> collection)
        {
            if (collection == null) {
                throw new ArgumentNullException(nameof(collection));
            }

            // we need list to implement at least IEnumerable<T> and IList because
            // NotifyCollectionChangedEventArgs expects an IList
            var list = collection as List<T> ?? collection.ToList();
            var disp = isLengthAboveResetThreshold(list.Count)
                ? SuppressChangeNotifications() : Disposable.Empty;

            using (disp) {

                // reset notification
                if (!this.areChangeNotificationsEnabled()) {
                    this._inner.AddRange(list);

                    if (this.ChangeTrackingEnabled) {
                        foreach (var item in list) {
                            addItemToPropertyTracking(item);
                        }
                    }
                }

                // range notification
                else if (RxApp.SupportsRangeNotifications) {
                    var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, this._inner.Count/*we are appending a range*/);

                    this._changing.OnNext(ea);

                    if (this._beforeItemsAdded.IsValueCreated) {
                        foreach (var item in list) {
                            this._beforeItemsAdded.Value.OnNext(item);
                        }
                    }

                    this._inner.AddRange(list);

                    this._changed.OnNext(ea);
                    if (this._itemsAdded.IsValueCreated) {
                        foreach (var item in list) {
                            this._itemsAdded.Value.OnNext(item);
                        }
                    }

                    if (this.ChangeTrackingEnabled) {
                        foreach (var item in list) {
                            addItemToPropertyTracking(item);
                        }
                    }
                } else {

                    // per item notification
                    foreach (var item in list) {
                        this.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Inserts the range.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="collection">The collection.</param>
        /// <exception cref="System.ArgumentNullException">collection</exception>
        public virtual void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection == null) {
                throw new ArgumentNullException(nameof(collection));
            }

            // we need list to implement at least IEnumerable<T> and IList because
            // NotifyCollectionChangedEventArgs expects an IList
            var list = collection as List<T> ?? collection.ToList();
            var disp = isLengthAboveResetThreshold(list.Count) ?
                SuppressChangeNotifications() : Disposable.Empty;

            using (disp) {

                // reset notification
                if (!this.areChangeNotificationsEnabled()) {
                    this._inner.InsertRange(index, list);

                    if (this.ChangeTrackingEnabled) {
                        foreach (var item in list) {
                            addItemToPropertyTracking(item);
                        }
                    }
                }

                // range notification
                else if (RxApp.SupportsRangeNotifications) {
                    var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index);

                    this._changing.OnNext(ea);
                    if (this._beforeItemsAdded.IsValueCreated) {
                        foreach (var item in list) {
                            this._beforeItemsAdded.Value.OnNext(item);
                        }
                    }

                    this._inner.InsertRange(index, list);

                    this._changed.OnNext(ea);
                    if (this._itemsAdded.IsValueCreated) {
                        foreach (var item in list) {
                            this._itemsAdded.Value.OnNext(item);
                        }
                    }

                    if (this.ChangeTrackingEnabled) {
                        foreach (var item in list) {
                            addItemToPropertyTracking(item);
                        }
                    }
                } else {

                    // per item notification
                    foreach (var item in list) {
                        this.Insert(index++, item);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public virtual void RemoveRange(int index, int count)
        {
            var disp = isLengthAboveResetThreshold(count) ?
                SuppressChangeNotifications() : Disposable.Empty;

            using (disp) {
                var items = new List<T>(count);

                foreach (var i in Enumerable.Range(index, count)) {
                    items.Add(this._inner[i]);
                }

                // reset notification
                if (!this.areChangeNotificationsEnabled()) {
                    this._inner.RemoveRange(index, count);

                    if (this.ChangeTrackingEnabled) {
                        foreach (var item in items) {
                            removeItemFromPropertyTracking(item);
                        }
                    }
                }

                // range notification
                else if (RxApp.SupportsRangeNotifications) {
                    var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, index);

                    this._changing.OnNext(ea);
                    if (this._beforeItemsRemoved.IsValueCreated) {
                        foreach (var item in items) {
                            this._beforeItemsRemoved.Value.OnNext(item);
                        }
                    }

                    this._inner.RemoveRange(index, count);
                    this._changed.OnNext(ea);

                    if (this.ChangeTrackingEnabled) {
                        foreach (var item in items) {
                            removeItemFromPropertyTracking(item);
                        }
                    }

                    if (this._itemsRemoved.IsValueCreated) {
                        foreach (var item in items) {
                            this._itemsRemoved.Value.OnNext(item);
                        }
                    }
                } else {

                    // per item notification
                    foreach (var item in items) {
                        this.Remove(item);
                    }
                }
            }
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <exception cref="System.ArgumentNullException">items</exception>
        public virtual void RemoveAll(IEnumerable<T> items)
        {
            if (items == null) {
                throw new ArgumentNullException(nameof(items));
            }

            var disp = isLengthAboveResetThreshold(items.Count()) ?
                SuppressChangeNotifications() : Disposable.Empty;

            using (disp) {

                // NB: If we don't do this, we'll break Collection<T>'s accounting of the length
                foreach (var v in items) {
                    Remove(v);
                }
            }
        }

        /// <summary>
        /// Sorts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <param name="comparer">The comparer.</param>
        public virtual void Sort(int index, int count, IComparer<T> comparer)
        {
            this._inner.Sort(index, count, comparer);
            Reset();
        }

        /// <summary>
        /// Sorts the specified comparison.
        /// </summary>
        /// <param name="comparison">The comparison.</param>
        public virtual void Sort(Comparison<T> comparison)
        {
            this._inner.Sort(comparison);
            Reset();
        }

        /// <summary>
        /// Sorts the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public virtual void Sort(IComparer<T> comparer = null)
        {
            this._inner.Sort(comparer ?? Comparer<T>.Default);
            Reset();
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public virtual void Reset()
        {
            publishResetNotification();
        }

        /// <summary>
        /// Publishes the reset notification.
        /// </summary>
        protected virtual void publishResetNotification()
        {
            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            this._changing.OnNext(ea);
            this._changed.OnNext(ea);
        }

        private bool isLengthAboveResetThreshold(int toChangeLength)
        {
            return (double)toChangeLength / this._inner.Count > this.ResetChangeThreshold &&
                toChangeLength > 10;
        }

        /*
         * IReactiveCollection<T>
         */

        /// <summary>
        /// Enables the ItemChanging and ItemChanged properties; when this is enabled, whenever a
        /// property on any object implementing IReactiveNotifyPropertyChanged changes, the change
        /// will be rebroadcast through ItemChanging/ItemChanged.
        /// </summary>
        public bool ChangeTrackingEnabled
        {
            get
            {
                return this._propertyChangeWatchers != null;
            }

            set
            {
                if (this._propertyChangeWatchers != null && value) return;
                if (this._propertyChangeWatchers == null && !value) return;

                if (value) {
                    this._propertyChangeWatchers = new Dictionary<object, RefcountDisposeWrapper>();
                    foreach (var item in this._inner) { addItemToPropertyTracking(item); }
                } else {
                    clearAllPropertyChangeWatchers();
                    this._propertyChangeWatchers = null;
                }
            }
        }

        /// <summary>
        /// Suppresses the change notifications.
        /// </summary>
        /// <returns></returns>
        public IDisposable SuppressChangeNotifications()
        {
            Interlocked.Increment(ref this._resetNotificationCount);

            if (!_hasWhinedAboutNoResetSub && this._resetSubCount == 0) {
                LogHost.Default.Warn("SuppressChangeNotifications was called (perhaps via AddRange), yet you do not");
                LogHost.Default.Warn("have a subscription to ShouldReset. This probably isn't what you want, as ItemsAdded");
                LogHost.Default.Warn("and friends will appear to 'miss' items");
                _hasWhinedAboutNoResetSub = true;
            }

            return new CompositeDisposable(this.suppressChangeNotifications(), Disposable.Create(() => {
                if (Interlocked.Decrement(ref this._resetNotificationCount) == 0) {
                    publishResetNotification();
                }
            }));
        }

        /// <summary>
        /// Fires before an item is going to be added to the collection.
        /// </summary>
        public IObservable<T> BeforeItemsAdded { get { return this._beforeItemsAdded.Value; } }

        /// <summary>
        /// Fires when items are added to the collection, once per item added. Functions that add
        /// multiple items such AddRange should fire this multiple times. The object provided is the
        /// item that was added.
        /// </summary>
        public IObservable<T> ItemsAdded { get { return this._itemsAdded.Value; } }

        /// <summary>
        /// Fires before an item will be removed from a collection, providing the item that will be removed.
        /// </summary>
        public IObservable<T> BeforeItemsRemoved { get { return this._beforeItemsRemoved.Value; } }

        /// <summary>
        /// Fires once an item has been removed from a collection, providing the item that was removed.
        /// </summary>
        public IObservable<T> ItemsRemoved { get { return this._itemsRemoved.Value; } }

        /// <summary>
        /// Fires before an items moves from one position in the collection to another, providing the
        /// item(s) to be moved as well as source and destination indices.
        /// </summary>
        public IObservable<IMoveInfo<T>> BeforeItemsMoved { get { return this._beforeItemsMoved.Value; } }

        /// <summary>
        /// Fires once one or more items moves from one position in the collection to another,
        /// providing the item(s) that was moved as well as source and destination indices.
        /// </summary>
        public IObservable<IMoveInfo<T>> ItemsMoved { get { return this._itemsMoved.Value; } }

        /// <summary>
        /// Provides Item Changing notifications for any item in collection that implements
        /// IReactiveNotifyPropertyChanged. This is only enabled when ChangeTrackingEnabled is set to True.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<T>> ItemChanging { get { return this._itemChanging.Value; } }

        /// <summary>
        /// Provides Item Changed notifications for any item in collection that implements
        /// IReactiveNotifyPropertyChanged. This is only enabled when ChangeTrackingEnabled is set to True.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<T>> ItemChanged { get { return this._itemChanged.Value; } }

        /// <summary>
        /// Fires when the collection count changes, regardless of reason
        /// </summary>
        public IObservable<int> CountChanging
        {
            get { return this._changing.Select(_ => this._inner.Count).DistinctUntilChanged(); }
        }

        /// <summary>
        /// Fires when the collection count changes, regardless of reason
        /// </summary>
        public IObservable<int> CountChanged
        {
            get { return this._changed.Select(_ => this._inner.Count).DistinctUntilChanged(); }
        }

        /// <summary>
        /// Gets the is empty changed.
        /// </summary>
        /// <value>The is empty changed.</value>
        public IObservable<bool> IsEmptyChanged
        {
            get { return this._changed.Select(_ => this._inner.Count == 0).DistinctUntilChanged(); }
        }

        /// <summary>
        /// This Observable is equivalent to the NotifyCollectionChanged event, but fires before the
        /// collection is changed
        /// </summary>
        public IObservable<NotifyCollectionChangedEventArgs> Changing
        {
            get { return this._changing; }
        }

        /// <summary>
        /// This Observable is equivalent to the NotifyCollectionChanged event, and fires after the
        /// collection is changed
        /// </summary>
        public IObservable<NotifyCollectionChangedEventArgs> Changed
        {
            get { return this._changed; }
        }

        /// <summary>
        /// This Observable is fired when a ShouldReset fires on the collection. This means that you
        /// should forget your previous knowledge of the state of the collection and reread it. This
        /// does *not* mean Clear, and if you interpret it as such, you are Doing It Wrong.
        /// </summary>
        public IObservable<Unit> ShouldReset
        {
            get
            {
                return refcountSubscribers(this._changed.SelectMany(x =>
                    x.Action != NotifyCollectionChangedAction.Reset ?
                        Observable<Unit>.Empty :
                        Observables.Unit), x => this._resetSubCount += x);
            }
        }

        /*
         * Property Change Tracking
         */

        private void addItemToPropertyTracking(T toTrack)
        {
            if (this._propertyChangeWatchers.ContainsKey(toTrack)) {
                this._propertyChangeWatchers[toTrack].AddRef();
                return;
            }

            var changing = Observable<IReactivePropertyChangedEventArgs<T>>.Never;
            var changed = Observable<IReactivePropertyChangedEventArgs<T>>.Never;
            var ro = toTrack as IReactiveObject;
            if (ro != null) {
                changing = ro.getChangingObservable().Select(i => new ReactivePropertyChangingEventArgs<T>(toTrack, i.PropertyName));
                changed = ro.getChangedObservable().Select(i => new ReactivePropertyChangedEventArgs<T>(toTrack, i.PropertyName));
                goto isSetup;
            }

            var inpc = toTrack as INotifyPropertyChanged;
            if (inpc != null) {
                changed = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(x => inpc.PropertyChanged += x, x => inpc.PropertyChanged -= x)
                    .Select(x => new ReactivePropertyChangedEventArgs<T>(toTrack, x.EventArgs.PropertyName));
                goto isSetup;
            }

            this.Log().Warn("Property change notifications are enabled and type {0} isn't INotifyPropertyChanged or IReactiveObject", typeof(T));

            isSetup:
            var toDispose = new[] {
                changing.Where(_ => this.areChangeNotificationsEnabled()).Subscribe(this._itemChanging.Value.OnNext),
                changed.Where(_ => this.areChangeNotificationsEnabled()).Subscribe(this._itemChanged.Value.OnNext),
            };

            this._propertyChangeWatchers.Add(toTrack,
                new RefcountDisposeWrapper(Disposable.Create(() => {
                    toDispose[0].Dispose(); toDispose[1].Dispose();
                    this._propertyChangeWatchers.Remove(toTrack);
                })));
        }

        private void removeItemFromPropertyTracking(T toUntrack)
        {
            this._propertyChangeWatchers[toUntrack].Release();
        }

        private void clearAllPropertyChangeWatchers()
        {
            while (this._propertyChangeWatchers.Count > 0) this._propertyChangeWatchers.Values.First().Release();
        }

        private static IObservable<TObs> refcountSubscribers<TObs>(IObservable<TObs> input, Action<int> block)
        {
            return Observable.Create<TObs>(subj => {
                block(1);

                return new CompositeDisposable(
                    input.Subscribe(subj),
                    Disposable.Create(() => block(-1)));
            });
        }

        /// <summary>
        /// The BinarySearch methods aren't technically on IList'T', they're implemented straight on
        /// List'T' but by proxying this call we can make use of the nice built in methods that
        /// operate on the internal array of the inner list instead of jumping around proxying
        /// through the index.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int BinarySearch(T item)
        {
            return this._inner.BinarySearch(item);
        }

        /// <summary>
        /// Binaries the search.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return this._inner.BinarySearch(item, comparer);
        }

        /// <summary>
        /// Binaries the search.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <param name="item">The item.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return this._inner.BinarySearch(index, count, item, comparer);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate
        /// through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public virtual void Add(T item)
        {
            InsertItem(this._inner.Count, item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public virtual void Clear()
        {
            ClearItems();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains
        /// a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see
        /// cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            return this._inner.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an
        /// <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements
        /// copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see
        /// cref="T:System.Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this._inner.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see
        /// cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also
        /// returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public virtual bool Remove(T item)
        {
            int index = this._inner.IndexOf(item);
            if (index < 0) return false;

            RemoveItem(index);
            return true;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public int Count { get { return this._inner.Count; } }

        /// <summary>
        /// Gets a value indicating whether the <see
        /// cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        public virtual bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            return this._inner.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the
        /// specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which <paramref name="item"/> should be inserted.
        /// </param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public virtual void Insert(int index, T item)
        {
            InsertItem(index, item);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public virtual void RemoveAt(int index)
        {
            RemoveItem(index);
        }

#if !SILVERLIGHT

        /// <summary>
        /// Moves the specified old index.
        /// </summary>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        public virtual void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

#endif

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <value>The item.</value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public virtual T this[int index]
        {
            get { return this._inner[index]; }
            set { SetItem(index, value); }
        }

        int IList.Add(object value)
        {
            Add((T)value);
            return this.Count - 1;
        }

        bool IList.Contains(object value)
        {
            return IsCompatibleObject(value) && Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return IsCompatibleObject(value) ? IndexOf((T)value) : -1;
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        bool IList.IsFixedSize { get { return false; } }

        void IList.Remove(object value)
        {
            if (IsCompatibleObject(value)) Remove((T)value);
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (T)value; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)this._inner).CopyTo(array, index);
        }

        bool ICollection.IsSynchronized { get { return false; } }

        object ICollection.SyncRoot { get { return this; } }

        private static bool IsCompatibleObject(object value)
        {
            return ((value is T) || ((value == null) && (default(T) == null)));
        }
    }

    /// <summary>
    /// interface for Move Info
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMoveInfo<out T>
    {
        /// <summary>
        /// Gets the moved items.
        /// </summary>
        /// <value>The moved items.</value>
        IEnumerable<T> MovedItems { get; }

        /// <summary>
        /// Gets from.
        /// </summary>
        /// <value>From.</value>
        int From { get; }

        /// <summary>
        /// Gets to.
        /// </summary>
        /// <value>To.</value>
        int To { get; }
    }

    internal class MoveInfo<T> : IMoveInfo<T>
    {
        public IEnumerable<T> MovedItems { get; protected set; }

        public int From { get; protected set; }

        public int To { get; protected set; }

        public MoveInfo(IEnumerable<T> movedItems, int from, int to)
        {
            this.MovedItems = movedItems;
            this.From = from;
            this.To = to;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :