// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.WpfApp.Services;

/// <summary>
/// Network message payload used to broadcast chat messages.
/// </summary>
public sealed class ChatNetworkMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatNetworkMessage"/> class.
    /// </summary>
    public ChatNetworkMessage()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatNetworkMessage" /> class with values.
    /// </summary>
    /// <param name="roomId">The unique identifier for the room.</param>
    /// <param name="roomName">The human-readable room name used as the MessageBus contract.</param>
    /// <param name="sender">The sender name.</param>
    /// <param name="text">The message text.</param>
    /// <param name="timestamp">The message timestamp.</param>
    public ChatNetworkMessage(string roomId, string roomName, string sender, string text, DateTimeOffset timestamp)
    {
        RoomId = roomId;
        RoomName = roomName;
        Sender = sender;
        Text = text;
        Timestamp = timestamp;
    }

    /// <summary>
    /// Gets or sets the room ID.
    /// </summary>
    public string RoomId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the room name.
    /// </summary>
    public string RoomName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sender.
    /// </summary>
    public string Sender { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message text.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the originating app instance id.
    /// </summary>
    public Guid InstanceId { get; set; }
}
