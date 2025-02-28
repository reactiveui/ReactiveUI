// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace ReactiveUI;

/// <summary>
/// Implementation logic for command binding.
/// </summary>
internal interface ICommandBinderImplementation : IEnableLogger
{
    /// <summary>
    /// Binds the command on a ViewModel to a control on the View.
    /// </summary>
    /// <param name="viewModel">The view model to bind to.</param>
    /// <param name="view">The view to bind to.</param>
    /// <param name="vmProperty">The name of the property on the View Model.</param>
    /// <param name="controlProperty">The name of the control on the View.</param>
    /// <param name="withParameter">A function if we want to pass a parameter to the ICommand.</param>
    /// <param name="toEvent">A event on the view that will also trigger the command.</param>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TProp">The type of the property on the view model.</typeparam>
    /// <typeparam name="TControl">The type of control on the view.</typeparam>
    /// <typeparam name="TParam">The type of the parameter to pass to the ICommand.</typeparam>
    /// <returns>A reactive binding. Often only used for disposing the binding.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    IReactiveBinding<TView, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp?>> vmProperty,
        Expression<Func<TView, TControl>> controlProperty,
        Expression<Func<TViewModel, TParam?>> withParameter,
        string? toEvent = null)
        where TView : class, IViewFor
        where TViewModel : class
        where TProp : ICommand;

    /// <summary>
    /// Binds the command on a ViewModel to a control on the View.
    /// </summary>
    /// <param name="viewModel">The view model to bind to.</param>
    /// <param name="view">The view to bind to.</param>
    /// <param name="vmProperty">The name of the property on the View Model.</param>
    /// <param name="controlProperty">The name of the control on the View.</param>
    /// <param name="withParameter">A observable if we want to pass a parameter to the ICommand.</param>
    /// <param name="toEvent">A event on the view that will also trigger the command.</param>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TProp">The type of the property on the view model.</typeparam>
    /// <typeparam name="TControl">The type of control on the view.</typeparam>
    /// <typeparam name="TParam">The type of the parameter to pass to the ICommand.</typeparam>
    /// <returns>A reactive binding. Often only used for disposing the binding.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    IReactiveBinding<TView, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp?>> vmProperty,
        Expression<Func<TView, TControl>> controlProperty,
        IObservable<TParam?> withParameter,
        string? toEvent = null)
        where TView : class, IViewFor
        where TViewModel : class
        where TProp : ICommand;
}
