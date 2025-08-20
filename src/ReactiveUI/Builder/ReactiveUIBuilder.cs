// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using Splat.Builder;

namespace ReactiveUI.Builder;

/// <summary>
/// A builder class for configuring ReactiveUI without using reflection.
/// This provides an AOT-compatible alternative to the reflection-based InitializeReactiveUI method.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ReactiveUIBuilder"/> class.
/// </remarks>
/// <param name="resolver">The dependency resolver to configure.</param>
public sealed class ReactiveUIBuilder(IMutableDependencyResolver resolver) : AppBuilder(resolver)
{
    // Ensure we always register against the resolver provided to the builder,
    // not Locator.Current, so tests that read from the local resolver see the services.
    private readonly IMutableDependencyResolver _resolver = resolver;

    private bool _coreRegistered;
    private bool _platformRegistered;

    /// <summary>
    /// Registers the core ReactiveUI services.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    [SuppressMessage("Trimming", "IL2046:'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Does not use reflection")]
    [SuppressMessage("AOT", "IL3051:'RequiresDynamicCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Does not use reflection")]
    public override AppBuilder WithCoreServices()
    {
        if (_coreRegistered)
        {
            return this;
        }

        // Immediately register the core ReactiveUI services into the provided resolver.
        var registrations = new Registrations();
#pragma warning disable IL2067 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.
        registrations.Register((f, t) => _resolver.RegisterConstant(f(), t));
#pragma warning restore IL2067 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.

        _coreRegistered = true;

        return this;
    }

    /// <summary>
    /// Registers the platform-specific ReactiveUI services.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("WithPlatformServices may use reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("WithPlatformServices may use reflection and will not work in AOT environments.")]
#endif
    public AppBuilder WithPlatformServices()
    {
        if (_platformRegistered)
        {
            return this;
        }

        // Immediately register the platform ReactiveUI services into the provided resolver.
        var platformRegistrations = new PlatformRegistrations();
#if NET6_0_OR_GREATER
        platformRegistrations.Register((f, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] t) => _resolver.RegisterConstant(f(), t));
#else
        platformRegistrations.Register((f, t) => _resolver.RegisterConstant(f(), t));
#endif

        _platformRegistered = true;
        return this;
    }

    /// <summary>
    /// Automatically registers all views that implement IViewFor from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for views.</param>
    /// <returns>The builder instance for method chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public AppBuilder WithViewsFromAssembly(Assembly assembly)
    {
        assembly.ArgumentNullExceptionThrowIfNull(nameof(assembly));

        // Register views immediately against the builder's resolver
        _resolver.RegisterViewsForViewModels(assembly);
        return this;
    }

    /// <summary>
    /// Registers a view type for a specific view model using generics and a parameterless constructor.
    /// This avoids reflection and is AOT-friendly.
    /// </summary>
    /// <typeparam name="TView">The concrete view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="contract">Optional contract.</param>
    /// <returns>The builder instance for method chaining.</returns>
#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Generic registration does not use reflection")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic registration does not use dynamic code")]
#endif
    public AppBuilder RegisterViewForViewModel<TView, TViewModel>(string? contract = null)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class
    {
        _resolver.Register(() => new TView(), typeof(IViewFor<TViewModel>), contract ?? string.Empty);
        return this;
    }

    /// <summary>
    /// Registers a view type as a lazy singleton for a specific view model using generics.
    /// This avoids reflection and is AOT-friendly.
    /// </summary>
    /// <typeparam name="TView">The concrete view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="contract">Optional contract.</param>
    /// <returns>The builder instance for method chaining.</returns>
#if NET6_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Generic registration does not use reflection")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Generic registration does not use dynamic code")]
#endif
    public AppBuilder RegisterSingletonViewForViewModel<TView, TViewModel>(string? contract = null)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class
    {
        _resolver.RegisterLazySingleton(() => new TView(), typeof(IViewFor<TViewModel>), contract ?? string.Empty);
        return this;
    }

    /// <summary>
    /// Registers a platform-specific registration module by type.
    /// </summary>
    /// <typeparam name="T">The type of the registration module that implements IWantsToRegisterStuff.</typeparam>
    /// <returns>The builder instance for method chaining.</returns>
    [SuppressMessage("Trimming", "IL2111:Method with parameters or return value with `DynamicallyAccessedMembersAttribute` is accessed via reflection. Trimmer can't guarantee availability of the requirements of the method.", Justification = "Does not use reflection")]
    internal AppBuilder WithPlatformModule<T>()
        where T : IWantsToRegisterStuff, new()
    {
        var registration = new T();
#if NET6_0_OR_GREATER
        registration.Register((f, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] t) => _resolver.RegisterConstant(f(), t));
#else
        registration.Register((f, t) => _resolver.RegisterConstant(f(), t));
#endif
        return this;
    }
}
