// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Device.Tests;

/// <summary>A routable view model the routed-host tests navigate to.</summary>
/// <param name="hostScreen">The screen that owns the router.</param>
/// <param name="urlPathSegment">The segment the host shows as the title.</param>
public sealed class TestRoutableViewModel(IScreen hostScreen, string urlPathSegment) : ReactiveObject, IRoutableViewModel
{
    /// <inheritdoc/>
    public string? UrlPathSegment { get; } = urlPathSegment;

    /// <inheritdoc/>
    public IScreen HostScreen { get; } = hostScreen;
}
