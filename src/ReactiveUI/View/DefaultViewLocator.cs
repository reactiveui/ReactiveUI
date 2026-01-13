// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Globalization;

namespace ReactiveUI;

/// <summary>
/// Default AOT-compatible implementation of <see cref="IViewLocator"/> that resolves views using compile-time registrations.
/// </summary>
/// <remarks>
/// <para>
/// This locator uses explicit view-to-viewmodel mappings registered via <see cref="Map{TViewModel, TView}"/>.
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
public sealed class DefaultViewLocator : IViewLocator
{
    // Lock object for synchronizing writes to _mappings.
#if NET9_0_OR_GREATER
    private readonly System.Threading.Lock _gate = new();
#else
    private readonly object _gate = new();
#endif

    // Cache for MakeGenericType calls in ResolveView(object).
    // Key: ViewModelType, Value: IViewFor<ViewModelType> interface type.
    private readonly ConcurrentDictionary<Type, Type> _viewForTypeCache = new();

    // Snapshot pattern: Readers access this volatile reference without locking.
    // Writers lock, clone, mutate, and swap the reference.
    // Keyed by (ViewModelType, Contract). Empty string represents default contract.
    private Dictionary<(Type ViewModelType, string Contract), Func<IViewFor>> _mappings = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultViewLocator"/> class.
    /// </summary>
    internal DefaultViewLocator()
    {
    }

    /// <summary>
    /// Registers a direct mapping from a view model type to a view factory.
    /// This is the recommended way to register views for AOT-compatible applications.
    /// </summary>
    /// <typeparam name="TViewModel">View model type.</typeparam>
    /// <typeparam name="TView">View type that implements IViewFor&lt;TViewModel&gt;.</typeparam>
    /// <param name="factory">Factory function that creates the view instance.</param>
    /// <param name="contract">Optional contract used to disambiguate multiple views for the same view model.</param>
    /// <returns>The locator for chaining.</returns>
    /// <example>
    /// <code language="csharp">
    /// <![CDATA[
    /// locator.Map<LoginViewModel, LoginView>(() => new LoginView())
    ///        .Map<MainViewModel, MainView>(() => new MainView());
    /// ]]>
    /// </code>
    /// </example>
    public DefaultViewLocator Map<TViewModel, TView>(Func<TView> factory, string? contract = null)
        where TViewModel : class
        where TView : class, IViewFor<TViewModel>
    {
        ArgumentExceptionHelper.ThrowIfNull(factory);

        var key = (typeof(TViewModel), contract ?? string.Empty);

        lock (_gate)
        {
            var current = Volatile.Read(ref _mappings);
            var newMappings = new Dictionary<(Type, string), Func<IViewFor>>(current)
            {
                // Covariance of delegates allows Func<TView> to be assigned to Func<IViewFor>
                [key] = factory
            };

            Interlocked.Exchange(ref _mappings, newMappings);
        }

        return this;
    }

    /// <summary>
    /// Removes a previously registered view mapping.
    /// </summary>
    /// <typeparam name="TViewModel">View model type to unmap.</typeparam>
    /// <param name="contract">Optional contract to unmap. If null, removes the default mapping.</param>
    /// <returns>The locator for chaining.</returns>
    public DefaultViewLocator Unmap<TViewModel>(string? contract = null)
        where TViewModel : class
    {
        var key = (typeof(TViewModel), contract ?? string.Empty);

        lock (_gate)
        {
            var current = Volatile.Read(ref _mappings);
            if (current.ContainsKey(key))
            {
                var newMappings = new Dictionary<(Type, string), Func<IViewFor>>(current);
                newMappings.Remove(key);
                Interlocked.Exchange(ref _mappings, newMappings);
            }
        }

        return this;
    }

    /// <summary>
    /// Resolves a view for a view model type known at compile time. Fully AOT-compatible.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type to resolve a view for.</typeparam>
    /// <param name="contract">Optional contract to disambiguate between multiple views for the same view model.</param>
    /// <returns>The resolved view or <see langword="null"/> when no registration is available.</returns>
    /// <remarks>
    /// <para>
    /// Resolution strategy:
    /// <list type="number">
    /// <item>Check for explicit mapping registered via <see cref="Map{TViewModel, TView}"/> with the specified contract.</item>
    /// <item>Query the service locator for <c>IViewFor&lt;TViewModel&gt;</c> with the specified contract.</item>
    /// <item>If a specific contract was requested and not found, fall back to the default contract (null).</item>
    /// </list>
    /// </para>
    /// </remarks>
    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract = null)
        where TViewModel : class
    {
        // Snapshot read - lock-free
        // Volatile.Read ensures we get the latest published dictionary reference.
        // The dictionary itself is immutable (copy-on-write), so no further locking is needed.
        var mappings = Volatile.Read(ref _mappings);
        var vmType = typeof(TViewModel);
        var contractKey = contract ?? string.Empty;

        // 1. Check explicit AOT mappings
        if (mappings.TryGetValue((vmType, contractKey), out var factory))
        {
            this.Log().Debug(CultureInfo.InvariantCulture, "Resolved IViewFor<{0}> from explicit mapping", typeof(TViewModel).Name);
            return (IViewFor<TViewModel>)factory();
        }

        // 2. Fallback to service locator
        var view = AppLocator.Current?.GetService<IViewFor<TViewModel>>(contract);
        if (view is not null)
        {
            this.Log().Debug(CultureInfo.InvariantCulture, "Resolved IViewFor<{0}> via service locator", typeof(TViewModel).Name);
            return view;
        }

        // 3. Fallback to default contract if specific contract was requested
        if (!string.IsNullOrEmpty(contract))
        {
            if (mappings.TryGetValue((vmType, string.Empty), out var defaultFactory))
            {
                this.Log().Debug(CultureInfo.InvariantCulture, "Resolved IViewFor<{0}> from default mapping as fallback", typeof(TViewModel).Name);
                return (IViewFor<TViewModel>)defaultFactory();
            }

            var defaultView = AppLocator.Current?.GetService<IViewFor<TViewModel>>();
            if (defaultView is not null)
            {
                this.Log().Debug(CultureInfo.InvariantCulture, "Resolved IViewFor<{0}> via service locator (default) as fallback", typeof(TViewModel).Name);
                return defaultView;
            }
        }

        this.Log().Warn(CultureInfo.InvariantCulture, "Failed to resolve view for {0}. Use Map<TViewModel, TView>() or register IViewFor<TViewModel> in the service locator.", typeof(TViewModel).Name);
        return null;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode("This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public IViewFor? ResolveView(object? instance, string? contract = null)
    {
        if (instance is null)
        {
            return null;
        }

        var vmType = instance.GetType();
        var contractKey = contract ?? string.Empty;

        // Snapshot read - lock-free
        var mappings = Volatile.Read(ref _mappings);

        // 1) Explicit AOT mappings first (fastest, no reflection beyond GetType and Dictionary lookup)
        if (mappings.TryGetValue((vmType, contractKey), out var factory))
        {
            var view = factory();
            if (view is { } viewFor)
            {
                viewFor.ViewModel = instance;
                return viewFor;
            }

            return null;
        }

        // 2) Fallback to service locator via runtime-constructed service type.
        // We cache the MakeGenericType result to avoid reflection overhead on subsequent calls.
        var serviceType = _viewForTypeCache.GetOrAdd(vmType, static t => typeof(IViewFor<>).MakeGenericType(t));

        var resolved = AppLocator.Current.GetService(serviceType, contract);
        if (resolved is IViewFor resolvedViewFor)
        {
            resolvedViewFor.ViewModel = instance;
            return resolvedViewFor;
        }

        // 3) If a specific contract was requested, try default contract as fallback.
        if (!string.IsNullOrEmpty(contract))
        {
            if (mappings.TryGetValue((vmType, string.Empty), out var defaultFactory))
            {
                var view = defaultFactory();
                if (view is { } v)
                {
                    v.ViewModel = instance;
                    return v;
                }

                return null;
            }

            var defaultResolved = AppLocator.Current?.GetService(serviceType);
            if (defaultResolved is IViewFor defaultResolvedViewFor)
            {
                defaultResolvedViewFor.ViewModel = instance;
                return defaultResolvedViewFor;
            }
        }

        return null;
    }
}
