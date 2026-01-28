// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.BlazorServer.Services;

namespace ReactiveUI.Builder.BlazorServer.Models;

/// <summary>
/// Network event describing a change in the rooms list.
/// </summary>
public sealed class RoomEventMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoomEventMessage"/> class.
    /// </summary>
    public RoomEventMessage()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoomEventMessage"/> class.
    /// </summary>
    /// <param name="kind">The event kind.</param>
    /// <param name="roomName">The room name.</param>
    public RoomEventMessage(RoomEventKind kind, string roomName)
    {
        Kind = kind;
        RoomName = roomName;
    }

    /// <summary>
    /// Gets or sets the event kind.
    /// </summary>
    public RoomEventKind Kind { get; set; }

    /// <summary>
    /// Gets or sets the room name for this event.
    /// </summary>
    public string RoomName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the originating instance id.
    /// </summary>
    public Guid InstanceId { get; set; }

    /// <summary>
    /// Gets or sets the current snapshot of room names. Used in response to SyncRequest.
    /// </summary>
    public List<string>? Snapshot { get; set; }
}
