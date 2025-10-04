// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;

namespace ReactiveUI;

/// <summary>
/// Adds an AOT-friendly mapping configuration to DefaultViewLocator so callers can avoid reflection.
/// </summary>
public sealed partial class DefaultViewLocator
{
    // Keyed by (ViewModelType, Contract). Empty string represents default contract.
    private readonly ConcurrentDictionary<(Type vmType, string contract), Func<IViewFor>> _aotMappings = new();

    /// <summary>
    /// Registers a direct mapping from a view model type to a view factory.
    /// This avoids reflection-based name lookup, improving AOT and trimming support.
    /// </summary>
    /// <typeparam name="TViewModel">View model type.</typeparam>
    /// <typeparam name="TView">View type.</typeparam>
    /// <param name="factory">Factory that builds the view.</param>
    /// <param name="contract">Optional contract used to disambiguate views.</param>
    /// <returns>The locator for chaining.</returns>
#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Mapping does not use reflection")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Mapping does not use dynamic code")]
#endif
    public DefaultViewLocator Map<TViewModel, TView>(Func<TView> factory, string? contract = null)
        where TViewModel : class
        where TView : class, IViewFor<TViewModel>
    {
        factory.ArgumentNullExceptionThrowIfNull(nameof(factory));
        _aotMappings[(typeof(TViewModel), contract ?? string.Empty)] = () => factory();
        return this;
    }

    /// <summary>
    /// Clears a previously registered mapping for an optional contract.
    /// </summary>
    /// <typeparam name="TViewModel">View model type.</typeparam>
    /// <param name="contract">Optional contract to unmap.</param>
    /// <returns>The locator for chaining.</returns>
    public DefaultViewLocator Unmap<TViewModel>(string? contract = null)
        where TViewModel : class
    {
        _ = _aotMappings.TryRemove((typeof(TViewModel), contract ?? string.Empty), out _);
        return this;
    }

    private IViewFor? TryResolveAOTMapping(Type viewModelType, string? contract)
    {
        // Try exact contract
        if (_aotMappings.TryGetValue((viewModelType, contract ?? string.Empty), out var f))
        {
            return f();
        }

        // Fallback to default contract if a specific contract was requested
        if (!string.IsNullOrEmpty(contract) && _aotMappings.TryGetValue((viewModelType, string.Empty), out var fDefault))
        {
            return fDefault();
        }

        return null;
    }
}
