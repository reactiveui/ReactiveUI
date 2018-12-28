// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

#pragma warning disable SA1600 // Elements should be documented -- not used for legacy
#pragma warning disable SA1201 // Ordering -- not used for legacy
#pragma warning disable SA1202 // Ordering -- not used for legacy
#pragma warning disable SA1124 // Do not use regions -- not used for legacy
#pragma warning disable RCS1165 // Unconstrained null check -- not used for legacy
#pragma warning disable CA1001 // Undisposed type -- not used for legacy

namespace ReactiveUI.Legacy
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    public class ReactiveList<T> : IReactiveList<T>, IReadOnlyReactiveList<T>, IList
    {
#if NET_461
        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanging;

        protected virtual void RaiseCollectionChanging(NotifyCollectionChangedEventArgs args)
        {
            // WPF doesn't seem to care much about Range notifications in this case,
            // so we'll just pass those right through
            CollectionChanging?.Invoke(this, args);
        }

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            // WPF throws "Range actions are not supported" under
            // certain circumstances, so we catch those here first
            CollectionChanged?.Invoke(this, ReplaceIfUnsupported(args));
        }

        private static NotifyCollectionChangedEventArgs ReplaceIfUnsupported(NotifyCollectionChangedEventArgs args)
        {
            // see System.Windows.Data.ListCollectionView.ValidateCollectionChangedEventArgs
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add when args.NewItems.Count != 1:
                case NotifyCollectionChangedAction.Remove when args.OldItems.Count != 1:
                case NotifyCollectionChangedAction.Replace when args.NewItems.Count != 1 || args.OldItems.Count != 1:
                case NotifyCollectionChangedAction.Move when args.NewItems.Count != 1:
                    return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                default:
                    return args;
            }
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }
#else
        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanging
        {
            add => CollectionChangingEventManager.AddHandler(this, value);
            remove => CollectionChangingEventManager.RemoveHandler(this, value);
        }

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => CollectionChangedEventManager.AddHandler(this, value);
            remove => CollectionChangedEventManager.RemoveHandler(this, value);
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add => PropertyChangingEventManager.AddHandler(this, value);
            remove => PropertyChangingEventManager.RemoveHandler(this, value);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => PropertyChangedEventManager.AddHandler(this, value);
            remove => PropertyChangedEventManager.RemoveHandler(this, value);
        }

        protected virtual void RaiseCollectionChanging(NotifyCollectionChangedEventArgs args)
        {
            CollectionChangingEventManager.DeliverEvent(this, args);
        }

        protected virtual void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChangedEventManager.DeliverEvent(this, args);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }
#endif

        private static bool _hasWhinedAboutNoResetSub;
        [IgnoreDataMember]
        private Subject<NotifyCollectionChangedEventArgs> _changing;
        [IgnoreDataMember]
        private Subject<NotifyCollectionChangedEventArgs> _changed;

        [DataMember]
        private List<T> _inner;

        [IgnoreDataMember]
        private Lazy<Subject<T>> _beforeItemsAdded;
        [IgnoreDataMember]
        private Lazy<Subject<T>> _itemsAdded;
        [IgnoreDataMember]
        private Lazy<Subject<T>> _beforeItemsRemoved;
        [IgnoreDataMember]
        private Lazy<Subject<T>> _itemsRemoved;
        [IgnoreDataMember]
        private Lazy<ISubject<IReactivePropertyChangedEventArgs<T>>> _itemChanging;
        [IgnoreDataMember]
        private Lazy<ISubject<IReactivePropertyChangedEventArgs<T>>> _itemChanged;
        [IgnoreDataMember]
        private Lazy<Subject<IMoveInfo<T>>> _beforeItemsMoved;
        [IgnoreDataMember]
        private Lazy<Subject<IMoveInfo<T>>> _itemsMoved;

        [IgnoreDataMember]
        private Dictionary<object, RefcountDisposeWrapper> _propertyChangeWatchers;

        [IgnoreDataMember]
        private int _resetSubCount;
        [IgnoreDataMember]
        private int _resetNotificationCount;

        [IgnoreDataMember]
        public double ResetChangeThreshold { get; set; }

        // NB: This exists so the serializer doesn't whine
        //
        // 2nd NB: VB.NET doesn't deal well with default parameters, create
        // some overloads so people can continue to make bad life choices instead
        // of using C#
        public ReactiveList()
        {
            SetupRx();
        }

        public ReactiveList(IEnumerable<T> initialContents)
        {
            SetupRx(initialContents);
        }

        public ReactiveList(IEnumerable<T> initialContents = null, double resetChangeThreshold = 0.3, IScheduler scheduler = null)
        {
            SetupRx(initialContents, resetChangeThreshold, scheduler);
        }

        [OnDeserialized]
        private void SetupRx(StreamingContext context)
        {
            SetupRx();
        }

        private void SetupRx(IEnumerable<T> initialContents = null, double resetChangeThreshold = 0.3, IScheduler scheduler = null)
        {
            scheduler = scheduler ?? RxApp.MainThreadScheduler;

            _inner = _inner ?? new List<T>();

            _changing = new Subject<NotifyCollectionChangedEventArgs>();
            _changing.Where(_ => this.AreChangeNotificationsEnabled()).Subscribe(RaiseCollectionChanging);

            _changed = new Subject<NotifyCollectionChangedEventArgs>();
            _changed.Where(_ => this.AreChangeNotificationsEnabled()).Subscribe(RaiseCollectionChanged);

            ResetChangeThreshold = resetChangeThreshold;

            _beforeItemsAdded = new Lazy<Subject<T>>(() => new Subject<T>());
            _itemsAdded = new Lazy<Subject<T>>(() => new Subject<T>());
            _beforeItemsRemoved = new Lazy<Subject<T>>(() => new Subject<T>());
            _itemsRemoved = new Lazy<Subject<T>>(() => new Subject<T>());
            _itemChanging = new Lazy<ISubject<IReactivePropertyChangedEventArgs<T>>>(() => new ScheduledSubject<IReactivePropertyChangedEventArgs<T>>(scheduler));
            _itemChanged = new Lazy<ISubject<IReactivePropertyChangedEventArgs<T>>>(() => new ScheduledSubject<IReactivePropertyChangedEventArgs<T>>(scheduler));
            _beforeItemsMoved = new Lazy<Subject<IMoveInfo<T>>>(() => new Subject<IMoveInfo<T>>());
            _itemsMoved = new Lazy<Subject<IMoveInfo<T>>>(() => new Subject<IMoveInfo<T>>());

            // NB: We have to do this instead of initializing _inner so that
            // Collection<T>'s accounting is correct
            foreach (var item in initialContents ?? Enumerable.Empty<T>())
            {
                Add(item);
            }

            // NB: ObservableCollection has a Secret Handshake with WPF where
            // they fire an INPC notification with the token "Item[]". Emulate
            // it here
            CountChanging.Where(_ => this.AreChangeNotificationsEnabled()).Subscribe(_ => this.RaisePropertyChanging(nameof(Count)));

            CountChanged.Where(_ => this.AreChangeNotificationsEnabled()).Subscribe(_ => this.RaisePropertyChanged(nameof(Count)));

            IsEmptyChanged.Where(_ => this.AreChangeNotificationsEnabled()).Subscribe(_ => this.RaisePropertyChanged(nameof(IsEmpty)));

            Changing.Where(_ => this.AreChangeNotificationsEnabled()).Subscribe(_ => this.RaisePropertyChanging("Item[]"));

            Changed.Where(_ => this.AreChangeNotificationsEnabled()).Subscribe(_ => this.RaisePropertyChanged("Item[]"));
        }

        /// <inheritdoc/>
        public bool IsEmpty => Count == 0;

        /*
         * Collection<T> core methods
         */

        protected void InsertItem(int index, T item)
        {
            if (!this.AreChangeNotificationsEnabled())
            {
                _inner.Insert(index, item);

                if (ChangeTrackingEnabled)
                {
                    AddItemToPropertyTracking(item);
                }

                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);

            _changing.OnNext(ea);
            if (_beforeItemsAdded.IsValueCreated)
            {
                _beforeItemsAdded.Value.OnNext(item);
            }

            _inner.Insert(index, item);

            _changed.OnNext(ea);
            if (_itemsAdded.IsValueCreated)
            {
                _itemsAdded.Value.OnNext(item);
            }

            if (ChangeTrackingEnabled)
            {
                AddItemToPropertyTracking(item);
            }
        }

        protected void RemoveItem(int index)
        {
            var item = _inner[index];

            if (!this.AreChangeNotificationsEnabled())
            {
                _inner.RemoveAt(index);

                if (ChangeTrackingEnabled)
                {
                    RemoveItemFromPropertyTracking(item);
                }

                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);

            _changing.OnNext(ea);
            if (_beforeItemsRemoved.IsValueCreated)
            {
                _beforeItemsRemoved.Value.OnNext(item);
            }

            _inner.RemoveAt(index);

            _changed.OnNext(ea);
            if (_itemsRemoved.IsValueCreated)
            {
                _itemsRemoved.Value.OnNext(item);
            }

            if (ChangeTrackingEnabled)
            {
                RemoveItemFromPropertyTracking(item);
            }
        }

        protected void MoveItem(int oldIndex, int newIndex)
        {
            var item = _inner[oldIndex];

            if (!this.AreChangeNotificationsEnabled())
            {
                _inner.RemoveAt(oldIndex);
                _inner.Insert(newIndex, item);

                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
            var mi = new MoveInfo<T>(new[] { item }, oldIndex, newIndex);

            _changing.OnNext(ea);
            if (_beforeItemsMoved.IsValueCreated)
            {
                _beforeItemsMoved.Value.OnNext(mi);
            }

            _inner.RemoveAt(oldIndex);
            _inner.Insert(newIndex, item);

            _changed.OnNext(ea);
            if (_itemsMoved.IsValueCreated)
            {
                _itemsMoved.Value.OnNext(mi);
            }
        }

        protected void SetItem(int index, T item)
        {
            if (!this.AreChangeNotificationsEnabled())
            {
                if (ChangeTrackingEnabled)
                {
                    RemoveItemFromPropertyTracking(_inner[index]);
                    AddItemToPropertyTracking(item);
                }

                _inner[index] = item;

                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, _inner[index], index);

            _changing.OnNext(ea);

            if (ChangeTrackingEnabled)
            {
                RemoveItemFromPropertyTracking(_inner[index]);
                AddItemToPropertyTracking(item);
            }

            _inner[index] = item;
            _changed.OnNext(ea);
        }

        protected void ClearItems()
        {
            if (!this.AreChangeNotificationsEnabled())
            {
                _inner.Clear();

                if (ChangeTrackingEnabled)
                {
                    ClearAllPropertyChangeWatchers();
                }

                return;
            }

            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

            _changing.OnNext(ea);
            _inner.Clear();
            _changed.OnNext(ea);

            if (ChangeTrackingEnabled)
            {
                ClearAllPropertyChangeWatchers();
            }
        }

        /*
         * List<T> methods we can make faster by possibly sending ShouldReset
         * notifications instead of thrashing the UI by readding items
         * one at a time
         */

        /// <inheritdoc/>
        public virtual void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            // we need list to implement at least IEnumerable<T> and IList
            // because NotifyCollectionChangedEventArgs expects an IList
            var list = collection as List<T> ?? collection.ToList();
            var disp = IsLengthAboveResetThreshold(list.Count)
                ? SuppressChangeNotifications() : Disposable.Empty;

            using (disp)
            {
                // reset notification
                if (!this.AreChangeNotificationsEnabled())
                {
                    _inner.AddRange(list);

                    if (ChangeTrackingEnabled)
                    {
                        foreach (var item in list)
                        {
                            AddItemToPropertyTracking(item);
                        }
                    }
                }

                // range notification
                else if (RxApp.SupportsRangeNotifications)
                {
                    var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, _inner.Count/*we are appending a range*/);

                    _changing.OnNext(ea);

                    if (_beforeItemsAdded.IsValueCreated)
                    {
                        foreach (var item in list)
                        {
                            _beforeItemsAdded.Value.OnNext(item);
                        }
                    }

                    _inner.AddRange(list);

                    _changed.OnNext(ea);
                    if (_itemsAdded.IsValueCreated)
                    {
                        foreach (var item in list)
                        {
                            _itemsAdded.Value.OnNext(item);
                        }
                    }

                    if (ChangeTrackingEnabled)
                    {
                        foreach (var item in list)
                        {
                            AddItemToPropertyTracking(item);
                        }
                    }
                }
                else
                {
                    // per item notification
                    foreach (var item in list)
                    {
                        Add(item);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public virtual void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            // we need list to implement at least IEnumerable<T> and IList
            // because NotifyCollectionChangedEventArgs expects an IList
            var list = collection as List<T> ?? collection.ToList();
            var disp = IsLengthAboveResetThreshold(list.Count) ?
                SuppressChangeNotifications() : Disposable.Empty;

            using (disp)
            {
                // reset notification
                if (!this.AreChangeNotificationsEnabled())
                {
                    _inner.InsertRange(index, list);

                    if (ChangeTrackingEnabled)
                    {
                        foreach (var item in list)
                        {
                            AddItemToPropertyTracking(item);
                        }
                    }
                }

                // range notification
                else if (RxApp.SupportsRangeNotifications)
                {
                    var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index);

                    _changing.OnNext(ea);
                    if (_beforeItemsAdded.IsValueCreated)
                    {
                        foreach (var item in list)
                        {
                            _beforeItemsAdded.Value.OnNext(item);
                        }
                    }

                    _inner.InsertRange(index, list);

                    _changed.OnNext(ea);
                    if (_itemsAdded.IsValueCreated)
                    {
                        foreach (var item in list)
                        {
                            _itemsAdded.Value.OnNext(item);
                        }
                    }

                    if (ChangeTrackingEnabled)
                    {
                        foreach (var item in list)
                        {
                            AddItemToPropertyTracking(item);
                        }
                    }
                }
                else
                {
                    // per item notification
                    foreach (var item in list)
                    {
                        Insert(index++, item);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public virtual void RemoveRange(int index, int count)
        {
            var disp = IsLengthAboveResetThreshold(count) ?
                SuppressChangeNotifications() : Disposable.Empty;

            using (disp)
            {
                var items = new List<T>(count);

                foreach (var i in Enumerable.Range(index, count))
                {
                    items.Add(_inner[i]);
                }

                // reset notification
                if (!this.AreChangeNotificationsEnabled())
                {
                    _inner.RemoveRange(index, count);

                    if (ChangeTrackingEnabled)
                    {
                        foreach (var item in items)
                        {
                            RemoveItemFromPropertyTracking(item);
                        }
                    }
                }

                // range notification
                else if (RxApp.SupportsRangeNotifications)
                {
                    var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, index);

                    _changing.OnNext(ea);
                    if (_beforeItemsRemoved.IsValueCreated)
                    {
                        foreach (var item in items)
                        {
                            _beforeItemsRemoved.Value.OnNext(item);
                        }
                    }

                    _inner.RemoveRange(index, count);
                    _changed.OnNext(ea);

                    if (ChangeTrackingEnabled)
                    {
                        foreach (var item in items)
                        {
                            RemoveItemFromPropertyTracking(item);
                        }
                    }

                    if (_itemsRemoved.IsValueCreated)
                    {
                        foreach (var item in items)
                        {
                            _itemsRemoved.Value.OnNext(item);
                        }
                    }
                }
                else
                {
                    // per item notification
                    foreach (var item in items)
                    {
                        Remove(item);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public virtual void RemoveAll(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var disp = IsLengthAboveResetThreshold(items.Count()) ?
                SuppressChangeNotifications() : Disposable.Empty;

            using (disp)
            {
                // NB: If we don't do this, we'll break Collection<T>'s
                // accounting of the length
                foreach (var v in items)
                {
                    Remove(v);
                }
            }
        }

        /// <inheritdoc/>
        public virtual void Sort(int index, int count, IComparer<T> comparer)
        {
            _inner.Sort(index, count, comparer);
            Reset();
        }

        /// <inheritdoc/>
        public virtual void Sort(Comparison<T> comparison)
        {
            _inner.Sort(comparison);
            Reset();
        }

        /// <inheritdoc/>
        public virtual void Sort(IComparer<T> comparer = null)
        {
            _inner.Sort(comparer ?? Comparer<T>.Default);
            Reset();
        }

        /// <inheritdoc/>
        public virtual void Reset()
        {
            var ea = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            _changing.OnNext(ea);
            _changed.OnNext(ea);
        }

        private bool IsLengthAboveResetThreshold(int toChangeLength)
        {
            return (double)toChangeLength / _inner.Count > ResetChangeThreshold &&
                toChangeLength > 10;
        }

        /*
         * IReactiveCollection<T>
         */

        /// <inheritdoc/>
        public bool ChangeTrackingEnabled
        {
            get => _propertyChangeWatchers != null;
            set
            {
                if (_propertyChangeWatchers != null && value)
                {
                    return;
                }

                if (_propertyChangeWatchers == null && !value)
                {
                    return;
                }

                if (value)
                {
                    _propertyChangeWatchers = new Dictionary<object, RefcountDisposeWrapper>();
                    foreach (var item in _inner)
                    {
                        AddItemToPropertyTracking(item);
                    }
                }
                else
                {
                    ClearAllPropertyChangeWatchers();
                    _propertyChangeWatchers = null;
                }
            }
        }

        /// <inheritdoc/>
        public IDisposable SuppressChangeNotifications()
        {
            NotifyCollectionChangedEventArgs resetEventArgs = null;

            if (Interlocked.Increment(ref _resetNotificationCount) == 1)
            {
                // Changes were not suppressed before, publish a Changing reset notification.
                resetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                _changing.OnNext(resetEventArgs);
            }

            if (!_hasWhinedAboutNoResetSub && _resetSubCount == 0)
            {
                LogHost.Default.Warn("SuppressChangeNotifications was called (perhaps via AddRange), yet you do not");
                LogHost.Default.Warn("have a subscription to ShouldReset. This probably isn't what you want, as ItemsAdded");
                LogHost.Default.Warn("and friends will appear to 'miss' items");
                _hasWhinedAboutNoResetSub = true;
            }

            return new CompositeDisposable(IReactiveObjectExtensions.SuppressChangeNotifications(this), Disposable.Create(() =>
            {
                if (Interlocked.Decrement(ref _resetNotificationCount) == 0)
                {
                    if (resetEventArgs == null)
                    {
                        resetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    }

                    _changed.OnNext(resetEventArgs);
                }
            }));
        }

        /// <inheritdoc/>
        public IObservable<T> BeforeItemsAdded => _beforeItemsAdded.Value;

        /// <inheritdoc/>
        public IObservable<T> ItemsAdded => _itemsAdded.Value;

        /// <inheritdoc/>
        public IObservable<T> BeforeItemsRemoved => _beforeItemsRemoved.Value;

        /// <inheritdoc/>
        public IObservable<T> ItemsRemoved => _itemsRemoved.Value;

        /// <inheritdoc/>
        public IObservable<IMoveInfo<T>> BeforeItemsMoved => _beforeItemsMoved.Value;

        /// <inheritdoc/>
        public IObservable<IMoveInfo<T>> ItemsMoved => _itemsMoved.Value;

        /// <inheritdoc/>
        public IObservable<IReactivePropertyChangedEventArgs<T>> ItemChanging => _itemChanging.Value;

        /// <inheritdoc/>
        public IObservable<IReactivePropertyChangedEventArgs<T>> ItemChanged => _itemChanged.Value;

        /// <inheritdoc/>
        public IObservable<int> CountChanging
        {
            get { return _changing.Select(_ => _inner.Count).DistinctUntilChanged(); }
        }

        /// <inheritdoc/>
        public IObservable<int> CountChanged
        {
            get { return _changed.Select(_ => _inner.Count).DistinctUntilChanged(); }
        }

        /// <inheritdoc/>
        public IObservable<bool> IsEmptyChanged
        {
            get { return _changed.Select(_ => _inner.Count == 0).DistinctUntilChanged(); }
        }

        /// <inheritdoc/>
        public IObservable<NotifyCollectionChangedEventArgs> Changing => _changing;

        /// <inheritdoc/>
        public IObservable<NotifyCollectionChangedEventArgs> Changed => _changed;

        /// <inheritdoc/>
        public IObservable<Unit> ShouldReset
        {
            get
            {
                return RefcountSubscribers(
                    _changed.SelectMany(x =>
                    x.Action != NotifyCollectionChangedAction.Reset ?
                        Observable<Unit>.Empty :
                        Observables.Unit), x => _resetSubCount += x);
            }
        }

        /*
         * Property Change Tracking
         */

        private static IObservable<TObs> RefcountSubscribers<TObs>(IObservable<TObs> input, Action<int> block)
        {
            return Observable.Create<TObs>(subj =>
            {
                block(1);

                return new CompositeDisposable(
                                               input.Subscribe(subj),
                                               Disposable.Create(() => block(-1)));
            });
        }

        private void AddItemToPropertyTracking(T toTrack)
        {
            if (_propertyChangeWatchers.ContainsKey(toTrack))
            {
                _propertyChangeWatchers[toTrack].AddRef();
                return;
            }

            var changing = Observable<IReactivePropertyChangedEventArgs<T>>.Never;
            var changed = Observable<IReactivePropertyChangedEventArgs<T>>.Never;
            var ro = toTrack as IReactiveObject;
            if (ro != null)
            {
                changing = ro.GetChangingObservable().Select(i => new ReactivePropertyChangingEventArgs<T>(toTrack, i.PropertyName));
                changed = ro.GetChangedObservable().Select(i => new ReactivePropertyChangedEventArgs<T>(toTrack, i.PropertyName));
                goto isSetup;
            }

            var inpc = toTrack as INotifyPropertyChanged;
            if (inpc != null)
            {
                changed = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(x => inpc.PropertyChanged += x, x => inpc.PropertyChanged -= x)
                    .Select(x => new ReactivePropertyChangedEventArgs<T>(toTrack, x.EventArgs.PropertyName));
                goto isSetup;
            }

            this.Log().Warn("Property change notifications are enabled and type {0} isn't INotifyPropertyChanged or IReactiveObject", typeof(T));

            isSetup:
            var toDispose = new[]
            {
                changing.Where(_ => this.AreChangeNotificationsEnabled()).Subscribe(_itemChanging.Value.OnNext),
                changed.Where(_ => this.AreChangeNotificationsEnabled()).Subscribe(_itemChanged.Value.OnNext),
            };

            _propertyChangeWatchers.Add(
                toTrack,
                new RefcountDisposeWrapper(Disposable.Create(() =>
                {
                    toDispose[0].Dispose();
                    toDispose[1].Dispose();
                    _propertyChangeWatchers.Remove(toTrack);
                })));
        }

        private void RemoveItemFromPropertyTracking(T toUntrack)
        {
            _propertyChangeWatchers[toUntrack].Release();
        }

        private void ClearAllPropertyChangeWatchers()
        {
            while (_propertyChangeWatchers.Count > 0)
            {
                _propertyChangeWatchers.Values.First().Release();
            }
        }

        #region Super Boring IList crap

        // The BinarySearch methods aren't technically on IList<T>, they're implemented straight on List<T>
        // but by proxying this call we can make use of the nice built in methods that operate on the internal
        // array of the inner list instead of jumping around proxying through the index.
        public int BinarySearch(T item)
        {
            return _inner.BinarySearch(item);
        }

        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return _inner.BinarySearch(item, comparer);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return _inner.BinarySearch(index, count, item, comparer);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public virtual void Add(T item)
        {
            InsertItem(_inner.Count, item);
        }

        /// <inheritdoc/>
        public virtual void Clear()
        {
            ClearItems();
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            return _inner.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _inner.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public virtual bool Remove(T item)
        {
            int index = _inner.IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            RemoveItem(index);
            return true;
        }

        /// <inheritdoc/>
        public int Count => _inner.Count;

        /// <inheritdoc/>
        public virtual bool IsReadOnly => false;

        /// <inheritdoc/>
        public int IndexOf(T item)
        {
            return _inner.IndexOf(item);
        }

        /// <inheritdoc/>
        public virtual void Insert(int index, T item)
        {
            InsertItem(index, item);
        }

        /// <inheritdoc/>
        public virtual void RemoveAt(int index)
        {
            RemoveItem(index);
        }

        public virtual void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

        /// <inheritdoc/>
        public virtual T this[int index]
        {
            get => _inner[index];
            set => SetItem(index, value);
        }

        /// <inheritdoc/>
        int IList.Add(object value)
        {
            Add((T)value);
            return Count - 1;
        }

        /// <inheritdoc/>
        bool IList.Contains(object value)
        {
            return IsCompatibleObject(value) && Contains((T)value);
        }

        /// <inheritdoc/>
        int IList.IndexOf(object value)
        {
            return IsCompatibleObject(value) ? IndexOf((T)value) : -1;
        }

        /// <inheritdoc/>
        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        /// <inheritdoc/>
        bool IList.IsFixedSize => false;

        /// <inheritdoc/>
        void IList.Remove(object value)
        {
            if (IsCompatibleObject(value))
            {
                Remove((T)value);
            }
        }

        /// <inheritdoc/>
        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)_inner).CopyTo(array, index);
        }

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc/>
        object ICollection.SyncRoot => this;

        private static bool IsCompatibleObject(object value)
        {
            return (value is T) || ((value == null) && (default(T) == null));
        }
        #endregion
    }
}

// vim: tw=120 ts=4 sw=4 et :
