// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Splat;

namespace ReactiveUI;

/// <summary>Default AOT-compatible implementation of <see cref="IViewLocator"/> that resolves views using compile-time registrations.</summary>
/// <remarks>
/// <para>
/// This locator uses explicit view-to-viewmodel mappings registered via <c>Map</c>.
/// When no mapping is found, it falls back to querying the service locator for <c>IViewFor&lt;TViewModel&gt;</c>.
/// </para>
/// <para>
/// This implementation is fully AOT-compatible and does not use reflection or runtime type discovery.
/// All view-viewmodel associations must be registered at application startup.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// // Register views at startup
/// var locator = new DefaultViewLocator();
/// locator.Map<LoginViewModel, LoginView>(() => new LoginView())
///        .Map<MainViewModel, MainView>(() => new MainView())
///        .Map<SettingsViewModel, SettingsView>(() => new SettingsView());
///
/// // Resolve at runtime (fully AOT-compatible)
/// var view = locator.ResolveView<LoginViewModel>();
/// ]]>
/// </code>
/// </example>
[System.Diagnostics.DebuggerDisplay("Mappings = {_mappings.Count}")]
public sealed class DefaultViewLocator : IViewLocator
{
    /// <summary>Lock object for synchronizing writes to _mappings.</summary>
#if NET9_0_OR_GREATER
    private readonly Lock _gate = new();
#else
    private readonly object _gate = new();
#endif

    /// <summary>
    /// Cache for MakeGenericType calls in ResolveView(object).
    /// Key: ViewModelType, Value: IViewFor&lt;ViewModelType&gt; interface type.
    /// </summary>
    private readonly ConcurrentDictionary<Type, Type> _viewForTypeCache = new();

    /// <summary>
    /// Snapshot pattern: Readers access this volatile reference without locking.
    /// Writers lock, clone, mutate, and swap the reference.
    /// Keyed by (ViewModelType, Contract). Empty string represents default contract.
    /// </summary>
    private Dictionary<(Type ViewModelType, string Contract), Func<IViewFor>> _mappings = [];

    /// <summary>Registers a direct mapping from a view model type to a view factory using the default contract.</summary>
    /// <typeparam name="TViewModel">View model type.</typeparam>
    /// <typeparam name="TView">View type that implements IViewFor&lt;TViewModel&gt;.</typeparam>
    /// <param name="factory">Factory function that creates the view instance.</param>
    /// <returns>The locator for chaining.</returns>
    [SuppressMessage(
        "Design",
        "SST2307:A generic method's type parameter appears in no parameter, so no caller can infer it",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public DefaultViewLocator Map<TViewModel, TView>(Func<TView> factory)
        where TViewModel : class
        where TView : class, IViewFor<TViewModel> =>
        Map<TViewModel, TView>(factory, null);

    /// <summary>
    /// Registers a direct mapping from a view model type to a view factory.
    /// This is the recommended way to register views for AOT-compatible applications.
    /// </summary>
    /// <typeparam name="TViewModel">View model type.</typeparam>
    /// <typeparam name="TView">View type that implements IViewFor&lt;TViewModel&gt;.</typeparam>
    /// <param name="factory">Factory function that creates the view instance.</param>
    /// <param name="contract">Optional contract used to disambiguate multiple views for the same view model.</param>
    /// <returns>The locator for chaining.</returns>
    [SuppressMessage(
        "Design",
        "SST2307:A generic method's type parameter appears in no parameter, so no caller can infer it",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public DefaultViewLocator Map<TViewModel, TView>(Func<TView> factory, string? contract)
        where TViewModel : class
        where TView : class, IViewFor<TViewModel>
    {
        ArgumentExceptionHelper.ThrowIfNull(factory);

        var key = (typeof(TViewModel), contract ?? string.Empty);

        lock (_gate)
        {
            var current = Volatile.Read(ref _mappings);
            Dictionary<(Type, string), Func<IViewFor>> newMappings = new(current)
            {
                [key] = factory
            };

            _ = Interlocked.Exchange(ref _mappings, newMappings);
        }

        return this;
    }

    /// <summary>Removes the default view mapping for the given view model type.</summary>
    /// <typeparam name="TViewModel">View model type to unmap.</typeparam>
    /// <returns>The locator for chaining.</returns>
    [SuppressMessage(
        "Design",
        "SST2307:A generic method's type parameter appears in no parameter, so no caller can infer it",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public DefaultViewLocator Unmap<TViewModel>()
        where TViewModel : class =>
        Unmap<TViewModel>(null);

    /// <summary>Removes a previously registered view mapping.</summary>
    /// <typeparam name="TViewModel">View model type to unmap.</typeparam>
    /// <param name="contract">Optional contract to unmap. If null, removes the default mapping.</param>
    /// <returns>The locator for chaining.</returns>
    [SuppressMessage(
        "Design",
        "SST2307:A generic method's type parameter appears in no parameter, so no caller can infer it",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public DefaultViewLocator Unmap<TViewModel>(string? contract)
        where TViewModel : class
    {
        var key = (typeof(TViewModel), contract ?? string.Empty);

        lock (_gate)
        {
            var current = Volatile.Read(ref _mappings);
            if (!current.ContainsKey(key))
            {
                return this;
            }

            Dictionary<(Type, string), Func<IViewFor>> newMappings = new(current);
            _ = newMappings.Remove(key);
            _ = Interlocked.Exchange(ref _mappings, newMappings);
        }

        return this;
    }

    /// <summary>Resolves a view for a view model type using the default contract. Fully AOT-compatible.</summary>
    /// <typeparam name="TViewModel">The view model type to resolve a view for.</typeparam>
    /// <returns>The resolved view or null when no registration is available.</returns>
    public IViewFor<TViewModel>? ResolveView<TViewModel>()
        where TViewModel : class =>
        ResolveView<TViewModel>(null);

    /// <summary>Resolves a view for a view model type known at compile time. Fully AOT-compatible.</summary>
    /// <typeparam name="TViewModel">The view model type to resolve a view for.</typeparam>
    /// <param name="contract">Optional contract to disambiguate between multiple views for the same view model.</param>
    /// <returns>The resolved view or null when no registration is available.</returns>
    /// <remarks>
    /// <para>
    /// Resolution strategy:
    /// <list type="number">
    /// <item>Check for explicit mapping registered via <c>Map</c> with the specified contract.</item>
    /// <item>Query the service locator for <c>IViewFor&lt;TViewModel&gt;</c> with the specified contract.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract)
        where TViewModel : class
    {
        var mappings = Volatile.Read(ref _mappings);
        var viewModelType = typeof(TViewModel);
        var contractKey = contract ?? string.Empty;
        if (mappings.TryGetValue((viewModelType, contractKey), out var factory))
        {
            this.Log().Debug(
                CultureInfo.InvariantCulture,
                "Resolved IViewFor<{0}> from explicit mapping",
                nameof(TViewModel));
            return (IViewFor<TViewModel>)factory();
        }

        var view = AppLocator.Current?.GetService<IViewFor<TViewModel>>(contract);
        if (view is not null)
        {
            this.Log().Debug(
                CultureInfo.InvariantCulture,
                "Resolved IViewFor<{0}> via service locator",
                nameof(TViewModel));
            return view;
        }

        this.Log().Warn(
                CultureInfo.InvariantCulture,
                "Failed to resolve view for {0}. Use Map<TViewModel, TView>() or register IViewFor<TViewModel> in the service locator.",
                nameof(TViewModel));
        return null;
    }

    /// <summary>Resolves a view for a view model instance using runtime type information and the default contract.</summary>
    /// <param name="instance">The view model instance to resolve a view for.</param>
    /// <returns>The resolved view or null when no registration is available.</returns>
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, " +
        "or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public IViewFor? ResolveView(object? instance) =>
        ResolveView(instance, null);

    /// <inheritdoc/>
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, " +
        "or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public IViewFor? ResolveView(object? instance, string? contract)
    {
        if (instance is null)
        {
            return null;
        }

        var viewModelType = instance.GetType();
        var contractKey = contract ?? string.Empty;
        var mappings = Volatile.Read(ref _mappings);
        if (mappings.TryGetValue((viewModelType, contractKey), out var factory))
        {
            var view = factory();
            if (view is not { } viewFor)
            {
                return null;
            }

            viewFor.ViewModel = instance;
            return viewFor;
        }

        var serviceType = _viewForTypeCache.GetOrAdd(viewModelType, static t => typeof(IViewFor<>).MakeGenericType(t));

        var resolved = AppLocator.Current.GetService(serviceType, contract);
        if (resolved is not IViewFor resolvedViewFor)
        {
            return null;
        }

        resolvedViewFor.ViewModel = instance;
        return resolvedViewFor;
    }
}
