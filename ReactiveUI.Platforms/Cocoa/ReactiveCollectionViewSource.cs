using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;
using Splat;

namespace ReactiveUI.Cocoa
{
    public class CollectionViewSectionInformation<TSource> : ISectionInformation<TSource, UICollectionView, UICollectionViewCell>
    {
        public IReactiveNotifyCollectionChanged<TSource> Collection { get; protected set; }
        public Action<UICollectionViewCell> InitializeCellAction { get; protected set; }
        public Func<object, NSString> CellKeySelector { get; protected set; }
    }

    public class CollectionViewSectionInformation<TSource, TCell> : CollectionViewSectionInformation<TSource>
        where TCell : UICollectionViewCell
    {
        public CollectionViewSectionInformation(IReactiveNotifyCollectionChanged<TSource> collection, Func<object, NSString> cellKeySelector, Action<TCell> initializeCellAction = null)
        {
            Collection = collection;
            CellKeySelector = cellKeySelector;

            if (initializeCellAction != null) {
                InitializeCellAction = cell => initializeCellAction((TCell)cell);
            }
        }

        public CollectionViewSectionInformation(IReactiveNotifyCollectionChanged<TSource> collection, NSString cellKey, Action<TCell> initializeCellAction = null)
            : this(collection, _ => cellKey, initializeCellAction)
        {
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

    public class ReactiveCollectionViewSource<TSource> : UICollectionViewSource, IEnableLogger, IDisposable, IReactiveNotifyPropertyChanged<ReactiveCollectionViewSource<TSource>>, IHandleObservableErrors, IReactiveObject
    {
        readonly CommonReactiveSource<TSource, UICollectionView, UICollectionViewCell, CollectionViewSectionInformation<TSource>> commonSource;
        readonly Subject<object> elementSelected = new Subject<object>();

        public ReactiveCollectionViewSource(UICollectionView collectionView, IReactiveNotifyCollectionChanged<TSource> collection, NSString cellKey, Action<UICollectionViewCell> initializeCellAction = null)
            : this(collectionView) {
            this.Data = new[] { new CollectionViewSectionInformation<TSource, UICollectionViewCell>(collection, cellKey, initializeCellAction) };
        }

        [Obsolete("Please bind your view model to the Data property.")]
        public ReactiveCollectionViewSource(UICollectionView collectionView, IReadOnlyList<CollectionViewSectionInformation<TSource>> sectionInformation)
            : this(collectionView) {
            this.Data = sectionInformation;
        }

        public ReactiveCollectionViewSource(UICollectionView collectionView) {
            setupRxObj();
            var adapter = new UICollectionViewAdapter(collectionView);
            this.commonSource = new CommonReactiveSource<TSource, UICollectionView, UICollectionViewCell, CollectionViewSectionInformation<TSource>>(adapter);
        }

        /// <summary>
        /// Gets or sets the data that should be displayed by this
        /// <see cref="ReactiveCollectionViewSource"/>.  You should
        /// probably bind your view model to this property.
        /// If the list implements <see cref="IReactiveNotifyCollectionChanged"/>,
        /// then the source will react to changes to the contents of the list as well.
        /// </summary>
        /// <value>The data.</value>
        public IReadOnlyList<CollectionViewSectionInformation<TSource>> Data
        {
            get { return commonSource.SectionInfo; }
            set {
                if (commonSource.SectionInfo == value)  return;

                this.raisePropertyChanging("Data");
                commonSource.SectionInfo = value;
                this.raisePropertyChanged("Data");
            }
        }

        /// <summary>
        /// Gets an IObservable that is a hook to <see cref="ItemSelected"/> calls.
        /// </summary>
        public IObservable<object> ElementSelected {
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

        public object ItemAt(NSIndexPath indexPath)
        {
            return commonSource.ItemAt(indexPath);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) commonSource.Dispose();
            base.Dispose(disposing);
        }

        public event PropertyChangingEventHandler PropertyChanging;

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) 
	    {
            var handler = PropertyChanging;
            if (handler != null) {
                handler(this, args);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, args);
            }
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        public IObservable<IObservedChange<ReactiveCollectionViewSource<TSource>, object>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IObservedChange<ReactiveCollectionViewSource<TSource>, object>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        void setupRxObj()
        {
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
            return this.suppressChangeNotifications();
        }
    }

    /// <summary>
    /// Extension methods for <see cref="ReactiveCollectionViewSource"/>.
    /// </summary>
    public static class ReactiveCollectionViewSourceExtensions
    {
        /// <summary>
        /// <para>Extension method that binds an observable of a list of collection
        /// sections as the source of a <see cref="UICollectionView"/>.</para>
        /// <para>If your <see cref="IReadOnlyList"/> is also an instance of
        /// <see cref="IReactiveNotifyCollectionChanged"/>, then this method
        /// will silently update the bindings whenever it changes as well.
        /// Otherwise, it will just log a message.</para>
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="sectionsObservable">Sections observable.</param>
        /// <param name="collectionView">Collection view.</param>
        /// <param name="initSource">Optionally initializes some property of
        /// the <see cref="ReactiveCollectionViewSource"/>.</param>
        /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
        public static IDisposable BindTo<TSource, TCell>(
            this IObservable<IReadOnlyList<CollectionViewSectionInformation<TSource, TCell>>> sectionsObservable,
            UICollectionView collectionView,
            Func<ReactiveCollectionViewSource<TSource>, IDisposable> initSource = null)
            where TCell : UICollectionViewCell
        {
            var source = new ReactiveCollectionViewSource<TSource>(collectionView);
            if (initSource != null) initSource(source);
            var bind = sectionsObservable.BindTo(source, x => x.Data);
            collectionView.Source = source;
            return new CompositeDisposable(bind, source);
        }

        /// <summary>
        /// Extension method that binds an observable of a collection
        /// as the source of a <see cref="UICollectionView"/>.
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="sourceObservable">Source collection observable.</param>
        /// <param name="collectionView">Collection view.</param>
        /// <param name="cellKey">Cell key.</param>
        /// <param name="initializeCellAction">Initialize cell action.</param>
        /// <param name="initSource">Optionally initializes some property of
        /// the <see cref="ReactiveCollectionViewSource"/>.</param>
        /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
        public static IDisposable BindTo<TSource,TCell>(
            this IObservable<IReactiveNotifyCollectionChanged<TSource>> sourceObservable,
            UICollectionView collectionView,
            NSString cellKey,
            Action<TCell> initializeCellAction = null,
            Func<ReactiveCollectionViewSource<TSource>, IDisposable> initSource = null)
            where TCell : UICollectionViewCell
        {
            return sourceObservable
                .Select(
                    src => new[]
                    {
                        new CollectionViewSectionInformation<TSource, TCell>(
                            src,
                            cellKey,
                            initializeCellAction)
                    })
                .BindTo(collectionView, initSource);
        }

        /// <summary>
        /// Extension method that binds an observable of a collection
        /// as the source of a <see cref="UICollectionView"/>.  Also registers
        /// the given class with an unspecified cellKey (you should probably
        /// not specify any other cellKeys).
        /// </summary>
        /// <returns>The <see cref="IDisposable"/> used to dispose this binding.</returns>
        /// <param name="sourceObservable">Source collection observable.</param>
        /// <param name="collectionView">Collection view.</param>
        /// <param name="initializeCellAction">Initialize cell action.</param>
        /// <param name="initSource">Optionally initializes some property of
        /// the <see cref="ReactiveCollectionViewSource"/>.</param>
        /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
        public static IDisposable BindTo<TSource, TCell>(
            this IObservable<IReactiveNotifyCollectionChanged<TSource>> sourceObservable,
            UICollectionView collectionView,
            Action<TCell> initializeCellAction = null,
            Func<ReactiveCollectionViewSource<TSource>, IDisposable> initSource = null)
            where TCell : UICollectionViewCell
        {
            var type = typeof(TCell);
            var cellKey = new NSString(type.ToString());
            collectionView.RegisterClassForCell(type, new NSString(cellKey));
            return sourceObservable
                .BindTo(collectionView, cellKey, initializeCellAction, initSource);
        }
    }
}
