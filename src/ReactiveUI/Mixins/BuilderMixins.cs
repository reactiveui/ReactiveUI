// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Splat.Builder;

namespace ReactiveUI.Builder;

/// <summary>
/// BuilderMixins.
/// </summary>
public static class BuilderMixins
{
    /// <summary>
    /// Configures the task pool scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="scheduler">The scheduler.</param>
    /// <param name="setRxApp">if set to <c>true</c> [set rx application].</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">scheduler.</exception>
    public static IReactiveUIBuilder WithTaskPoolScheduler(this IReactiveUIBuilder builder, IScheduler scheduler, bool setRxApp = true)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.WithTaskPoolScheduler(scheduler, setRxApp);
        return builder;
    }

    /// <summary>
    /// Configures the main thread scheduler.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="scheduler">The scheduler.</param>
    /// <param name="setRxApp">if set to <c>true</c> [set rx application].</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">builder.</exception>
    public static IReactiveUIBuilder WithMainThreadScheduler(this IReactiveUIBuilder builder, IScheduler scheduler, bool setRxApp = true)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.WithMainThreadScheduler(scheduler, setRxApp);
        return builder;
    }

    /// <summary>
    /// Configures the registration on build.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configureAction">The configure action.</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">builder.</exception>
    public static IReactiveUIBuilder WithRegistrationOnBuild(this IReactiveUIBuilder builder, Action<IMutableDependencyResolver> configureAction)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.WithRegistrationOnBuild(configureAction);
        return builder;
    }

    /// <summary>
    /// Configures the registration immediately.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configureAction">The configure action.</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">builder.</exception>
    public static IReactiveUIBuilder WithRegistration(this IReactiveUIBuilder builder, Action<IMutableDependencyResolver> configureAction)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.WithRegistration(configureAction);
        return builder;
    }

    /// <summary>
    /// Configures the views from the assembly.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="assembly">The assembly.</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">builder.</exception>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public static IReactiveUIBuilder WithViewsFromAssembly(this IReactiveUIBuilder builder, Assembly assembly)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.WithViewsFromAssembly(assembly);
        return builder;
    }

    /// <summary>
    /// Registers a platform-specific registration module by type.
    /// </summary>
    /// <typeparam name="T">The type of the registration module that implements IWantsToRegisterStuff.</typeparam>
    /// <param name="builder">The builder.</param>
    /// <returns>
    /// The builder instance for method chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">builder.</exception>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public static IReactiveUIBuilder WithPlatformModule<T>(this IReactiveUIBuilder builder)
        where T : IWantsToRegisterStuff, new()
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.WithPlatformModule<T>();
        return builder;
    }

    /// <summary>
    /// Using the splat module.
    /// </summary>
    /// <typeparam name="T">The Splat Module Type.</typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="registrationModule">The registration module to add.</param>
    /// <returns>
    /// The builder instance for method chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">builder.</exception>
    public static IReactiveUIBuilder UsingSplatModule<T>(this IReactiveUIBuilder builder, T registrationModule)
        where T : IModule
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.UsingSplatModule(registrationModule);
        return builder;
    }

    /// <summary>
    /// Uses the splat builder.
    /// </summary>
    /// <param name="reactiveUIBuilder">The reactive UI builder.</param>
    /// <param name="appBuilder">The application builder.</param>
    /// <returns>
    /// The builder instance for method chaining.
    /// </returns>
    public static IReactiveUIBuilder UsingSplatBuilder(this IReactiveUIBuilder reactiveUIBuilder, Action<IAppBuilder>? appBuilder)
    {
        if (reactiveUIBuilder == null)
        {
            throw new ArgumentNullException(nameof(reactiveUIBuilder));
        }

        appBuilder?.Invoke(reactiveUIBuilder);
        return reactiveUIBuilder;
    }

    /// <summary>
    /// Configures a custom platform implementation for ReactiveUI.
    /// </summary>
    /// <param name="reactiveUIBuilder">The reactive UI builder.</param>
    /// <param name="mainThreadScheduler">The main thread scheduler for the platform.</param>
    /// <param name="platformServices">The platform-specific service registrations.</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIBuilder.</exception>
    public static IReactiveUIBuilder ForCustomPlatform(
        this IReactiveUIBuilder reactiveUIBuilder,
        IScheduler mainThreadScheduler,
        Action<IMutableDependencyResolver> platformServices)
    {
        if (reactiveUIBuilder == null)
        {
            throw new ArgumentNullException(nameof(reactiveUIBuilder));
        }

        reactiveUIBuilder
            .WithMainThreadScheduler(mainThreadScheduler)
            .WithRegistration(platformServices);
        return reactiveUIBuilder;
    }

    /// <summary>
    /// Configures ReactiveUI for multiple platforms simultaneously.
    /// </summary>
    /// <param name="reactiveUIBuilder">The reactive UI builder.</param>
    /// <param name="platformConfigurations">The platform configuration actions.</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIBuilder.</exception>
    public static IReactiveUIBuilder ForPlatforms(this IReactiveUIBuilder reactiveUIBuilder, params Action<IReactiveUIBuilder>[] platformConfigurations)
    {
        if (reactiveUIBuilder == null)
        {
            throw new ArgumentNullException(nameof(reactiveUIBuilder));
        }

        reactiveUIBuilder.ForPlatforms(platformConfigurations);
        return reactiveUIBuilder;
    }

    /// <summary>
    /// Configures the ReactiveUI message bus.
    /// </summary>
    /// <param name="reactiveUIBuilder">The reactive UI builder.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIBuilder.</exception>
    public static IReactiveUIBuilder ConfigureMessageBus(this IReactiveUIBuilder reactiveUIBuilder, Action<MessageBus> configure)
    {
        if (reactiveUIBuilder == null)
        {
            throw new ArgumentNullException(nameof(reactiveUIBuilder));
        }

        reactiveUIBuilder.ConfigureMessageBus(configure);
        return reactiveUIBuilder;
    }

    /// <summary>
    /// Configures the ReactiveUI view locator.
    /// </summary>
    /// <param name="reactiveUIBuilder">The reactive UI builder.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIBuilder.</exception>
    public static IReactiveUIBuilder ConfigureViewLocator(this IReactiveUIBuilder reactiveUIBuilder, Action<DefaultViewLocator> configure)
    {
        if (reactiveUIBuilder == null)
        {
            throw new ArgumentNullException(nameof(reactiveUIBuilder));
        }

        reactiveUIBuilder.ConfigureViewLocator(configure);
        return reactiveUIBuilder;
    }

    /// <summary>
    /// Configures the ReactiveUI suspension driver.
    /// </summary>
    /// <param name="reactiveUIBuilder">The reactive UI builder.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIBuilder.</exception>
    public static IReactiveUIBuilder ConfigureSuspensionDriver(this IReactiveUIBuilder reactiveUIBuilder, Action<ISuspensionDriver> configure)
    {
        if (reactiveUIBuilder == null)
        {
            throw new ArgumentNullException(nameof(reactiveUIBuilder));
        }

        reactiveUIBuilder.ConfigureSuspensionDriver(configure);
        return reactiveUIBuilder;
    }

    /// <summary>
    /// Registers a custom view model with the dependency resolver.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="reactiveUIBuilder">The reactive UI builder.</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIBuilder.</exception>
    public static IReactiveUIBuilder RegisterViewModel<TViewModel>(this IReactiveUIBuilder reactiveUIBuilder)
        where TViewModel : class, IReactiveObject, new()
    {
        if (reactiveUIBuilder == null)
        {
            throw new ArgumentNullException(nameof(reactiveUIBuilder));
        }

        reactiveUIBuilder.RegisterViewModel<TViewModel>();
        return reactiveUIBuilder;
    }

    /// <summary>
    /// Registers a custom view model with the dependency resolver.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="reactiveUIBuilder">The reactive UI builder.</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIBuilder.</exception>
#if NET6_0_OR_GREATER
    public static IReactiveUIBuilder RegisterSingletonViewModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TViewModel>(this IReactiveUIBuilder reactiveUIBuilder)
#else
    public static IReactiveUIBuilder RegisterSingletonViewModel<TViewModel>(this IReactiveUIBuilder reactiveUIBuilder)
#endif
        where TViewModel : class, IReactiveObject, new()
    {
        if (reactiveUIBuilder == null)
        {
            throw new ArgumentNullException(nameof(reactiveUIBuilder));
        }

        reactiveUIBuilder.RegisterSingletonViewModel<TViewModel>();
        return reactiveUIBuilder;
    }

    /// <summary>
    /// Registers a custom view for a specific view model.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="reactiveUIBuilder">The reactive UI builder.</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIBuilder.</exception>
    public static IReactiveUIBuilder RegisterView<TView, TViewModel>(this IReactiveUIBuilder reactiveUIBuilder)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class, IReactiveObject
    {
        if (reactiveUIBuilder == null)
        {
            throw new ArgumentNullException(nameof(reactiveUIBuilder));
        }

        reactiveUIBuilder.RegisterView<TView, TViewModel>();
        return reactiveUIBuilder;
    }

    /// <summary>
    /// Registers a custom view for a specific view model.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="reactiveUIBuilder">The reactive UI builder.</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIBuilder.</exception>
    public static IReactiveUIBuilder RegisterSingletonView<TView, TViewModel>(this IReactiveUIBuilder reactiveUIBuilder)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class, IReactiveObject
    {
        if (reactiveUIBuilder == null)
        {
            throw new ArgumentNullException(nameof(reactiveUIBuilder));
        }

        reactiveUIBuilder.RegisterSingletonView<TView, TViewModel>();
        return reactiveUIBuilder;
    }

    /// <summary>
    /// Resolves a single instance and passes it to the action.
    /// </summary>
    /// <typeparam name="T">The type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T>(this IReactiveUIInstance reactiveUIInstance, Action<T?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        action?.Invoke(reactiveUIInstance.Current.GetService<T>());
        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves two instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(current.GetService<T1>(), current.GetService<T2>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves three instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(current.GetService<T1>(), current.GetService<T2>(), current.GetService<T3>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves four instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?> action)
    {
        if (reactiveUIInstance == null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(current.GetService<T1>(), current.GetService<T2>(), current.GetService<T3>(), current.GetService<T4>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves five instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fifth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(current.GetService<T1>(), current.GetService<T2>(), current.GetService<T3>(), current.GetService<T4>(), current.GetService<T5>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves six instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fifth type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2>(),
                   current.GetService<T3>(),
                   current.GetService<T4>(),
                   current.GetService<T5>(),
                   current.GetService<T6>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves seven instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fifth type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2>(),
                   current.GetService<T3>(),
                   current.GetService<T4>(),
                   current.GetService<T5>(),
                   current.GetService<T6>(),
                   current.GetService<T7>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves eight instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fifth type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2>(),
                   current.GetService<T3>(),
                   current.GetService<T4>(),
                   current.GetService<T5>(),
                   current.GetService<T6>(),
                   current.GetService<T7>(),
                   current.GetService<T8>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves nine instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fifth type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2>(),
                   current.GetService<T3>(),
                   current.GetService<T4>(),
                   current.GetService<T5>(),
                   current.GetService<T6>(),
                   current.GetService<T7>(),
                   current.GetService<T8>(),
                   current.GetService<T9>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves ten instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fifth type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2>(),
                   current.GetService<T3>(),
                   current.GetService<T4>(),
                   current.GetService<T5>(),
                   current.GetService<T6>(),
                   current.GetService<T7>(),
                   current.GetService<T8>(),
                   current.GetService<T9>(),
                   current.GetService<T10>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves eleven instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fifth type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2>(),
                   current.GetService<T3>(),
                   current.GetService<T4>(),
                   current.GetService<T5>(),
                   current.GetService<T6>(),
                   current.GetService<T7>(),
                   current.GetService<T8>(),
                   current.GetService<T9>(),
                   current.GetService<T10>(),
                   current.GetService<T11>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves twelve instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fifth type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
    /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2>(),
                   current.GetService<T3>(),
                   current.GetService<T4>(),
                   current.GetService<T5>(),
                   current.GetService<T6>(),
                   current.GetService<T7>(),
                   current.GetService<T8>(),
                   current.GetService<T9>(),
                   current.GetService<T10>(),
                   current.GetService<T11>(),
                   current.GetService<T12>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves thirteen instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fifth type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
    /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
    /// <typeparam name="T13">The thirteenth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2>(),
                   current.GetService<T3>(),
                   current.GetService<T4>(),
                   current.GetService<T5>(),
                   current.GetService<T6>(),
                   current.GetService<T7>(),
                   current.GetService<T8>(),
                   current.GetService<T9>(),
                   current.GetService<T10>(),
                   current.GetService<T11>(),
                   current.GetService<T12>(),
                   current.GetService<T13>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves fourteen instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fifth type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
    /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
    /// <typeparam name="T13">The thirteenth type to resolve.</typeparam>
    /// <typeparam name="T14">The fourteenth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2>(),
                   current.GetService<T3>(),
                   current.GetService<T4>(),
                   current.GetService<T5>(),
                   current.GetService<T6>(),
                   current.GetService<T7>(),
                   current.GetService<T8>(),
                   current.GetService<T9>(),
                   current.GetService<T10>(),
                   current.GetService<T11>(),
                   current.GetService<T12>(),
                   current.GetService<T13>(),
                   current.GetService<T14>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves fifteen instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fifth type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
    /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
    /// <typeparam name="T13">The thirteenth type to resolve.</typeparam>
    /// <typeparam name="T14">The fourteenth type to resolve.</typeparam>
    /// <typeparam name="T15">The fifteenth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current is null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2>(),
                   current.GetService<T3>(),
                   current.GetService<T4>(),
                   current.GetService<T5>(),
                   current.GetService<T6>(),
                   current.GetService<T7>(),
                   current.GetService<T8>(),
                   current.GetService<T9>(),
                   current.GetService<T10>(),
                   current.GetService<T11>(),
                   current.GetService<T12>(),
                   current.GetService<T13>(),
                   current.GetService<T14>(),
                   current.GetService<T15>());
        }

        return reactiveUIInstance;
    }

    /// <summary>
    /// Resolves sixteen instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fifth type to resolve.</typeparam>
    /// <typeparam name="T6">The sixth type to resolve.</typeparam>
    /// <typeparam name="T7">The seventh type to resolve.</typeparam>
    /// <typeparam name="T8">The eighth type to resolve.</typeparam>
    /// <typeparam name="T9">The ninth type to resolve.</typeparam>
    /// <typeparam name="T10">The tenth type to resolve.</typeparam>
    /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
    /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
    /// <typeparam name="T13">The thirteenth type to resolve.</typeparam>
    /// <typeparam name="T14">The fourteenth type to resolve.</typeparam>
    /// <typeparam name="T15">The fifteenth type to resolve.</typeparam>
    /// <typeparam name="T16">The sixteenth type to resolve.</typeparam>
    /// <param name="reactiveUIInstance">The reactive UI instance.</param>
    /// <param name="action">The action.</param>
    /// <returns>
    /// IReactiveUIInstance instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
    public static IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this IReactiveUIInstance reactiveUIInstance, Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?> action)
    {
        if (reactiveUIInstance is null)
        {
            throw new ArgumentNullException(nameof(reactiveUIInstance));
        }

        if (reactiveUIInstance.Current == null)
        {
            return reactiveUIInstance;
        }

        if (action is not null)
        {
            var current = reactiveUIInstance.Current;
            action(
                current.GetService<T1>(),
                current.GetService<T2>(),
                current.GetService<T3>(),
                current.GetService<T4>(),
                current.GetService<T5>(),
                current.GetService<T6>(),
                current.GetService<T7>(),
                current.GetService<T8>(),
                current.GetService<T9>(),
                current.GetService<T10>(),
                current.GetService<T11>(),
                current.GetService<T12>(),
                current.GetService<T13>(),
                current.GetService<T14>(),
                current.GetService<T15>(),
                current.GetService<T16>());
        }

        return reactiveUIInstance;
    }
}
