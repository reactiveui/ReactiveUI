// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// The reason a single <see cref="ReactiveChange{T}"/> occurred. Range and reset operations are flattened to one
/// change per affected item, so this is a per-item reason rather than a batch reason.
/// </summary>
public enum ReactiveChangeReason
{
    /// <summary>An item was added to the collection.</summary>
    Add,

    /// <summary>An item was removed from the collection (a reset is reported as one remove per prior item).</summary>
    Remove,

    /// <summary>An item replaced an existing item at the same index.</summary>
    Replace,

    /// <summary>An item moved from one index to another (count is unchanged).</summary>
    Move,

    /// <summary>An item signalled that it should be re-evaluated without being added or removed.</summary>
    Refresh,
}
