// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Windows.Input;
using ReactiveUI.Primitives.Disposables;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// Implements command binding for <see cref="CommandBinderMixins"/> extension methods by wiring ViewModel
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
    /// <param name="viewModelProperty">An expression specifying the command property on the view model to bind. Cannot be null.</param>
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
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                    DynamicallyAccessedMemberTypes.NonPublicEvents |
                                    DynamicallyAccessedMemberTypes.PublicProperties)]
    TControl,
        TParam>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp?>> viewModelProperty,
        Expression<Func<TView, TControl>> controlProperty,
        Expression<Func<TViewModel, TParam?>> withParameter,
        string? toEvent)
        where TView : class, IViewFor
        where TViewModel : class
        where TProp : ICommand
        where TControl : class
    {
        ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);
        ArgumentExceptionHelper.ThrowIfNull(controlProperty);

        var viewModelExpression = Reflection.Rewrite(viewModelProperty.Body);
        var controlExpression = Reflection.Rewrite(controlProperty.Body);
        var parameterExpression = Reflection.Rewrite(withParameter.Body);

        var source = new MapSignal<object, TProp>(Reflection.ViewModelWhenAnyValue(viewModel, view, viewModelExpression), static x => (TProp)x!);

        // Observe the parameter through the view's current view model (not the originally supplied one) so the
        // parameter rebinds when the view model instance is replaced, matching the command source above.
        var parameter = new MapSignal<object, TParam?>(Reflection.ViewModelWhenAnyValue(viewModel, view, parameterExpression), static x => (TParam?)x);

        var bindingDisposable = BindCommandInternal<TView, TProp, TParam, TControl>(
            source,
            view,
            controlExpression,
            parameter,
            toEvent);

        return new ReactiveBinding<TView, TProp>(
            view,
            controlExpression,
            viewModelExpression,
            source,
            BindingDirection.OneWay,
            bindingDisposable);
    }

    /// <summary>
    /// Binds a command from the view model to a control on the view, enabling the control to execute the command with
    /// a parameter using the default event for the control type.
    /// </summary>
    /// <remarks>This method uses reflection to observe properties and events, which may be affected by
    /// trimming in some deployment scenarios. The binding is one-way, from the view model command to the view control.</remarks>
    /// <typeparam name="TView">The type of the view implementing the IViewFor interface.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model containing the command property.</typeparam>
    /// <typeparam name="TProp">The type of the command property to bind, implementing ICommand.</typeparam>
    /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the command when it is executed.</typeparam>
    /// <param name="viewModel">The view model instance containing the command to bind. Can be null if the binding should be established without
    /// an initial view model.</param>
    /// <param name="view">The view instance containing the control to which the command will be bound. Cannot be null.</param>
    /// <param name="viewModelProperty">An expression specifying the command property on the view model to bind. Cannot be null.</param>
    /// <param name="controlProperty">An expression specifying the control on the view to which the command will be bound. Cannot be null.</param>
    /// <param name="withParameter">An expression specifying the parameter to pass to the command when it is executed. Can be null if the command
    /// does not require a parameter.</param>
    /// <returns>An IReactiveBinding{TView, TProp} representing the established command binding. Disposing the returned object
    /// will remove the binding.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public IReactiveBinding<TView, TProp> BindCommand<
        TView,
        TViewModel,
        TProp,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                    DynamicallyAccessedMemberTypes.NonPublicEvents |
                                    DynamicallyAccessedMemberTypes.PublicProperties)]
    TControl,
        TParam>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp?>> viewModelProperty,
        Expression<Func<TView, TControl>> controlProperty,
        Expression<Func<TViewModel, TParam?>> withParameter)
        where TView : class, IViewFor
        where TViewModel : class
        where TProp : ICommand
        where TControl : class =>
        BindCommand<TView, TViewModel, TProp, TControl, TParam>(viewModel, view, viewModelProperty, controlProperty, withParameter, null);

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
    /// <param name="viewModelProperty">An expression specifying the command property on the view model to bind.</param>
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
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                    DynamicallyAccessedMemberTypes.NonPublicEvents |
                                    DynamicallyAccessedMemberTypes.PublicProperties)]
    TControl,
        TParam>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp?>> viewModelProperty,
        Expression<Func<TView, TControl>> controlProperty,
        IObservable<TParam?> withParameter,
        string? toEvent)
        where TView : class, IViewFor
        where TViewModel : class
        where TProp : ICommand
        where TControl : class
    {
        ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);
        ArgumentExceptionHelper.ThrowIfNull(controlProperty);

        var viewModelExpression = Reflection.Rewrite(viewModelProperty.Body);
        var controlExpression = Reflection.Rewrite(controlProperty.Body);

        var source = new MapSignal<object, TProp>(Reflection.ViewModelWhenAnyValue(viewModel, view, viewModelExpression), static x => (TProp)x!);

        var bindingDisposable = BindCommandInternal<TView, TProp, TParam, TControl>(
            source,
            view,
            controlExpression,
            withParameter,
            toEvent);

        return new ReactiveBinding<TView, TProp>(
            view,
            controlExpression,
            viewModelExpression,
            source,
            BindingDirection.OneWay,
            bindingDisposable);
    }

    /// <summary>
    /// Binds a command from the view model to a control on the view, enabling the control to execute the command with
    /// an parameter stream using the default event for the control type.
    /// </summary>
    /// <remarks>This method uses reflection to observe and bind to members, which may be affected by trimming
    /// in some environments. The binding is one-way, from the view model command to the view control. If the control or
    /// command property is not found, the binding will not be established.</remarks>
    /// <typeparam name="TView">The type of the view implementing the IViewFor interface.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model containing the command property.</typeparam>
    /// <typeparam name="TProp">The type of the command property, which must implement ICommand.</typeparam>
    /// <typeparam name="TControl">The type of the control on the view to which the command will be bound.</typeparam>
    /// <typeparam name="TParam">The type of the parameter passed to the command when it is executed.</typeparam>
    /// <param name="viewModel">The view model instance containing the command to bind. Can be null if the view model is not available at
    /// binding time.</param>
    /// <param name="view">The view instance containing the control to which the command will be bound.</param>
    /// <param name="viewModelProperty">An expression specifying the command property on the view model to bind.</param>
    /// <param name="controlProperty">An expression specifying the control on the view that will trigger the command.</param>
    /// <param name="withParameter">An observable sequence providing the parameter to pass to the command when it is executed. The latest value is
    /// used for each command invocation.</param>
    /// <returns>An IReactiveBinding{TView, TProp} representing the established binding between the command and the control.
    /// Disposing the binding will remove the command association.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public IReactiveBinding<TView, TProp> BindCommand<
        TView,
        TViewModel,
        TProp,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                    DynamicallyAccessedMemberTypes.NonPublicEvents |
                                    DynamicallyAccessedMemberTypes.PublicProperties)]
    TControl,
        TParam>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp?>> viewModelProperty,
        Expression<Func<TView, TControl>> controlProperty,
        IObservable<TParam?> withParameter)
        where TView : class, IViewFor
        where TViewModel : class
        where TProp : ICommand
        where TControl : class =>
        BindCommand<TView, TViewModel, TProp, TControl, TParam>(viewModel, view, viewModelProperty, controlProperty, withParameter, null);

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
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    private static MultipleDisposable BindCommandInternal<
        TView,
        TProp,
        TParam,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents |
                                    DynamicallyAccessedMemberTypes.NonPublicEvents |
                                    DynamicallyAccessedMemberTypes.PublicProperties)]
    TControl>(
        IObservable<TProp> source,
        TView view,
        Expression controlExpression,
        IObservable<TParam?> withParameter,
        string? toEvent)
        where TView : class, IViewFor
        where TProp : ICommand
        where TControl : class
    {
        SwapDisposable currentBinding = new();
        var currentControl = default(TControl);
        var isInitialBind = true;

        // Only use the in-place command rebinding shortcut for Command-property bindings. When binding to an event
        // (toEvent is set) the customizer would update the control's Command property and short-circuit the real
        // rebind, leaving the event wired to the previous view model's command. In that case do a full rebind.
        var rebindingCustomizer = string.IsNullOrEmpty(toEvent)
            ? AppLocator.Current.GetService<ICreatesCustomizedCommandRebinding>()
            : null;
        var boxedParameter = new MapSignal<TParam?, object?>(withParameter, static p => (object?)p);
        var controlValues = new MapSignal<IObservedChange<TView, object?>, object?>(
            view.SubscribeToExpressionChain<TView, object?>(
                controlExpression,
                false,
                false,
                false),
            static x => x.GetValue());
        var bindInfo = new CombineLatest2Observable<TProp, object?, (TProp command, object? host)>(source, controlValues, static (command, host) => (command, host));

        var subscription = bindInfo.Subscribe(new DelegateObserver<(TProp command, object? host)>(tuple =>
        {
            var (command, host) = tuple;
            if (host is null)
            {
                return;
            }

            var control = host as TControl;
            var isSameControl = !isInitialBind && ReferenceEquals(control, currentControl);
            if (isSameControl && control is not null && rebindingCustomizer?.TryUpdateCommand(control, command) is true)
            {
                return;
            }

            isInitialBind = false;
            currentControl = control;
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
        }));

        return new(subscription, currentBinding);
    }

    /// <summary>
    /// Combines the latest value of two sources through a selector, emitting once both have produced a value.
    /// Specialised command-binding combine-latest.
    /// </summary>
    /// <typeparam name="T1">The first source element type.</typeparam>
    /// <typeparam name="T2">The second source element type.</typeparam>
    /// <typeparam name="TResult">The combined result type.</typeparam>
    /// <param name="first">The first source.</param>
    /// <param name="second">The second source.</param>
    /// <param name="selector">Combines the latest value of each source.</param>
    private sealed class CombineLatest2Observable<T1, T2, TResult>(
        IObservable<T1> first,
        IObservable<T2> second,
        Func<T1, T2, TResult> selector) : IObservable<TResult>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TResult> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Sink(observer, selector, first, second);
        }

        /// <summary>Tracks the latest value of each source and emits the combination once both have reported.</summary>
        private sealed class Sink : IDisposable
        {
            /// <summary>The number of combined sources.</summary>
            private const int SourceCount = 2;

            /// <summary>Guards the latest values and the completion counter.</summary>
#if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
#else
            private readonly object _gate = new();
#endif

            /// <summary>The observer receiving the combined values.</summary>
            private readonly IObserver<TResult> _downstream;

            /// <summary>Combines the latest value of each source.</summary>
            private readonly Func<T1, T2, TResult> _selector;

            /// <summary>The subscription to the first source.</summary>
            private readonly IDisposable _firstSubscription;

            /// <summary>The subscription to the second source.</summary>
            private readonly IDisposable _secondSubscription;

            /// <summary>The latest value of the first source.</summary>
            private T1 _latest1 = default!;

            /// <summary>The latest value of the second source.</summary>
            private T2 _latest2 = default!;

            /// <summary>Whether the first source has reported a value.</summary>
            private bool _has1;

            /// <summary>Whether the second source has reported a value.</summary>
            private bool _has2;

            /// <summary>The number of sources that have completed.</summary>
            private int _doneCount;

            /// <summary>Whether the downstream has terminated.</summary>
            private bool _stopped;

            /// <summary>Initializes a new instance of the <see cref="Sink"/> class and subscribes to both sources.</summary>
            /// <param name="downstream">The observer receiving the combined values.</param>
            /// <param name="selector">Combines the latest value of each source.</param>
            /// <param name="first">The first source.</param>
            /// <param name="second">The second source.</param>
            public Sink(IObserver<TResult> downstream, Func<T1, T2, TResult> selector, IObservable<T1> first, IObservable<T2> second)
            {
                _downstream = downstream;
                _selector = selector;
                _firstSubscription = first.Subscribe(new FirstObserver(this));
                _secondSubscription = second.Subscribe(new SecondObserver(this));
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                _firstSubscription.Dispose();
                _secondSubscription.Dispose();
            }

            /// <summary>Records the first source's latest value and emits if both are present.</summary>
            /// <param name="value">The reported value.</param>
            private void OnNext1(T1 value)
            {
                TResult result;
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _latest1 = value;
                    _has1 = true;
                    if (!_has2)
                    {
                        return;
                    }

                    result = _selector(_latest1, _latest2);
                }

                _downstream.OnNext(result);
            }

            /// <summary>Records the second source's latest value and emits if both are present.</summary>
            /// <param name="value">The reported value.</param>
            private void OnNext2(T2 value)
            {
                TResult result;
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _latest2 = value;
                    _has2 = true;
                    if (!_has1)
                    {
                        return;
                    }

                    result = _selector(_latest1, _latest2);
                }

                _downstream.OnNext(result);
            }

            /// <summary>Forwards an error from either source.</summary>
            /// <param name="error">The error to forward.</param>
            private void OnErrorAny(Exception error)
            {
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnError(error);
            }

            /// <summary>Completes the downstream once both sources have completed.</summary>
            private void OnCompletedAny()
            {
                lock (_gate)
                {
                    if (_stopped || ++_doneCount < SourceCount)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnCompleted();
            }

            /// <summary>Routes the first source's notifications to the parent sink.</summary>
            /// <param name="parent">The owning sink.</param>
            private sealed class FirstObserver(Sink parent) : IObserver<T1>
            {
                /// <inheritdoc/>
                public void OnNext(T1 value) => parent.OnNext1(value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.OnErrorAny(error);

                /// <inheritdoc/>
                public void OnCompleted() => parent.OnCompletedAny();
            }

            /// <summary>Routes the second source's notifications to the parent sink.</summary>
            /// <param name="parent">The owning sink.</param>
            private sealed class SecondObserver(Sink parent) : IObserver<T2>
            {
                /// <inheritdoc/>
                public void OnNext(T2 value) => parent.OnNext2(value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.OnErrorAny(error);

                /// <inheritdoc/>
                public void OnCompleted() => parent.OnCompletedAny();
            }
        }
    }
}
