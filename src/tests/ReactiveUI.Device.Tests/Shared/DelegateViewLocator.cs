// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Device.Tests;

/// <summary>A view locator that resolves views through a delegate and records the contracts it was asked for.</summary>
/// <param name="resolve">Creates the view for a view model and contract.</param>
public sealed class DelegateViewLocator(Func<object, string?, IViewFor?> resolve) : IViewLocator
{
    /// <summary>Gets the contracts passed to the locator, oldest first.</summary>
    public List<string?> Contracts { get; } = [];

    /// <inheritdoc/>
    public IViewFor? ResolveView<TViewModel>(TViewModel viewModel, string? contract)
        where TViewModel : class => Resolve(viewModel, contract);

    /// <inheritdoc/>
    [RequiresDynamicCode("Resolves a view from the view model's runtime type.")]
    public IViewFor? ResolveView(object? viewModel, string? contract) => Resolve(viewModel, contract);

    /// <summary>Records the contract and resolves the view.</summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The contract.</param>
    /// <returns>The view, or <see langword="null"/> for a null view model.</returns>
    private IViewFor? Resolve(object? viewModel, string? contract)
    {
        Contracts.Add(contract);
        return viewModel is null ? null : resolve(viewModel, contract);
    }
}
