// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// The shopping list for the recipes a cook picked. It comes from a shopping plug-in whose views are registered only
/// with the service locator.
/// </summary>
/// <param name="hostScreen">The window the list is shown in.</param>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}")]
public sealed class ShoppingListViewModel(IScreen hostScreen) : ReactiveObject, IRoutableViewModel
{
    /// <inheritdoc/>
    public string UrlPathSegment => "shopping-list";

    /// <inheritdoc/>
    public IScreen HostScreen { get; } = hostScreen;
}
