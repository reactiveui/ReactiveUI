// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder;

/// <summary>
/// Extension methods for configuring ReactiveUI with the Splat builder.
/// </summary>
public static class RxAppBuilder
{
    /// <summary>
    /// Creates a ReactiveUI builder with the Splat Locator instance.
    /// </summary>
    /// <returns>The ReactiveUI builder instance.</returns>
    public static ReactiveUIBuilder CreateReactiveUIBuilder() =>
        new(AppLocator.CurrentMutable);

    /// <summary>
    /// Creates a ReactiveUI builder with the specified dependency resolver.
    /// </summary>
    /// <param name="resolver">The dependency resolver to use.</param>
    /// <returns>The ReactiveUI builder instance.</returns>
    public static ReactiveUIBuilder CreateReactiveUIBuilder(this IMutableDependencyResolver resolver) =>
        new(resolver);

    /// <summary>
    /// Configures the ReactiveUI message bus.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static ReactiveUIBuilder ConfigureMessageBus(this ReactiveUIBuilder builder, Action<MessageBus> configure)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithRegistrationOnBuild(resolver =>
            resolver.Register<IMessageBus>(() =>
            {
                var messageBus = new MessageBus();
                configure(messageBus);
                return messageBus;
            }));
    }

    /// <summary>
    /// Configures the ReactiveUI view locator.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static ReactiveUIBuilder ConfigureViewLocator(this ReactiveUIBuilder builder, Action<DefaultViewLocator> configure)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithRegistrationOnBuild(resolver =>
            resolver.Register<IViewLocator>(() =>
            {
                var viewLocator = new DefaultViewLocator();
                configure(viewLocator);
                return viewLocator;
            }));
    }

    /// <summary>
    /// Configures the ReactiveUI suspension driver.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static ReactiveUIBuilder ConfigureSuspensionDriver(this ReactiveUIBuilder builder, Action<ISuspensionDriver> configure)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithRegistrationOnBuild(resolver =>
        {
            var currentDriver = AppLocator.Current.GetService<ISuspensionDriver>();
            if (currentDriver != null)
            {
                configure(currentDriver);
            }
        });
    }

    /// <summary>
    /// Registers a custom view model with the dependency resolver.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static ReactiveUIBuilder RegisterViewModel<TViewModel>(this ReactiveUIBuilder builder)
        where TViewModel : class, IReactiveObject, new()
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithRegistration(resolver => resolver.Register<TViewModel>(() => new()));
    }

    /// <summary>
    /// Registers a custom view model with the dependency resolver.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("This method uses 'new()' constraint which may require dynamic code generation.")]
    [RequiresUnreferencedCode("This method uses 'new()' constraint which may require dynamic code generation.")]
#endif
    public static ReactiveUIBuilder RegisterSingletonViewModel<TViewModel>(this ReactiveUIBuilder builder)
        where TViewModel : class, IReactiveObject, new()
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithRegistration(resolver => resolver.RegisterLazySingleton<TViewModel>(() => new()));
    }

    /// <summary>
    /// Registers a custom view for a specific view model.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static ReactiveUIBuilder RegisterView<TView, TViewModel>(this ReactiveUIBuilder builder)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class, IReactiveObject
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithRegistration(resolver => resolver.Register<IViewFor<TViewModel>>(() => new TView()));
    }

    /// <summary>
    /// Registers a custom view for a specific view model.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static ReactiveUIBuilder RegisterSingletonView<TView, TViewModel>(this ReactiveUIBuilder builder)
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class, IReactiveObject
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.WithRegistration(resolver => resolver.RegisterLazySingleton<IViewFor<TViewModel>>(() => new TView()));
    }
}
