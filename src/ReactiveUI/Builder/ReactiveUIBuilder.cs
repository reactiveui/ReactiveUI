// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

using Splat.Builder;

namespace ReactiveUI.Builder;

/// <summary>
/// A builder class for configuring ReactiveUI services with AOT compatibility.
/// Extends the Splat AppBuilder to provide ReactiveUI-specific configuration.
/// </summary>
public sealed class ReactiveUIBuilder : AppBuilder, IReactiveUIBuilder, IReactiveUIInstance
{
    private bool _platformRegistered;
    private bool _coreRegistered;
    private bool _setRxAppMainScheduler;
    private bool _setRxAppTaskPoolScheduler;
    private IObserver<Exception>? _exceptionHandler;
    private ISuspensionHost? _suspensionHost;
    private int? _smallCacheLimit;
    private int? _bigCacheLimit;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveUIBuilder" /> class.
    /// </summary>
    /// <param name="resolver">The dependency resolver to configure.</param>
    /// <param name="current">The configured services.</param>
    /// <exception cref="ArgumentNullException">resolver.</exception>
    public ReactiveUIBuilder(IMutableDependencyResolver resolver, IReadonlyDependencyResolver? current)
        : base(resolver, current) => CurrentMutable.InitializeSplat();

    /// <summary>
    /// Gets a scheduler used to schedule work items that
    /// should be run "on the UI thread". In normal mode, this will be
    /// DispatcherScheduler, and in Unit Test mode this will be Immediate,
    /// to simplify writing common unit tests.
    /// </summary>
    public IScheduler? MainThreadScheduler { get; private set; }

    /// <summary>
    /// Gets the a the scheduler used to schedule work items to
    /// run in a background thread. In both modes, this will run on the TPL
    /// Task Pool.
    /// </summary>
    public IScheduler? TaskpoolScheduler { get; private set; }

    /// <summary>
    /// Configures the main thread scheduler for ReactiveUI.
    /// </summary>
    /// <param name="scheduler">The main thread scheduler to use.</param>
    /// <param name="setRxApp">if set to <c>true</c> [set rx application].</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    public IReactiveUIBuilder WithMainThreadScheduler(IScheduler scheduler, bool setRxApp = true)
    {
        _setRxAppMainScheduler = setRxApp;
        MainThreadScheduler = scheduler;
        return this;
    }

    /// <summary>
    /// Configures the task pool scheduler for ReactiveUI.
    /// </summary>
    /// <param name="scheduler">The task pool scheduler to use.</param>
    /// <param name="setRxApp">if set to <c>true</c> [set rx application].</param>
    /// <returns>
    /// The builder instance for chaining.
    /// </returns>
    public IReactiveUIBuilder WithTaskPoolScheduler(IScheduler scheduler, bool setRxApp = true)
    {
        _setRxAppTaskPoolScheduler = setRxApp;
        TaskpoolScheduler = scheduler;
        return this;
    }

    /// <summary>
    /// Adds a custom ReactiveUI registration action.
    /// </summary>
    /// <param name="configureAction">The configuration action.</param>
    /// <returns>The builder instance for chaining.</returns>
    public IReactiveUIBuilder WithRegistrationOnBuild(Action<IMutableDependencyResolver> configureAction)
    {
        WithCustomRegistration(configureAction);
        return this;
    }

    /// <summary>
    /// Adds a custom ReactiveUI registration action.
    /// </summary>
    /// <param name="configureAction">The configuration action.</param>
    /// <returns>The builder instance for chaining.</returns>
    public IReactiveUIBuilder WithRegistration(Action<IMutableDependencyResolver> configureAction)
    {
        if (configureAction is null)
        {
            throw new ArgumentNullException(nameof(configureAction));
        }

        configureAction(CurrentMutable);
        return this;
    }

    /// <summary>
    /// Registers services using an IWantsToRegisterStuff implementation.
    /// This method provides a migration path for users with existing IWantsToRegisterStuff implementations.
    /// </summary>
    /// <param name="registration">The registration implementation.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if registration is null.</exception>
    public IReactiveUIBuilder WithRegistration(IWantsToRegisterStuff registration)
    {
        ArgumentExceptionHelper.ThrowIfNull(registration);

        var registrar = new DependencyResolverRegistrar(CurrentMutable);
        registration.Register(registrar);

        return this;
    }

    /// <summary>
    /// Registers the platform-specific ReactiveUI services.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public IReactiveUIBuilder WithPlatformServices()
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
    public override IAppBuilder WithCoreServices()
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
    [RequiresUnreferencedCode("Scans assembly for IViewFor implementations using reflection. For AOT compatibility, use the ReactiveUIBuilder pattern to RegisterView explicitly.")]
    public IReactiveUIBuilder WithViewsFromAssembly(Assembly assembly)
    {
        ArgumentExceptionHelper.ThrowIfNull(assembly);

        // Register views immediately against the builder's resolver
        CurrentMutable.RegisterViewsForViewModels(assembly);
        return this;
    }

    /// <summary>
    /// Registers a platform-specific registration module by type.
    /// </summary>
    /// <typeparam name="T">The type of the registration module that implements IWantsToRegisterStuff.</typeparam>
    /// <returns>The builder instance for method chaining.</returns>
    public IReactiveUIBuilder WithPlatformModule<T>()
        where T : IWantsToRegisterStuff, new()
    {
        var registration = new T();
        var registrar = new DependencyResolverRegistrar(CurrentMutable);
        registration.Register(registrar);
        return this;
    }

    /// <summary>
    /// Using the splat module.
    /// </summary>
    /// <typeparam name="T">The Splat Module Type.</typeparam>
    /// <param name="registrationModule">The registration module to add.</param>
    /// <returns>
    /// The builder instance for method chaining.
    /// </returns>
    public IReactiveUIBuilder UsingSplatModule<T>(T registrationModule)
        where T : IModule
    {
        UsingModule(registrationModule);
        return this;
    }

    /// <summary>
    /// Uses the splat builder.
    /// </summary>
    /// <param name="appBuilder">The application builder.</param>
    /// <returns>
    /// The builder instance for method chaining.
    /// </returns>
    public IReactiveUIBuilder UsingSplatBuilder(Action<IAppBuilder> appBuilder)
    {
        appBuilder?.Invoke(this);
        return this;
    }

    /// <summary>
    /// Configures a custom platform implementation for ReactiveUI.
    /// </summary>
    /// <param name="mainThreadScheduler">The main thread scheduler for the platform.</param>
    /// <param name="platformServices">The platform-specific service registrations.</param>
    /// <returns>The builder instance for chaining.</returns>
    public IReactiveUIBuilder ForCustomPlatform(
        IScheduler mainThreadScheduler,
        Action<IMutableDependencyResolver> platformServices) =>
            WithMainThreadScheduler(mainThreadScheduler)
            .WithRegistrationOnBuild(platformServices);

    /// <summary>
    /// Configures ReactiveUI for multiple platforms simultaneously.
    /// </summary>
    /// <param name="platformConfigurations">The platform configuration actions.</param>
    /// <returns>The builder instance for chaining.</returns>
    public IReactiveUIBuilder ForPlatforms(params Action<IReactiveUIBuilder>[] platformConfigurations)
    {
        if (platformConfigurations is null)
        {
            throw new ArgumentNullException(nameof(platformConfigurations));
        }

        foreach (var configurePlatform in platformConfigurations)
        {
            configurePlatform(this);
        }

        return this;
    }

    /// <summary>
    /// Configures the ReactiveUI message bus.
    /// </summary>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The builder instance for chaining.</returns>
    public IReactiveUIBuilder ConfigureMessageBus(Action<MessageBus> configure) =>
        WithRegistrationOnBuild(resolver =>
            resolver.Register<IMessageBus>(() =>
            {
                var messageBus = new MessageBus();
                configure(messageBus);
                return messageBus;
            }));

    /// <summary>
    /// Configures the ReactiveUI view locator.
    /// </summary>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The builder instance for chaining.</returns>
    public IReactiveUIBuilder ConfigureViewLocator(Action<DefaultViewLocator> configure) =>
        WithRegistrationOnBuild(resolver =>
            resolver.Register<IViewLocator>(() =>
            {
                var viewLocator = new DefaultViewLocator();
                configure(viewLocator);
                return viewLocator;
            }));

    /// <summary>
    /// Configures the ReactiveUI suspension driver.
    /// </summary>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The builder instance for chaining.</returns>
    public IReactiveUIBuilder ConfigureSuspensionDriver(Action<ISuspensionDriver> configure) =>
        WithRegistrationOnBuild(resolver =>
        {
            var currentDriver = Current?.GetService<ISuspensionDriver>();
            if (currentDriver != null)
            {
                configure(currentDriver);
            }
        });

    /// <summary>
    /// Configures a custom exception handler for unhandled errors in ReactiveUI observables.
    /// </summary>
    /// <param name="exceptionHandler">The custom exception handler to use.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if exceptionHandler is null.</exception>
    public IReactiveUIBuilder WithExceptionHandler(IObserver<Exception> exceptionHandler)
    {
        _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
        return this;
    }

    /// <summary>
    /// Configures the non-generic suspension host for application lifecycle management.
    /// </summary>
    /// <returns>The builder instance for chaining.</returns>
    public IReactiveUIBuilder WithSuspensionHost()
    {
        _suspensionHost = new SuspensionHost();
        return this;
    }

    /// <summary>
    /// Configures a typed suspension host for application lifecycle management.
    /// </summary>
    /// <typeparam name="TAppState">The type of the application state to manage.</typeparam>
    /// <returns>The builder instance for chaining.</returns>
    public IReactiveUIBuilder WithSuspensionHost<TAppState>()
    {
        _suspensionHost = new SuspensionHost<TAppState>();
        return this;
    }

    /// <summary>
    /// Configures custom cache size limits for ReactiveUI's internal memoizing caches.
    /// </summary>
    /// <param name="smallCacheLimit">The small cache limit to use (must be greater than 0).</param>
    /// <param name="bigCacheLimit">The big cache limit to use (must be greater than 0).</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if either cache limit is less than or equal to 0.</exception>
    public IReactiveUIBuilder WithCacheSizes(int smallCacheLimit, int bigCacheLimit)
    {
        if (smallCacheLimit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smallCacheLimit), "Small cache limit must be greater than 0.");
        }

        if (bigCacheLimit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bigCacheLimit), "Big cache limit must be greater than 0.");
        }

        _smallCacheLimit = smallCacheLimit;
        _bigCacheLimit = bigCacheLimit;
        return this;
    }

    /// <summary>
    /// Registers a custom view model with the dependency resolver.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <returns>The builder instance for chaining.</returns>
    public IReactiveUIBuilder RegisterViewModel<TViewModel>()
        where TViewModel : class, IReactiveObject, new() =>
            WithRegistration(static resolver => resolver.Register<TViewModel>(static () => new()));

    /// <summary>
    /// Registers a custom view model with the dependency resolver.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <returns>The builder instance for chaining.</returns>
#if NET6_0_OR_GREATER
    public IReactiveUIBuilder RegisterSingletonViewModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TViewModel>()
#else
    public IReactiveUIBuilder RegisterSingletonViewModel<TViewModel>()
#endif
        where TViewModel : class, IReactiveObject, new() =>
            WithRegistration(static resolver => resolver.RegisterLazySingleton<TViewModel>(static () => new()));

    /// <summary>
    /// Registers a custom view for a specific view model.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <returns>The builder instance for chaining.</returns>
    public IReactiveUIBuilder RegisterView<TView, TViewModel>()
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class, IReactiveObject =>
            WithRegistration(static resolver => resolver.Register<IViewFor<TViewModel>>(static () => new TView()));

    /// <summary>
    /// Registers a custom view for a specific view model.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <returns>The builder instance for chaining.</returns>
    public IReactiveUIBuilder RegisterSingletonView<TView, TViewModel>()
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class, IReactiveObject =>
            WithRegistration(static resolver => resolver.RegisterLazySingleton<IViewFor<TViewModel>>(static () => new TView()));

    /// <summary>
    /// Builds the application and returns the ReactiveUI instance wrapper.
    /// </summary>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown if building the app instance fails.</exception>
    public IReactiveUIInstance BuildApp()
    {
        if (Build() is not IReactiveUIInstance appInstance || appInstance.Current is null)
        {
            throw new InvalidOperationException("Failed to create ReactiveUIInstance instance");
        }

        // Initialize static state (cache sizes, exception handler, suspension host)
        InitializeStaticState();

        // Mark ReactiveUI as initialized via builder pattern
        RxAppBuilder.MarkAsInitialized();

        return appInstance;
    }

    /// <summary>
    /// Resolves a single instance and passes it to the action.
    /// </summary>
    /// <typeparam name="T">The type to resolve.</typeparam>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T>(Action<T?> action)
    {
        if (Current is null)
        {
            return this;
        }

        action?.Invoke(Current.GetService<T>());
        return this;
    }

    /// <summary>
    /// Resolves two instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2>(Action<T1?, T2?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
            action(current.GetService<T1>(), current.GetService<T2>());
        }

        return this;
    }

    /// <summary>
    /// Resolves three instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3>(Action<T1?, T2?, T3?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
            action(current.GetService<T1>(), current.GetService<T2>(), current.GetService<T3>());
        }

        return this;
    }

    /// <summary>
    /// Resolves four instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3, T4>(Action<T1?, T2?, T3?, T4?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
            action(current.GetService<T1>(), current.GetService<T2>(), current.GetService<T3>(), current.GetService<T4>());
        }

        return this;
    }

    /// <summary>
    /// Resolves five instances and passes them to the action.
    /// </summary>
    /// <typeparam name="T1">The first type to resolve.</typeparam>
    /// <typeparam name="T2">The second type to resolve.</typeparam>
    /// <typeparam name="T3">The third type to resolve.</typeparam>
    /// <typeparam name="T4">The fourth type to resolve.</typeparam>
    /// <typeparam name="T5">The fifth type to resolve.</typeparam>
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5>(Action<T1?, T2?, T3?, T4?, T5?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
            action(current.GetService<T1>(), current.GetService<T2>(), current.GetService<T3>(), current.GetService<T4>(), current.GetService<T5>());
        }

        return this;
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
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6>(Action<T1?, T2?, T3?, T4?, T5?, T6?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2>(),
                   current.GetService<T3>(),
                   current.GetService<T4>(),
                   current.GetService<T5>(),
                   current.GetService<T6>());
        }

        return this;
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
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
            action(
                   current.GetService<T1>(),
                   current.GetService<T2>(),
                   current.GetService<T3>(),
                   current.GetService<T4>(),
                   current.GetService<T5>(),
                   current.GetService<T6>(),
                   current.GetService<T7>());
        }

        return this;
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
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
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

        return this;
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
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
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

        return this;
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
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
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

        return this;
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
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
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

        return this;
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
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
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

        return this;
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
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
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

        return this;
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
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
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

        return this;
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
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
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

        return this;
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
    /// <param name="action">The action.</param>
    /// <returns>IReactiveUIInstance instance for chaining.</returns>
    public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?> action)
    {
        if (Current is null)
        {
            return this;
        }

        if (action is not null)
        {
            var current = Current;
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

        return this;
    }

    /// <summary>
    /// Gets the platform-specific default small cache limit.
    /// </summary>
    /// <returns>The default small cache limit for the current platform.</returns>
    private static int GetPlatformDefaultSmallCacheLimit()
    {
#if ANDROID || IOS
        return 32;
#else
        return 64;
#endif
    }

    /// <summary>
    /// Gets the platform-specific default big cache limit.
    /// </summary>
    /// <returns>The default big cache limit for the current platform.</returns>
    private static int GetPlatformDefaultBigCacheLimit()
    {
#if ANDROID || IOS
        return 64;
#else
        return 256;
#endif
    }

    /// <summary>
    /// Initializes the static state for ReactiveUI based on builder configuration.
    /// This includes cache sizes, exception handler, and suspension host.
    /// </summary>
    private void InitializeStaticState()
    {
        // Initialize cache sizes - use configured values or platform defaults
        var smallCache = _smallCacheLimit ?? GetPlatformDefaultSmallCacheLimit();
        var bigCache = _bigCacheLimit ?? GetPlatformDefaultBigCacheLimit();
        RxCacheSize.Initialize(smallCache, bigCache);

        // Initialize exception handler if configured
        if (_exceptionHandler is not null)
        {
            RxState.InitializeExceptionHandler(_exceptionHandler);
        }

        // Initialize suspension host if configured
        if (_suspensionHost is not null)
        {
            RxSuspension.InitializeSuspensionHost(_suspensionHost);
        }
    }

    private void ConfigureSchedulers() =>
            WithCustomRegistration(_ =>
            {
                if (MainThreadScheduler != null && _setRxAppMainScheduler)
                {
                    RxSchedulers.MainThreadScheduler = MainThreadScheduler;
                }

                if (TaskpoolScheduler != null && _setRxAppTaskPoolScheduler)
                {
                    RxSchedulers.TaskpoolScheduler = TaskpoolScheduler;
                }
            });
}
