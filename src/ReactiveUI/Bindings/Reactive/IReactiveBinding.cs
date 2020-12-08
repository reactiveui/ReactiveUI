// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace ReactiveUI
{
    /// <summary>
    /// This interface represents the result of a Bind/OneWayBind and gives
    /// information about the binding. When this object is disposed, it will
    /// destroy the binding it is describing (i.e. most of the time you won't
    /// actually care about this object, just that it is disposable).
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public interface IReactiveBinding<out TView, out TValue> : IDisposable
        where TView : IViewFor
    {
        /// <summary>
        /// Gets an expression representing the property on the viewmodel bound to the view.
        /// This can be a child property, for example x.Foo.Bar.Baz in which case
        /// that will be the expression.
        /// </summary>
        Expression ViewModelExpression { get; }

        /// <summary>
        /// Gets the instance of the view this binding is applied to.
        /// </summary>
        TView View { get; }

        /// <summary>
        /// Gets an expression representing the property on the view bound to the viewmodel.
        /// This can be a child property, for example x.Foo.Bar.Baz in which case
        /// that will be the expression.
        /// </summary>
        Expression ViewExpression { get; }

        /// <summary>
        /// Gets an observable representing changed values for the binding.
        /// </summary>
        IObservable<TValue?> Changed { get; }

        /// <summary>
        /// Gets the direction of the binding.
        /// </summary>
        BindingDirection Direction { get; }
    }
}
