// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>A <see cref="ViewModelViewHost"/> that records the view model type it resolved a view for each time.</summary>
[DebuggerDisplay("LoggingViewModelViewHost: {ResolvedViewModelTypes.Count} resolved")]
public sealed class LoggingViewModelViewHost : ViewModelViewHost
{
    /// <summary>Gets the view model type names resolved so far, in order.</summary>
    public List<string> ResolvedViewModelTypes { get; } = [];

    /// <inheritdoc/>
    protected override void ResolveViewForViewModel(object? viewModel, string? contract)
    {
        if (viewModel is not null)
        {
            ResolvedViewModelTypes.Add(viewModel.GetType().Name);
        }

        base.ResolveViewForViewModel(viewModel, contract);
    }
}
