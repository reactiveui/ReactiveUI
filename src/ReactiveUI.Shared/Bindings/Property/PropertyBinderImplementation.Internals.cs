// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Reflection;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Disposables;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Private binding-pipeline helpers and observable adapters for <see cref="PropertyBinderImplementation"/>.</summary>
public partial class PropertyBinderImplementation
{
    /// <summary>Attempts to convert a view model value to the view type, falling back to a direct pass-through when assignable.</summary>
    /// <typeparam name="TViewModelPropertyType">The declared view model property type.</typeparam>
    /// <param name="value">The current view model value.</param>
    /// <param name="converter">The resolved binding converter.</param>
    /// <param name="viewType">The view property type to convert to.</param>
    /// <param name="conversionHint">An optional converter hint.</param>
    /// <param name="viewModelToViewConverterOverride">The caller-supplied converter override, if any.</param>
    /// <returns>A tuple indicating whether a value was produced and the converted value.</returns>
    private static (bool Success, object? Value) ConvertViewModelValue<TViewModelPropertyType>(
        object? value,
        object converter,
        Type viewType,
        object? conversionHint,
        IBindingTypeConverter? viewModelToViewConverterOverride)
    {
        var runtimeType = value?.GetType() ?? typeof(TViewModelPropertyType);
        if (BindingTypeConverterDispatch.TryConvertAny(converter, runtimeType, value, viewType, conversionHint, out var tmp))
        {
            return (true, tmp);
        }

        return viewModelToViewConverterOverride is null && viewType.IsAssignableFrom(typeof(TViewModelPropertyType))
            ? (true, (object?)value)
            : (false, null);
    }

    /// <summary>Binds an observable to a target member directly using compiled accessors.</summary>
    /// <typeparam name="TTarget">The target object type.</typeparam>
    /// <typeparam name="TValue">The value type emitted by the returned observable.</typeparam>
    /// <typeparam name="TObs">The element type produced by <paramref name="changeObservable"/>.</typeparam>
    /// <param name="changeObservable">The observable providing values to set.</param>
    /// <param name="target">The target object.</param>
    /// <param name="viewExpression">The rewritten member expression describing the target member.</param>
    /// <returns>
    /// A tuple containing the subscription <see cref="IDisposable"/> and an observable sequence of values that were effectively set.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when a required getter cannot be resolved.</exception>
    private (IDisposable disposable, IObservable<TValue> value) BindToDirect<TTarget, TValue, TObs>(
        IObservable<TObs> changeObservable,
        TTarget target,
        Expression viewExpression)
        where TTarget : class
    {
        ArgumentExceptionHelper.ThrowIfNull(changeObservable);
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(viewExpression);

        var memberInfo = viewExpression.GetMemberInfo();

        var setter = Reflection.GetValueSetterOrThrow(memberInfo);
        var getter = Reflection.GetValueFetcherOrThrow(memberInfo) ??
                     throw new InvalidOperationException("getter was not found.");

        var setObservableWithEmit =
            _expressionCompiler.IsDirectMemberAccess(viewExpression)
                ? _expressionCompiler.CreateDirectSetObservable<TTarget, TValue, TObs>(
                    target,
                    changeObservable,
                    viewExpression,
                    getter,
                    setter,
                    _converterResolver.GetSetMethodConverter)
                : _expressionCompiler.CreateChainedSetObservable<TTarget, TValue, TObs>(
                    target,
                    changeObservable,
                    viewExpression,
                    _expressionCompiler.GetExpressionChainArray(viewExpression.GetParent()!) ?? [],
                    getter,
                    setter,
                    _converterResolver.GetSetMethodConverter);

        IObservable<TValue> setObservable = new MapSignal<(bool ShouldEmit, TValue Value), TValue>(setObservableWithEmit, static x => x.Value);
        var subscription = SubscribeWithBindingErrorHandling(setObservable, viewExpression);

        return (subscription, setObservable);
    }

    /// <summary>Subscribes to <paramref name="setObservable"/> and applies binding error handling consistent with the binding engine.</summary>
    /// <typeparam name="TValue">The element type of the observable.</typeparam>
    /// <param name="setObservable">The observable to subscribe to.</param>
    /// <param name="viewExpression">The view expression used for diagnostic messages.</param>
    /// <returns>The subscription disposable.</returns>
    /// <exception cref="TargetInvocationException">
    /// Thrown when the binding receives an exception with an inner exception, matching legacy behavior.
    /// </exception>
    private IDisposable SubscribeWithBindingErrorHandling<TValue>(
        IObservable<TValue> setObservable,
        Expression viewExpression)
    {
        ArgumentExceptionHelper.ThrowIfNull(setObservable);
        ArgumentExceptionHelper.ThrowIfNull(viewExpression);

        return setObservable.Subscribe(new DelegateObserver<TValue>(
            static _ => { },
            ex =>
            {
                this.Log().Error(ex, $"{viewExpression} Binding received an Exception!");
                if (ex.InnerException is null)
                {
                    return;
                }

                throw new TargetInvocationException(
                    $"{viewExpression} Binding received an Exception!",
                    ex.InnerException);
            }));
    }

    /// <summary>Core two-way binding implementation that wires up view-model-to-view and view-to-view-model change pipelines.</summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the view model property.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the view property.</typeparam>
    /// <typeparam name="TDontCare">A dummy type used only to signal view updates.</typeparam>
    /// <param name="request">The bundled inputs describing the two-way binding to create.</param>
    /// <returns>The configured two-way reactive binding, or null if hooks blocked the binding.</returns>
    private ReactiveBinding<TView, (object? view, bool isViewModel)> BindImpl<
        TViewModel,
        TView,
        TViewModelPropertyType,
        TViewPropertyType,
        TDontCare>(in TwoWayBindRequest<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare> request)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(request.ViewModelProperty);
        ArgumentExceptionHelper.ThrowIfNull(request.ViewProperty);

        var view = request.View;
        Signal<bool> signalInitialUpdate = new();
        var viewModelExpression = Reflection.Rewrite(request.ViewModelProperty.Body);
        var viewExpression = Reflection.Rewrite(request.ViewProperty.Body);

        Expression[] viewModelExpressionChainArray = [.. viewModelExpression.GetExpressionChain()];
        Expression[] viewExpressionChainArray = [.. viewExpression.GetExpressionChain()];

        Reflection.CompiledPropertyChain<object?, TViewModelPropertyType> viewModelChainGetter = new(viewModelExpressionChainArray);
        Reflection.CompiledPropertyChain<TView, TViewPropertyType> viewChainGetter = new(viewExpressionChainArray);
        Reflection.CompiledPropertyChainSetter<TView, object?> viewChainSetter = new(viewExpressionChainArray);
        Reflection.CompiledPropertyChainSetter<object?, object?> viewModelChainSetter = new(viewModelExpressionChainArray);

        var viewModelToViewConverter = request.ViewModelToViewConverter;
        var viewToViewModelConverter = request.ViewToViewModelConverter;

        var viewChanges = new MapSignal<TViewPropertyType?, bool>(
            view.WhenAnyDynamic(viewExpression, static x => (TViewPropertyType?)x.Value),
            static _ => false);

        var somethingChanged = BuildChangeSource(
            request.TriggerUpdate,
            request.SignalViewUpdate,
            request.ViewModel,
            view,
            viewModelExpression,
            viewChanges,
            signalInitialUpdate);

        var changeWithValues = new MapSignal<bool, (bool isValid, object? view, bool isViewModel)>(
            new ScheduledChangeObservable<TView>(somethingChanged, this, view),
            isViewModelChange =>
                ProjectChange(isViewModelChange, view, viewModelChainGetter, viewChainGetter, viewModelToViewConverter, viewToViewModelConverter));

        var ret = _hookEvaluator.EvaluateBindingHooks(
            request.ViewModel,
            view,
            viewModelExpression,
            viewExpression,
            BindingDirection.TwoWay);
        if (!ret)
        {
            return null!;
        }

        var changes = new Signal<(object? view, bool isViewModel)>();
        var bindingDisposable = WireTwoWayBinding(changeWithValues, changes, view, viewChainSetter, viewModelChainSetter);

        signalInitialUpdate.OnNext(true);

        return new(
            view,
            viewExpression,
            viewModelExpression,
            changes,
            BindingDirection.TwoWay,
            bindingDisposable);
    }

    /// <summary>Wires the two-way change pipeline: multicasts valid changes and applies view/view model writes.</summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="changeWithValues">The validated change stream.</param>
    /// <param name="changes">The multicast subject shared with external subscribers.</param>
    /// <param name="view">The view instance.</param>
    /// <param name="viewChainSetter">Writes values into the view property chain.</param>
    /// <param name="viewModelChainSetter">Writes values into the view model property chain.</param>
    /// <returns>The disposables for the wired subscriptions.</returns>
    private DisposableBag WireTwoWayBinding<TView>(
        IObservable<(bool isValid, object? view, bool isViewModel)> changeWithValues,
        Signal<(object? view, bool isViewModel)> changes,
        TView view,
        Reflection.CompiledPropertyChainSetter<TView, object?> viewChainSetter,
        Reflection.CompiledPropertyChainSetter<object?, object?> viewModelChainSetter)
        where TView : class, IViewFor
    {
        // Filter to valid changes and project to the (view, isViewModel) pair, then multicast through a shared subject
        // so the internal setter and external subscribers share one upstream subscription.
        var projected = changeWithValues
            .Choose(static value => value.isValid ? (true, (value.view, value.isViewModel)) : (false, default));
        var upstreamConnection = projected.Subscribe(changes);

        var setterSubscription = changes.Subscribe(new DelegateObserver<(object? view, bool isViewModel)>(latestValue =>
        {
            if (latestValue.isViewModel)
            {
                SetViewValue(view, () => viewChainSetter.TrySetValue(view, latestValue.view, false));
            }
            else
            {
                _ = viewModelChainSetter.TrySetValue(view.ViewModel, latestValue.view, false);
            }
        }));

        return new(upstreamConnection, setterSubscription);
    }

    /// <summary>
    /// Routes each two-way change signal through <see cref="ScheduleForBinding{TView}"/> so the subsequent view
    /// read and write run where the platform binder requires (e.g. the WPF dispatcher). A fused
    /// <c>SelectMany</c>-over-single: it forwards each scheduled value, ignores the inner completion, and completes
    /// only when the source does.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="source">The change-signal source.</param>
    /// <param name="owner">The binder providing the scheduling hook.</param>
    /// <param name="view">The view participating in the binding.</param>
    private sealed class ScheduledChangeObservable<TView>(IObservable<bool> source, PropertyBinderImplementation owner, TView view)
        : IObservable<bool>
        where TView : class
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            return source.Subscribe(new Sink(observer, owner, view));
        }

        /// <summary>Forwards each source signal through the binder's scheduling hook.</summary>
        /// <param name="downstream">The downstream observer.</param>
        /// <param name="owner">The binder providing the scheduling hook.</param>
        /// <param name="view">The view participating in the binding.</param>
        private sealed class Sink(IObserver<bool> downstream, PropertyBinderImplementation owner, TView view) : IObserver<bool>
        {
            /// <inheritdoc/>
            public void OnNext(bool value) =>
                owner.ScheduleForBinding(view, value)
                    .Subscribe(new DelegateObserver<bool>(downstream.OnNext, downstream.OnError));

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }

    /// <summary>Forwards the first value of a source then completes and unsubscribes. Specialised binding <c>Take(1)</c>.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source observable.</param>
    private sealed class Take1Observable<T>(IObservable<T> source) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            var sink = new Sink(observer);
            return sink.Run(source);
        }

        /// <summary>Forwards the first value, then completes the downstream and disposes the subscription.</summary>
        /// <param name="downstream">The observer receiving the forwarded value.</param>
        private sealed class Sink(IObserver<T> downstream) : IObserver<T>, IDisposable
        {
            /// <summary>The subscription to the source.</summary>
            private IDisposable? _subscription;

            /// <summary>Whether the first value has been delivered.</summary>
            private bool _done;

            /// <summary>Subscribes to the source.</summary>
            /// <param name="source">The source observable.</param>
            /// <returns>The sink, which disposes the run.</returns>
            public Sink Run(IObservable<T> source)
            {
                _subscription = source.Subscribe(this);
                return this;
            }

            /// <inheritdoc/>
            public void OnNext(T value)
            {
                if (_done)
                {
                    return;
                }

                _done = true;
                downstream.OnNext(value);
                downstream.OnCompleted();
                Dispose();
            }

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                if (_done)
                {
                    return;
                }

                downstream.OnError(error);
            }

            /// <inheritdoc/>
            public void OnCompleted()
            {
                if (_done)
                {
                    return;
                }

                downstream.OnCompleted();
            }

            /// <inheritdoc/>
            public void Dispose() => _subscription?.Dispose();
        }
    }
}
