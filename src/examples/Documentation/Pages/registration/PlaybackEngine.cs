// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>A playback engine. Each instance gets the next number, so the examples can tell instances apart.</summary>
[System.Diagnostics.DebuggerDisplay("PlaybackEngine InstanceId = {InstanceId}")]
public sealed class PlaybackEngine : IPlaybackEngine
{
    /// <summary>The number handed to the next engine that is created.</summary>
    private static int _nextInstanceId = 1;

    /// <summary>Initializes a new instance of the <see cref="PlaybackEngine"/> class.</summary>
    public PlaybackEngine()
    {
        InstanceId = _nextInstanceId;
        _nextInstanceId++;
    }

    /// <inheritdoc/>
    public int InstanceId { get; }
}
