// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace ReactiveUI;

/// <summary>Represents an active binding between a view and a view model property, tracking direction and change notifications.</summary>
/// <typeparam name="TView">The type of the view.</typeparam>
/// <typeparam name="TValue">The type of the bound value.</typeparam>
/// <param name="view">The view participating in the binding.</param>
/// <param name="viewExpression">The expression identifying the bound view property.</param>
/// <param name="viewModelExpression">The expression identifying the bound view model property.</param>
/// <param name="changed">An observable that emits the bound value whenever it changes.</param>
/// <param name="direction">The direction of the binding.</param>
/// <param name="bindingDisposable">A disposable that tears down the binding when disposed.</param>
public class ReactiveBinding<TView, TValue>(
    TView view,
    Expression viewExpression,
    Expression viewModelExpression,
    IObservable<TValue?> changed,
    BindingDirection direction,
    IDisposable bindingDisposable) : IReactiveBinding<TView, TValue>
    where TView : IViewFor
{
    /// <inheritdoc />
    public Expression ViewModelExpression { get; } = viewModelExpression;

    /// <inheritdoc />
    public TView View { get; } = view;

    /// <inheritdoc />
    public Expression ViewExpression { get; } = viewExpression;

    /// <inheritdoc />
    public IObservable<TValue?> Changed { get; } = changed;

    /// <inheritdoc />
    public BindingDirection Direction { get; } = direction;

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Disposes of resources inside the class.</summary>
    /// <param name="isDisposing">If we are disposing managed resources.</param>
    protected virtual void Dispose(bool isDisposing)
    {
        if (!isDisposing)
        {
            return;
        }

        bindingDisposable.Dispose();
    }
}
