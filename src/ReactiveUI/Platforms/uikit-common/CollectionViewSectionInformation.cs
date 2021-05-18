// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Foundation;
using UIKit;

namespace ReactiveUI
{
    /// <summary>
    /// Class used to extract a common API between <see cref="UIKit.UICollectionView"/>
    /// and <see cref="UIKit.UICollectionViewCell"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    public class CollectionViewSectionInformation<TSource> : ISectionInformation<UICollectionViewCell>
    {
        /// <inheritdoc/>
        public INotifyCollectionChanged? Collection { get; protected set; }

        /// <inheritdoc/>
        public Action<UICollectionViewCell>? InitializeCellAction { get; protected set; }

        /// <inheritdoc/>
        public Func<object?, NSString>? CellKeySelector { get; protected set; }
    }

    /// <summary>
    /// Class used to extract a common API between <see cref="UIKit.UICollectionView"/>
    /// and <see cref="UIKit.UICollectionViewCell"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TCell">The type of the UI collection view cell.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    public class CollectionViewSectionInformation<TSource, TCell> : CollectionViewSectionInformation<TSource>
        where TCell : UICollectionViewCell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewSectionInformation{TSource, TCell}"/> class.
        /// </summary>
        /// <param name="collection">The notify collection changed.</param>
        /// <param name="cellKeySelector">The key selector function.</param>
        /// <param name="initializeCellAction">The cell initialization action.</param>
        public CollectionViewSectionInformation(INotifyCollectionChanged collection, Func<object?, NSString> cellKeySelector, Action<TCell>? initializeCellAction = null)
        {
            Collection = collection;
            CellKeySelector = cellKeySelector;

            if (initializeCellAction is not null)
            {
                InitializeCellAction = cell => initializeCellAction((TCell)cell);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionViewSectionInformation{TSource, TCell}"/> class.
        /// </summary>
        /// <param name="collection">The notify collection changed.</param>
        /// <param name="cellKey">The key selector function.</param>
        /// <param name="initializeCellAction">The cell initialization action.</param>
        public CollectionViewSectionInformation(INotifyCollectionChanged collection, NSString cellKey, Action<TCell>? initializeCellAction = null)
            : this(collection, _ => cellKey, initializeCellAction)
        {
        }
    }
}
