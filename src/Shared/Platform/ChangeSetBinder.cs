// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// Materializes an <see cref="IReactiveChangeSet{T}"/> stream into an indexable list, applying each change in
/// order. This is the allocation-light replacement for the DynamicData <c>SourceList</c> + <c>Connect()</c> bind
/// that platform list adapters relied on: it subscribes once, maintains a single backing <see cref="List{T}"/>,
/// surfaces per-change and per-batch callbacks for the adapter to raise its own notifications, and tears the
/// subscription down on dispose. No operator pipeline, scheduler, or intermediate change-set buffering is involved.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
internal sealed class ChangeSetBinder<T> : IDisposable
{
    /// <summary>The materialized current items, kept in sync with the change-set stream.</summary>
    private readonly List<T> _items = [];

    /// <summary>Optional per-change callback raised as each change is applied (fine-grained binding).</summary>
    private readonly Action<ReactiveChange<T>>? _onChange;

    /// <summary>Optional callback raised once after each batch is applied (coarse binding).</summary>
    private readonly Action? _onBatch;

    /// <summary>The source subscription, torn down on dispose.</summary>
    private readonly IDisposable _subscription;

    /// <summary>Initializes a new instance of the <see cref="ChangeSetBinder{T}"/> class.</summary>
    /// <param name="source">The change-set stream to materialize.</param>
    /// <param name="onChange">Optional callback raised for each change as it is applied (fine-grained binding).</param>
    /// <param name="onBatch">Optional callback raised once after each change-set batch is applied (coarse binding).</param>
    public ChangeSetBinder(
        IObservable<IReactiveChangeSet<T>> source,
        Action<ReactiveChange<T>>? onChange = null,
        Action? onBatch = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);

        _onChange = onChange;
        _onBatch = onBatch;
        _subscription = source.Subscribe(new DelegateObserver<IReactiveChangeSet<T>>(OnChangeSet));
    }

    /// <summary>Gets the number of materialized items.</summary>
    public int Count => _items.Count;

    /// <summary>Gets the materialized item at the specified index.</summary>
    /// <param name="index">The index.</param>
    /// <returns>The item at the index.</returns>
    public T this[int index] => _items[index];

    /// <inheritdoc/>
    public void Dispose() => _subscription.Dispose();

    /// <summary>Applies a batch of changes to the backing list and raises the callbacks.</summary>
    /// <param name="changeSet">The change-set batch.</param>
    private void OnChangeSet(IReactiveChangeSet<T> changeSet)
    {
        for (var i = 0; i < changeSet.Count; i++)
        {
            var change = changeSet[i];
            Apply(change);
            _onChange?.Invoke(change);
        }

        _onBatch?.Invoke();
    }

    /// <summary>Applies a single change to the backing list.</summary>
    /// <param name="change">The change to apply.</param>
    private void Apply(ReactiveChange<T> change)
    {
        switch (change.Reason)
        {
            case ReactiveChangeReason.Add:
                {
                    Insert(change.CurrentIndex, change.Current);
                    break;
                }

            case ReactiveChangeReason.Remove:
                {
                    RemoveAt(change.CurrentIndex, change.Current);
                    break;
                }

            case ReactiveChangeReason.Replace:
                {
                    if (change.CurrentIndex >= 0 && change.CurrentIndex < _items.Count)
                    {
                        _items[change.CurrentIndex] = change.Current;
                    }

                    break;
                }

            case ReactiveChangeReason.Move:
                {
                    RemoveAt(change.PreviousIndex, change.Current);
                    Insert(change.CurrentIndex, change.Current);
                    break;
                }

            case ReactiveChangeReason.Refresh:
                break;
        }
    }

    /// <summary>Inserts an item at the index, appending when the index is out of range.</summary>
    /// <param name="index">The target index.</param>
    /// <param name="item">The item to insert.</param>
    private void Insert(int index, T item)
    {
        if (index >= 0 && index <= _items.Count)
        {
            _items.Insert(index, item);
            return;
        }

        _items.Add(item);
    }

    /// <summary>Removes the item at the index, falling back to removal by value when the index is unusable.</summary>
    /// <param name="index">The target index, or a value out of range to remove by value.</param>
    /// <param name="item">The item to remove by value when the index is unusable.</param>
    private void RemoveAt(int index, T item)
    {
        if (index >= 0 && index < _items.Count)
        {
            _items.RemoveAt(index);
            return;
        }

        _items.Remove(item);
    }
}
