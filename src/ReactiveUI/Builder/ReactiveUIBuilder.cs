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
        : base(resolver, current)
    {
        CurrentMutable.InitializeSplat();

        // Register the ConverterService instance so it's accessible to registrations
        CurrentMutable.RegisterConstant(() => ConverterService);
    }

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
    /// Gets the converter service used for binding type conversions.
    /// </summary>
    /// <remarks>
    /// This service provides access to three specialized registries:
    /// <list type="bullet">
    /// <item><description><see cref="ConverterService.TypedConverters"/> - For exact type-pair converters</description></item>
    /// <item><description><see cref="ConverterService.FallbackConverters"/> - For fallback converters with runtime type checking</description></item>
    /// <item><description><see cref="ConverterService.SetMethodConverters"/> - For set-method converters</description></item>
    /// </list>
    /// Use the <c>WithConverter*</c> methods to register converters during application initialization.
    /// </remarks>
    public ConverterService ConverterService { get; } = new();

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
            // Register all standard converters to the ConverterService
            RegisterStandardConverters();

            // Immediately register the core ReactiveUI services into the provided resolver (Splat).
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
    /// Registers a typed binding converter using the concrete type.
    /// </summary>
    /// <typeparam name="TFrom">The source type for the conversion.</typeparam>
    /// <typeparam name="TTo">The target type for the conversion.</typeparam>
    /// <param name="converter">The converter instance to register.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if converter is null.</exception>
    /// <example>
    /// <code>
    /// RxAppBuilder.CreateReactiveUIBuilder()
    ///     .WithConverter(new MyCustomConverter&lt;int, string&gt;())
    ///     .BuildApp();
    /// </code>
    /// </example>
    public IReactiveUIBuilder WithConverter<TFrom, TTo>(BindingTypeConverter<TFrom, TTo> converter)
    {
        ArgumentExceptionHelper.ThrowIfNull(converter);
        ConverterService.TypedConverters.Register(converter);
        return this;
    }

    /// <summary>
    /// Registers a typed binding converter using the interface.
    /// </summary>
    /// <param name="converter">The converter instance to register.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if converter is null.</exception>
    /// <example>
    /// <code>
    /// IBindingTypeConverter converter = new MyCustomConverter();
    /// RxAppBuilder.CreateReactiveUIBuilder()
    ///     .WithConverter(converter)
    ///     .BuildApp();
    /// </code>
    /// </example>
    public IReactiveUIBuilder WithConverter(IBindingTypeConverter converter)
    {
        ArgumentExceptionHelper.ThrowIfNull(converter);
        ConverterService.TypedConverters.Register(converter);
        return this;
    }

    /// <summary>
    /// Registers a typed binding converter via factory (lazy instantiation).
    /// </summary>
    /// <typeparam name="TFrom">The source type for the conversion.</typeparam>
    /// <typeparam name="TTo">The target type for the conversion.</typeparam>
    /// <param name="factory">The factory function that creates the converter.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if factory is null.</exception>
    /// <example>
    /// <code>
    /// RxAppBuilder.CreateReactiveUIBuilder()
    ///     .WithConverter(() => new MyCustomConverter&lt;int, string&gt;())
    ///     .BuildApp();
    /// </code>
    /// </example>
    public IReactiveUIBuilder WithConverter<TFrom, TTo>(Func<BindingTypeConverter<TFrom, TTo>> factory)
    {
        ArgumentExceptionHelper.ThrowIfNull(factory);
        ConverterService.TypedConverters.Register(factory());
        return this;
    }

    /// <summary>
    /// Registers a typed binding converter via factory (interface, lazy instantiation).
    /// </summary>
    /// <param name="factory">The factory function that creates the converter.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if factory is null.</exception>
    /// <example>
    /// <code>
    /// RxAppBuilder.CreateReactiveUIBuilder()
    ///     .WithConverter(() => (IBindingTypeConverter)new MyCustomConverter())
    ///     .BuildApp();
    /// </code>
    /// </example>
    public IReactiveUIBuilder WithConverter(Func<IBindingTypeConverter> factory)
    {
        ArgumentExceptionHelper.ThrowIfNull(factory);
        ConverterService.TypedConverters.Register(factory());
        return this;
    }

    /// <summary>
    /// Registers a fallback binding converter.
    /// </summary>
    /// <param name="converter">The fallback converter instance to register.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if converter is null.</exception>
    /// <remarks>
    /// Fallback converters are used when no exact type-pair converter is found.
    /// They perform runtime type checking via <see cref="IBindingFallbackConverter.GetAffinityForObjects(Type, Type)"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// RxAppBuilder.CreateReactiveUIBuilder()
    ///     .WithFallbackConverter(new MyFallbackConverter())
    ///     .BuildApp();
    /// </code>
    /// </example>
    public IReactiveUIBuilder WithFallbackConverter(IBindingFallbackConverter converter)
    {
        ArgumentExceptionHelper.ThrowIfNull(converter);
        ConverterService.FallbackConverters.Register(converter);
        return this;
    }

    /// <summary>
    /// Registers a fallback binding converter via factory (lazy instantiation).
    /// </summary>
    /// <param name="factory">The factory function that creates the fallback converter.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if factory is null.</exception>
    /// <example>
    /// <code>
    /// RxAppBuilder.CreateReactiveUIBuilder()
    ///     .WithFallbackConverter(() => new MyFallbackConverter())
    ///     .BuildApp();
    /// </code>
    /// </example>
    public IReactiveUIBuilder WithFallbackConverter(Func<IBindingFallbackConverter> factory)
    {
        ArgumentExceptionHelper.ThrowIfNull(factory);
        ConverterService.FallbackConverters.Register(factory());
        return this;
    }

    /// <summary>
    /// Registers a set-method binding converter.
    /// </summary>
    /// <param name="converter">The set-method converter instance to register.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if converter is null.</exception>
    /// <remarks>
    /// Set-method converters are used for special binding scenarios where the target
    /// uses a method (e.g., TableLayoutPanel.SetColumn) instead of a property setter.
    /// </remarks>
    /// <example>
    /// <code>
    /// RxAppBuilder.CreateReactiveUIBuilder()
    ///     .WithSetMethodConverter(new MySetMethodConverter())
    ///     .BuildApp();
    /// </code>
    /// </example>
    public IReactiveUIBuilder WithSetMethodConverter(ISetMethodBindingConverter converter)
    {
        ArgumentExceptionHelper.ThrowIfNull(converter);
        ConverterService.SetMethodConverters.Register(converter);
        return this;
    }

    /// <summary>
    /// Registers a set-method binding converter via factory (lazy instantiation).
    /// </summary>
    /// <param name="factory">The factory function that creates the set-method converter.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if factory is null.</exception>
    /// <example>
    /// <code>
    /// RxAppBuilder.CreateReactiveUIBuilder()
    ///     .WithSetMethodConverter(() => new MySetMethodConverter())
    ///     .BuildApp();
    /// </code>
    /// </example>
    public IReactiveUIBuilder WithSetMethodConverter(Func<ISetMethodBindingConverter> factory)
    {
        ArgumentExceptionHelper.ThrowIfNull(factory);
        ConverterService.SetMethodConverters.Register(factory());
        return this;
    }

    /// <summary>
    /// Imports all converters from a Splat dependency resolver into the builder.
    /// </summary>
    /// <param name="resolver">The Splat resolver to import converters from.</param>
    /// <returns>The builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if resolver is null.</exception>
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
    /// <example>
    /// <code>
    /// // Import all converters from current Splat locator
    /// RxAppBuilder.CreateReactiveUIBuilder()
    ///     .WithConvertersFrom(AppLocator.Current)
    ///     .BuildApp();
    /// </code>
    /// </example>
    public IReactiveUIBuilder WithConvertersFrom(IReadonlyDependencyResolver resolver)
    {
        ArgumentExceptionHelper.ThrowIfNull(resolver);

        // Import typed converters
        var typedConverters = resolver.GetServices<IBindingTypeConverter>();
        foreach (var converter in typedConverters)
        {
            if (converter is not null)
            {
                ConverterService.TypedConverters.Register(converter);
            }
        }

        // Import fallback converters
        var fallbackConverters = resolver.GetServices<IBindingFallbackConverter>();
        foreach (var converter in fallbackConverters)
        {
            if (converter is not null)
            {
                ConverterService.FallbackConverters.Register(converter);
            }
        }

        // Import set-method converters
        var setMethodConverters = resolver.GetServices<ISetMethodBindingConverter>();
        foreach (var converter in setMethodConverters)
        {
            if (converter is not null)
            {
                ConverterService.SetMethodConverters.Register(converter);
            }
        }

        return this;
    }

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

        // Set the global converter service
        RxConverters.SetService(ConverterService);

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
    /// Registers all standard ReactiveUI converters to the ConverterService.
    /// This mirrors the converters registered in Registrations.cs but targets the new ConverterService.
    /// </summary>
    private void RegisterStandardConverters()
    {
        // General converters
        ConverterService.TypedConverters.Register(new EqualityTypeConverter());
        ConverterService.TypedConverters.Register(new StringConverter());

        // Numeric → String converters
        ConverterService.TypedConverters.Register(new ByteToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableByteToStringTypeConverter());
        ConverterService.TypedConverters.Register(new ShortToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableShortToStringTypeConverter());
        ConverterService.TypedConverters.Register(new IntegerToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableIntegerToStringTypeConverter());
        ConverterService.TypedConverters.Register(new LongToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableLongToStringTypeConverter());
        ConverterService.TypedConverters.Register(new SingleToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableSingleToStringTypeConverter());
        ConverterService.TypedConverters.Register(new DoubleToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableDoubleToStringTypeConverter());
        ConverterService.TypedConverters.Register(new DecimalToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableDecimalToStringTypeConverter());

        // String → Numeric converters
        ConverterService.TypedConverters.Register(new StringToByteTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableByteTypeConverter());
        ConverterService.TypedConverters.Register(new StringToShortTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableShortTypeConverter());
        ConverterService.TypedConverters.Register(new StringToIntegerTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableIntegerTypeConverter());
        ConverterService.TypedConverters.Register(new StringToLongTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableLongTypeConverter());
        ConverterService.TypedConverters.Register(new StringToSingleTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableSingleTypeConverter());
        ConverterService.TypedConverters.Register(new StringToDoubleTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableDoubleTypeConverter());
        ConverterService.TypedConverters.Register(new StringToDecimalTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableDecimalTypeConverter());

        // Boolean ↔ String converters
        ConverterService.TypedConverters.Register(new BooleanToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableBooleanToStringTypeConverter());
        ConverterService.TypedConverters.Register(new StringToBooleanTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableBooleanTypeConverter());

        // Guid ↔ String converters
        ConverterService.TypedConverters.Register(new GuidToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableGuidToStringTypeConverter());
        ConverterService.TypedConverters.Register(new StringToGuidTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableGuidTypeConverter());

        // DateTime ↔ String converters
        ConverterService.TypedConverters.Register(new DateTimeToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableDateTimeToStringTypeConverter());
        ConverterService.TypedConverters.Register(new StringToDateTimeTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableDateTimeTypeConverter());

        // DateTimeOffset ↔ String converters
        ConverterService.TypedConverters.Register(new DateTimeOffsetToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableDateTimeOffsetToStringTypeConverter());
        ConverterService.TypedConverters.Register(new StringToDateTimeOffsetTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableDateTimeOffsetTypeConverter());

        // TimeSpan ↔ String converters
        ConverterService.TypedConverters.Register(new TimeSpanToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableTimeSpanToStringTypeConverter());
        ConverterService.TypedConverters.Register(new StringToTimeSpanTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableTimeSpanTypeConverter());

#if NET6_0_OR_GREATER
        // DateOnly ↔ String converters (.NET 6+)
        ConverterService.TypedConverters.Register(new DateOnlyToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableDateOnlyToStringTypeConverter());
        ConverterService.TypedConverters.Register(new StringToDateOnlyTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableDateOnlyTypeConverter());

        // TimeOnly ↔ String converters (.NET 6+)
        ConverterService.TypedConverters.Register(new TimeOnlyToStringTypeConverter());
        ConverterService.TypedConverters.Register(new NullableTimeOnlyToStringTypeConverter());
        ConverterService.TypedConverters.Register(new StringToTimeOnlyTypeConverter());
        ConverterService.TypedConverters.Register(new StringToNullableTimeOnlyTypeConverter());
#endif

        // Uri ↔ String converters
        ConverterService.TypedConverters.Register(new UriToStringTypeConverter());
        ConverterService.TypedConverters.Register(new StringToUriTypeConverter());
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
