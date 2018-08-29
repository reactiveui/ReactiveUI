// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using ReactiveUI.Legacy;
using Splat;
using UIKit;
using NSAction = System.Action;

namespace ReactiveUI
{

    internal class UICollectionViewAdapter : IUICollViewAdapter<UICollectionView, UICollectionViewCell>
    {
        private readonly UICollectionView _view;
        private readonly BehaviorSubject<bool> _isReloadingData;
        private int _inFlightReloads;

        internal UICollectionViewAdapter(UICollectionView view)
        {
            _view = view;
            _isReloadingData = new BehaviorSubject<bool>(false);
        }

        public IObservable<bool> IsReloadingData => _isReloadingData.AsObservable();

        public void ReloadData()
        {
            ++_inFlightReloads;
            _view.ReloadData();

            if (_inFlightReloads == 1)
            {
                Debug.Assert(!_isReloadingData.Value);
                _isReloadingData.OnNext(true);
            }

            // since ReloadData() queues the appropriate messages on the UI thread, we know we're done reloading
            // when this subsequent message is processed (with one caveat - see FinishReloadData for details)
            RxApp.MainThreadScheduler.Schedule(FinishReloadData);
        }

        // UICollectionView no longer has these methods so these are no-ops
        public void BeginUpdates()
        {
        }

        public void EndUpdates()
        {
        }

        public void PerformUpdates(Action updates, Action completion)
        {
            _view.PerformBatchUpdates(new NSAction(updates), (completed) => completion());
        }

        public void InsertSections(NSIndexSet indexes)
        {
            _view.InsertSections(indexes);
        }

        public void DeleteSections(NSIndexSet indexes)
        {
            _view.DeleteSections(indexes);
        }

        public void ReloadSections(NSIndexSet indexes)
        {
            _view.ReloadSections(indexes);
        }

        public void MoveSection(int fromIndex, int toIndex)
        {
            _view.MoveSection(fromIndex, toIndex);
        }

        public void InsertItems(NSIndexPath[] paths)
        {
            _view.InsertItems(paths);
        }

        public void DeleteItems(NSIndexPath[] paths)
        {
            _view.DeleteItems(paths);
        }

        public void ReloadItems(NSIndexPath[] paths)
        {
            _view.ReloadItems(paths);
        }

        public void MoveItem(NSIndexPath path, NSIndexPath newPath)
        {
            _view.MoveItem(path, newPath);
        }

        public UICollectionViewCell DequeueReusableCell(NSString cellKey, NSIndexPath path)
        {
            return (UICollectionViewCell)_view.DequeueReusableCell(cellKey, path);
        }

        private void FinishReloadData()
        {
            --_inFlightReloads;

            if (_inFlightReloads == 0)
            {
                // this is required because sometimes iOS schedules further work that results in calls to GetCell
                // that work could happen after FinishReloadData unless we force layout here
                // of course, we can't have that work running after IsReloading ticks to false because otherwise
                // some updates may occur before the calls to GetCell and thus the calls to GetCell could fail due to invalid indexes
                _view.LayoutIfNeeded();
                Debug.Assert(_isReloadingData.Value);
                _isReloadingData.OnNext(false);
            }
        }
    }

    /// <summary>
    /// ReactiveCollectionViewSource is a Collection View Source that is
    /// connected to a Read Only List that automatically updates the View based
    /// on the contents of the list. The collection changes are buffered and
    /// View items are animated in and out as items are added.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    public class ReactiveCollectionViewSource<TSource> : UICollectionViewSource, IEnableLogger, IDisposable, IReactiveNotifyPropertyChanged<ReactiveCollectionViewSource<TSource>>, IHandleObservableErrors, IReactiveObject
    {
        private readonly CommonReactiveSource<TSource, UICollectionView, UICollectionViewCell, CollectionViewSectionInformation<TSource>> _commonSource;
        private readonly Subject<object> _elementSelected = new Subject<object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewSource{TSource}"/> class.
        /// </summary>
        /// <param name="collectionView">The ui collection view.</param>
        /// <param name="collection">The notify collection chaged.</param>
        /// <param name="cellKey">The cell key.</param>
        /// <param name="initializeCellAction">The cell initialization action.</param>
        public ReactiveCollectionViewSource(UICollectionView collectionView, INotifyCollectionChanged collection, NSString cellKey, Action<UICollectionViewCell> initializeCellAction = null)
            : this(collectionView)
        {
            Data = new[] { new CollectionViewSectionInformation<TSource, UICollectionViewCell>(collection, cellKey, initializeCellAction) };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewSource{TSource}"/> class.
        /// </summary>
        /// <param name="collectionView">The ui collection view.</param>
        /// <param name="sectionInformation">The section information.</param>
        [Obsolete("Please bind your view model to the Data property.")]
        public ReactiveCollectionViewSource(UICollectionView collectionView, IReadOnlyList<CollectionViewSectionInformation<TSource>> sectionInformation)
            : this(collectionView)
        {
            Data = sectionInformation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewSource{TSource}"/> class.
        /// </summary>
        /// <param name="collectionView">The ui collection view.</param>
        public ReactiveCollectionViewSource(UICollectionView collectionView)
        {
            SetupRxObj();
            var adapter = new UICollectionViewAdapter(collectionView);
            _commonSource = new CommonReactiveSource<TSource, UICollectionView, UICollectionViewCell, CollectionViewSectionInformation<TSource>>(adapter);
        }

        /// <summary>
        /// Gets or sets the data that should be displayed by this
        /// <see cref="ReactiveCollectionViewSource"/>.  You should
        /// probably bind your view model to this property.
        /// If the list implements <see cref="IReactiveNotifyCollectionChanged{T}"/>,
        /// then the source will react to changes to the contents of the list as well.
        /// </summary>
        /// <value>The data.</value>
        public IReadOnlyList<CollectionViewSectionInformation<TSource>> Data
        {
            get => _commonSource.SectionInfo;
            set
            {
                if (_commonSource.SectionInfo == value)
                {
                    return;
                }

                this.RaisingPropertyChanging(nameof(Data));
                _commonSource.SectionInfo = value;
                this.RaisingPropertyChanged(nameof(Data));
            }
        }

        /// <summary>
        /// Gets an IObservable that is a hook to <see cref="ItemSelected"/> calls.
        /// </summary>
        public IObservable<object> ElementSelected => _elementSelected;

        /// <inheritdoc/>
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return _commonSource.GetCell(indexPath);
        }

        /// <inheritdoc/>
        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return _commonSource.NumberOfSections();
        }

        /// <inheritdoc/>
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return _commonSource.RowsInSection((int)section);
        }

        /// <inheritdoc/>
        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            _elementSelected.OnNext(_commonSource.ItemAt(indexPath));
        }

        /// <summary>
        /// Returns the Item at the specified index path.
        /// </summary>
        /// <param name="indexPath">The index path.</param>
        /// <returns>The object at the specified index.</returns>
        public object ItemAt(NSIndexPath indexPath)
        {
            return _commonSource.ItemAt(indexPath);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _commonSource.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add => PropertyChangingEventManager.AddHandler(this, value);
            remove => PropertyChangingEventManager.RemoveHandler(this, value);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => PropertyChangedEventManager.AddHandler(this, value);
            remove => PropertyChangedEventManager.RemoveHandler(this, value);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewSource<TSource>>> Changing => this.GetChangingObservable();

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewSource<TSource>>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        private void SetupRxObj()
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
            return IReactiveObjectExtensions.SuppressChangeNotifications(this);
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
        /// <typeparam name="TSource">Type of the view source.</typeparam>
        /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
        public static IDisposable BindTo<TSource, TCell>(
            this IObservable<IReadOnlyList<CollectionViewSectionInformation<TSource, TCell>>> sectionsObservable,
            UICollectionView collectionView,
            Func<ReactiveCollectionViewSource<TSource>, IDisposable> initSource = null)
            where TCell : UICollectionViewCell
        {
            var source = new ReactiveCollectionViewSource<TSource>(collectionView);
            if (initSource != null)
            {
                initSource(source);
            }

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
        /// <typeparam name="TSource">Type of the source.</typeparam>
        /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
        public static IDisposable BindTo<TSource, TCell>(
            this IObservable<INotifyCollectionChanged> sourceObservable,
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
        /// <typeparam name="TSource">Type of the source.</typeparam>
        /// <typeparam name="TCell">Type of the <see cref="UICollectionViewCell"/>.</typeparam>
        public static IDisposable BindTo<TSource, TCell>(
            this IObservable<INotifyCollectionChanged> sourceObservable,
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
