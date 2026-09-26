// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>A row in the recipe list with a photo, using <see cref="ReactiveImageItemView{TViewModel}"/>.</summary>
[System.Diagnostics.DebuggerDisplay("RecipeImageRowView")]
public sealed class RecipeImageRowView : ReactiveImageItemView<RecipeItem>;
