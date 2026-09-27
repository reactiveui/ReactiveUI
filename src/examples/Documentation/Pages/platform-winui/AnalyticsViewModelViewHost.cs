// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// A <see cref="ViewModelViewHost"/> that records which view it resolved for each reading, standing in for the
/// dashboard's usage analytics. It overrides <c>ResolveViewForViewModel</c> to run the base resolution first, then
/// note the view it chose.
/// </summary>
[System.Diagnostics.DebuggerDisplay("AnalyticsViewModelViewHost")]
public sealed class AnalyticsViewModelViewHost : ViewModelViewHost
{
    /// <summary>Gets the resolved view names recorded so far, in order.</summary>
    public List<string> ResolvedViews { get; } = [];

    /// <inheritdoc/>
    protected override void ResolveViewForViewModel(object? viewModel, string? contract)
    {
        base.ResolveViewForViewModel(viewModel, contract);
        ResolvedViews.Add(Content?.GetType().Name ?? "(nothing)");
    }
}
