// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.RoutingDynamicData;

/// <summary>The page that shows the detail of one product.</summary>
/// <param name="hostScreen">The screen that owns the router this page navigates through.</param>
/// <param name="productName">The name of the product shown.</param>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}")]
public sealed class ProductDetailPageViewModel(IScreen hostScreen, string productName) : ReactiveObject, IRoutableViewModel
{
    /// <inheritdoc/>
    public string? UrlPathSegment => $"inventory/{ProductName}";

    /// <inheritdoc/>
    public IScreen HostScreen { get; } = hostScreen;

    /// <summary>Gets the name of the product shown.</summary>
    public string ProductName { get; } = productName;
}
