// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

using Foundation;

using UIKit;

namespace ReactiveUI;

/// <summary>
/// Class used to extract a common API between <see cref="UIKit.UITableView"/>
/// and <see cref="UIKit.UITableViewCell"/>.
/// </summary>
/// <typeparam name="TSource">The type of the source.</typeparam>
public class TableSectionInformation<TSource> : ISectionInformation<UITableViewCell>
{
    /// <inheritdoc/>
    public INotifyCollectionChanged? Collection { get; protected set; }

    /// <inheritdoc/>
    public Action<UITableViewCell>? InitializeCellAction { get; protected set; }

    /// <inheritdoc/>
    public Func<object?, NSString>? CellKeySelector { get; protected set; }

    /// <summary>
    /// Gets or sets the size hint.
    /// </summary>
    public float SizeHint { get; protected set; }

    /// <summary>
    /// Gets or sets the header of this section.
    /// </summary>
    /// <value>The header, or null if a header shouldn't be used.</value>
    public TableSectionHeader? Header { get; set; }

    /// <summary>
    /// Gets or sets the footer of this section.
    /// </summary>
    /// <value>The footer, or null if a footer shouldn't be used.</value>
    public TableSectionHeader? Footer { get; set; }
}

/// <summary>
/// Class used to extract a common API between <see cref="UIKit.UICollectionView"/>
/// and <see cref="UIKit.UICollectionViewCell"/>.
/// </summary>
/// <typeparam name="TSource">The type of the source.</typeparam>
/// <typeparam name="TCell">The type of the cell.</typeparam>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
public class TableSectionInformation<TSource, TCell> : TableSectionInformation<TSource>
    where TCell : UITableViewCell
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableSectionInformation{TSource, TCell}"/> class.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="cellKeySelector">The cell key selector.</param>
    /// <param name="sizeHint">The size hint.</param>
    /// <param name="initializeCellAction">The initialize cell action.</param>
    public TableSectionInformation(INotifyCollectionChanged collection, Func<object?, NSString>? cellKeySelector, float sizeHint, Action<TCell>? initializeCellAction = null)
    {
        Collection = collection;
        SizeHint = sizeHint;
        CellKeySelector = cellKeySelector;
        if (initializeCellAction is not null)
        {
            InitializeCellAction = cell => initializeCellAction((TCell)cell);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableSectionInformation{TSource, TCell}"/> class.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="cellKey">The cell key.</param>
    /// <param name="sizeHint">The size hint.</param>
    /// <param name="initializeCellAction">The initialize cell action.</param>
    public TableSectionInformation(INotifyCollectionChanged collection, NSString cellKey, float sizeHint, Action<TCell>? initializeCellAction = null)
        : this(collection, _ => cellKey, sizeHint, initializeCellAction)
    {
    }
}
