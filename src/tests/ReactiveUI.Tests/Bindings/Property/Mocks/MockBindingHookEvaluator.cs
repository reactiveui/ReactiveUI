// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Expression = System.Linq.Expressions.Expression;

namespace ReactiveUI.Tests.Bindings.Property.Mocks;

/// <summary>Test mock for <see cref="IBindingHookEvaluator"/>.</summary>
/// <remarks>
/// This mock provides configurable behavior for testing binding hook evaluation
/// without requiring registered Splat hooks.
/// </remarks>
internal sealed class MockBindingHookEvaluator : IBindingHookEvaluator
{
    /// <summary>The value returned from <see cref="EvaluateBindingHooks{TViewModel, TView}"/>.</summary>
    private bool _returnValue = true;

    /// <inheritdoc/>
    public bool EvaluateBindingHooks<TViewModel, TView>(
        TViewModel? viewModel,
        TView view,
        Expression viewModelExpression,
        Expression viewExpression,
        BindingDirection direction)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(viewModelExpression);
        ArgumentNullException.ThrowIfNull(viewExpression);

        return _returnValue;
    }

    /// <summary>Configures the return value for <see cref="EvaluateBindingHooks{TViewModel, TView}"/>.</summary>
    /// <param name="value">True to allow binding; false to reject binding.</param>
    internal void SetReturnValue(bool value) => _returnValue = value;
}
