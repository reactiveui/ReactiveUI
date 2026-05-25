// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>
/// Default implementation of <see cref="IPropertyBindingExpressionCompiler"/> that compiles and analyzes property binding expressions.
/// </summary>
/// <remarks>
/// This service handles expression chain analysis, compiled accessor creation, set-then-get logic,
/// and observable creation for property bindings. It abstracts the complexity of expression tree
/// manipulation and compilation required for reactive property bindings.
/// </remarks>
[RequiresUnreferencedCode(
    "Uses reflection over expression trees and compiled property accessors which may be trimmed.")]
[RequiresDynamicCode(
    "Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
internal class PropertyBindingExpressionCompiler : IPropertyBindingExpressionCompiler
{
    /// <inheritdoc/>
    public Func<object?, object?, object?[]?, (bool ShouldEmit, object? Value)> CreateSetThenGet(
        Expression viewExpression,
        Func<object?, object?[]?, object?> getter,
        Action<object?, object?, object?[]?> setter,
        Func<Type?, Type?, Func<object?, object?, object?[]?, object?>?> getSetConverter)
    {
        ArgumentExceptionHelper.ThrowIfNull(viewExpression);
        ArgumentExceptionHelper.ThrowIfNull(getter);
        ArgumentExceptionHelper.ThrowIfNull(setter);
        ArgumentExceptionHelper.ThrowIfNull(getSetConverter);

        return (paramTarget, paramValue, paramParams) =>
        {
            var converter = getSetConverter(paramValue?.GetType(), viewExpression.Type);

            if (converter is null)
            {
                var currentValue = getter(paramTarget, paramParams);
                if (EqualityComparer<object?>.Default.Equals(currentValue, paramValue))
                {
                    return (false, currentValue);
                }

                setter(paramTarget, paramValue, paramParams);
                return (true, getter(paramTarget, paramParams));
            }

            var existing = getter(paramTarget, paramParams);
            var converted = converter(existing, paramValue, paramParams);
            if (EqualityComparer<object?>.Default.Equals(existing, converted))
            {
                return (false, existing);
            }

            setter(paramTarget, converted, paramParams);
            return (true, getter(paramTarget, paramParams));
        };
    }

    /// <inheritdoc/>
    public bool IsDirectMemberAccess(Expression viewExpression) =>
        viewExpression.GetParent()?.NodeType == ExpressionType.Parameter;

    /// <inheritdoc/>
    public Expression[]? GetExpressionChainArray(Expression? expression) =>
        expression is null ? null : [.. expression.GetExpressionChain()];

    /// <inheritdoc/>
    public bool ShouldReplayOnHostChanges(Expression[]? hostExpressionChain)
    {
        if (hostExpressionChain is null)
        {
            return true;
        }

        for (var i = 0; i < hostExpressionChain.Length; i++)
        {
            if (hostExpressionChain[i] is MemberExpression member &&
                string.Equals(member.Member.Name, nameof(IViewFor.ViewModel), StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public IObservable<(bool ShouldEmit, TValue Value)> CreateDirectSetObservable<TTarget, TValue, TObs>(
        TTarget? target,
        IObservable<TObs> observedChanged,
        Expression viewExpression,
        Func<object?, object?[]?, object?> getter,
        Action<object?, object?, object?[]?> setter,
        Func<Type?, Type?, Func<object?, object?, object?[]?, object?>?> getSetConverter)
        where TTarget : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(observedChanged);
        ArgumentExceptionHelper.ThrowIfNull(viewExpression);
        ArgumentExceptionHelper.ThrowIfNull(getter);
        ArgumentExceptionHelper.ThrowIfNull(setter);
        ArgumentExceptionHelper.ThrowIfNull(getSetConverter);

        var synchronizedChanges = new SynchronizeObservable<TObs>(observedChanged);
        var setThenGet = CreateSetThenGet(viewExpression, getter, setter, getSetConverter);
        var arguments = viewExpression.GetArgumentsArray();

        return new ChooseObservable<TObs, (bool ShouldEmit, TValue Value)>(
            synchronizedChanges,
            value =>
            {
                var (shouldEmit, raw) = setThenGet(target, value, arguments);
                if (!shouldEmit)
                {
                    return (false, default);
                }

                var projected = raw is null ? default! : (TValue)raw;
                return (true, (shouldEmit, projected));
            });
    }

    /// <inheritdoc/>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public IObservable<(bool ShouldEmit, TValue Value)> CreateChainedSetObservable<TTarget, TValue, TObs>(
        TTarget? target,
        IObservable<TObs> observedChanged,
        Expression viewExpression,
        Expression[] hostExpressionChain,
        Func<object?, object?[]?, object?> getter,
        Action<object?, object?, object?[]?> setter,
        Func<Type?, Type?, Func<object?, object?, object?[]?, object?>?> getSetConverter)
        where TTarget : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(observedChanged);
        ArgumentExceptionHelper.ThrowIfNull(viewExpression);
        ArgumentExceptionHelper.ThrowIfNull(hostExpressionChain);
        ArgumentExceptionHelper.ThrowIfNull(getter);
        ArgumentExceptionHelper.ThrowIfNull(setter);
        ArgumentExceptionHelper.ThrowIfNull(getSetConverter);

        var synchronizedChanges = new SynchronizeObservable<TObs>(observedChanged);
        var setThenGet = CreateSetThenGet(viewExpression, getter, setter, getSetConverter);
        var arguments = viewExpression.GetArgumentsArray();

        var hostExpression = viewExpression.GetParent() ??
                             throw new InvalidOperationException("Host expression was not found.");
        var hostChanges = new SynchronizeObservable<object?>(target.WhenAnyDynamic(hostExpression, x => x.Value));
        var propertyDefaultValue = CreateDefaultValueForType(viewExpression.Type);
        var shouldReplayOnHostChanges = ShouldReplayOnHostChanges(hostExpressionChain);

        ChainedSetContext context = new(
            setThenGet,
            getter,
            arguments,
            propertyDefaultValue,
            shouldReplayOnHostChanges,
            hostExpressionChain);

        return new ChainedSetObservable<TValue, TObs>(context, hostChanges, synchronizedChanges, target);
    }

    /// <summary>
    /// Creates the default value instance for <paramref name="type"/> used by the "replay on host changes" logic.
    /// </summary>
    /// <param name="type">The member type.</param>
    /// <returns>
    /// A boxed default value for value types, or <see langword="null"/> for reference types.
    /// </returns>
    private static object? CreateDefaultValueForType(Type type)
    {
        ArgumentExceptionHelper.ThrowIfNull(type);

        return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
    }

    /// <summary>
    /// Immutable dependencies shared across a single chained set-observable subscription.
    /// </summary>
    /// <param name="SetThenGet">Sets the value and reads back the effective value, reporting whether it changed.</param>
    /// <param name="Getter">Reads the current value of the bound member.</param>
    /// <param name="Arguments">The indexer arguments for the bound member, if any.</param>
    /// <param name="PropertyDefaultValue">The default value for the bound member type.</param>
    /// <param name="ShouldReplayOnHostChanges">Whether the last value should be replayed when the host changes.</param>
    /// <param name="HostExpressionChain">The expression chain used to resolve the host.</param>
    private sealed record ChainedSetContext(
        Func<object?, object?, object?[]?, (bool ShouldEmit, object? Value)> SetThenGet,
        Func<object?, object?[]?, object?> Getter,
        object?[]? Arguments,
        object? PropertyDefaultValue,
        bool ShouldReplayOnHostChanges,
        Expression[] HostExpressionChain);

    /// <summary>
    /// Mutable per-subscription state for a chained set-observable.
    /// </summary>
    private sealed class ChainedSetState
    {
        /// <summary>Gets or sets the most recently resolved host.</summary>
        public object? LatestHost { get; set; }

        /// <summary>Gets or sets the host currently bound to.</summary>
        public object? CurrentHost { get; set; }

        /// <summary>Gets or sets the most recently observed source value.</summary>
        public object? LastObservedValue { get; set; }

        /// <summary>Gets or sets a value indicating whether a source value has been observed.</summary>
        public bool HasObservedValue { get; set; }
    }

    /// <summary>Serializes a source's notifications under a per-subscription lock. A tailored <c>Synchronize</c>.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source observable.</param>
    private sealed class SynchronizeObservable<T>(IObservable<T> source) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer, new()));
        }

        /// <summary>Delivers each notification to the downstream while holding a gate.</summary>
        /// <param name="downstream">The observer receiving serialized notifications.</param>
        /// <param name="gate">The gate held during each notification.</param>
        private sealed class Sink(IObserver<T> downstream, object gate) : IObserver<T>
        {
            /// <inheritdoc/>
            public void OnNext(T value)
            {
                lock (gate)
                {
                    downstream.OnNext(value);
                }
            }

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                lock (gate)
                {
                    downstream.OnError(error);
                }
            }

            /// <inheritdoc/>
            public void OnCompleted()
            {
                lock (gate)
                {
                    downstream.OnCompleted();
                }
            }
        }
    }

    /// <summary>Forwards only the values chosen by a chooser. Specialised set-observable filter-map.</summary>
    /// <typeparam name="TIn">The source element type.</typeparam>
    /// <typeparam name="TOut">The forwarded element type.</typeparam>
    /// <param name="source">The source observable.</param>
    /// <param name="chooser">Maps a source value to (forward, value); when forward is false the value is skipped.</param>
    private sealed class ChooseObservable<TIn, TOut>(IObservable<TIn> source, Func<TIn, (bool HasValue, TOut Value)> chooser) : IObservable<TOut>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer, chooser));
        }

        /// <summary>Applies the chooser to each value and forwards only the chosen ones.</summary>
        /// <param name="downstream">The observer receiving chosen values.</param>
        /// <param name="chooser">Maps a source value to (forward, value).</param>
        private sealed class Sink(IObserver<TOut> downstream, Func<TIn, (bool HasValue, TOut Value)> chooser) : IObserver<TIn>
        {
            /// <inheritdoc/>
            public void OnNext(TIn value)
            {
                (bool HasValue, TOut Value) result;
                try
                {
                    result = chooser(value);
                }
                catch (Exception ex)
                {
                    downstream.OnError(ex);
                    return;
                }

                if (!result.HasValue)
                {
                    return;
                }

                downstream.OnNext(result.Value);
            }

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }

    /// <summary>
    /// On subscription, wires the host and value change streams to the chained-set handlers. A tailored replacement for
    /// the prior <c>Observable.Create</c>.
    /// </summary>
    /// <typeparam name="TValue">The bound member value type.</typeparam>
    /// <typeparam name="TObs">The observed source element type.</typeparam>
    /// <param name="context">The chained-set binding context.</param>
    /// <param name="hostChanges">The host change stream.</param>
    /// <param name="synchronizedChanges">The synchronized source change stream.</param>
    /// <param name="target">The binding target.</param>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection, which may be trimmed.")]
    [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation.")]
    private sealed class ChainedSetObservable<TValue, TObs>(
        ChainedSetContext context,
        IObservable<object?> hostChanges,
        IObservable<TObs> synchronizedChanges,
        object target) : IObservable<(bool ShouldEmit, TValue Value)>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<(bool ShouldEmit, TValue Value)> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            ChainedSetState state = new();

            var hostDisposable = hostChanges.Subscribe(new DelegateObserver<object?>(
                hostValue => OnHostChanged(context, state, observer, hostValue),
                observer.OnError));

            var changeDisposable = synchronizedChanges.Subscribe(new DelegateObserver<TObs>(
                value => OnValueChanged(context, state, target, observer, value),
                observer.OnError));

            return new DisposableBag(hostDisposable, changeDisposable);
        }

        /// <summary>
        /// Determines whether the current value of the bound member on <paramref name="host"/> equals its type default.
        /// </summary>
        /// <param name="context">The binding context.</param>
        /// <param name="host">The host object to inspect.</param>
        /// <returns><see langword="true"/> when the member value equals the type default; otherwise <see langword="false"/>.</returns>
        private static bool HostPropertyEqualsDefault(ChainedSetContext context, object? host)
        {
            if (host is null)
            {
                return false;
            }

            var currentValue = context.Getter(host, context.Arguments);
            return EqualityComparer<object?>.Default.Equals(currentValue, context.PropertyDefaultValue);
        }

        /// <summary>
        /// Applies <paramref name="value"/> to <paramref name="host"/> and emits the effective value when it changed.
        /// </summary>
        /// <param name="context">The binding context.</param>
        /// <param name="state">The mutable subscription state.</param>
        /// <param name="observer">The observer to emit to.</param>
        /// <param name="host">The host object to set the value on.</param>
        /// <param name="value">The value to set.</param>
        private static void ApplyValueToHost(
            ChainedSetContext context,
            ChainedSetState state,
            IObserver<(bool ShouldEmit, TValue Value)> observer,
            object? host,
            object? value)
        {
            if (host is null || !state.HasObservedValue)
            {
                return;
            }

            var (shouldEmit, result) = context.SetThenGet(host, value, context.Arguments);
            if (!shouldEmit)
            {
                return;
            }

            observer.OnNext((shouldEmit, result is null ? default! : (TValue)result));
        }

        /// <summary>
        /// Handles a change of the binding host, replaying the last observed value when appropriate.
        /// </summary>
        /// <param name="context">The binding context.</param>
        /// <param name="state">The mutable subscription state.</param>
        /// <param name="observer">The observer to emit to.</param>
        /// <param name="hostValue">The new host value.</param>
        private static void OnHostChanged(
            ChainedSetContext context,
            ChainedSetState state,
            IObserver<(bool ShouldEmit, TValue Value)> observer,
            object? hostValue)
        {
            state.LatestHost = hostValue;

            if (ReferenceEquals(hostValue, state.CurrentHost))
            {
                return;
            }

            state.CurrentHost = hostValue;

            if (!context.ShouldReplayOnHostChanges || !state.HasObservedValue || !HostPropertyEqualsDefault(context, hostValue))
            {
                return;
            }

            ApplyValueToHost(context, state, observer, hostValue, state.LastObservedValue);
        }

        /// <summary>
        /// Handles a new observed source value, resolving the current host and applying the value to it.
        /// </summary>
        /// <param name="context">The binding context.</param>
        /// <param name="state">The mutable subscription state.</param>
        /// <param name="target">The root binding target.</param>
        /// <param name="observer">The observer to emit to.</param>
        /// <param name="value">The newly observed value.</param>
        private static void OnValueChanged(
            ChainedSetContext context,
            ChainedSetState state,
            object target,
            IObserver<(bool ShouldEmit, TValue Value)> observer,
            object? value)
        {
            state.HasObservedValue = true;
            state.LastObservedValue = value;

            var host = state.LatestHost;

            if (context.HostExpressionChain is not null)
            {
                host = ResolveHostFromChain(target, context.HostExpressionChain);
                state.LatestHost = host;
            }

            if (host is null)
            {
                return;
            }

            ApplyValueToHost(context, state, observer, host, value);
        }

        /// <summary>
        /// Resolves the current host object for the binding by evaluating the host expression chain.
        /// </summary>
        /// <param name="target">The root binding target.</param>
        /// <param name="hostExpressionChain">The expression chain used to compute the host.</param>
        /// <returns>
        /// The resolved host object, or <see langword="null"/> if the chain cannot be evaluated.
        /// </returns>
        private static object? ResolveHostFromChain(object target, Expression[] hostExpressionChain)
        {
            ArgumentExceptionHelper.ThrowIfNull(target);
            ArgumentExceptionHelper.ThrowIfNull(hostExpressionChain);

            if (!Reflection.TryGetValueForPropertyChain(out object? host, target, hostExpressionChain))
            {
                return null;
            }

            return host;
        }
    }
}
