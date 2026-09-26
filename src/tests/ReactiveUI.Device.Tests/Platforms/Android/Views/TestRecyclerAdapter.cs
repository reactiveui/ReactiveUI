// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;
using AndroidX.RecyclerView.Widget;
using ReactiveUI.AndroidX;

namespace ReactiveUI.Device.Tests;

/// <summary>A reactive RecyclerView adapter that shows each view model in an <c>item_layout</c> row.</summary>
/// <param name="items">The change-set stream of the rows.</param>
public sealed class TestRecyclerAdapter(IObservable<IReactiveChangeSet<TestViewModel>> items) : ReactiveRecyclerViewAdapter<TestViewModel>(items)
{
    /// <inheritdoc/>
    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        ArgumentNullException.ThrowIfNull(parent);

        var view = LayoutInflater.From(parent.Context)!.Inflate(Resource.Layout.item_layout, parent, false)!;
        return new TestViewHolder(view);
    }
}
