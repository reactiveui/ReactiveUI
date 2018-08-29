// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;

namespace ReactiveUI
{
    internal class ReactiveBinding<TView, TViewModel, TValue> : IReactiveBinding<TView, TViewModel, TValue>
        where TViewModel : class
        where TView : IViewFor
    {
        private IDisposable _bindingDisposable;

        public ReactiveBinding(
            TView view,
            TViewModel viewModel,
            Expression viewExpression,
            Expression viewModelExpression,
            IObservable<TValue> changed,
            BindingDirection direction,
            IDisposable bindingDisposable)
        {
            View = view;
            ViewModel = viewModel;
            ViewExpression = viewExpression;
            ViewModelExpression = viewModelExpression;
            Direction = direction;
            Changed = changed;

            _bindingDisposable = bindingDisposable;
        }

        /// <summary>
        /// The instance of the view model this binding is applied to.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public TViewModel ViewModel { get; private set; }

        /// <summary>
        /// An expression representing the propertyon the viewmodel bound to the view.
        /// This can be a child property, for example x.Foo.Bar.Baz in which case
        /// that will be the expression.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        public Expression ViewModelExpression { get; private set; }

        /// <summary>
        /// The instance of the view this binding is applied to.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public TView View { get; private set; }

        /// <summary>
        /// An expression representing the property on the view bound to the viewmodel.
        /// This can be a child property, for example x.Foo.Bar.Baz in which case
        /// that will be the expression.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        public Expression ViewExpression { get; private set; }

        /// <summary>
        /// An observable representing changed values for the binding.
        /// </summary>
        /// <value>
        /// The changed.
        /// </value>
        public IObservable<TValue> Changed { get; private set; }

        /// <summary>
        /// Gets the direction of the binding.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        public BindingDirection Direction { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _bindingDisposable?.Dispose();
            }
        }
    }
}
