// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Splat;
using Splat.Builder;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Builder;
#else
namespace ReactiveUI.Builder;
#endif
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
[SuppressMessage(
    "Design",
    "SST2307:Generic method type parameters should be inferable from the parameters",
    Justification = "Registration and module methods take the target type as an explicit generic argument by design; it identifies the type to register and cannot be inferred from the parameters.")]
public static partial class BuilderMixins
{
    /// <summary>Provides build-finalization extension members for <see cref="IAppBuilder"/>.</summary>
    /// <param name="appBuilder">The application builder to configure. Must implement <see cref="IReactiveUIBuilder"/>.</param>
    extension(IAppBuilder appBuilder)
    {
        /// <summary>Builds and configures the application using the ReactiveUI builder pattern.</summary>
        /// <remarks>Use this extension method to finalize application setup when working with ReactiveUI. This
        /// method should be called after all necessary configuration has been applied to the builder.</remarks>
        /// <returns>An <see cref="IReactiveUIBuilder"/> instance representing the configured application.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="appBuilder"/> does not implement <see cref="IReactiveUIBuilder"/>.</exception>
        public IReactiveUIBuilder BuildApp()
        {
            ArgumentExceptionHelper.ThrowIfNull(appBuilder);
            if (appBuilder is not IReactiveUIBuilder reactiveUiBuilder)
            {
                throw new InvalidOperationException(
                    "The provided IAppBuilder is not an IReactiveUIBuilder. Ensure you are using the ReactiveUI builder pattern.");
            }

            _ = reactiveUiBuilder.Build();
            return reactiveUiBuilder;
        }
    }

    /// <summary>Provides configuration and registration extension members for <see cref="IReactiveUIBuilder"/>.</summary>
    /// <param name="builder">The ReactiveUI builder instance.</param>
    extension(IReactiveUIBuilder builder)
    {
        /// <summary>
        /// Registers view-to-viewmodel mappings inline using a fluent builder.
        /// This method is fully AOT-compatible when all view types are known at compile time.
        /// </summary>
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
        [SuppressMessage(
            "Design",
            "SST2326:Interface instances should not be narrowed to concrete types",
            Justification = "DefaultViewLocator exposes view-registration APIs not present on IViewLocator; reaching them requires the concrete type.")]
        public IReactiveUIBuilder RegisterViews(
            Action<ViewMappingBuilder> configure)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);
            ArgumentExceptionHelper.ThrowIfNull(configure);

            var viewLocator = (AppLocator.Current.GetService<IViewLocator>() as DefaultViewLocator)
                              ?? throw new InvalidOperationException(
                                  "DefaultViewLocator must be registered before calling RegisterViews. " +
                                  "Ensure you've called WithPlatformModule() or manually registered DefaultViewLocator.");

            ViewMappingBuilder mappingBuilder = new(viewLocator);
            configure(mappingBuilder);
            return builder;
        }

        /// <summary>
        /// Registers views using a reusable view module.
        /// This method is fully AOT-compatible when all view types are known at compile time.
        /// </summary>
        /// <typeparam name="TModule">The view module type to register.</typeparam>
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
        [SuppressMessage(
            "Design",
            "SST2326:Interface instances should not be narrowed to concrete types",
            Justification = "DefaultViewLocator exposes view-registration APIs not present on IViewLocator; reaching them requires the concrete type.")]
        public IReactiveUIBuilder WithViewModule<TModule>()
            where TModule : IViewModule, new()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            var viewLocator = (AppLocator.Current.GetService<IViewLocator>() as DefaultViewLocator)
                              ?? throw new InvalidOperationException(
                                  "DefaultViewLocator must be registered before calling WithViewModule. " +
                                  "Ensure you've called WithPlatformModule() or manually registered DefaultViewLocator.");

            TModule module = new();
            module.RegisterViews(viewLocator);
            return builder;
        }

        /// <summary>Configures the task pool scheduler, also setting the RxApp scheduler.</summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">scheduler.</exception>
        public IReactiveUIBuilder WithTaskPoolScheduler(
            ISequencer scheduler) =>
            WithTaskPoolScheduler(builder, scheduler, true);

        /// <summary>Configures the task pool scheduler.</summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="setRxApp">if set to <c>true</c> [set rx application].</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">scheduler.</exception>
        public IReactiveUIBuilder WithTaskPoolScheduler(
            ISequencer scheduler,
            bool setRxApp)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.WithTaskPoolScheduler(scheduler, setRxApp);
            return builder;
        }

        /// <summary>Configures the main thread scheduler, also setting the RxApp scheduler.</summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder WithMainThreadScheduler(
            ISequencer scheduler) =>
            WithMainThreadScheduler(builder, scheduler, true);

        /// <summary>Configures the main thread scheduler.</summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="setRxApp">if set to <c>true</c> [set rx application].</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder WithMainThreadScheduler(
            ISequencer scheduler,
            bool setRxApp)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.WithMainThreadScheduler(scheduler, setRxApp);
            return builder;
        }

        /// <summary>Configures the registration on build.</summary>
        /// <param name="configureAction">The configure action.</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder WithRegistrationOnBuild(
            Action<IMutableDependencyResolver> configureAction)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.WithRegistrationOnBuild(configureAction);
            return builder;
        }

        /// <summary>Configures the registration immediately.</summary>
        /// <param name="configureAction">The configure action.</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder WithRegistration(
            Action<IMutableDependencyResolver> configureAction)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.WithRegistration(configureAction);
            return builder;
        }

        /// <summary>Configures the views from the assembly.</summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        [RequiresUnreferencedCode(
            "Scans assembly for IViewFor implementations using reflection. For AOT compatibility, use the ReactiveUIBuilder pattern to RegisterView explicitly.")]
        public IReactiveUIBuilder WithViewsFromAssembly(Assembly assembly)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.WithViewsFromAssembly(assembly);
            return builder;
        }

        /// <summary>Registers a platform-specific registration module by type.</summary>
        /// <typeparam name="T">The type of the registration module that implements IWantsToRegisterStuff.</typeparam>
        /// <returns>
        /// The builder instance for method chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder WithPlatformModule<T>()
            where T : IWantsToRegisterStuff, new()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.WithPlatformModule<T>();
            return builder;
        }

        /// <summary>Using the splat module.</summary>
        /// <typeparam name="T">The Splat Module Type.</typeparam>
        /// <param name="registrationModule">The registration module to add.</param>
        /// <returns>
        /// The builder instance for method chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder UsingSplatModule<T>(T registrationModule)
            where T : IModule
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.UsingSplatModule(registrationModule);
            return builder;
        }

        /// <summary>Registers a typed binding converter using the concrete type.</summary>
        /// <typeparam name="TFrom">The source type for the conversion.</typeparam>
        /// <typeparam name="TTo">The target type for the conversion.</typeparam>
        /// <param name="converter">The converter instance to register.</param>
        /// <returns>The builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if builder or converter is null.</exception>
        public IReactiveUIBuilder WithConverter<TFrom, TTo>(
            BindingTypeConverter<TFrom, TTo> converter)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);
            _ = builder.WithConverter(converter);
            return builder;
        }

        /// <summary>Registers a typed binding converter using the interface.</summary>
        /// <param name="converter">The converter instance to register.</param>
        /// <returns>The builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if builder or converter is null.</exception>
        public IReactiveUIBuilder WithConverter(
            IBindingTypeConverter converter)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);
            _ = builder.WithConverter(converter);
            return builder;
        }

        /// <summary>Registers a typed binding converter via factory (lazy instantiation).</summary>
        /// <typeparam name="TFrom">The source type for the conversion.</typeparam>
        /// <typeparam name="TTo">The target type for the conversion.</typeparam>
        /// <param name="factory">The factory function that creates the converter.</param>
        /// <returns>The builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if builder or factory is null.</exception>
        public IReactiveUIBuilder WithConverter<TFrom, TTo>(
            Func<BindingTypeConverter<TFrom, TTo>> factory)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);
            _ = builder.WithConverter(factory);
            return builder;
        }

        /// <summary>Registers a typed binding converter via factory (interface, lazy instantiation).</summary>
        /// <param name="factory">The factory function that creates the converter.</param>
        /// <returns>The builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if builder or factory is null.</exception>
        public IReactiveUIBuilder WithConverter(
            Func<IBindingTypeConverter> factory)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);
            _ = builder.WithConverter(factory);
            return builder;
        }

        /// <summary>Registers multiple typed converters at once.</summary>
        /// <param name="converters">The converters to register.</param>
        /// <returns>The builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if builder or converters is null.</exception>
        public IReactiveUIBuilder WithConverters(
            params IBindingTypeConverter[] converters)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);
            ArgumentExceptionHelper.ThrowIfNull(converters);

            foreach (var converter in converters)
            {
                _ = builder.WithConverter(converter);
            }

            return builder;
        }

        /// <summary>Registers a fallback binding converter.</summary>
        /// <param name="converter">The fallback converter instance to register.</param>
        /// <returns>The builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if builder or converter is null.</exception>
        /// <remarks>
        /// Fallback converters are used when no exact type-pair converter is found.
        /// They perform runtime type checking via <see cref="IBindingFallbackConverter.GetAffinityForObjects(Type, Type)"/>.
        /// </remarks>
        public IReactiveUIBuilder WithFallbackConverter(
            IBindingFallbackConverter converter)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);
            _ = builder.WithFallbackConverter(converter);
            return builder;
        }

        /// <summary>Registers a fallback binding converter via factory (lazy instantiation).</summary>
        /// <param name="factory">The factory function that creates the fallback converter.</param>
        /// <returns>The builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if builder or factory is null.</exception>
        public IReactiveUIBuilder WithFallbackConverter(
            Func<IBindingFallbackConverter> factory)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);
            _ = builder.WithFallbackConverter(factory);
            return builder;
        }

        /// <summary>Registers a set-method binding converter.</summary>
        /// <param name="converter">The set-method converter instance to register.</param>
        /// <returns>The builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if builder or converter is null.</exception>
        /// <remarks>
        /// Set-method converters are used for special binding scenarios where the target
        /// uses a method (e.g., TableLayoutPanel.SetColumn) instead of a property setter.
        /// </remarks>
        public IReactiveUIBuilder WithSetMethodConverter(
            ISetMethodBindingConverter converter)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);
            _ = builder.WithSetMethodConverter(converter);
            return builder;
        }

        /// <summary>Registers a set-method binding converter via factory (lazy instantiation).</summary>
        /// <param name="factory">The factory function that creates the set-method converter.</param>
        /// <returns>The builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if builder or factory is null.</exception>
        public IReactiveUIBuilder WithSetMethodConverter(
            Func<ISetMethodBindingConverter> factory)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);
            _ = builder.WithSetMethodConverter(factory);
            return builder;
        }

        /// <summary>Imports all converters from a Splat dependency resolver into the builder.</summary>
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
        public IReactiveUIBuilder WithConvertersFrom(
            IReadonlyDependencyResolver resolver)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);
            _ = builder.WithConvertersFrom(resolver);
            return builder;
        }

        /// <summary>Uses the splat builder.</summary>
        /// <param name="appBuilder">The application builder.</param>
        /// <returns>
        /// The builder instance for method chaining.
        /// </returns>
        public IReactiveUIBuilder UsingSplatBuilder(
            Action<IAppBuilder>? appBuilder)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            appBuilder?.Invoke(builder);
            return builder;
        }

        /// <summary>Configures a custom platform implementation for ReactiveUI.</summary>
        /// <param name="mainThreadScheduler">The main thread scheduler for the platform.</param>
        /// <param name="platformServices">The platform-specific service registrations.</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder ForCustomPlatform(
            ISequencer mainThreadScheduler,
            Action<IMutableDependencyResolver> platformServices)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder
                .WithMainThreadScheduler(mainThreadScheduler)
                .WithRegistration(platformServices);
            return builder;
        }

        /// <summary>Configures ReactiveUI for multiple platforms simultaneously.</summary>
        /// <param name="platformConfigurations">The platform configuration actions.</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder ForPlatforms(
            params Action<IReactiveUIBuilder>[] platformConfigurations)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.ForPlatforms(platformConfigurations);
            return builder;
        }

        /// <summary>Configures the ReactiveUI message bus.</summary>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder WithMessageBus()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.WithMessageBus();
            return builder;
        }

        /// <summary>Configures the ReactiveUI message bus.</summary>
        /// <param name="configure">The configuration action.</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder WithMessageBus(
            Action<IMessageBus> configure)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.WithMessageBus(configure);
            return builder;
        }

        /// <summary>Registers a custom message bus instance.</summary>
        /// <param name="messageBus">The message bus instance to use.</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder WithMessageBus(IMessageBus messageBus)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.WithMessageBus(messageBus);
            return builder;
        }

        /// <summary>Configures the ReactiveUI view locator.</summary>
        /// <param name="configure">The configuration action.</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder ConfigureViewLocator(
            Action<DefaultViewLocator> configure)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.ConfigureViewLocator(configure);
            return builder;
        }

        /// <summary>Configures the ReactiveUI suspension driver.</summary>
        /// <param name="configure">The configuration action.</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder ConfigureSuspensionDriver(
            Action<ISuspensionDriver> configure)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.ConfigureSuspensionDriver(configure);
            return builder;
        }

        /// <summary>Registers a custom view model with the dependency resolver.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder RegisterViewModel<TViewModel>()
            where TViewModel : class, IReactiveObject, new()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.RegisterViewModel<TViewModel>();
            return builder;
        }

        /// <summary>Registers a constant instance of the specified view model type for use with the ReactiveUI builder.</summary>
        /// <typeparam name="TViewModel">The type of the view model to register. Must be a class that implements IReactiveObject and has a parameterless
        /// constructor.</typeparam>
        /// <returns>The same ReactiveUI builder instance, to allow for method chaining.</returns>
        public IReactiveUIBuilder RegisterConstantViewModel<TViewModel>()
            where TViewModel : class, IReactiveObject, new()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.RegisterConstantViewModel<TViewModel>();
            return builder;
        }

        /// <summary>Registers a custom view model with the dependency resolver.</summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
#if NET6_0_OR_GREATER
        public IReactiveUIBuilder RegisterSingletonViewModel<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        TViewModel>()
#else
        public IReactiveUIBuilder RegisterSingletonViewModel<TViewModel>()
#endif
            where TViewModel : class, IReactiveObject, new()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.RegisterSingletonViewModel<TViewModel>();
            return builder;
        }

        /// <summary>Registers a custom view for a specific view model.</summary>
        /// <typeparam name="TView">The view type.</typeparam>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder RegisterView<TView, TViewModel>()
            where TView : class, IViewFor<TViewModel>, new()
            where TViewModel : class, IReactiveObject
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.RegisterView<TView, TViewModel>();
            return builder;
        }

        /// <summary>Registers a custom view for a specific view model.</summary>
        /// <typeparam name="TView">The view type.</typeparam>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">builder.</exception>
        public IReactiveUIBuilder RegisterSingletonView<TView, TViewModel>()
            where TView : class, IViewFor<TViewModel>, new()
            where TViewModel : class, IReactiveObject
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            _ = builder.RegisterSingletonView<TView, TViewModel>();
            return builder;
        }
    }
}
