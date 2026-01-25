// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace ReactiveUI;

/// <summary>
/// Implements command binding for <see cref="CommandBinder"/> extension methods by wiring ViewModel
/// <see cref="ICommand"/> instances to view controls and keeping the binding up to date as the command
/// and/or control instance changes.
/// </summary>
/// <remarks>
/// <para>
/// This implementation uses expression rewriting and dynamic observation (via <c>WhenAny*</c> infrastructure)
/// to locate and track members described by expression trees.
/// </para>
/// <para>
/// For trimming/AOT: the public binding entry points are annotated because they may require reflection over
/// members that are not statically visible to the trimmer, and may require dynamic code paths depending on
/// platform/runtime.
/// </para>
/// </remarks>
public class CommandBinderImplementation : ICommandBinderImplementation
{
    /// <summary>
    /// Binds a command from the view model to a control on the view, enabling the control to execute the command with
    /// an optional parameter when triggered by a specified event.
    /// </summary>
    /// <remarks>This method uses reflection to observe properties and events, which may be affected by
    /// trimming in some deployment scenarios. The binding is one-way, from the view model command to the view control.
    /// If the specified event is not found on the control, an exception may be thrown at runtime.</remarks>
    /// <typeparam name="TView">The type of the view implementing the IViewFor interface.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model containing the command property.</typeparam>
    /// <typeparam name="TProp">The type of the command property to bind, implementing ICommand.</typeparam>
    /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the command when it is executed.</typeparam>
    /// <param name="viewModel">The view model instance containing the command to bind. Can be null if the binding should be established without
    /// an initial view model.</param>
    /// <param name="view">The view instance containing the control to which the command will be bound. Cannot be null.</param>
    /// <param name="vmProperty">An expression specifying the command property on the view model to bind. Cannot be null.</param>
    /// <param name="controlProperty">An expression specifying the control on the view to which the command will be bound. Cannot be null.</param>
    /// <param name="withParameter">An expression specifying the parameter to pass to the command when it is executed. Can be null if the command
    /// does not require a parameter.</param>
    /// <param name="toEvent">The name of the event on the control that triggers the command execution. If null, a default event is used based
    /// on the control type.</param>
    /// <returns>An IReactiveBinding{TView, TProp} representing the established command binding. Disposing the returned object
    /// will remove the binding.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public IReactiveBinding<TView, TProp> BindCommand<
        TView,
        TViewModel,
        TProp,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] TControl,
        TParam>(
            TViewModel? viewModel,
            TView view,
            Expression<Func<TViewModel, TProp?>> vmProperty,
            Expression<Func<TView, TControl>> controlProperty,
            Expression<Func<TViewModel, TParam?>> withParameter,
            string? toEvent = null)
        where TView : class, IViewFor
        where TViewModel : class
        where TProp : ICommand
        where TControl : class
    {
        ArgumentExceptionHelper.ThrowIfNull(vmProperty);
        ArgumentExceptionHelper.ThrowIfNull(controlProperty);

        var vmExpression = Reflection.Rewrite(vmProperty.Body);
        var controlExpression = Reflection.Rewrite(controlProperty.Body);

        var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Cast<TProp>();

        var bindingDisposable = BindCommandInternal<TView, TProp, TParam, TControl>(
            source,
            view,
            controlExpression,
            withParameter.ToObservable(viewModel),
            toEvent);

        return new ReactiveBinding<TView, TProp>(
            view,
            controlExpression,
            vmExpression,
            source,
            BindingDirection.OneWay,
            bindingDisposable);
    }

    /// <summary>
    /// Binds a command from the view model to a control on the view, enabling the control to execute the command with
    /// an optional parameter stream and event trigger.
    /// </summary>
    /// <remarks>This method uses reflection to observe and bind to members, which may be affected by trimming
    /// in some environments. The binding is one-way, from the view model command to the view control. If the control or
    /// command property is not found, the binding will not be established. The method is suitable for scenarios where
    /// commands need to be dynamically bound to controls with support for parameter streams and custom event
    /// triggers.</remarks>
    /// <typeparam name="TView">The type of the view implementing the IViewFor interface.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model containing the command property.</typeparam>
    /// <typeparam name="TProp">The type of the command property, which must implement ICommand.</typeparam>
    /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the command when it is executed.</typeparam>
    /// <param name="viewModel">The view model instance containing the command to bind. Can be null if the view model is not available at
    /// binding time.</param>
    /// <param name="view">The view instance containing the control to which the command will be bound.</param>
    /// <param name="vmProperty">An expression specifying the command property on the view model to bind.</param>
    /// <param name="controlProperty">An expression specifying the control on the view that will trigger the command.</param>
    /// <param name="withParameter">An observable sequence providing the parameter to pass to the command when it is executed. The latest value is
    /// used for each command invocation.</param>
    /// <param name="toEvent">The name of the event on the control that triggers the command. If null, a default event is used based on the
    /// control type.</param>
    /// <returns>An IReactiveBinding{TView, TProp} representing the established binding between the command and the control.
    /// Disposing the binding will remove the command association.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public IReactiveBinding<TView, TProp> BindCommand<
        TView,
        TViewModel,
        TProp,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] TControl,
        TParam>(
            TViewModel? viewModel,
            TView view,
            Expression<Func<TViewModel, TProp?>> vmProperty,
            Expression<Func<TView, TControl>> controlProperty,
            IObservable<TParam?> withParameter,
            string? toEvent = null)
        where TView : class, IViewFor
        where TViewModel : class
        where TProp : ICommand
        where TControl : class
    {
        ArgumentExceptionHelper.ThrowIfNull(vmProperty);
        ArgumentExceptionHelper.ThrowIfNull(controlProperty);

        var vmExpression = Reflection.Rewrite(vmProperty.Body);
        var controlExpression = Reflection.Rewrite(controlProperty.Body);

        var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Cast<TProp>();

        var bindingDisposable = BindCommandInternal<TView, TProp, TParam, TControl>(
            source,
            view,
            controlExpression,
            withParameter,
            toEvent);

        return new ReactiveBinding<TView, TProp>(
            view,
            controlExpression,
            vmExpression,
            source,
            BindingDirection.OneWay,
            bindingDisposable);
    }

    /// <summary>
    /// Binds an observable command to a control property or event on a view, updating the binding when the command or
    /// control instance changes.
    /// </summary>
    /// <remarks>This method observes both the command and the control instance, rebinding as either changes.
    /// It supports platform-specific command rebinding optimizations if available. The returned IDisposable should be
    /// disposed to clean up the binding and prevent memory leaks.</remarks>
    /// <typeparam name="TView">The type of the view that implements IViewFor.</typeparam>
    /// <typeparam name="TProp">The type of the command to bind, which must implement ICommand.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the command.</typeparam>
    /// <typeparam name="TControl">The type of the control to which the command is bound. Must be a class with public or non-public events and
    /// public properties.</typeparam>
    /// <param name="source">An observable sequence that provides the command instances to bind to the control.</param>
    /// <param name="view">The view instance containing the control to which the command will be bound.</param>
    /// <param name="controlExpression">An expression that identifies the control property or field on the view to bind the command to.</param>
    /// <param name="withParameter">An observable sequence that provides the parameter values to pass to the command when invoked. The parameter may
    /// be null.</param>
    /// <param name="toEvent">The name of the event on the control to bind the command to. If null or empty, the default event is used.</param>
    /// <returns>An IDisposable that can be used to unbind the command and release associated resources.</returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    private static IDisposable BindCommandInternal<
        TView,
        TProp,
        TParam,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] TControl>(
            IObservable<TProp> source,
            TView view,
            Expression controlExpression,
            IObservable<TParam?> withParameter,
            string? toEvent)
        where TView : class, IViewFor
        where TProp : ICommand
        where TControl : class
    {
        // SerialDisposable safely replaces and disposes the previous binding when a new one is assigned.
        var currentBinding = new SerialDisposable();
        var currentControl = default(TControl);
        var isInitialBind = true;

        // Check for optional platform-specific command rebinding customization
        var rebindingCustomizer = AppLocator.Current.GetService<ICreatesCustomizedCommandRebinding>();

        // Cache boxing of parameter values once to avoid rebuilding the Select pipeline on every rebind.
        var boxedParameter = withParameter.Select(static p => (object?)p);

        // Observe the control expression chain and extract the current control instance.
        var controlValues =
            view.SubscribeToExpressionChain<TView, object?>(
                    controlExpression,
                    beforeChange: false,
                    skipInitial: false,
                    suppressWarnings: false)
                .Select(static x => x.GetValue());

        // CombineLatest ensures rebinding occurs when either the command or control changes.
        // ValueTuple avoids per-notification heap allocations.
        var bindInfo = source.CombineLatest(controlValues, static (command, host) => (command, host));

        var subscription = bindInfo.Subscribe(tuple =>
        {
            var (command, host) = tuple;

            // Preserve existing behavior: if the control is currently null,
            // do not tear down or recreate the existing binding.
            if (host is null)
            {
                return;
            }

            // Match original semantics: allow null if the cast fails.
            var control = host as TControl;

            // Try platform-specific optimization: if only the command changed (not the control),
            // attempt to update the command directly without full rebind
            var isSameControl = !isInitialBind && ReferenceEquals(control, currentControl);
            if (isSameControl && control is not null && rebindingCustomizer is not null)
            {
                if (rebindingCustomizer.TryUpdateCommand(control, command))
                {
                    // Successfully updated command without rebinding
                    return;
                }
            }

            // Full rebind (first bind, control changed, or customizer not available/failed)
            isInitialBind = false;
            currentControl = control;

            // Assigning to SerialDisposable disposes the previous binding deterministically.
            currentBinding.Disposable =
                !string.IsNullOrEmpty(toEvent)
                    ? CreatesCommandBinding.BindCommandToObject<TControl, object>(
                        command,
                        control,
                        boxedParameter,
                        toEvent!)
                    : CreatesCommandBinding.BindCommandToObject(
                        command,
                        control,
                        boxedParameter);
        });

        // Dispose ordering: stop producing new bindings first, then dispose the active binding.
        return new CompositeDisposable(subscription, currentBinding);
    }
}
