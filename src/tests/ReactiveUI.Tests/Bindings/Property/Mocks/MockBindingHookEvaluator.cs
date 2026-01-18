// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.Property.Mocks;

using Expression = System.Linq.Expressions.Expression;

/// <summary>
/// Test mock for <see cref="IBindingHookEvaluator"/>.
/// </summary>
/// <remarks>
/// This mock provides configurable behavior for testing binding hook evaluation
/// without requiring registered Splat hooks.
/// </remarks>
internal class MockBindingHookEvaluator : IBindingHookEvaluator
{
    private bool _returnValue = true;

    /// <summary>
    /// Configures the return value for <see cref="EvaluateBindingHooks{TViewModel, TView}"/>.
    /// </summary>
    /// <param name="value">True to allow binding; false to reject binding.</param>
    public void SetReturnValue(bool value)
    {
        _returnValue = value;
    }

    /// <inheritdoc/>
    public bool EvaluateBindingHooks<TViewModel, TView>(
        TViewModel? viewModel,
        TView view,
        Expression vmExpression,
        Expression viewExpression,
        BindingDirection direction)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(vmExpression);
        ArgumentNullException.ThrowIfNull(viewExpression);

        return _returnValue;
    }
}
