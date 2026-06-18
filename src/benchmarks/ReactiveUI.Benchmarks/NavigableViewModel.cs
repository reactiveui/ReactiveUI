// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Benchmarks;

/// <summary>A minimal routable view model used to drive <see cref="RoutingState"/> navigation benchmarks.</summary>
internal sealed class NavigableViewModel : ReactiveObject, IRoutableViewModel
{
    /// <inheritdoc/>
    public string? UrlPathSegment => "bench";

    /// <inheritdoc/>
    public IScreen HostScreen => null!;
}
