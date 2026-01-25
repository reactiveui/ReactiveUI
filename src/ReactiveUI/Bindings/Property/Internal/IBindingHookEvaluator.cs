// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Internal interface for evaluating binding hooks.
/// </summary>
/// <remarks>
/// This service evaluates registered IPropertyBindingHook instances for pre-binding validation.
/// Hooks can reject bindings based on custom logic, property names, types, or other criteria.
/// </remarks>
internal interface IBindingHookEvaluator
{
    /// <summary>
    /// Evaluates all registered binding hooks.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="viewModel">The view model instance.</param>
    /// <param name="view">The view instance.</param>
    /// <param name="vmExpression">The rewritten view model expression.</param>
    /// <param name="viewExpression">The rewritten view expression.</param>
    /// <param name="direction">The binding direction.</param>
    /// <returns>True if binding should proceed; otherwise false.</returns>
    /// <remarks>
    /// This method iterates through all registered IPropertyBindingHook instances (from Splat)
    /// and calls their ExecuteHook method. If any hook returns false, the binding is rejected
    /// and a warning is logged. Hook evaluation uses early termination - the first hook that
    /// rejects stops further evaluation.
    /// </remarks>
    bool EvaluateBindingHooks<TViewModel, TView>(
        TViewModel? viewModel,
        TView view,
        Expression vmExpression,
        Expression viewExpression,
        BindingDirection direction)
        where TViewModel : class
        where TView : class, IViewFor;
}
