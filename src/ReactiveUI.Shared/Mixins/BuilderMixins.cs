// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Splat;
using Splat.Builder;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Builder;
#else
namespace ReactiveUI.Builder;
#endif
/// <summary>
/// Provides extension methods for configuring and building ReactiveUI applications using a fluent builder pattern.
/// These methods add view mappings, view modules, bulk converter registration and Splat builder callbacks to the
/// members that <see cref="IReactiveUIBuilder"/> already declares.
/// </summary>
/// <remarks>Scheduler, registration, converter, message bus, view model and view configuration are members of
/// <see cref="IReactiveUIBuilder"/> itself; call them on the builder. The members here cover only what the interface
/// does not declare, and all of them return the builder for chaining. For AOT (Ahead-Of-Time) environments, prefer
/// explicit registration methods over reflection-based approaches for maximum compatibility.</remarks>
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
        /// <returns>An <see cref="IReactiveUIBuilder"/> instance representing the configured application.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="appBuilder"/> does not implement <see cref="IReactiveUIBuilder"/>.</exception>
        /// <remarks>Use this extension method to finalize application setup when working with ReactiveUI. This
        /// method should be called after all necessary configuration has been applied to the builder.</remarks>
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
        public IReactiveUIBuilder RegisterViews(
            Action<ViewMappingBuilder> configure)
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);
            ArgumentExceptionHelper.ThrowIfNull(configure);

            var viewLocator = (AppLocator.Current.GetService<IViewLocator>() as DefaultViewLocator)
                              ?? throw new InvalidOperationException(
                                  "DefaultViewLocator must be registered before calling RegisterViews. "
                                  + "Ensure you've called WithPlatformModule() or manually registered DefaultViewLocator.");

            configure(viewLocator.CreateMappingBuilder());
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
        ///         locator.CreateMappingBuilder()
        ///             .Map<LoginViewModel, LoginView>()
        ///             .Map<RegisterViewModel, RegisterView>();
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
        public IReactiveUIBuilder WithViewModule<TModule>()
            where TModule : IViewModule, new()
        {
            ArgumentExceptionHelper.ThrowIfNull(builder);

            var viewLocator = (AppLocator.Current.GetService<IViewLocator>() as DefaultViewLocator)
                              ?? throw new InvalidOperationException(
                                  "DefaultViewLocator must be registered before calling WithViewModule. "
                                  + "Ensure you've called WithPlatformModule() or manually registered DefaultViewLocator.");

            TModule module = new();
            module.RegisterViews(viewLocator);
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
    }
}
