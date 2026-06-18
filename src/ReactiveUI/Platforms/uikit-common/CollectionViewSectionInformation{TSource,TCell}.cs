// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using Foundation;
using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Class used to extract a common API between <see cref="UICollectionView"/> and <see cref="UICollectionViewCell"/>.</summary>
/// <typeparam name="TSource">The type of the source.</typeparam>
/// <typeparam name="TCell">The type of the UI collection view cell.</typeparam>
public class CollectionViewSectionInformation<TSource, TCell> : CollectionViewSectionInformation<TSource>
    where TCell : UICollectionViewCell
{
    /// <summary>Initializes a new instance of the <see cref="CollectionViewSectionInformation{TSource, TCell}"/> class.</summary>
    /// <param name="collection">The notify collection changed.</param>
    /// <param name="cellKeySelector">The key selector function.</param>
    public CollectionViewSectionInformation(INotifyCollectionChanged collection, Func<object?, NSString> cellKeySelector)
        : this(collection, cellKeySelector, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="CollectionViewSectionInformation{TSource, TCell}"/> class.</summary>
    /// <param name="collection">The notify collection changed.</param>
    /// <param name="cellKeySelector">The key selector function.</param>
    /// <param name="initializeCellAction">The cell initialization action.</param>
    public CollectionViewSectionInformation(INotifyCollectionChanged collection, Func<object?, NSString> cellKeySelector, Action<TCell>? initializeCellAction)
    {
        Collection = collection;
        CellKeySelector = cellKeySelector;

        if (initializeCellAction is null)
        {
            return;
        }

        InitializeCellAction = cell => initializeCellAction((TCell)cell);
    }

    /// <summary>Initializes a new instance of the <see cref="CollectionViewSectionInformation{TSource, TCell}"/> class.</summary>
    /// <param name="collection">The notify collection changed.</param>
    /// <param name="cellKey">The key selector function.</param>
    public CollectionViewSectionInformation(INotifyCollectionChanged collection, NSString cellKey)
        : this(collection, _ => cellKey, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="CollectionViewSectionInformation{TSource, TCell}"/> class.</summary>
    /// <param name="collection">The notify collection changed.</param>
    /// <param name="cellKey">The key selector function.</param>
    /// <param name="initializeCellAction">The cell initialization action.</param>
    public CollectionViewSectionInformation(INotifyCollectionChanged collection, NSString cellKey, Action<TCell>? initializeCellAction)
        : this(collection, _ => cellKey, initializeCellAction)
    {
    }
}
