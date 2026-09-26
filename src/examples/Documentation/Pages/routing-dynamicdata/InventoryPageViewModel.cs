// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.RoutingDynamicData;

/// <summary>The page that lists every product in the shop.</summary>
/// <param name="hostScreen">The screen that owns the router this page navigates through.</param>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}")]
public sealed class InventoryPageViewModel(IScreen hostScreen) : ReactiveObject, IRoutableViewModel
{
    /// <inheritdoc/>
    public string? UrlPathSegment => "inventory";

    /// <inheritdoc/>
    public IScreen HostScreen { get; } = hostScreen;
}
