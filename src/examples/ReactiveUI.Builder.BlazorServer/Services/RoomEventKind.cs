// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.BlazorServer.Services;

/// <summary>
/// The type of room event.
/// </summary>
public enum RoomEventKind
{
    /// <summary>
    /// A new room was created.
    /// </summary>
    Add,

    /// <summary>
    /// A room was removed.
    /// </summary>
    Remove,

    /// <summary>
    /// Request others to broadcast their current rooms.
    /// </summary>
    SyncRequest,
}
