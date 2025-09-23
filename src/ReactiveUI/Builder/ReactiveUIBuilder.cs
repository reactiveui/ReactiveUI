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
    private bool _setRxAppTaskpoolScheduler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveUIBuilder" /> class.
    /// </summary>
    /// <param name="resolver">The dependency resolver to configure.</param>
    /// <param name="current">The configured services.</param>
    /// <exception cref="System.ArgumentNullException">resolver.</exception>
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
        _setRxAppTaskpoolScheduler = setRxApp;
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
    /// Registers the platform-specific ReactiveUI services.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("ProcessRegistrationForNamespace uses reflection to locate types which may be trimmed.")]
    [RequiresDynamicCode("Calls ReactiveUI.IWantsToRegisterStuff.Register(Action<Func<Object>, Type>)")]
#endif
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
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("ProcessRegistrationForNamespace uses reflection to locate types which may be trimmed.")]
    [RequiresDynamicCode("Calls ReactiveUI.IWantsToRegisterStuff.Register(Action<Func<Object>, Type>)")]
    [SuppressMessage("AOT", "IL3051:'RequiresDynamicCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "In Splat")]
    [SuppressMessage("Trimming", "IL2046:'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "In Splat")]
#endif
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
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public IReactiveUIBuilder WithViewsFromAssembly(Assembly assembly)
    {
        assembly.ArgumentNullExceptionThrowIfNull(nameof(assembly));

        // Register views immediately against the builder's resolver
        CurrentMutable.RegisterViewsForViewModels(assembly);
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
    public IReactiveUIBuilder WithPlatformModule<T>()
        where T : IWantsToRegisterStuff, new()
    {
        var registration = new T();
        registration.Register((f, t) => CurrentMutable.RegisterConstant(f(), t));
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

    private void ConfigureSchedulers() =>
            WithCustomRegistration(_ =>
            {
                if (MainThreadScheduler != null && _setRxAppMainScheduler)
                {
                    RxApp.MainThreadScheduler = MainThreadScheduler;
                }

                if (TaskpoolScheduler != null && _setRxAppTaskpoolScheduler)
                {
                    RxApp.TaskpoolScheduler = TaskpoolScheduler;
                }
            });
}
