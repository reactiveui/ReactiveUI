// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>
/// A view locator that asks another locator first and shows a placeholder when it has no view, so a screen that is
/// still being built shows "coming soon" rather than nothing.
/// </summary>
/// <param name="inner">The locator to ask first.</param>
public sealed class PlaceholderViewLocator(IViewLocator inner) : IViewLocator
{
    /// <inheritdoc/>
    public IViewFor? ResolveView<TViewModel>(TViewModel viewModel, string? contract)
        where TViewModel : class =>
        inner.ResolveView(viewModel, contract) ?? new PlaceholderView { ViewModel = viewModel };

    /// <inheritdoc/>
    [RequiresDynamicCode("Resolves a view from the view model's runtime type.")]
    public IViewFor? ResolveView(object? viewModel, string? contract) =>
        inner.ResolveView(viewModel, contract) ?? new PlaceholderView { ViewModel = viewModel };
}
