﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Subjects;
using Foundation;
using ReactiveUI.Legacy;
using Splat;
using UIKit;

namespace ReactiveUI
{
    /// <summary>
    /// ReactiveCollectionViewSource is a Collection View Source that is
    /// connected to a Read Only List that automatically updates the View based
    /// on the contents of the list. The collection changes are buffered and
    /// View items are animated in and out as items are added.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    public class ReactiveCollectionViewSource<TSource> : UICollectionViewSource, IEnableLogger, IReactiveNotifyPropertyChanged<ReactiveCollectionViewSource<TSource>>, IHandleObservableErrors, IReactiveObject
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
            var adapter = new UICollectionViewAdapter(collectionView);
            _commonSource = new CommonReactiveSource<TSource, UICollectionView, UICollectionViewCell, CollectionViewSectionInformation<TSource>>(adapter);
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

        /// <summary>
        /// Gets or sets the data that should be displayed by this
        /// <see cref="ReactiveCollectionViewSource{TSource}"/>.  You should
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

        /// <summary>
        /// Gets an Observable that signals *before* a property is about to
        /// be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewSource<TSource>>> Changing => this.GetChangingObservable();

        /// <summary>
        /// Gets an an Observable that signals *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewSource<TSource>>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

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
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
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

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _commonSource?.Dispose();
                _elementSelected?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
