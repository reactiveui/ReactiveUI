// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using Splat.Builder;

namespace ReactiveUI.Builder;

/// <summary>
/// IReactiveUIBuilder.
/// </summary>
/// <seealso cref="Splat.Builder.IAppBuilder" />
public interface IReactiveUIBuilder : IAppBuilder
{
    /// <summary>
    /// Configures the message bus.
    /// </summary>
    /// <param name="configure">A delegate to configure the message bus.</param>
    /// <returns>The builder instance for chaining.</returns>
    IReactiveUIBuilder ConfigureMessageBus(Action<MessageBus> configure);

    /// <summary>
    /// Configures the suspension driver.
    /// </summary>
    /// <param name="configure">A delegate to configure the suspension driver.</param>
    /// <returns>The builder instance for chaining.</returns>
    IReactiveUIBuilder ConfigureSuspensionDriver(Action<ISuspensionDriver> configure);

    /// <summary>
    /// Configures the view locator.
    /// </summary>
    /// <param name="configure">A delegate to configure the view locator.</param>
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
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
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

        /// <summary>
        /// Builds the application and returns the ReactiveUI instance wrapper.
        /// </summary>
        /// <returns>IReactiveUIInstance instance for chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown if building the app instance fails.</exception>
        IReactiveUIInstance BuildApp();

        /// <summary>
        /// Resolves a single instance and passes it to the action.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>IReactiveUIInstance instance for chaining.</returns>
        IReactiveUIInstance WithInstance<T>(Action<T?> action);

        /// <summary>
        /// Resolves two instances and passes them to the action.
        /// </summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>IReactiveUIInstance instance for chaining.</returns>
        IReactiveUIInstance WithInstance<T1, T2>(Action<T1?, T2?> action);

        /// <summary>
        /// Resolves three instances and passes them to the action.
        /// </summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>IReactiveUIInstance instance for chaining.</returns>
        IReactiveUIInstance WithInstance<T1, T2, T3>(Action<T1?, T2?, T3?> action);

        /// <summary>
        /// Resolves four instances and passes them to the action.
        /// </summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>IReactiveUIInstance instance for chaining.</returns>
        IReactiveUIInstance WithInstance<T1, T2, T3, T4>(Action<T1?, T2?, T3?, T4?> action);

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
        IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5>(Action<T1?, T2?, T3?, T4?, T5?> action);

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
        IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6>(Action<T1?, T2?, T3?, T4?, T5?, T6?> action);

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
        IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?> action);

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
        IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?> action);

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
        IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?> action);

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
        IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?> action);

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
        IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?> action);

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
        IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?> action);

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
        IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?> action);

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
        IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?> action);

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
        IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?> action);

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
        IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?> action);
    }
}
