// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AndroidX.RecyclerView.Widget;
using DynamicData;

namespace ReactiveUI.AndroidX;

/// <summary>
/// An adapter for the Android <see cref="RecyclerView"/>.
/// </summary>
/// <typeparam name="TViewModel">The type of ViewModel that this adapter holds.</typeparam>
public abstract class ReactiveRecyclerViewAdapter<TViewModel> : RecyclerView.Adapter
    where TViewModel : class, IReactiveObject
{
    /// <summary>
    /// The source list that backs the adapter.
    /// </summary>
    private readonly SourceList<TViewModel> _list;

    /// <summary>
    /// The subscription that keeps the bindings in sync with the source list.
    /// </summary>
    private readonly IDisposable _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveRecyclerViewAdapter{TViewModel}"/> class.
    /// </summary>
    /// <param name="backingList">The backing list.</param>
    protected ReactiveRecyclerViewAdapter(IObservable<IChangeSet<TViewModel>> backingList)
    {
        _list = new(backingList);

        _inner = _list
            .Connect()
            .ForEachChange(UpdateBindings)
            .Subscribe();
    }

    /// <inheritdoc/>
    public override int ItemCount => _list.Count;

    /// <inheritdoc/>
    public override int GetItemViewType(int position) => GetItemViewType(position, GetViewModelByPosition(position));

    /// <summary>
    /// Determine the View that will be used/re-used in lists where
    /// the list contains different cell designs.
    /// </summary>
    /// <param name="position">The position of the current view in the list.</param>
    /// <param name="viewModel">The ViewModel associated with the current View.</param>
    /// <returns>An ID to be used in OnCreateViewHolder.</returns>
    public virtual int GetItemViewType(int position, TViewModel? viewModel) => base.GetItemViewType(position);

    /// <inheritdoc/>
    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        ArgumentExceptionHelper.ThrowIfNull(holder);

        if (holder is not IViewFor viewForHolder)
        {
            throw new ArgumentException("Holder must be derived from IViewFor", nameof(holder));
        }

        viewForHolder.ViewModel = GetViewModelByPosition(position);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _inner.Dispose();
            _list.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Gets the view model at the specified position, or null if the position is out of range.
    /// </summary>
    /// <param name="position">The position in the list.</param>
    /// <returns>The view model at the position, or null.</returns>
    private TViewModel? GetViewModelByPosition(int position) => position >= _list.Count ? null : _list.Items[position];

    /// <summary>
    /// Updates the adapter bindings in response to a change in the source list.
    /// </summary>
    /// <param name="change">The change to apply.</param>
    private void UpdateBindings(Change<TViewModel> change)
    {
        switch (change.Reason)
        {
            case ListChangeReason.Add:
                {
                    NotifyItemInserted(change.Item.CurrentIndex);
                    break;
                }

            case ListChangeReason.Remove:
                {
                    NotifyItemRemoved(change.Item.CurrentIndex);
                    break;
                }

            case ListChangeReason.Moved:
                {
                    NotifyItemMoved(change.Item.PreviousIndex, change.Item.CurrentIndex);
                    break;
                }

            case ListChangeReason.Replace:
            case ListChangeReason.Refresh:
                {
                    NotifyItemChanged(change.Item.CurrentIndex);
                    break;
                }

            case ListChangeReason.AddRange:
                {
                    NotifyItemRangeInserted(change.Range.Index, change.Range.Count);
                    break;
                }

            case ListChangeReason.RemoveRange:
            case ListChangeReason.Clear:
                {
                    NotifyItemRangeRemoved(change.Range.Index, change.Range.Count);
                    break;
                }
        }
    }
}
