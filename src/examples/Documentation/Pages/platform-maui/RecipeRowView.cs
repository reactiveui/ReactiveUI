// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>A row in the recipe list, showing a recipe's name and category with <see cref="ReactiveTextItemView{TViewModel}"/>.</summary>
[System.Diagnostics.DebuggerDisplay("RecipeRowView")]
public sealed class RecipeRowView : ReactiveTextItemView<RecipeItem>;
