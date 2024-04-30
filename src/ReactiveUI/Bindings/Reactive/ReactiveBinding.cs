// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

internal class ReactiveBinding<TView, TValue>(
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

    /// <summary>
    /// Disposes of resources inside the class.
    /// </summary>
    /// <param name="isDisposing">If we are disposing managed resources.</param>
    protected virtual void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            bindingDisposable.Dispose();
        }
    }
}
