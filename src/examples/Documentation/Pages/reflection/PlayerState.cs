// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Reflection;

/// <summary>The shell state a music player keeps across a suspend and resume: what track was playing, and where.</summary>
/// <param name="Track">The track that was playing.</param>
/// <param name="PositionSeconds">How far into the track playback had reached.</param>
[System.Diagnostics.DebuggerDisplay("Track = {Track}, PositionSeconds = {PositionSeconds}")]
public sealed record PlayerState(string Track, int PositionSeconds);
