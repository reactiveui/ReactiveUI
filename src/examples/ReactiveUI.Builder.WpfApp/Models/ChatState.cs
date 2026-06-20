// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Builder.WpfApp.Models;

/// <summary>The persisted chat application state.</summary>
public class ChatState
{
    /// <summary>Gets or sets the available rooms.</summary>
    [SuppressMessage("Major Code Smell", "S4004:Collection properties should be read only", Justification = "Public setter required for System.Text.Json deserialization of persisted state.")]
    public List<ChatRoom> Rooms { get; set; } = [];

    /// <summary>Gets or sets the local user's display name.</summary>
    public string? DisplayName { get; set; }
}
