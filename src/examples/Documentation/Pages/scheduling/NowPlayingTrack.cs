// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Scheduling;

/// <summary>A track a music player's now-playing subject can announce.</summary>
/// <param name="Title">The track title.</param>
/// <param name="Artist">The performing artist.</param>
[System.Diagnostics.DebuggerDisplay("{Title} - {Artist}")]
public sealed record NowPlayingTrack(string Title, string Artist);
