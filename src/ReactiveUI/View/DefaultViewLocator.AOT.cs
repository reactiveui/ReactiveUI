// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Adds an AOT-friendly mapping configuration to DefaultViewLocator so callers can avoid reflection.
/// </summary>
public sealed partial class DefaultViewLocator
{
    private readonly ConcurrentDictionary<Type, Func<object?>> _aotMappings = new();

    /// <summary>
    /// Registers a direct mapping from a view model type to a view factory.
    /// This avoids reflection-based name lookup, improving AOT and trimming support.
    /// </summary>
    /// <typeparam name="TViewModel">View model type.</typeparam>
    /// <typeparam name="TView">View type.</typeparam>
    /// <param name="factory">Factory that builds the view.</param>
    /// <param name="contract">Optional contract (unused here; included for future parity with Splat locator).</param>
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
        _aotMappings[typeof(TViewModel)] = () => factory();
        return this;
    }

    /// <summary>
    /// Clears a previously registered mapping.
    /// </summary>
    /// <typeparam name="TViewModel">View model type.</typeparam>
    /// <returns>The locator for chaining.</returns>
    public DefaultViewLocator Unmap<TViewModel>()
        where TViewModel : class
    {
        _aotMappings.TryRemove(typeof(TViewModel), out _);
        return this;
    }

    private IViewFor? TryResolveAOTMapping(Type viewModelType)
    {
        if (_aotMappings.TryGetValue(viewModelType, out var f))
        {
            var v = f();
            return v as IViewFor;
        }

        return null;
    }
}
