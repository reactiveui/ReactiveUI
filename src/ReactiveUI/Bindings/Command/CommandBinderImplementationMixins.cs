// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace ReactiveUI;

/// <summary>
/// Internal implementation details which performs Binding ICommand's to controls.
/// </summary>
internal static class CommandBinderImplementationMixins
{
    /// <summary>
    /// Binds an ICommand property on the view model to a control on the view, wiring the command to the specified event
    /// on the control.
    /// </summary>
    /// <remarks>This method uses reflection to observe events and properties on the control and may not be
    /// compatible with all trimming scenarios. The binding will automatically enable or disable the control based on
    /// the command's CanExecute state.</remarks>
    /// <typeparam name="TView">The type of the view, which must implement IViewFor.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TProp">The type of the ICommand property on the view model.</typeparam>
    /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
    /// <param name="this">The command binder implementation used to perform the binding.</param>
    /// <param name="viewModel">The view model instance containing the ICommand property to bind. Can be null if the view is not currently
    /// associated with a view model.</param>
    /// <param name="view">The view instance containing the control to which the command will be bound.</param>
    /// <param name="propertyName">An expression identifying the ICommand property on the view model to bind.</param>
    /// <param name="controlName">An expression identifying the control on the view to which the command will be bound.</param>
    /// <param name="toEvent">The name of the event on the control that will trigger the command. If null, a default event is used based on
    /// the control type.</param>
    /// <returns>An IReactiveBinding{TView, TProp} representing the established binding between the command and the control
    /// event.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public static IReactiveBinding<TView, TProp> BindCommand<TView, TViewModel, TProp, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] TControl>(
        this ICommandBinderImplementation @this,
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp?>> propertyName,
        Expression<Func<TView, TControl>> controlName,
        string? toEvent = null)
            where TView : class, IViewFor
            where TViewModel : class
            where TProp : ICommand
            where TControl : class =>
        @this.BindCommand(viewModel, view, propertyName, controlName, Observable<object>.Empty, toEvent);

    /// <summary>
    /// Binds a command from the view model to a control on the view, using a parameter expression to supply the command
    /// parameter dynamically.
    /// </summary>
    /// <remarks>This overload allows the command parameter to be supplied dynamically by observing changes in
    /// the view model. The binding will automatically update the command parameter as the observed value changes. The
    /// method uses reflection and may be affected by trimming in environments that perform code trimming.</remarks>
    /// <typeparam name="TView">The type of the view implementing the IViewFor interface.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model containing the command.</typeparam>
    /// <typeparam name="TProp">The type of the command property on the view model.</typeparam>
    /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the command when it is executed.</typeparam>
    /// <param name="this">The command binder implementation used to perform the binding.</param>
    /// <param name="viewModel">The view model instance containing the command to bind. Can be null if the view is not currently bound to a view
    /// model.</param>
    /// <param name="view">The view instance containing the control to which the command will be bound.</param>
    /// <param name="propertyName">An expression identifying the command property on the view model to bind.</param>
    /// <param name="controlName">An expression identifying the control on the view to which the command will be bound.</param>
    /// <param name="withParameter">An expression that specifies how to obtain the parameter value to pass to the command when it is executed.
    /// Cannot be null.</param>
    /// <param name="toEvent">The name of the event on the control that triggers the command execution. If null, a default event is used based
    /// on the control type.</param>
    /// <returns>An object representing the binding between the command and the control, which can be disposed to unbind.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public static IReactiveBinding<TView, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
        this ICommandBinderImplementation @this,
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp?>> propertyName,
        Expression<Func<TView, TControl>> controlName,
        Expression<Func<TViewModel, TParam>> withParameter,
        string? toEvent = null)
        where TView : class, IViewFor
        where TViewModel : class
        where TProp : ICommand
        where TControl : class
    {
        ArgumentExceptionHelper.ThrowIfNull(withParameter);

        var paramExpression = Reflection.Rewrite(withParameter.Body);
        var param = Reflection.ViewModelWhenAnyValue(viewModel, view, paramExpression);

        return @this.BindCommand(viewModel, view, propertyName, controlName, param, toEvent);
    }
}
