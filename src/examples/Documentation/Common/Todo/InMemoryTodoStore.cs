// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Todo;

/// <summary>A <see cref="ITodoStore"/> that keeps its rows in memory, standing in for a database.</summary>
[System.Diagnostics.DebuggerDisplay("Rows = {_rows.Count}")]
public sealed class InMemoryTodoStore : ITodoStore
{
    /// <summary>The stored rows, in the order they were added.</summary>
    private readonly List<TodoItem> _rows = [];

    /// <summary>The identifier the next stored item receives.</summary>
    private int _nextId = 1;

    /// <summary>Gets or sets how long each call takes, as a network round trip would.</summary>
    public TimeSpan Latency { get; set; }

    /// <summary>Creates a store holding four household tasks, one of them finished.</summary>
    /// <returns>The seeded store.</returns>
    public static InMemoryTodoStore CreateSeeded()
    {
        InMemoryTodoStore store = new();
        _ = store.Seed("Buy groceries", isDone: false);
        _ = store.Seed("Pay electricity bill", isDone: true);
        _ = store.Seed("Book dentist appointment", isDone: false);
        _ = store.Seed("Renew car registration", isDone: false);
        return store;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TodoItem>> QueryAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(Latency, cancellationToken).ConfigureAwait(false);
        return [.. _rows.Select(static row => row.Clone())];
    }

    /// <inheritdoc/>
    public async Task<TodoItem> AddAsync(string title, CancellationToken cancellationToken)
    {
        await Task.Delay(Latency, cancellationToken).ConfigureAwait(false);
        if (_rows.Exists(row => string.Equals(row.Title, title, StringComparison.OrdinalIgnoreCase)))
        {
            throw new TodoStoreException($"'{title}' is already on the list.");
        }

        return Seed(title, isDone: false).Clone();
    }

    /// <inheritdoc/>
    public async Task<TodoItem> CompleteAsync(int id, CancellationToken cancellationToken)
    {
        await Task.Delay(Latency, cancellationToken).ConfigureAwait(false);
        TodoItem row = _rows.Find(row => row.Id == id) ?? throw new TodoStoreException($"Item {id} does not exist.");
        row.IsDone = true;
        return row.Clone();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await Task.Delay(Latency, cancellationToken).ConfigureAwait(false);
        _ = _rows.RemoveAll(row => row.Id == id);
    }

    /// <summary>Stores a row directly, without the latency of a call.</summary>
    /// <param name="title">The title of the row.</param>
    /// <param name="isDone">Whether the task is finished.</param>
    /// <returns>The stored row.</returns>
    private TodoItem Seed(string title, bool isDone)
    {
        TodoItem row = new() { Id = _nextId, Title = title, IsDone = isDone };
        _nextId++;
        _rows.Add(row);
        return row;
    }
}
