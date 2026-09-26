// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>A view locator that always resolves the wide layout, ignoring the requested contract.</summary>
[System.Diagnostics.DebuggerDisplay("AlwaysWideViewLocator")]
public sealed class AlwaysWideViewLocator : IViewLocator
{
    /// <inheritdoc/>
    public IViewFor? ResolveView<TViewModel>(TViewModel viewModel, string? contract)
        where TViewModel : class =>
        viewModel is RecipeListViewModel ? new RecipeListWideContentView() : null;

    /// <inheritdoc/>
    public IViewFor? ResolveView(object? viewModel, string? contract) =>
        viewModel is RecipeListViewModel ? new RecipeListWideContentView() : null;

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Matches IViewLocator.ResolveViewUnsafe; this locator itself builds no types at run time.")]
    public IViewFor? ResolveViewUnsafe(object? viewModel, string? contract) =>
        ResolveView(viewModel, contract);
}
