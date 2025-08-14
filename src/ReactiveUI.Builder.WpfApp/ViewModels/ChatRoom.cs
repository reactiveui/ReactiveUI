// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace ReactiveUI.Builder.WpfApp.ViewModels;

/// <summary>
/// Represents a chat room with messages and members.
/// </summary>
public class ChatRoom
{
    /// <summary>
    /// Gets or sets the room id.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Gets or sets the room name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the messages in the room.
    /// </summary>
    public ObservableCollection<ChatMessage> Messages { get; set; } = new();

    /// <summary>
    /// Gets or sets the members in the room.
    /// </summary>
    public List<string> Members { get; set; } = new();
}
