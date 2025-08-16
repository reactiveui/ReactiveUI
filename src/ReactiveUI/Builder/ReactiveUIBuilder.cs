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
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("WithCoreServices may use reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("WithCoreServices may use reflection and will not work in AOT environments.")]
#endif
    public override AppBuilder WithCoreServices()
    {
        if (_coreRegistered)
        {
            base.WithCoreServices();
            return this;
        }

        // Immediately register the core ReactiveUI services into the provided resolver.
        var registrations = new Registrations();
        registrations.Register((f, t) => _resolver.RegisterConstant(f(), t));

        _coreRegistered = true;

        base.WithCoreServices();
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
        platformRegistrations.Register((f, t) => _resolver.RegisterConstant(f(), t));

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
    /// Registers a platform-specific registration module by type.
    /// </summary>
    /// <typeparam name="T">The type of the registration module that implements IWantsToRegisterStuff.</typeparam>
    /// <returns>The builder instance for method chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    internal AppBuilder WithPlatformModule<T>()
        where T : IWantsToRegisterStuff, new()
    {
        var registration = new T();
        registration.Register((f, t) => _resolver.RegisterConstant(f(), t));
        return this;
    }
}
