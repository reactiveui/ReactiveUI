// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Todo;

/// <summary>The database a to-do list reads and writes.</summary>
public interface ITodoStore
{
    /// <summary>Reads every item.</summary>
    /// <param name="cancellationToken">A token that cancels the read.</param>
    /// <returns>A copy of every stored item, in the order they were added.</returns>
    Task<IReadOnlyList<TodoItem>> QueryAsync(CancellationToken cancellationToken);

    /// <summary>Stores a new item and assigns its identifier.</summary>
    /// <param name="title">The title of the new item.</param>
    /// <param name="cancellationToken">A token that cancels the write.</param>
    /// <returns>A copy of the stored item.</returns>
    /// <exception cref="TodoStoreException">The title is already on the list.</exception>
    Task<TodoItem> AddAsync(string title, CancellationToken cancellationToken);

    /// <summary>Marks an item as finished.</summary>
    /// <param name="id">The identifier of the item.</param>
    /// <param name="cancellationToken">A token that cancels the write.</param>
    /// <returns>A copy of the updated item.</returns>
    Task<TodoItem> CompleteAsync(int id, CancellationToken cancellationToken);

    /// <summary>Removes an item.</summary>
    /// <param name="id">The identifier of the item.</param>
    /// <param name="cancellationToken">A token that cancels the write.</param>
    /// <returns>A task that completes when the item is removed.</returns>
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
