// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Internal interface for compiling and analyzing property binding expressions.
/// </summary>
/// <remarks>
/// This service handles expression chain analysis, compiled accessor creation, set-then-get logic,
/// and observable creation for property bindings. It abstracts the complexity of expression tree
/// manipulation and compilation required for reactive property bindings.
/// </remarks>
internal interface IPropertyBindingExpressionCompiler
{
    /// <summary>
    /// Creates the set-then-get function for property binding.
    /// </summary>
    /// <param name="viewExpression">The view expression.</param>
    /// <param name="getter">The compiled getter.</param>
    /// <param name="setter">The compiled setter.</param>
    /// <param name="getSetConverter">Optional set-method converter resolver.</param>
    /// <returns>
    /// A delegate that sets a value and returns whether it should be emitted.
    /// The tuple contains a boolean indicating if the value should be emitted and the actual value.
    /// </returns>
    /// <remarks>
    /// The set-then-get pattern is used to detect if a property setter modifies the value
    /// being set (e.g., coercion, validation). The returned delegate will set the value,
    /// then get it back to detect changes.
    /// </remarks>
    Func<object?, object?, object?[]?, (bool ShouldEmit, object? Value)> CreateSetThenGet(
        Expression viewExpression,
        Func<object?, object?[]?, object?> getter,
        Action<object?, object?, object?[]?> setter,
        Func<Type?, Type?, Func<object?, object?, object?[]?, object?>?> getSetConverter);

    /// <summary>
    /// Determines if a view expression is a direct member on the root parameter.
    /// </summary>
    /// <param name="viewExpression">The view expression to analyze.</param>
    /// <returns>True if the expression is a direct member access; otherwise false.</returns>
    /// <remarks>
    /// Direct member access means the expression is of the form "x => x.Property" with no
    /// intermediate navigation (e.g., "x => x.Foo.Bar" is not direct).
    /// </remarks>
    bool IsDirectMemberAccess(Expression viewExpression);

    /// <summary>
    /// Gets the expression chain array for host change tracking.
    /// </summary>
    /// <param name="expression">The expression to analyze.</param>
    /// <returns>An array of expressions representing the chain, or null if not applicable.</returns>
    /// <remarks>
    /// Expression chains are used to detect when intermediate objects in a property path change.
    /// For example, in "x => x.Foo.Bar", the chain would include expressions for both Foo and Bar.
    /// </remarks>
    Expression[]? GetExpressionChainArray(Expression? expression);

    /// <summary>
    /// Determines if values should be replayed when the host changes.
    /// </summary>
    /// <param name="hostExpressionChain">The host expression chain.</param>
    /// <returns>True if values should be replayed; otherwise false.</returns>
    /// <remarks>
    /// Replay is typically needed when binding to IViewFor.ViewModel properties, where the
    /// entire view model instance can change and bindings need to be re-evaluated.
    /// </remarks>
    bool ShouldReplayOnHostChanges(Expression[]? hostExpressionChain);

    /// <summary>
    /// Creates an observable for direct member binding (no intermediate chain).
    /// </summary>
    /// <typeparam name="TTarget">The target object type.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <typeparam name="TObs">The observed change type.</typeparam>
    /// <param name="target">The target object instance.</param>
    /// <param name="observedChanged">The observable tracking property changes.</param>
    /// <param name="viewExpression">The view expression.</param>
    /// <param name="getter">The compiled getter.</param>
    /// <param name="setter">The compiled setter.</param>
    /// <param name="getSetConverter">Optional set-method converter resolver.</param>
    /// <returns>An observable that emits tuples of (shouldEmit, value) when the property changes.</returns>
    /// <remarks>
    /// Direct set observables are optimized for simple property bindings without intermediate
    /// object navigation. They don't need to track host changes.
    /// </remarks>
    IObservable<(bool ShouldEmit, TValue Value)> CreateDirectSetObservable<TTarget, TValue, TObs>(
        TTarget? target,
        IObservable<TObs> observedChanged,
        Expression viewExpression,
        Func<object?, object?[]?, object?> getter,
        Action<object?, object?, object?[]?> setter,
        Func<Type?, Type?, Func<object?, object?, object?[]?, object?>?> getSetConverter)
        where TTarget : class;

    /// <summary>
    /// Creates an observable for chained member binding (with intermediate host changes).
    /// </summary>
    /// <typeparam name="TTarget">The target object type.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <typeparam name="TObs">The observed change type.</typeparam>
    /// <param name="target">The target object instance.</param>
    /// <param name="observedChanged">The observable tracking property changes.</param>
    /// <param name="viewExpression">The view expression.</param>
    /// <param name="hostExpressionChain">The host expression chain.</param>
    /// <param name="getter">The compiled getter.</param>
    /// <param name="setter">The compiled setter.</param>
    /// <param name="getSetConverter">Optional set-method converter resolver.</param>
    /// <returns>An observable that emits tuples of (shouldEmit, value) when the property changes.</returns>
    /// <remarks>
    /// Chained set observables handle complex property paths where intermediate objects can change.
    /// They track host changes and replay values when the host (e.g., ViewModel) changes.
    /// </remarks>
    IObservable<(bool ShouldEmit, TValue Value)> CreateChainedSetObservable<TTarget, TValue, TObs>(
        TTarget? target,
        IObservable<TObs> observedChanged,
        Expression viewExpression,
        Expression[] hostExpressionChain,
        Func<object?, object?[]?, object?> getter,
        Action<object?, object?, object?[]?> setter,
        Func<Type?, Type?, Func<object?, object?, object?[]?, object?>?> getSetConverter)
        where TTarget : class;
}
