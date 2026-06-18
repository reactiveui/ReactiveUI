// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Builder.WpfApp.Models;

/// <summary>Notification that the chat state has changed and observers should refresh.</summary>
public sealed record ChatStateChanged
{
    /// <summary>Gets the moment, in UTC, at which the chat state change was signalled.</summary>
    [SuppressMessage("Major Code Smell", "S6354:Use a testable date/time provider", Justification = "Not available all TFMs")]
    public DateTimeOffset Timestamp { get; init; } =
#if NET8_0_OR_GREATER
        TimeProvider.System.GetUtcNow();
#else
        DateTimeOffset.UtcNow;
#endif
}
