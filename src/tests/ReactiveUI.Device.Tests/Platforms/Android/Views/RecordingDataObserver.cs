// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AndroidX.RecyclerView.Widget;

namespace ReactiveUI.Device.Tests;

/// <summary>Records the change notifications a RecyclerView adapter raises, as text such as <c>insert 2 1</c>.</summary>
public sealed class RecordingDataObserver : RecyclerView.AdapterDataObserver
{
    /// <summary>Gets the recorded notifications, oldest first.</summary>
    public List<string> Events { get; } = [];

    /// <inheritdoc/>
    public override void OnChanged() => Events.Add("reset");

    /// <inheritdoc/>
    public override void OnItemRangeInserted(int positionStart, int itemCount) => Events.Add($"insert {positionStart} {itemCount}");

    /// <inheritdoc/>
    public override void OnItemRangeRemoved(int positionStart, int itemCount) => Events.Add($"remove {positionStart} {itemCount}");

    /// <inheritdoc/>
    public override void OnItemRangeMoved(int fromPosition, int toPosition, int itemCount) => Events.Add($"move {fromPosition} {toPosition} {itemCount}");

    /// <inheritdoc/>
    public override void OnItemRangeChanged(int positionStart, int itemCount) => Events.Add($"change {positionStart} {itemCount}");

    /// <inheritdoc/>
    public override void OnItemRangeChanged(int positionStart, int itemCount, Java.Lang.Object? payload) => Events.Add($"change {positionStart} {itemCount}");
}
