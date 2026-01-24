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
    /// Binds a command from the view model to a control on the view, enabling the control to execute the command with
    /// an optional parameter when a specified event is raised.
    /// </summary>
    /// <remarks>This method uses reflection to observe events and properties on the control, which may be
    /// affected by trimming in some deployment scenarios. The binding remains active until it is disposed. If the
    /// specified event is not found on the control, an exception may be thrown at runtime.</remarks>
    /// <typeparam name="TView">The type of the view implementing the IViewFor interface.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model containing the command to bind.</typeparam>
    /// <typeparam name="TProp">The type of the command property on the view model, typically implementing ICommand.</typeparam>
    /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the command when it is executed.</typeparam>
    /// <param name="viewModel">The view model instance containing the command to bind. Can be null if the binding should be established without
    /// an initial view model.</param>
    /// <param name="view">The view instance containing the control to which the command will be bound. Cannot be null.</param>
    /// <param name="vmProperty">An expression identifying the command property on the view model to bind.</param>
    /// <param name="controlProperty">An expression identifying the control on the view that will trigger the command.</param>
    /// <param name="withParameter">An expression specifying the parameter to pass to the command when it is executed.</param>
    /// <param name="toEvent">The name of the event on the control that triggers the command execution. If null, a default event is used based
    /// on the control type.</param>
    /// <returns>An IReactiveBinding{TView, TProp} representing the established binding between the command and the control.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    IReactiveBinding<TView, TProp> BindCommand<TView, TViewModel, TProp, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] TControl, TParam>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp?>> vmProperty,
        Expression<Func<TView, TControl>> controlProperty,
        Expression<Func<TViewModel, TParam?>> withParameter,
        string? toEvent = null)
            where TView : class, IViewFor
            where TViewModel : class
            where TProp : ICommand
            where TControl : class;

    /// <summary>
    /// Binds a command from the view model to a control on the view, enabling the control to execute the command with a
    /// specified parameter when an event is raised.
    /// </summary>
    /// <remarks>This method uses reflection to observe events and properties on the control, which may be
    /// affected by trimming in some deployment scenarios. The binding remains active until the returned
    /// IReactiveBinding is disposed. If the specified event does not exist on the control, an exception may be thrown
    /// at runtime.</remarks>
    /// <typeparam name="TView">The type of the view that implements the IViewFor interface.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model containing the command to bind.</typeparam>
    /// <typeparam name="TProp">The type of the command property on the view model. Must implement ICommand.</typeparam>
    /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the command when the event is raised.</typeparam>
    /// <param name="viewModel">The view model instance containing the command to bind. Can be null if the binding should be established without
    /// an initial view model.</param>
    /// <param name="view">The view instance containing the control to which the command will be bound. Cannot be null.</param>
    /// <param name="vmProperty">An expression identifying the command property on the view model to bind.</param>
    /// <param name="controlProperty">An expression identifying the control on the view to which the command will be bound.</param>
    /// <param name="withParameter">An observable that provides the parameter value to pass to the command when the event is raised. Can emit null
    /// values.</param>
    /// <param name="toEvent">The name of the event on the control that triggers the command execution. If null, a default event is used based
    /// on the control type.</param>
    /// <returns>An IReactiveBinding{TView, TProp} instance representing the established binding between the command and the
    /// control.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    IReactiveBinding<TView, TProp> BindCommand<TView, TViewModel, TProp, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] TControl, TParam>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp?>> vmProperty,
        Expression<Func<TView, TControl>> controlProperty,
        IObservable<TParam?> withParameter,
        string? toEvent = null)
            where TView : class, IViewFor
            where TViewModel : class
            where TProp : ICommand
            where TControl : class;
}
