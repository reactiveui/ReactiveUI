// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.BlazorServer.Models;

/// <summary>Notification that the chat state has changed and observers should refresh.</summary>
public sealed record ChatStateChanged
{
    /// <summary>Gets the moment, in UTC, at which the chat state change was signalled.</summary>
    public DateTimeOffset Timestamp { get; init; } = TimeProvider.System.GetUtcNow();
}
