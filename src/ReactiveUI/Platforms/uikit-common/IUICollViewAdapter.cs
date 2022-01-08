// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Foundation;

namespace ReactiveUI;

/// <summary>
/// Interface used to extract a common API between <see cref="UIKit.UITableView"/>
/// and <see cref="UIKit.UICollectionView"/>.
/// </summary>
/// <typeparam name="TUIView">The ui view type.</typeparam>
/// <typeparam name="TUIViewCell">The ui view call type.</typeparam>
internal interface IUICollViewAdapter<TUIView, TUIViewCell>
{
    /// <summary>
    /// Gets the is reloading data.
    /// </summary>
    IObservable<bool> IsReloadingData { get; }

    /// <summary>
    /// Reloads the data.
    /// </summary>
    void ReloadData();

    /// <summary>
    /// Begins the updates.
    /// </summary>
    void BeginUpdates();

    /// <summary>
    /// Performs the updates.
    /// </summary>
    /// <param name="updates">The updates.</param>
    /// <param name="completion">The completion.</param>
    void PerformUpdates(Action updates, Action completion);

    /// <summary>
    /// Ends the updates.
    /// </summary>
    void EndUpdates();

    /// <summary>
    /// Inserts the sections.
    /// </summary>
    /// <param name="indexes">The indexes.</param>
    void InsertSections(NSIndexSet indexes);

    /// <summary>
    /// Deletes the sections.
    /// </summary>
    /// <param name="indexes">The indexes.</param>
    void DeleteSections(NSIndexSet indexes);

    /// <summary>
    /// Reloads the sections.
    /// </summary>
    /// <param name="indexes">The indexes.</param>
    void ReloadSections(NSIndexSet indexes);

    /// <summary>
    /// Moves the section.
    /// </summary>
    /// <param name="fromIndex">From index.</param>
    /// <param name="toIndex">To index.</param>
    void MoveSection(int fromIndex, int toIndex);

    /// <summary>
    /// Inserts the items.
    /// </summary>
    /// <param name="paths">The paths.</param>
    void InsertItems(NSIndexPath[] paths);

    /// <summary>
    /// Deletes the items.
    /// </summary>
    /// <param name="paths">The paths.</param>
    void DeleteItems(NSIndexPath[] paths);

    /// <summary>
    /// Reloads the items.
    /// </summary>
    /// <param name="paths">The paths.</param>
    void ReloadItems(NSIndexPath[] paths);

    /// <summary>
    /// Moves the item.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="newPath">The new path.</param>
    void MoveItem(NSIndexPath path, NSIndexPath newPath);

    /// <summary>
    /// Dequeues the reusable cell.
    /// </summary>
    /// <param name="cellKey">The cell key.</param>
    /// <param name="path">The path.</param>
    /// <returns>The ui view cell.</returns>
    TUIViewCell DequeueReusableCell(NSString cellKey, NSIndexPath path);
}
