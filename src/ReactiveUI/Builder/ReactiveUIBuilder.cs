// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

namespace ReactiveUI.Builder;

/// <summary>
/// A builder class for configuring ReactiveUI without using reflection.
/// This provides an AOT-compatible alternative to the reflection-based InitializeReactiveUI method.
/// </summary>
public sealed class ReactiveUIBuilder
{
    private readonly IMutableDependencyResolver _resolver;
    private readonly List<Action<IMutableDependencyResolver>> _registrations = [];
    private bool _coreRegistered;
    private bool _platformRegistered;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveUIBuilder"/> class.
    /// </summary>
    /// <param name="resolver">The dependency resolver to configure.</param>
    public ReactiveUIBuilder(IMutableDependencyResolver resolver) => _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));

    /// <summary>
    /// Registers the core ReactiveUI services.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public ReactiveUIBuilder WithCoreServices()
    {
        if (_coreRegistered)
        {
            return this;
        }

        _registrations.Add(resolver =>
        {
            var registrations = new Registrations();
            registrations.Register((f, t) => resolver.RegisterConstant(f(), t));
        });

        _coreRegistered = true;
        return this;
    }

    /// <summary>
    /// Registers the platform-specific ReactiveUI services.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public ReactiveUIBuilder WithPlatformServices()
    {
        if (_platformRegistered)
        {
            return this;
        }

        _registrations.Add(resolver =>
        {
            var platformRegistrations = new PlatformRegistrations();
            platformRegistrations.Register((f, t) => resolver.RegisterConstant(f(), t));
        });

        _platformRegistered = true;
        return this;
    }

    /// <summary>
    /// Registers a custom registration module.
    /// </summary>
    /// <param name="registrationModule">The registration module to add.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ReactiveUIBuilder WithModule(IReactiveUIModule registrationModule)
    {
        registrationModule.ArgumentNullExceptionThrowIfNull(nameof(registrationModule));

        _registrations.Add(resolver => registrationModule.Configure(resolver));
        return this;
    }

    /// <summary>
    /// Registers a custom registration action.
    /// </summary>
    /// <param name="configureAction">The configuration action to add.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ReactiveUIBuilder WithCustomRegistration(Action<IMutableDependencyResolver> configureAction)
    {
        configureAction.ArgumentNullExceptionThrowIfNull(nameof(configureAction));

        _registrations.Add(configureAction);
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
    public ReactiveUIBuilder WithViewsFromAssembly(Assembly assembly)
    {
        assembly.ArgumentNullExceptionThrowIfNull(nameof(assembly));

        _registrations.Add(resolver => resolver.RegisterViewsForViewModels(assembly));
        return this;
    }

    /// <summary>
    /// Builds and applies all registrations to the dependency resolver.
    /// </summary>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public void Build()
    {
        // Ensure core services are always registered
        if (!_coreRegistered)
        {
            WithCoreServices();
        }

        // Apply all registrations
        foreach (var registration in _registrations)
        {
            registration(_resolver);
        }
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
    internal ReactiveUIBuilder WithPlatformModule<T>()
        where T : IWantsToRegisterStuff, new()
    {
        _registrations.Add(resolver =>
        {
            var registration = new T();
            registration.Register((f, t) => resolver.RegisterConstant(f(), t));
        });
        return this;
    }
}
