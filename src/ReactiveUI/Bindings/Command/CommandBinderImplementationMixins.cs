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
    /// Binds a command from the view model to a control on the view, enabling the control to execute the command with a
    /// specified parameter when an event is raised.
    /// </summary>
    /// <remarks>
    /// <para>The binding enables the control to execute the command when the specified event is raised,
    /// and automatically manages the enabled state of the control based on the command's CanExecute state.</para>
    /// <para>This method uses reflection to observe events and properties on the control, which may be
    /// affected by trimming in some deployment scenarios.</para>
    /// </remarks>
    /// <typeparam name="TView">The type of the view that implements the IViewFor interface.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model containing the command to bind.</typeparam>
    /// <typeparam name="TProp">The type of the command property on the view model. Must implement ICommand.</typeparam>
    /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
    /// <param name="this">The command binder implementation used to perform the binding.</param>
    /// <param name="viewModel">The view model instance containing the command to bind. Used for type inference.
    /// Can be null if the binding should be established without an initial view model.</param>
    /// <param name="view">The view instance containing the control to which the command will be bound. Cannot be null.</param>
    /// <param name="propertyName">An expression identifying the command property on the view model to bind. Cannot be null.</param>
    /// <param name="controlName">An expression identifying the control on the view to which the command will be bound. Cannot be null.</param>
    /// <param name="toEvent">The name of the event on the control that triggers the command execution. If null, a default event is used based
    /// on the control type. If the specified event does not exist on the control, an exception may be thrown at runtime.
    /// NOTE: If this parameter is used inside WhenActivated, it's important to dispose the binding when the view is deactivated.</param>
    /// <returns>An IReactiveBinding{TView, TProp} representing the established binding between the command and the control.
    /// It will remain active until disposed.</returns>
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
}
