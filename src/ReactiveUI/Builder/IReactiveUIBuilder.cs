// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using Splat.Builder;

namespace ReactiveUI.Builder
{
    /// <summary>
    /// IReactiveUIBuilder.
    /// </summary>
    /// <seealso cref="Splat.Builder.IAppBuilder" />
    public interface IReactiveUIBuilder : IAppBuilder
    {
        /// <summary>
        /// Configures the message bus.
        /// </summary>
        /// <param name="configure">The configure.</param>
        /// <returns>The builder instance for chaining.</returns>
        IReactiveUIBuilder ConfigureMessageBus(Action<MessageBus> configure);

        /// <summary>
        /// Configures the suspension driver.
        /// </summary>
        /// <param name="configure">The configure.</param>
        /// <returns>The builder instance for chaining.</returns>
        IReactiveUIBuilder ConfigureSuspensionDriver(Action<ISuspensionDriver> configure);

        /// <summary>
        /// Configures the view locator.
        /// </summary>
        /// <param name="configure">The configure.</param>
        /// <returns>The builder instance for chaining.</returns>
        IReactiveUIBuilder ConfigureViewLocator(Action<DefaultViewLocator> configure);

        /// <summary>
        /// Fors the custom platform.
        /// </summary>
        /// <param name="mainThreadScheduler">The main thread scheduler.</param>
        /// <param name="platformServices">The platform services.</param>
        /// <returns>The builder instance for chaining.</returns>
        IReactiveUIBuilder ForCustomPlatform(IScheduler mainThreadScheduler, Action<IMutableDependencyResolver> platformServices);

        /// <summary>
        /// Fors the platforms.
        /// </summary>
        /// <param name="platformConfigurations">The platform configurations.</param>
        /// <returns>The builder instance for chaining.</returns>
        IReactiveUIBuilder ForPlatforms(params Action<IReactiveUIBuilder>[] platformConfigurations);

        /// <summary>
        /// Registers the singleton view.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
        IReactiveUIBuilder RegisterSingletonView<TView, TViewModel>()
            where TView : class, IViewFor<TViewModel>, new()
            where TViewModel : class, IReactiveObject;

        /// <summary>
        /// Registers the singleton view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
#if NET6_0_OR_GREATER
        IReactiveUIBuilder RegisterSingletonViewModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TViewModel>()
#else
        IReactiveUIBuilder RegisterSingletonViewModel<TViewModel>()
#endif
            where TViewModel : class, IReactiveObject, new();

        /// <summary>
        /// Registers the view.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
        IReactiveUIBuilder RegisterView<TView, TViewModel>()
            where TView : class, IViewFor<TViewModel>, new()
            where TViewModel : class, IReactiveObject;

        /// <summary>
        /// Registers the view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
        IReactiveUIBuilder RegisterViewModel<TViewModel>()
            where TViewModel : class, IReactiveObject, new();

        /// <summary>
        /// Withes the main thread scheduler.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="setRxApp">if set to <c>true</c> [set rx application].</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        IReactiveUIBuilder WithMainThreadScheduler(IScheduler scheduler, bool setRxApp = true);

        /// <summary>
        /// Withes the platform module.
        /// </summary>
        /// <typeparam name="T">The type of the registration module that implements IWantsToRegisterStuff.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
        IReactiveUIBuilder WithPlatformModule<T>()
            where T : IWantsToRegisterStuff, new();

        /// <summary>
        /// Withes the platform services.
        /// </summary>
        /// <returns>The builder instance for chaining.</returns>
        IReactiveUIBuilder WithPlatformServices();

        /// <summary>
        /// Withes the registration.
        /// </summary>
        /// <param name="configureAction">The configure action.</param>
        /// <returns>The builder instance for chaining.</returns>
        IReactiveUIBuilder WithRegistration(Action<IMutableDependencyResolver> configureAction);

        /// <summary>
        /// Withes the registration on build.
        /// </summary>
        /// <param name="configureAction">The configure action.</param>
        /// <returns>The builder instance for chaining.</returns>
        IReactiveUIBuilder WithRegistrationOnBuild(Action<IMutableDependencyResolver> configureAction);

        /// <summary>
        /// Withes the task pool scheduler.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="setRxApp">if set to <c>true</c> [set rx application].</param>
        /// <returns>
        /// The builder instance for chaining.
        /// </returns>
        IReactiveUIBuilder WithTaskPoolScheduler(IScheduler scheduler, bool setRxApp = true);

        /// <summary>
        /// Withes the views from assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The builder instance for chaining.</returns>

#if NET6_0_OR_GREATER
        [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
        [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
        IReactiveUIBuilder WithViewsFromAssembly(Assembly assembly);

        /// <summary>
        /// Using the splat module.
        /// </summary>
        /// <typeparam name="T">The Splat Module Type.</typeparam>
        /// <param name="registrationModule">The registration module to add.</param>
        /// <returns>
        /// The builder instance for method chaining.
        /// </returns>
        IReactiveUIBuilder UsingSplatModule<T>(T registrationModule)
            where T : IModule;
    }
}
