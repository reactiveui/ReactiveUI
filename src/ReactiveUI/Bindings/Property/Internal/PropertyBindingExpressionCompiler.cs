// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

namespace ReactiveUI;

/// <summary>
/// Default implementation of <see cref="IPropertyBindingExpressionCompiler"/> that compiles and analyzes property binding expressions.
/// </summary>
/// <remarks>
/// This service handles expression chain analysis, compiled accessor creation, set-then-get logic,
/// and observable creation for property bindings. It abstracts the complexity of expression tree
/// manipulation and compilation required for reactive property bindings.
/// </remarks>
[RequiresUnreferencedCode("Uses reflection over expression trees and compiled property accessors which may be trimmed.")]
[RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
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

        var synchronizedChanges = observedChanged.Synchronize();
        var setThenGet = CreateSetThenGet(viewExpression, getter, setter, getSetConverter);
        var arguments = viewExpression.GetArgumentsArray();

        return synchronizedChanges.Select(value => setThenGet(target, value, arguments))
                                  .Where(result => result.ShouldEmit)
                                  .Select(result => (result.ShouldEmit, result.Value is null ? default! : (TValue)result.Value));
    }

    /// <inheritdoc/>
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

        var synchronizedChanges = observedChanged.Synchronize();
        var setThenGet = CreateSetThenGet(viewExpression, getter, setter, getSetConverter);
        var arguments = viewExpression.GetArgumentsArray();

        var hostExpression = viewExpression.GetParent() ?? throw new InvalidOperationException("Host expression was not found.");
        var hostChanges = target.WhenAnyDynamic(hostExpression, x => x.Value).Synchronize();
        var propertyDefaultValue = CreateDefaultValueForType(viewExpression.Type);
        var shouldReplayOnHostChanges = ShouldReplayOnHostChanges(hostExpressionChain);

        return Observable.Create<(bool ShouldEmit, TValue Value)>(observer =>
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            object? latestHost = null;
            object? currentHost = null;
            object? lastObservedValue = null;
            var hasObservedValue = false;

            bool HostPropertyEqualsDefault(object? host)
            {
                if (host is null)
                {
                    return false;
                }

                var currentValue = getter(host, arguments);
                return EqualityComparer<object?>.Default.Equals(currentValue, propertyDefaultValue);
            }

            void ApplyValueToHost(object? host, object? value)
            {
                if (host is null || !hasObservedValue)
                {
                    return;
                }

                var (shouldEmit, result) = setThenGet(host, value, arguments);
                if (!shouldEmit)
                {
                    return;
                }

                observer.OnNext((shouldEmit, result is null ? default! : (TValue)result));
            }

            var hostDisposable = hostChanges.Subscribe(
                hostValue =>
                {
                    latestHost = hostValue;

                    if (ReferenceEquals(hostValue, currentHost))
                    {
                        return;
                    }

                    currentHost = hostValue;

                    if (!shouldReplayOnHostChanges || !hasObservedValue || !HostPropertyEqualsDefault(hostValue))
                    {
                        return;
                    }

                    ApplyValueToHost(hostValue, lastObservedValue);
                },
                observer.OnError);

            var changeDisposable = synchronizedChanges.Subscribe(
                value =>
                {
                    hasObservedValue = true;
                    lastObservedValue = value;

                    var host = latestHost;

                    if (hostExpressionChain is not null)
                    {
                        host = ResolveHostFromChain(target, hostExpressionChain);
                        latestHost = host;
                    }

                    if (host is null)
                    {
                        return;
                    }

                    ApplyValueToHost(host, value);
                },
                observer.OnError);

            return new CompositeDisposable(hostDisposable, changeDisposable);
        });
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

        object? host = target;

        if (!Reflection.TryGetValueForPropertyChain(out host, host, hostExpressionChain))
        {
            return null;
        }

        return host;
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
}
