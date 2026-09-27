// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>A <see cref="ViewModelViewHost{TViewModel}"/> for <see cref="RecipeListViewModel"/> that counts each resolution.</summary>
[DebuggerDisplay("LoggingRecipeViewHost: {ResolutionCount} resolutions")]
public sealed class LoggingRecipeViewHost : ViewModelViewHost<RecipeListViewModel>
{
    /// <summary>Gets the number of times this host has resolved a view.</summary>
    public int ResolutionCount { get; private set; }

    /// <inheritdoc/>
    protected override void ResolveViewForViewModel(object? viewModel, string? contract)
    {
        ResolutionCount++;
        base.ResolveViewForViewModel(viewModel, contract);
    }
}
