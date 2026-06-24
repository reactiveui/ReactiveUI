// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.Bindings.Property.Mocks;

using Expression = System.Linq.Expressions.Expression;

/// <summary>Test mock for <see cref="IPropertyBindingExpressionCompiler"/>.</summary>
/// <remarks>
/// This mock provides configurable behavior for testing property binding expression compilation
/// without requiring actual expression tree compilation.
/// </remarks>
[SuppressMessage("Major Code Smell", "S4018:Generic methods should provide type parameters", Justification = "Type parameter cannot be inferred.")]
internal sealed class MockPropertyBindingExpressionCompiler : IPropertyBindingExpressionCompiler
{
    /// <summary>The configured set-then-get function returned by the compiler.</summary>
    private Func<object?, object?, object?[]?, (bool ShouldEmit, object? Value)>? _setThenGetFunc;

    /// <summary>The value indicating whether the expression is treated as a direct member access.</summary>
    private bool _isDirectMemberAccess;

    /// <summary>The configured expression chain returned by the compiler.</summary>
    private Expression[]? _expressionChainArray;

    /// <summary>The value indicating whether bindings replay on host changes.</summary>
    private bool _shouldReplayOnHostChanges = true;

    /// <summary>Configures the return value for <see cref="CreateSetThenGet"/>.</summary>
    /// <param name="func">The function to return.</param>
    public void SetSetThenGetFunction(Func<object?, object?, object?[]?, (bool ShouldEmit, object? Value)> func) =>
        _setThenGetFunc = func;

    /// <summary>Configures the return value for <see cref="IsDirectMemberAccess"/>.</summary>
    /// <param name="value">True if the expression should be treated as direct member access.</param>
    public void SetIsDirectMemberAccess(bool value) => _isDirectMemberAccess = value;

    /// <summary>Configures the return value for <see cref="GetExpressionChainArray"/>.</summary>
    /// <param name="chain">The expression chain to return.</param>
    public void SetExpressionChainArray(Expression[]? chain) => _expressionChainArray = chain;

    /// <summary>Configures the return value for <see cref="ShouldReplayOnHostChanges"/>.</summary>
    /// <param name="value">True if values should be replayed on host changes.</param>
    public void SetShouldReplayOnHostChanges(bool value) => _shouldReplayOnHostChanges = value;

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

        return (Func<object?, object?, object?[]?, (bool ShouldEmit, object? Value)>?)_setThenGetFunc ?? ((target, value, parameters) =>
        {
            var current = getter(target, parameters);
            if (EqualityComparer<object?>.Default.Equals(current, value))
            {
                return (false, current);
            }

            setter(target, value, parameters);
            return (true, getter(target, parameters));
        });
    }

    /// <inheritdoc/>
    public bool IsDirectMemberAccess(Expression viewExpression)
    {
        ArgumentNullException.ThrowIfNull(viewExpression);
        return _isDirectMemberAccess;
    }

    /// <inheritdoc/>
    public Expression[]? GetExpressionChainArray(Expression? expression) => _expressionChainArray;

    /// <inheritdoc/>
    public bool ShouldReplayOnHostChanges(Expression[]? hostExpressionChain) => _shouldReplayOnHostChanges;

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
