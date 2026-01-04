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
    /// Binds a command on the ViewModel to a control on the View (no explicit event name).
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TProp">The command property type.</typeparam>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <param name="this">The command binder implementation.</param>
    /// <param name="viewModel">The view model instance.</param>
    /// <param name="view">The view instance.</param>
    /// <param name="propertyName">Expression selecting the command property on the view model.</param>
    /// <param name="controlName">Expression selecting the control on the view.</param>
    /// <param name="toEvent">Optional event name on the control that will trigger the command.</param>
    /// <returns>A reactive binding representing the command binding.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="this"/>, <paramref name="view"/>, <paramref name="propertyName"/>, or <paramref name="controlName"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Trimming note: requires only public properties on the control type.
    /// </remarks>
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
    /// Binds a command on the ViewModel to a control on the View (optional explicit event name).
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TProp">The command property type.</typeparam>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <typeparam name="TParam">The command parameter type.</typeparam>
    /// <param name="this">The command binder implementation.</param>
    /// <param name="viewModel">The view model instance.</param>
    /// <param name="view">The view instance.</param>
    /// <param name="propertyName">Expression selecting the command property on the view model.</param>
    /// <param name="controlName">Expression selecting the control on the view.</param>
    /// <param name="withParameter">Expression selecting a command parameter value from the view model.</param>
    /// <param name="toEvent">Optional event name on the control that will trigger the command.</param>
    /// <returns>A reactive binding representing the command binding.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="this"/>, <paramref name="view"/>, <paramref name="propertyName"/>, or <paramref name="controlName"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Trimming note: if <paramref name="toEvent"/> is specified, implementations may reflect over public events.
    /// </remarks>
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
