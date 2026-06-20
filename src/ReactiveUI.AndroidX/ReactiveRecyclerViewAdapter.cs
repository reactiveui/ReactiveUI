// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AndroidX.RecyclerView.Widget;
using ReactiveUI.Internal;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.AndroidX;
#else
namespace ReactiveUI.AndroidX;
#endif
/// <summary>An adapter for the Android <see cref="RecyclerView"/>.</summary>
/// <typeparam name="TViewModel">The type of ViewModel that this adapter holds.</typeparam>
public abstract class ReactiveRecyclerViewAdapter<TViewModel> : RecyclerView.Adapter
    where TViewModel : class, IReactiveObject
{
    /// <summary>The materialized change-set binding that backs the adapter.</summary>
    private readonly ChangeSetBinder<TViewModel> _binder;

    /// <summary>Initializes a new instance of the <see cref="ReactiveRecyclerViewAdapter{TViewModel}"/> class.</summary>
    /// <param name="backingList">The backing list.</param>
    protected ReactiveRecyclerViewAdapter(IObservable<IReactiveChangeSet<TViewModel>> backingList) =>
        _binder = new(backingList, onChange: UpdateBindings);

    /// <inheritdoc/>
    public override int ItemCount => _binder.Count;

    /// <inheritdoc/>
    public override int GetItemViewType(int position) => GetItemViewType(position, GetViewModelByPosition(position));

    /// <summary>Determine the View that will be used/re-used in lists where the list contains different cell designs.</summary>
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
            _binder.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>Gets the view model at the specified position, or null if the position is out of range.</summary>
    /// <param name="position">The position in the list.</param>
    /// <returns>The view model at the position, or null.</returns>
    private TViewModel? GetViewModelByPosition(int position) => position >= _binder.Count ? null : _binder[position];

    /// <summary>
    /// Raises fine-grained <see cref="RecyclerView.Adapter"/> notifications in response to a single change.
    /// Range and reset operations are flattened to one change per item upstream, so only the per-item
    /// notifications are needed here.
    /// </summary>
    /// <param name="change">The change to apply.</param>
    private void UpdateBindings(ReactiveChange<TViewModel> change)
    {
        switch (change.Reason)
        {
            case ReactiveChangeReason.Add:
                {
                    NotifyItemInserted(change.CurrentIndex);
                    break;
                }

            case ReactiveChangeReason.Remove:
                {
                    NotifyItemRemoved(change.CurrentIndex);
                    break;
                }

            case ReactiveChangeReason.Move:
                {
                    NotifyItemMoved(change.PreviousIndex, change.CurrentIndex);
                    break;
                }

            case ReactiveChangeReason.Replace or ReactiveChangeReason.Refresh:
                {
                    NotifyItemChanged(change.CurrentIndex);
                    break;
                }
        }
    }
}
