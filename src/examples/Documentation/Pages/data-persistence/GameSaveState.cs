// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.DataPersistence;

/// <summary>The whole state of a game session, saved and restored across launches.</summary>
/// <param name="Level">The level the player has reached.</param>
/// <param name="Score">The player's score.</param>
[System.Diagnostics.DebuggerDisplay("Level {Level}, Score {Score}")]
public sealed record GameSaveState(int Level, int Score);
