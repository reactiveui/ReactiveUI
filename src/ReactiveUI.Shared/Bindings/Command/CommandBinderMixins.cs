// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Windows.Input;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// Provides static methods for binding commands from a view model to controls on a view, enabling declarative command
/// wiring in reactive user interfaces.
/// </summary>
/// <remarks>The CommandBinderMixins class offers extension methods to facilitate the binding of ICommand properties
/// from a view model to specific controls on a view. These methods support advanced scenarios such as specifying custom
/// events for command invocation and passing parameters from the view model. Bindings created using these methods
/// should be disposed appropriately, especially when used within activation lifecycles, to prevent memory leaks or
/// unintended behavior. The methods use reflection and may be affected by trimming in certain deployment
/// scenarios.</remarks>
public static class CommandBinderMixins
{
    /// <summary>The command binder implementation resolved from the service locator or the default implementation.</summary>
    private static readonly ICommandBinderImplementation _binderImplementation;

    /// <summary>Initializes static members of the <see cref="CommandBinderMixins"/> class.</summary>
    /// <remarks>This static constructor ensures that the command binding implementation is set up before any
    /// static members of the CommandBinderMixins class are accessed. It attempts to retrieve an ICommandBinderImplementation
    /// from the application's service locator; if none is available, a default implementation is used.</remarks>
    static CommandBinderMixins() => _binderImplementation = AppLocator.Current.GetService<ICommandBinderImplementation>() ??
                                                      new CommandBinderImplementation();

    /// <summary>Provides command binding extension members for views implementing <see cref="IViewFor"/>.</summary>
    /// <typeparam name="TView">The type of the view implementing the IViewFor interface.</typeparam>
    /// <param name="view">The view instance to which the command will be bound.</param>
    extension<TView>(TView view)
        where TView : class, IViewFor
    {
        /// <summary>Binds a command from the view model to a control on the view using the default event and an observable parameter.</summary>
        /// <typeparam name="TViewModel">The type of the view model containing the command property.</typeparam>
        /// <typeparam name="TProp">The type of the command property, which must implement ICommand.</typeparam>
        /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
        /// <typeparam name="TParam">The type of the parameter passed to the command when it is executed.</typeparam>
        /// <param name="viewModel">The view model instance containing the command property.</param>
        /// <param name="propertyName">An expression identifying the command property on the view model to bind.</param>
        /// <param name="controlName">An expression identifying the control on the view to which the command will be bound.</param>
        /// <param name="withParameter">An observable that provides the parameter to pass to the command when it is executed.</param>
        /// <returns>An IReactiveBinding instance representing the active binding between the command and the control.</returns>
        [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
        public IReactiveBinding<TView, TProp> BindCommand<
            TViewModel,
            TProp,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                        DynamicallyAccessedMemberTypes.NonPublicEvents |
                                        DynamicallyAccessedMemberTypes.PublicProperties)]
        TControl,
            TParam>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TProp?>> propertyName,
            Expression<Func<TView, TControl>> controlName,
            IObservable<TParam?> withParameter)
            where TViewModel : class
            where TProp : ICommand
            where TControl : class
        {
            ArgumentExceptionHelper.ThrowIfNull(view);
            ArgumentExceptionHelper.ThrowIfNull(propertyName);
            ArgumentExceptionHelper.ThrowIfNull(controlName);
            ArgumentExceptionHelper.ThrowIfNull(withParameter);

            return _binderImplementation.BindCommand(viewModel, view, propertyName, controlName, withParameter);
        }

        /// <summary>
        /// Binds a command from the view model to a control on the view, enabling the control to execute the command with a
        /// parameter when triggered.
        /// </summary>
        /// <remarks>This method uses reflection to dynamically observe events and properties on the control,
        /// which may be affected by trimming in some deployment scenarios. The binding remains active until the returned
        /// IReactiveBinding is disposed.</remarks>
        /// <typeparam name="TViewModel">The type of the view model containing the command property.</typeparam>
        /// <typeparam name="TProp">The type of the command property, which must implement ICommand.</typeparam>
        /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
        /// <typeparam name="TParam">The type of the parameter passed to the command when it is executed.</typeparam>
        /// <param name="viewModel">The view model instance containing the command property. May be null if the view is not currently bound to a
        /// view model.</param>
        /// <param name="propertyName">An expression identifying the command property on the view model to bind. Cannot be null.</param>
        /// <param name="controlName">An expression identifying the control on the view to which the command will be bound. Cannot be null.</param>
        /// <param name="withParameter">An observable that provides the parameter to pass to the command when it is executed. Cannot be null.</param>
        /// <param name="toEvent">The name of the event on the control that triggers the command. If null, a default event is used based on the
        /// control type.
        /// NOTE: If this parameter is used inside WhenActivated, it's important to dispose the binding when the view is deactivated.</param>
        /// <returns>An IReactiveBinding instance representing the active binding between the command and the control.</returns>
        [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
        public IReactiveBinding<TView, TProp> BindCommand<
            TViewModel,
            TProp,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                        DynamicallyAccessedMemberTypes.NonPublicEvents |
                                        DynamicallyAccessedMemberTypes.PublicProperties)]
        TControl,
            TParam>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TProp?>> propertyName,
            Expression<Func<TView, TControl>> controlName,
            IObservable<TParam?> withParameter,
            string? toEvent)
            where TViewModel : class
            where TProp : ICommand
            where TControl : class
        {
            ArgumentExceptionHelper.ThrowIfNull(view);
            ArgumentExceptionHelper.ThrowIfNull(propertyName);
            ArgumentExceptionHelper.ThrowIfNull(controlName);
            ArgumentExceptionHelper.ThrowIfNull(withParameter);

            return _binderImplementation.BindCommand(viewModel, view, propertyName, controlName, withParameter, toEvent);
        }

        /// <summary>Binds a command from the view model to a control on the view using the default event.</summary>
        /// <typeparam name="TViewModel">The type of the view model containing the command property.</typeparam>
        /// <typeparam name="TProp">The type of the command property to bind, implementing ICommand.</typeparam>
        /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
        /// <param name="viewModel">The view model instance containing the command property.</param>
        /// <param name="propertyName">An expression identifying the command property on the view model to bind.</param>
        /// <param name="controlName">An expression identifying the control on the view to bind the command to.</param>
        /// <returns>An object representing the binding between the command and the control, which can be disposed to unbind.</returns>
        [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
        public IReactiveBinding<TView, TProp> BindCommand<
            TViewModel,
            TProp,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                        DynamicallyAccessedMemberTypes.NonPublicEvents |
                                        DynamicallyAccessedMemberTypes.PublicProperties)]
        TControl>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TProp?>> propertyName,
            Expression<Func<TView, TControl>> controlName)
            where TViewModel : class
            where TProp : ICommand
            where TControl : class =>
            BindCommand(view, viewModel, propertyName, controlName, (string?)null);

        /// <summary>
        /// Binds a command from the view model to a control on the view, enabling the control to execute the specified
        /// command when triggered.
        /// </summary>
        /// <remarks>This method uses reflection to observe events and properties on the control and may be
        /// affected by trimming in environments that remove unused members. The binding enables the control to execute the
        /// command when the specified event is raised, and automatically manages the enabled state of the control based on
        /// the command's CanExecute state.</remarks>
        /// <typeparam name="TViewModel">The type of the view model containing the command property.</typeparam>
        /// <typeparam name="TProp">The type of the command property to bind, implementing ICommand.</typeparam>
        /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
        /// <param name="viewModel">The view model instance containing the command property. Can be null if the view's ViewModel property is used.</param>
        /// <param name="propertyName">An expression identifying the command property on the view model to bind. Cannot be null.</param>
        /// <param name="controlName">An expression identifying the control on the view to bind the command to. Cannot be null.</param>
        /// <param name="toEvent">The name of the event on the control that triggers the command. If null, a default event is used based on the
        /// control type.
        /// NOTE: If this parameter is used inside WhenActivated, it's important to dispose the binding when the view is deactivated.</param>
        /// <returns>An object representing the binding between the command and the control, which can be disposed to unbind.</returns>
        [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
        public IReactiveBinding<TView, TProp> BindCommand<
            TViewModel,
            TProp,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                        DynamicallyAccessedMemberTypes.NonPublicEvents |
                                        DynamicallyAccessedMemberTypes.PublicProperties)]
        TControl>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TProp?>> propertyName,
            Expression<Func<TView, TControl>> controlName,
            string? toEvent)
            where TViewModel : class
            where TProp : ICommand
            where TControl : class
        {
            ArgumentExceptionHelper.ThrowIfNull(view);
            ArgumentExceptionHelper.ThrowIfNull(propertyName);
            ArgumentExceptionHelper.ThrowIfNull(controlName);

            return _binderImplementation.BindCommand(viewModel, view, propertyName, controlName, Signal.None<object?>(), toEvent);
        }

        /// <summary>Binds a command from the view model to a control using the default event and a view model expression parameter.</summary>
        /// <typeparam name="TViewModel">The type of the view model containing the command property.</typeparam>
        /// <typeparam name="TProp">The type of the command property, typically implementing ICommand.</typeparam>
        /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
        /// <typeparam name="TParam">The type of the parameter passed to the command when it is executed.</typeparam>
        /// <param name="viewModel">The view model instance containing the command property.</param>
        /// <param name="propertyName">An expression identifying the command property on the view model to bind.</param>
        /// <param name="controlName">An expression identifying the control on the view to which the command will be bound.</param>
        /// <param name="withParameter">An expression specifying the parameter to pass to the command when it is executed.</param>
        /// <returns>An IReactiveBinding representing the established binding between the command and the control.</returns>
        [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
        public IReactiveBinding<TView, TProp> BindCommand<
            TViewModel,
            TProp,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                        DynamicallyAccessedMemberTypes.NonPublicEvents |
                                        DynamicallyAccessedMemberTypes.PublicProperties)]
        TControl,
            TParam>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TProp?>> propertyName,
            Expression<Func<TView, TControl>> controlName,
            Expression<Func<TViewModel, TParam?>> withParameter)
            where TViewModel : class
            where TProp : ICommand
            where TControl : class
        {
            ArgumentExceptionHelper.ThrowIfNull(view);
            ArgumentExceptionHelper.ThrowIfNull(propertyName);
            ArgumentExceptionHelper.ThrowIfNull(controlName);
            ArgumentExceptionHelper.ThrowIfNull(withParameter);

            return _binderImplementation.BindCommand(viewModel, view, propertyName, controlName, withParameter);
        }

        /// <summary>
        /// Binds a command from the view model to a control on the view, enabling the control to execute the command with a
        /// specified parameter when triggered.
        /// </summary>
        /// <remarks>This method uses reflection to observe events and properties on the control and view model,
        /// which may be affected by trimming in some deployment scenarios. The binding remains active until the returned
        /// IReactiveBinding is disposed.</remarks>
        /// <typeparam name="TViewModel">The type of the view model containing the command property.</typeparam>
        /// <typeparam name="TProp">The type of the command property, typically implementing ICommand.</typeparam>
        /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
        /// <typeparam name="TParam">The type of the parameter passed to the command when it is executed.</typeparam>
        /// <param name="viewModel">The view model instance containing the command property. May be null if the view is not currently bound to a
        /// view model.</param>
        /// <param name="propertyName">An expression identifying the command property on the view model to bind. Cannot be null.</param>
        /// <param name="controlName">An expression identifying the control on the view to which the command will be bound. Cannot be null.</param>
        /// <param name="withParameter">An expression specifying the parameter to pass to the command when it is executed. Cannot be null.</param>
        /// <param name="toEvent">The name of the event on the control that triggers the command execution. If null, a default event is used based
        /// on the control type.
        /// NOTE: If this parameter is used inside WhenActivated, it's important to dispose the binding when the view is deactivated.</param>
        /// <returns>An IReactiveBinding{TView, TProp} representing the established binding between the command and the control.</returns>
        [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
        public IReactiveBinding<TView, TProp> BindCommand<
            TViewModel,
            TProp,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                        DynamicallyAccessedMemberTypes.NonPublicEvents |
                                        DynamicallyAccessedMemberTypes.PublicProperties)]
        TControl,
            TParam>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TProp?>> propertyName,
            Expression<Func<TView, TControl>> controlName,
            Expression<Func<TViewModel, TParam?>> withParameter,
            string? toEvent)
            where TViewModel : class
            where TProp : ICommand
            where TControl : class
        {
            ArgumentExceptionHelper.ThrowIfNull(view);
            ArgumentExceptionHelper.ThrowIfNull(propertyName);
            ArgumentExceptionHelper.ThrowIfNull(controlName);
            ArgumentExceptionHelper.ThrowIfNull(withParameter);

            return _binderImplementation.BindCommand(viewModel, view, propertyName, controlName, withParameter, toEvent);
        }
    }
}
