// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Collections;

/// <summary>One row on the game's leaderboard.</summary>
/// <param name="Name">The player's name.</param>
/// <param name="Score">The player's score.</param>
[System.Diagnostics.DebuggerDisplay("{Name}: {Score}")]
public sealed record Player(string Name, int Score);
