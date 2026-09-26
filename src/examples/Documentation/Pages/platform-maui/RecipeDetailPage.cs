// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>The view for <see cref="RecipeDetailViewModel"/>. The view locator finds it because it implements <see cref="IViewFor{T}"/>.</summary>
[DisableAnimation]
[System.Diagnostics.DebuggerDisplay("RecipeDetailPage")]
public sealed class RecipeDetailPage : ReactiveContentPage<RecipeDetailViewModel>;
