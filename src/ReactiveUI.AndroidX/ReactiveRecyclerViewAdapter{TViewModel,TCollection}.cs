// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;

using AndroidX.RecyclerView.Widget;

using DynamicData;
using DynamicData.Binding;

namespace ReactiveUI.AndroidX;

/// <summary>
/// An adapter for the Android <see cref="RecyclerView"/>.
/// </summary>
/// <typeparam name="TViewModel">The type of ViewModel that this adapter holds.</typeparam>
/// <typeparam name="TCollection">The type of collection.</typeparam>
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