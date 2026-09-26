// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// A <see cref="ReactiveTabbedPage{TViewModel}"/> for the recipe book's tab bar. .NET MAUI's <c>TabbedPage</c> asks
/// the current thread for a dispatcher as soon as it is built, so this type only appears in
/// <see cref="TabbedPageAndMultiPageExamples"/>, which the page never runs headless.
/// </summary>
[System.Diagnostics.DebuggerDisplay("RecipeTabbedPage")]
public sealed class RecipeTabbedPage : ReactiveTabbedPage<RecipeListViewModel>;
