// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public interface IReactiveBinding<out TView, out TViewModel, out TValue> : IDisposable
        where TViewModel : class
        where TView : IViewFor
    {
        /// <summary>
        /// The instance of the view model this binding is applied to.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        TViewModel ViewModel { get; }

        /// <summary>
        /// An expression representing the propertyon the viewmodel bound to the view.
        /// This can be a child property, for example x.Foo.Bar.Baz in which case
        /// that will be the expression.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        Expression ViewModelExpression { get; }

        /// <summary>
        /// The instance of the view this binding is applied to.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        TView View { get; }

        /// <summary>
        /// An expression representing the property on the view bound to the viewmodel.
        /// This can be a child property, for example x.Foo.Bar.Baz in which case
        /// that will be the expression.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        Expression ViewExpression { get; }

        /// <summary>
        /// An observable representing changed values for the binding.
        /// </summary>
        /// <value>
        /// The changed.
        /// </value>
        IObservable<TValue> Changed { get; }

        /// <summary>
        /// Gets the direction of the binding.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        BindingDirection Direction { get; }
    }
}
