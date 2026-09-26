// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Primitives.Concurrency;

namespace ReactiveUI.Documentation.Scheduling;

/// <summary>A unit of work an <see cref="ISequencer"/> runs directly, without going through a lambda.</summary>
/// <param name="trackTitle">The track to announce when the scheduler runs this item.</param>
public sealed class NowPlayingWorkItem(string trackTitle) : IWorkItem
{
    /// <inheritdoc/>
    public void Execute() => Console.WriteLine($"Now playing: {trackTitle}");
}
