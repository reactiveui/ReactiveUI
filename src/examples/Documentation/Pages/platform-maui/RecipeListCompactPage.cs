// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// A second view for <see cref="RecipeListViewModel"/>, registered under the "Compact" contract. A phone in landscape
/// picks this one; the default view picks the other.
/// </summary>
[System.Diagnostics.DebuggerDisplay("RecipeListCompactPage")]
public sealed class RecipeListCompactPage : ReactiveContentPage<RecipeListViewModel>;
