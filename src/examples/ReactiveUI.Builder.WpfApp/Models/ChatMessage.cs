// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Builder.WpfApp.Models;

/// <summary>A single chat message.</summary>
public class ChatMessage
{
    /// <summary>Gets or sets the sender name.</summary>
    public string Sender { get; set; } = string.Empty;

    /// <summary>Gets or sets the message text.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Gets or sets the timestamp.</summary>
    [SuppressMessage("Major Code Smell", "S6354:Use a testable date/time provider", Justification = "Not available all TFMs")]
    public DateTimeOffset Timestamp { get; set; } =
#if NET8_0_OR_GREATER
        TimeProvider.System.GetUtcNow();
#else
        DateTimeOffset.Now;
#endif
}
