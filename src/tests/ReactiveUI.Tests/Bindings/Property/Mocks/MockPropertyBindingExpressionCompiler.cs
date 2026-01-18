// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.Property.Mocks;

using Expression = System.Linq.Expressions.Expression;

/// <summary>
/// Test mock for <see cref="IPropertyBindingExpressionCompiler"/>.
/// </summary>
/// <remarks>
/// This mock provides configurable behavior for testing property binding expression compilation
/// without requiring actual expression tree compilation.
/// </remarks>
internal class MockPropertyBindingExpressionCompiler : IPropertyBindingExpressionCompiler
{
    private Func<object?, object?, object?[]?, (bool ShouldEmit, object? Value)>? _setThenGetFunc;
    private bool _isDirectMemberAccess;
    private Expression[]? _expressionChainArray;
    private bool _shouldReplayOnHostChanges = true;

    /// <summary>
    /// Configures the return value for <see cref="CreateSetThenGet"/>.
    /// </summary>
    /// <param name="func">The function to return.</param>
    public void SetSetThenGetFunction(Func<object?, object?, object?[]?, (bool ShouldEmit, object? Value)> func)
    {
        _setThenGetFunc = func;
    }

    /// <summary>
    /// Configures the return value for <see cref="IsDirectMemberAccess"/>.
    /// </summary>
    /// <param name="value">True if the expression should be treated as direct member access.</param>
    public void SetIsDirectMemberAccess(bool value)
    {
        _isDirectMemberAccess = value;
    }

    /// <summary>
    /// Configures the return value for <see cref="GetExpressionChainArray"/>.
    /// </summary>
    /// <param name="chain">The expression chain to return.</param>
    public void SetExpressionChainArray(Expression[]? chain)
    {
        _expressionChainArray = chain;
    }

    /// <summary>
    /// Configures the return value for <see cref="ShouldReplayOnHostChanges"/>.
    /// </summary>
    /// <param name="value">True if values should be replayed on host changes.</param>
    public void SetShouldReplayOnHostChanges(bool value)
    {
        _shouldReplayOnHostChanges = value;
    }

    /// <inheritdoc/>
    public Func<object?, object?, object?[]?, (bool ShouldEmit, object? Value)> CreateSetThenGet(
        Expression viewExpression,
        Func<object?, object?[]?, object?> getter,
        Action<object?, object?, object?[]?> setter,
        Func<Type?, Type?, Func<object?, object?, object?[]?, object?>?> getSetConverter)
    {
        ArgumentNullException.ThrowIfNull(viewExpression);
        ArgumentNullException.ThrowIfNull(getter);
        ArgumentNullException.ThrowIfNull(setter);
        ArgumentNullException.ThrowIfNull(getSetConverter);

        if (_setThenGetFunc is not null)
        {
            return _setThenGetFunc;
        }

        // Default implementation: simple set-then-get
        return (target, value, parameters) =>
        {
            var current = getter(target, parameters);
            if (EqualityComparer<object?>.Default.Equals(current, value))
            {
                return (false, current);
            }

            setter(target, value, parameters);
            return (true, getter(target, parameters));
        };
    }

    /// <inheritdoc/>
    public bool IsDirectMemberAccess(Expression viewExpression)
    {
        ArgumentNullException.ThrowIfNull(viewExpression);
        return _isDirectMemberAccess;
    }

    /// <inheritdoc/>
    public Expression[]? GetExpressionChainArray(Expression? expression)
    {
        return _expressionChainArray;
    }

    /// <inheritdoc/>
    public bool ShouldReplayOnHostChanges(Expression[]? hostExpressionChain)
    {
        return _shouldReplayOnHostChanges;
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
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(observedChanged);
        ArgumentNullException.ThrowIfNull(viewExpression);
        ArgumentNullException.ThrowIfNull(getter);
        ArgumentNullException.ThrowIfNull(setter);
        ArgumentNullException.ThrowIfNull(getSetConverter);

        var setThenGet = CreateSetThenGet(viewExpression, getter, setter, getSetConverter);
        var arguments = viewExpression.GetArgumentsArray();

        return observedChanged.Synchronize()
                              .Select(value => setThenGet(target, value, arguments))
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
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(observedChanged);
        ArgumentNullException.ThrowIfNull(viewExpression);
        ArgumentNullException.ThrowIfNull(hostExpressionChain);
        ArgumentNullException.ThrowIfNull(getter);
        ArgumentNullException.ThrowIfNull(setter);
        ArgumentNullException.ThrowIfNull(getSetConverter);

        // Simplified mock implementation - just uses direct observable
        // Tests requiring complex host change logic should use the real implementation
        var setThenGet = CreateSetThenGet(viewExpression, getter, setter, getSetConverter);
        var arguments = viewExpression.GetArgumentsArray();

        return observedChanged.Synchronize()
                              .Select(value => setThenGet(target, value, arguments))
                              .Where(result => result.ShouldEmit)
                              .Select(result => (result.ShouldEmit, result.Value is null ? default! : (TValue)result.Value));
    }
}
