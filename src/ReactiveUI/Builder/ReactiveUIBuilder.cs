// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Splat.Builder;

namespace ReactiveUI.Builder;

/// <summary>
/// A builder class for configuring ReactiveUI services with AOT compatibility.
/// Extends the Splat AppBuilder to provide ReactiveUI-specific configuration.
/// </summary>
public sealed class ReactiveUIBuilder : AppBuilder
{
    private readonly IMutableDependencyResolver _resolver;
    private IScheduler? _mainThreadScheduler;
    private IScheduler? _taskPoolScheduler;
    private bool _platformRegistered;
    private bool _coreRegistered;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveUIBuilder"/> class.
    /// </summary>
    /// <param name="resolver">The dependency resolver to configure.</param>
    public ReactiveUIBuilder(IMutableDependencyResolver resolver)
        : base(resolver)
    {
        _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));

        _resolver.InitializeSplat();
    }

    /// <summary>
    /// Configures the main thread scheduler for ReactiveUI.
    /// </summary>
    /// <param name="scheduler">The main thread scheduler to use.</param>
    /// <returns>The builder instance for chaining.</returns>
    public ReactiveUIBuilder WithMainThreadScheduler(IScheduler scheduler)
    {
        _mainThreadScheduler = scheduler;
        return this;
    }

    /// <summary>
    /// Configures the task pool scheduler for ReactiveUI.
    /// </summary>
    /// <param name="scheduler">The task pool scheduler to use.</param>
    /// <returns>The builder instance for chaining.</returns>
    public ReactiveUIBuilder WithTaskPoolScheduler(IScheduler scheduler)
    {
        _taskPoolScheduler = scheduler;
        return this;
    }

    /// <summary>
    /// Adds a custom ReactiveUI registration action.
    /// </summary>
    /// <param name="configureAction">The configuration action.</param>
    /// <returns>The builder instance for chaining.</returns>
    public ReactiveUIBuilder WithRegistrationOnBuild(Action<IMutableDependencyResolver> configureAction)
    {
        WithCustomRegistration(configureAction);
        return this;
    }

    /// <summary>
    /// Adds a custom ReactiveUI registration action.
    /// </summary>
    /// <param name="configureAction">The configuration action.</param>
    /// <returns>The builder instance for chaining.</returns>
    public ReactiveUIBuilder WithRegistration(Action<IMutableDependencyResolver> configureAction)
    {
        if (configureAction is null)
        {
            throw new ArgumentNullException(nameof(configureAction));
        }

        configureAction(_resolver);
        return this;
    }

    /// <summary>
    /// Registers the platform-specific ReactiveUI services.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public ReactiveUIBuilder WithPlatformServices()
    {
        if (_platformRegistered)
        {
            return this;
        }

        // Immediately register the platform ReactiveUI services into the provided resolver.
        WithPlatformModule<PlatformRegistrations>();

        _platformRegistered = true;
        return this;
    }

    /// <summary>
    /// Registers the core ReactiveUI services in an AOT-compatible manner.
    /// </summary>
    /// <returns>The builder instance for chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("ReactiveUI uses reflection to register some services. Ensure that the necessary types are preserved.")]
    [RequiresDynamicCode("ReactiveUI uses reflection to register some services. Ensure that the necessary types are preserved.")]
#endif
    public override AppBuilder WithCoreServices()
    {
        if (!_coreRegistered)
        {
            // Immediately register the core ReactiveUI services into the provided resolver.
            WithPlatformModule<Registrations>();
            _coreRegistered = true;
        }

        // Configure schedulers if specified
        ConfigureSchedulers();

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

        // Register views immediately against the builder's resolver
        _resolver.RegisterViewsForViewModels(assembly);
        return this;
    }

    /// <summary>
    /// Registers a platform-specific registration module by type.
    /// </summary>
    /// <typeparam name="T">The type of the registration module that implements IWantsToRegisterStuff.</typeparam>
    /// <returns>The builder instance for method chaining.</returns>
    [SuppressMessage("Trimming", "IL2111:Method with parameters or return value with `DynamicallyAccessedMembersAttribute` is accessed via reflection. Trimmer can't guarantee availability of the requirements of the method.", Justification = "Does not use reflection")]
    public ReactiveUIBuilder WithPlatformModule<T>()
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

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("ReactiveUI uses reflection to register some services. Ensure that the necessary types are preserved.")]
    [RequiresDynamicCode("ReactiveUI uses reflection to register some services. Ensure that the necessary types are preserved.")]
#endif
    private void ConfigureSchedulers() =>
        WithCustomRegistration(_ =>
        {
            if (_mainThreadScheduler != null)
            {
                RxApp.MainThreadScheduler = _mainThreadScheduler;
            }

            if (_taskPoolScheduler != null)
            {
                RxApp.TaskpoolScheduler = _taskPoolScheduler;
            }
        });
}
