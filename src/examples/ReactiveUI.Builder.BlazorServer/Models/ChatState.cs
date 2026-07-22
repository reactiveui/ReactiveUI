// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.BlazorServer.Models;

/// <summary>The persisted chat application state.</summary>
public class ChatState
{
    /// <summary>Gets the available rooms.</summary>
    public List<ChatRoom> Rooms { get; } = [];

    /// <summary>Gets or sets the local user's display name.</summary>
    public string? DisplayName { get; set; }
}
