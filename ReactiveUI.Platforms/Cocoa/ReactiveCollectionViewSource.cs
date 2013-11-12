using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;

namespace ReactiveUI.Cocoa
{
    public class CollectionViewSectionInformation : ISectionInformation<UICollectionView, UICollectionViewCell>
    {
        public IReactiveNotifyCollectionChanged Collection { get; protected set; }
        public Action<UICollectionViewCell> InitializeCellAction { get; protected set; }
        public NSString CellKey { get; protected set; }
    }

    public class CollectionViewSectionInformation<TCell> : CollectionViewSectionInformation
        where TCell : UICollectionViewCell
    {
        public CollectionViewSectionInformation(IReactiveNotifyCollectionChanged collection, string cellKey, Action<TCell> initializeCellAction = null)
        {
            Collection = collection;
            CellKey = new NSString(cellKey);

            if (initializeCellAction != null) {
                InitializeCellAction = cell => initializeCellAction((TCell)cell);
            }
        }
    }

    class UICollectionViewAdapter : IUICollViewAdapter<UICollectionView, UICollectionViewCell>
    {
        readonly UICollectionView view;
        internal UICollectionViewAdapter(UICollectionView view) { this.view = view; }

        public void ReloadData() { view.ReloadData(); }
        public void PerformBatchUpdates(Action updates) { view.PerformBatchUpdates(new NSAction(updates), null); }
        public void InsertItems(NSIndexPath[] paths) { view.InsertItems(paths); }
        public void DeleteItems(NSIndexPath[] paths) { view.DeleteItems(paths); }
        public void ReloadItems(NSIndexPath[] paths) { view.ReloadItems(paths); }
        public void MoveItem(NSIndexPath path, NSIndexPath newPath) { view.MoveItem(path, newPath); }
        public UICollectionViewCell DequeueReusableCell(NSString cellKey, NSIndexPath path)
        {
            return (UICollectionViewCell)view.DequeueReusableCell(cellKey, path);
        }
    }

    public class ReactiveCollectionViewSource : UICollectionViewSource, IEnableLogger, IDisposable, IReactiveNotifyPropertyChanged, IHandleObservableErrors
    {
        readonly CommonReactiveSource<UICollectionView, UICollectionViewCell, CollectionViewSectionInformation> commonSource;
        readonly Subject<object> elementSelected = new Subject<object>();

        public ReactiveCollectionViewSource(UICollectionView collectionView, IReactiveNotifyCollectionChanged collection, string cellKey, Action<UICollectionViewCell> initializeCellAction = null)
            : this(collectionView) {
            this.Data = new[] { new CollectionViewSectionInformation<UICollectionViewCell>(collection, cellKey, initializeCellAction) };
        }

        [Obsolete("Please bind your view model to the Data property.")]
        public ReactiveCollectionViewSource(UICollectionView collectionView, IReadOnlyList<CollectionViewSectionInformation> sectionInformation)
            : this(collectionView) {
            this.Data = sectionInformation;
        }

        public ReactiveCollectionViewSource(UICollectionView collectionView) {
            setupRxObj();
            var adapter = new UICollectionViewAdapter(collectionView);
            this.commonSource = new CommonReactiveSource<UICollectionView, UICollectionViewCell, CollectionViewSectionInformation>(adapter);
        }

        /// <summary>
        /// Gets or sets the data that should be displayed by this
        /// <see cref="ReactiveCollectionViewSource"/>.  You should
        /// probably bind your view model to this property.
        /// </summary>
        /// <value>The data.</value>
        public IReadOnlyList<CollectionViewSectionInformation> Data {
            get { return commonSource.SectionInfo; }
            set {
                raisePropertyChanging("Data");
                commonSource.SectionInfo = value;
                raisePropertyChanged("Data");
            }
        }

        /// <summary>
        /// Gets an IObservable that is a hook to <see cref="ItemSelected"/> calls.
        /// </summary>
        public IObservable<object> ElementSelected
        {
            get { return elementSelected; }
        }

        public IObservable<IEnumerable<NotifyCollectionChangedEventArgs>> DidPerformUpdates {
            get { return commonSource.DidPerformUpdates; }
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return commonSource.GetCell(indexPath);
        }

        public override int NumberOfSections(UICollectionView collectionView)
        {
            return commonSource.NumberOfSections();
        }

        public override int GetItemsCount(UICollectionView collectionView, int section)
        {
            return commonSource.RowsInSection(section);
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            elementSelected.OnNext(commonSource.ItemAt(indexPath));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) commonSource.Dispose();
            base.Dispose(disposing);
        }



        // Boring copy-paste of ReactiveObject et al follows:
        [field:IgnoreDataMember]
        public event PropertyChangingEventHandler PropertyChanging;

        [field:IgnoreDataMember]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IObservedChange<object, object>> Changing {
            get { return changingSubject; }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IObservedChange<object, object>> Changed {
            get { return changedSubject; }
        }

        [IgnoreDataMember]
        protected Lazy<PropertyInfo[]> allPublicProperties;

        [IgnoreDataMember]
        Subject<IObservedChange<object, object>> changingSubject;

        [IgnoreDataMember]
        Subject<IObservedChange<object, object>> changedSubject;

        [IgnoreDataMember]
        long changeNotificationsSuppressed = 0;

        [IgnoreDataMember]
        readonly ScheduledSubject<Exception> thrownExceptions = new ScheduledSubject<Exception>(Scheduler.Immediate, RxApp.DefaultExceptionHandler);

        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions { get { return thrownExceptions; } }

        [OnDeserialized]
        void setupRxObj(StreamingContext sc) { setupRxObj(); }

        void setupRxObj()
        {
            changingSubject = new Subject<IObservedChange<object, object>>();
            changedSubject = new Subject<IObservedChange<object, object>>();

            allPublicProperties = new Lazy<PropertyInfo[]>(() =>
                GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray());
        }

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change
        /// notifications.</returns>
        public IDisposable SuppressChangeNotifications()
        {
            Interlocked.Increment(ref changeNotificationsSuppressed);
            return Disposable.Create(() =>
                Interlocked.Decrement(ref changeNotificationsSuppressed));
        }

        protected internal void raisePropertyChanging(string propertyName)
        {
            Contract.Requires(propertyName != null);

            if (!areChangeNotificationsEnabled || changingSubject == null)
                return;

            var handler = this.PropertyChanging;
            if (handler != null) {
                var e = new PropertyChangingEventArgs(propertyName);
                handler(this, e);
            }

            notifyObservable(new ObservedChange<object, object>() {
                PropertyName = propertyName, Sender = this, Value = null
            }, changingSubject);
        }

        protected internal void raisePropertyChanged(string propertyName)
        {
            Contract.Requires(propertyName != null);

            this.Log().Debug("{0:X}.{1} changed", this.GetHashCode(), propertyName);

            if (!areChangeNotificationsEnabled || changedSubject == null) {
                this.Log().Debug("Suppressed change");
                return;
            }

            var handler = this.PropertyChanged;
            if (handler != null) {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }

            notifyObservable(new ObservedChange<object, object>() {
                PropertyName = propertyName, Sender = this, Value = null
            }, changedSubject);
        }

        protected bool areChangeNotificationsEnabled {
            get {
                return (Interlocked.Read(ref changeNotificationsSuppressed) == 0);
            }
        }

        internal void notifyObservable<T>(T item, Subject<T> subject)
        {
            try {
                subject.OnNext(item);
            } catch (Exception ex) {
                this.Log().ErrorException("ReactiveObject Subscriber threw exception", ex);
                thrownExceptions.OnNext(ex);
            }
        }

        /// <summary>
        /// RaiseAndSetIfChanged fully implements a Setter for a read-write
        /// property on a ReactiveObject, using CallerMemberName to raise the notification
        /// and the ref to the backing field to set the property.
        /// </summary>
        /// <typeparam name="TObj">The type of the This.</typeparam>
        /// <typeparam name="TRet">The type of the return value.</typeparam>
        /// <param name="This">The <see cref="ReactiveObject"/> raising the notification.</param>
        /// <param name="backingField">A Reference to the backing field for this
        /// property.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">The name of the property, usually
        /// automatically provided through the CallerMemberName attribute.</param>
        /// <returns>The newly set value, normally discarded.</returns>
        public TRet RaiseAndSetIfChanged<TRet>(
                ref TRet backingField,
                TRet newValue,
                [CallerMemberName] string propertyName = null)
        {
            Contract.Requires(propertyName != null);

            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue)) {
                return newValue;
            }

            raisePropertyChanging(propertyName);
            backingField = newValue;
            raisePropertyChanged(propertyName);
            return newValue;
        }

        /// <summary>
        /// Use this method in your ReactiveObject classes when creating custom
        /// properties where raiseAndSetIfChanged doesn't suffice.
        /// </summary>
        /// <param name="This">The instance of ReactiveObject on which the property has changed.</param>
        /// <param name="propertyName">
        /// A string representing the name of the property that has been changed.
        /// Leave <c>null</c> to let the runtime set to caller member name.
        /// </param>
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            raisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Use this method in your ReactiveObject classes when creating custom
        /// properties where raiseAndSetIfChanged doesn't suffice.
        /// </summary>
        /// <param name="This">The instance of ReactiveObject on which the property has changed.</param>
        /// <param name="propertyName">
        /// A string representing the name of the property that has been changed.
        /// Leave <c>null</c> to let the runtime set to caller member name.
        /// </param>
        public void RaisePropertyChanging([CallerMemberName] string propertyName = null)
        {
            raisePropertyChanging(propertyName);
        }
    }
}
