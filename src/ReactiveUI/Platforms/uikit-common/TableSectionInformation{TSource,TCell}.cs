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
/// <typeparam name="TCell">The type of the cell.</typeparam>
public class TableSectionInformation<TSource, TCell> : TableSectionInformation<TSource>
    where TCell : UITableViewCell
{
    /// <summary>Initializes a new instance of the <see cref="TableSectionInformation{TSource, TCell}"/> class.</summary>
    /// <param name="collection">The collection.</param>
    /// <param name="cellKeySelector">The cell key selector.</param>
    /// <param name="sizeHint">The size hint.</param>
    public TableSectionInformation(INotifyCollectionChanged collection, Func<object?, NSString>? cellKeySelector, float sizeHint)
        : this(collection, cellKeySelector, sizeHint, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TableSectionInformation{TSource, TCell}"/> class.</summary>
    /// <param name="collection">The collection.</param>
    /// <param name="cellKeySelector">The cell key selector.</param>
    /// <param name="sizeHint">The size hint.</param>
    /// <param name="initializeCellAction">The initialize cell action.</param>
    public TableSectionInformation(INotifyCollectionChanged collection, Func<object?, NSString>? cellKeySelector, float sizeHint, Action<TCell>? initializeCellAction)
    {
        Collection = collection;
        SizeHint = sizeHint;
        CellKeySelector = cellKeySelector;

        if (initializeCellAction is null)
        {
            return;
        }

        InitializeCellAction = cell => initializeCellAction((TCell)cell);
    }

    /// <summary>Initializes a new instance of the <see cref="TableSectionInformation{TSource, TCell}"/> class.</summary>
    /// <param name="collection">The collection.</param>
    /// <param name="cellKey">The cell key.</param>
    /// <param name="sizeHint">The size hint.</param>
    public TableSectionInformation(INotifyCollectionChanged collection, NSString cellKey, float sizeHint)
        : this(collection, _ => cellKey, sizeHint, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TableSectionInformation{TSource, TCell}"/> class.</summary>
    /// <param name="collection">The collection.</param>
    /// <param name="cellKey">The cell key.</param>
    /// <param name="sizeHint">The size hint.</param>
    /// <param name="initializeCellAction">The initialize cell action.</param>
    public TableSectionInformation(INotifyCollectionChanged collection, NSString cellKey, float sizeHint, Action<TCell>? initializeCellAction)
        : this(collection, _ => cellKey, sizeHint, initializeCellAction)
    {
    }
}
