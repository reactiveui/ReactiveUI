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
    /// Binds a command from the ViewModel to an explicitly specified control on the view.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TProp">The property type of the command.</typeparam>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <param name="viewModel">The view model instance.</param>
    /// <param name="view">The view instance.</param>
    /// <param name="vmProperty">An expression selecting the ViewModel command to bind.</param>
    /// <param name="controlProperty">An expression selecting the control on the view.</param>
    /// <param name="withParameter">An expression selecting the ViewModel property to pass as the command parameter.</param>
    /// <param name="toEvent">
    /// If specified, binds to the given event instead of the default command event.
    /// If used inside <c>WhenActivated</c>, ensure the returned binding is disposed when the view deactivates.
    /// </param>
    /// <returns>An <see cref="IReactiveBinding{TView, TProp}"/> representing the binding; dispose it to disconnect.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="vmProperty"/> or <paramref name="controlProperty"/> is <see langword="null"/>.
    /// </exception>
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
    /// Binds a command from the ViewModel to an explicitly specified control on the view.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TProp">The property type of the command.</typeparam>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <param name="viewModel">The view model instance.</param>
    /// <param name="view">The view instance.</param>
    /// <param name="vmProperty">An expression selecting the ViewModel command to bind.</param>
    /// <param name="controlProperty">An expression selecting the control on the view.</param>
    /// <param name="withParameter">An observable providing values to pass as the command parameter.</param>
    /// <param name="toEvent">
    /// If specified, binds to the given event instead of the default command event.
    /// If used inside <c>WhenActivated</c>, ensure the returned binding is disposed when the view deactivates.
    /// </param>
    /// <returns>An <see cref="IReactiveBinding{TView, TProp}"/> representing the binding; dispose it to disconnect.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="vmProperty"/> or <paramref name="controlProperty"/> is <see langword="null"/>.
    /// </exception>
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
    /// Wires the current command/control pair to an <see cref="ICommand"/> binding, and updates that wiring
    /// whenever the command instance or control instance changes.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TProp">The command type.</typeparam>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <param name="source">Observable producing command instances.</param>
    /// <param name="view">The view instance used to observe the control expression chain.</param>
    /// <param name="controlExpression">The rewritten expression identifying the control on the view.</param>
    /// <param name="withParameter">Observable producing command parameter values.</param>
    /// <param name="toEvent">Optional event name override for the binding.</param>
    /// <returns>
    /// A disposable that tears down the observation subscription and the most-recently-created command binding.
    /// </returns>
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
