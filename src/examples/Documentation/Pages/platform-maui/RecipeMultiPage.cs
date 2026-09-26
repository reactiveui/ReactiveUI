// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;
using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// A <see cref="ReactiveMultiPage{TPage, TViewModel}"/> over <see cref="ContentPage"/>, the base <see cref="ReactiveTabbedPage{TViewModel}"/>
/// itself builds on. Like <see cref="RecipeTabbedPage"/>, it needs a dispatcher to construct, so
/// <see cref="TabbedPageAndMultiPageExamples"/> never runs it headless.
/// </summary>
[System.Diagnostics.DebuggerDisplay("RecipeMultiPage")]
public sealed class RecipeMultiPage : ReactiveMultiPage<ContentPage, RecipeListViewModel>
{
    /// <inheritdoc/>
    protected override ContentPage CreateDefault(object item) => new();
}
