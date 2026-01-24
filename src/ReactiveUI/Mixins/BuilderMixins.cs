// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

using Splat.Builder;

namespace ReactiveUI.Builder;

/// <summary>
/// Provides extension methods for configuring and building ReactiveUI applications using a fluent builder pattern.
/// These methods enable registration of views, view models, schedulers, converters, and platform-specific modules in a
/// type-safe and AOT-compatible manner.
/// </summary>
/// <remarks>The BuilderMixins class contains a comprehensive set of extension methods designed to simplify and
/// standardize the setup of ReactiveUI applications. It supports advanced scenarios such as custom platform
/// integration, dependency resolver configuration, and bulk registration of converters. All methods are intended to be
/// used with the ReactiveUI builder interfaces and support method chaining for fluent configuration. For AOT
/// (Ahead-Of-Time) environments, prefer explicit registration methods over reflection-based approaches for maximum
/// compatibility.</remarks>
public static class BuilderMixins
{
    /// <summary>
    /// Registers view-to-viewmodel mappings inline using a fluent builder.
    /// This method is fully AOT-compatible when all view types are known at compile time.
    /// </summary>
    /// <param name="builder">The ReactiveUI builder instance.</param>
    /// <param name="configure">Configuration action for registering views.</param>
    /// <returns>The builder for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when builder or configure is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when DefaultViewLocator is not registered in the service locator.</exception>
    /// <example>
    /// <code language="csharp">
    /// <![CDATA[
    /// new ReactiveUIBuilder()
    ///     .WithPlatformModule<WpfRegistrations>()
    ///     .RegisterViews(views => views
    ///         .Map<LoginViewModel, LoginView>()
    ///         .Map<MainViewModel, MainView>()
    ///         .Map<SettingsViewModel, SettingsView>())
    ///     .Build();
    /// ]]>
    /// </code>
    /// </example>
    public static IReactiveUIBuilder RegisterViews(
        this IReactiveUIBuilder builder,
        Action<ViewMappingBuilder> configure)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);
        ArgumentExceptionHelper.ThrowIfNull(configure);

        var viewLocator = AppLocator.Current.GetService<IViewLocator>() as DefaultViewLocator
            ?? throw new InvalidOperationException(
                "DefaultViewLocator must be registered before calling RegisterViews. " +
                "Ensure you've called WithPlatformModule() or manually registered DefaultViewLocator.");

        var mappingBuilder = new ViewMappingBuilder(viewLocator);
        configure(mappingBuilder);
        return builder;
    }

    /// <summary>
    /// Registers views using a reusable view module.
    /// This method is fully AOT-compatible when all view types are known at compile time.
    /// </summary>
    /// <typeparam name="TModule">The view module type to register.</typeparam>
    /// <param name="builder">The ReactiveUI builder instance.</param>
    /// <returns>The builder for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when builder is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when DefaultViewLocator is not registered in the service locator.</exception>
    /// <example>
    /// <code language="csharp">
    /// <![CDATA[
    /// public class AuthenticationViewModule : IViewModule
    /// {
    ///     public void RegisterViews(DefaultViewLocator locator)
    ///     {
    ///         locator.Map<LoginViewModel, LoginView>(() => new LoginView())
    ///                .Map<RegisterViewModel, RegisterView>(() => new RegisterView());
    ///     }
    /// }
    ///
    /// new ReactiveUIBuilder()
    ///     .WithPlatformModule<WpfRegistrations>()
    ///     .WithViewModule<AuthenticationViewModule>()
    ///     .Build();
    /// ]]>
    /// </code>
    /// </example>
    public static IReactiveUIBuilder WithViewModule<TModule>(this IReactiveUIBuilder builder)
        where TModule : IViewModule, new()
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);

        var viewLocator = AppLocator.Current.GetService<IViewLocator>() as DefaultViewLocator
            ?? throw new InvalidOperationException(
                "DefaultViewLocator must be registered before calling WithViewModule. " +
                "Ensure you've called WithPlatformModule() or manually registered DefaultViewLocator.");

        var module = new TModule();
        module.RegisterViews(viewLocator);
        return builder;
    }

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
        ArgumentExceptionHelper.ThrowIfNull(builder);

        builder.WithTaskPoolScheduler(scheduler, setRxApp);
        return builder;
    }

    /// <summary>
    /// Builds and configures the application using the ReactiveUI builder pattern.
    /// </summary>
    /// <remarks>Use this extension method to finalize application setup when working with ReactiveUI. This
    /// method should be called after all necessary configuration has been applied to the builder.</remarks>
    /// <param name="appBuilder">The application builder to configure. Must implement <see cref="IReactiveUIBuilder"/>.</param>
    /// <returns>An <see cref="IReactiveUIBuilder"/> instance representing the configured application.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="appBuilder"/> does not implement <see cref="IReactiveUIBuilder"/>.</exception>
    public static IReactiveUIBuilder BuildApp(this IAppBuilder appBuilder)
    {
        ArgumentExceptionHelper.ThrowIfNull(appBuilder);
        if (appBuilder is not IReactiveUIBuilder reactiveUiBuilder)
        {
            throw new InvalidOperationException(
                "The provided IAppBuilder is not an IReactiveUIBuilder. Ensure you are using the ReactiveUI builder pattern.");
        }

        reactiveUiBuilder.BuildApp();
        return reactiveUiBuilder;
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
        ArgumentExceptionHelper.ThrowIfNull(builder);

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
        ArgumentExceptionHelper.ThrowIfNull(builder);

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
        ArgumentExceptionHelper.ThrowIfNull(builder);

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
    [RequiresUnreferencedCode("Scans assembly for IViewFor implementations using reflection. For AOT compatibility, use the ReactiveUIBuilder pattern to RegisterView explicitly.")]
    public static IReactiveUIBuilder WithViewsFromAssembly(this IReactiveUIBuilder builder, Assembly assembly)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);

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
    public static IReactiveUIBuilder WithPlatformModule<T>(this IReactiveUIBuilder builder)
        where T : IWantsToRegisterStuff, new()
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);

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
        ArgumentExceptionHelper.ThrowIfNull(builder);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIBuilder);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIBuilder);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIBuilder);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIBuilder);

        reactiveUIBuilder.ConfigureMessageBus(configure);
        return reactiveUIBuilder;
    }

    /// <summary>
    /// Registers a custom message bus instance.
    /// </summary>
    /// <param name="reactiveUIBuilder">The reactive UI builder.</param>
    /// <param name="messageBus">The message bus instance to use.</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">reactiveUIBuilder.</exception>
    public static IReactiveUIBuilder WithMessageBus(this IReactiveUIBuilder reactiveUIBuilder, IMessageBus messageBus)
    {
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIBuilder);

        reactiveUIBuilder.WithMessageBus(messageBus);
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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIBuilder);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIBuilder);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIBuilder);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIBuilder);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIBuilder);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIBuilder);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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
        ArgumentExceptionHelper.ThrowIfNull(reactiveUIInstance);

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

    /// <summary>
    /// Registers a typed binding converter using the concrete type.
    /// </summary>
    /// <typeparam name="TFrom">The source type for the conversion.</typeparam>
    /// <typeparam name="TTo">The target type for the conversion.</typeparam>
    /// <param name="builder">The ReactiveUI builder.</param>
    /// <param name="converter">The converter instance to register.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder or converter is null.</exception>
    public static IReactiveUIBuilder WithConverter<TFrom, TTo>(
        this IReactiveUIBuilder builder,
        BindingTypeConverter<TFrom, TTo> converter)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);
        builder.WithConverter(converter);
        return builder;
    }

    /// <summary>
    /// Registers a typed binding converter using the interface.
    /// </summary>
    /// <param name="builder">The ReactiveUI builder.</param>
    /// <param name="converter">The converter instance to register.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder or converter is null.</exception>
    public static IReactiveUIBuilder WithConverter(
        this IReactiveUIBuilder builder,
        IBindingTypeConverter converter)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);
        builder.WithConverter(converter);
        return builder;
    }

    /// <summary>
    /// Registers a typed binding converter via factory (lazy instantiation).
    /// </summary>
    /// <typeparam name="TFrom">The source type for the conversion.</typeparam>
    /// <typeparam name="TTo">The target type for the conversion.</typeparam>
    /// <param name="builder">The ReactiveUI builder.</param>
    /// <param name="factory">The factory function that creates the converter.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder or factory is null.</exception>
    public static IReactiveUIBuilder WithConverter<TFrom, TTo>(
        this IReactiveUIBuilder builder,
        Func<BindingTypeConverter<TFrom, TTo>> factory)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);
        builder.WithConverter(factory);
        return builder;
    }

    /// <summary>
    /// Registers a typed binding converter via factory (interface, lazy instantiation).
    /// </summary>
    /// <param name="builder">The ReactiveUI builder.</param>
    /// <param name="factory">The factory function that creates the converter.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder or factory is null.</exception>
    public static IReactiveUIBuilder WithConverter(
        this IReactiveUIBuilder builder,
        Func<IBindingTypeConverter> factory)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);
        builder.WithConverter(factory);
        return builder;
    }

    /// <summary>
    /// Registers multiple typed converters at once.
    /// </summary>
    /// <param name="builder">The ReactiveUI builder.</param>
    /// <param name="converters">The converters to register.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder or converters is null.</exception>
    public static IReactiveUIBuilder WithConverters(
        this IReactiveUIBuilder builder,
        params IBindingTypeConverter[] converters)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);
        ArgumentExceptionHelper.ThrowIfNull(converters);

        foreach (var converter in converters)
        {
            builder.WithConverter(converter);
        }

        return builder;
    }

    /// <summary>
    /// Registers a fallback binding converter.
    /// </summary>
    /// <param name="builder">The ReactiveUI builder.</param>
    /// <param name="converter">The fallback converter instance to register.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder or converter is null.</exception>
    /// <remarks>
    /// Fallback converters are used when no exact type-pair converter is found.
    /// They perform runtime type checking via <see cref="IBindingFallbackConverter.GetAffinityForObjects(Type, Type)"/>.
    /// </remarks>
    public static IReactiveUIBuilder WithFallbackConverter(
        this IReactiveUIBuilder builder,
        IBindingFallbackConverter converter)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);
        builder.WithFallbackConverter(converter);
        return builder;
    }

    /// <summary>
    /// Registers a fallback binding converter via factory (lazy instantiation).
    /// </summary>
    /// <param name="builder">The ReactiveUI builder.</param>
    /// <param name="factory">The factory function that creates the fallback converter.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder or factory is null.</exception>
    public static IReactiveUIBuilder WithFallbackConverter(
        this IReactiveUIBuilder builder,
        Func<IBindingFallbackConverter> factory)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);
        builder.WithFallbackConverter(factory);
        return builder;
    }

    /// <summary>
    /// Registers a set-method binding converter.
    /// </summary>
    /// <param name="builder">The ReactiveUI builder.</param>
    /// <param name="converter">The set-method converter instance to register.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder or converter is null.</exception>
    /// <remarks>
    /// Set-method converters are used for special binding scenarios where the target
    /// uses a method (e.g., TableLayoutPanel.SetColumn) instead of a property setter.
    /// </remarks>
    public static IReactiveUIBuilder WithSetMethodConverter(
        this IReactiveUIBuilder builder,
        ISetMethodBindingConverter converter)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);
        builder.WithSetMethodConverter(converter);
        return builder;
    }

    /// <summary>
    /// Registers a set-method binding converter via factory (lazy instantiation).
    /// </summary>
    /// <param name="builder">The ReactiveUI builder.</param>
    /// <param name="factory">The factory function that creates the set-method converter.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder or factory is null.</exception>
    public static IReactiveUIBuilder WithSetMethodConverter(
        this IReactiveUIBuilder builder,
        Func<ISetMethodBindingConverter> factory)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);
        builder.WithSetMethodConverter(factory);
        return builder;
    }

    /// <summary>
    /// Imports all converters from a Splat dependency resolver into the builder.
    /// </summary>
    /// <param name="builder">The ReactiveUI builder.</param>
    /// <param name="resolver">The Splat resolver to import converters from.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder or resolver is null.</exception>
    /// <remarks>
    /// <para>
    /// This is a migration helper to ease transition from Splat-based registration
    /// to the new ConverterService-based registration.
    /// </para>
    /// <para>
    /// This method imports all three converter types:
    /// <list type="bullet">
    /// <item><description>Typed converters (<see cref="IBindingTypeConverter"/>)</description></item>
    /// <item><description>Fallback converters (<see cref="IBindingFallbackConverter"/>)</description></item>
    /// <item><description>Set-method converters (<see cref="ISetMethodBindingConverter"/>)</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public static IReactiveUIBuilder WithConvertersFrom(
        this IReactiveUIBuilder builder,
        IReadonlyDependencyResolver resolver)
    {
        ArgumentExceptionHelper.ThrowIfNull(builder);
        builder.WithConvertersFrom(resolver);
        return builder;
    }
}
