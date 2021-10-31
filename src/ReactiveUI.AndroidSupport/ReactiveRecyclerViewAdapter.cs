// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Android.Support.V7.Widget;
using Android.Views;
using DynamicData;
using DynamicData.Binding;

namespace ReactiveUI.AndroidSupport;

/// <summary>
/// An adapter for the Android <see cref="RecyclerView"/>.
/// Override the <see cref="RecyclerView.Adapter.CreateViewHolder(ViewGroup, int)"/> method
/// to create the your <see cref="ReactiveRecyclerViewViewHolder{TViewModel}"/> based ViewHolder.
/// </summary>
/// <typeparam name="TViewModel">The type of ViewModel that this adapter holds.</typeparam>
public abstract class ReactiveRecyclerViewAdapter<TViewModel> : RecyclerView.Adapter
    where TViewModel : class, IReactiveObject
{
    private readonly ISourceList<TViewModel> _list;

    private readonly IDisposable _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveRecyclerViewAdapter{TViewModel}"/> class.
    /// </summary>
    /// <param name="backingList">The backing list.</param>
    protected ReactiveRecyclerViewAdapter(IObservable<IChangeSet<TViewModel>> backingList)
    {
        _list = new SourceList<TViewModel>(backingList);

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
        if (holder is null)
        {
            throw new ArgumentNullException(nameof(holder));
        }

        if (!(holder is IViewFor viewForHolder))
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

    private TViewModel? GetViewModelByPosition(int position) => position >= _list.Count ? null : _list.Items.ElementAt(position);

    private void UpdateBindings(Change<TViewModel> change)
    {
        switch (change.Reason)
        {
            case ListChangeReason.Add:
                NotifyItemInserted(change.Item.CurrentIndex);
                break;
            case ListChangeReason.Remove:
                NotifyItemRemoved(change.Item.CurrentIndex);
                break;
            case ListChangeReason.Moved:
                NotifyItemMoved(change.Item.PreviousIndex, change.Item.CurrentIndex);
                break;
            case ListChangeReason.Replace:
            case ListChangeReason.Refresh:
                NotifyItemChanged(change.Item.CurrentIndex);
                break;
            case ListChangeReason.AddRange:
                NotifyItemRangeInserted(change.Range.Index, change.Range.Count);
                break;
            case ListChangeReason.RemoveRange:
            case ListChangeReason.Clear:
                NotifyItemRangeRemoved(change.Range.Index, change.Range.Count);
                break;
        }
    }
}

/// <summary>
/// An adapter for the Android <see cref="RecyclerView"/>.
/// Override the <see cref="RecyclerView.Adapter.CreateViewHolder(ViewGroup, int)"/> method
/// to create the your <see cref="ReactiveRecyclerViewViewHolder{TViewModel}"/> based ViewHolder.
/// </summary>
/// <typeparam name="TViewModel">The type of ViewModel that this adapter holds.</typeparam>
/// <typeparam name="TCollection">The type of collection.</typeparam>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
public abstract class ReactiveRecyclerViewAdapter<TViewModel, TCollection> : ReactiveRecyclerViewAdapter<TViewModel>
    where TViewModel : class, IReactiveObject
    where TCollection : ICollection<TViewModel>, INotifyCollectionChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveRecyclerViewAdapter{TViewModel, TCollection}"/> class.
    /// </summary>
    /// <param name="backingList">The backing list.</param>
    protected ReactiveRecyclerViewAdapter(TCollection backingList)
        : base(backingList.ToObservableChangeSet<TCollection, TViewModel>())
    {
    }
}