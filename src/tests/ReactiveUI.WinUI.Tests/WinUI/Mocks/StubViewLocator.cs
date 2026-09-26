// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.WinUI.Mocks;

/// <summary>A view locator whose answers are configured per test and whose requests are recorded.</summary>
/// <remarks>
/// The view hosts ask for a view with the current contract first and, when that yields nothing, fall back to a
/// contract-free lookup. Recording every requested contract lets a test assert which of those two lookups ran.
/// </remarks>
public sealed class StubViewLocator : IViewLocator
{
    /// <summary>Gets the view returned when no contract is supplied.</summary>
    /// <value>The contract-free view, or <see langword="null"/> to resolve nothing.</value>
    public IViewFor? ContractlessView { get; init; }

    /// <summary>Gets the contract whose lookup returns <see cref="ContractView"/>.</summary>
    public string? Contract { get; init; }

    /// <summary>Gets the view returned for <see cref="Contract"/>.</summary>
    public IViewFor? ContractView { get; init; }

    /// <summary>Gets the contracts this locator has been asked about, in order.</summary>
    public List<string?> RequestedContracts { get; } = [];

    /// <summary>Gets the number of lookups made through the ahead-of-time safe run-time-type overload.</summary>
    public int RuntimeTypeLookups { get; private set; }

    /// <summary>Gets the number of lookups made through the reflective run-time-type overload.</summary>
    public int UnsafeLookups { get; private set; }

    /// <inheritdoc/>
    public IViewFor? ResolveView<TViewModel>(TViewModel viewModel, string? contract)
        where TViewModel : class => Resolve(contract);

    /// <inheritdoc/>
    public IViewFor? ResolveView(object? viewModel, string? contract)
    {
        RuntimeTypeLookups++;
        return Resolve(contract);
    }

    /// <inheritdoc/>
    [RequiresDynamicCode("Resolves a view from the view model's runtime type.")]
    public IViewFor? ResolveViewUnsafe(object? viewModel, string? contract)
    {
        UnsafeLookups++;
        return Resolve(contract);
    }

    /// <summary>Records the request and returns the view configured for the contract.</summary>
    /// <param name="contract">The requested contract.</param>
    /// <returns>The configured view, or <see langword="null"/> when nothing matches.</returns>
    private IViewFor? Resolve(string? contract)
    {
        RequestedContracts.Add(contract);

        if (contract is null)
        {
            return ContractlessView;
        }

        return string.Equals(contract, Contract, StringComparison.Ordinal) ? ContractView : null;
    }
}
