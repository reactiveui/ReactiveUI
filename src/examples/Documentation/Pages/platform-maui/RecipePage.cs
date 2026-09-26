// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>A <see cref="ReactivePage{TViewModel}"/>, the older <c>Page</c> wrapper <see cref="ReactiveContentPage{TViewModel}"/> replaced.</summary>
[System.Diagnostics.DebuggerDisplay("RecipePage")]
public sealed class RecipePage : ReactivePage<RecipeListViewModel>;
