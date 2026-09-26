// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>The real clock, backed by a <see cref="TimeProvider"/> so a test can supply its own.</summary>
/// <param name="timeProvider">The time provider the clock reads from.</param>
[System.Diagnostics.DebuggerDisplay("PantryClock Now = {Now}")]
public sealed class PantryClock(TimeProvider timeProvider) : IPantryClock
{
    /// <summary>Initializes a new instance of the <see cref="PantryClock"/> class using the system time.</summary>
    public PantryClock()
        : this(TimeProvider.System)
    {
    }

    /// <inheritdoc/>
    public DateTimeOffset Now => timeProvider.GetUtcNow();
}
