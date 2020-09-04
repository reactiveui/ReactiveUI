// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;

namespace ReactiveUI
{
    internal class ReactiveBinding<TView, TViewModel, TValue> : IReactiveBinding<TView, TViewModel, TValue>
        where TViewModel : class
        where TView : IViewFor
    {
        private readonly IDisposable _bindingDisposable;

        [Obsolete("This constructor will be removed in the future.")]
        public ReactiveBinding(
            TView view,
            TViewModel? viewModel,
            Expression viewExpression,
            Expression viewModelExpression,
            IObservable<TValue> changed,
            BindingDirection direction,
            IDisposable bindingDisposable)
        {
            View = view;
            ViewExpression = viewExpression;
            ViewModelExpression = viewModelExpression;
            Direction = direction;
            Changed = changed;

            _bindingDisposable = bindingDisposable;
        }

        public ReactiveBinding(
            TView view,
            Expression viewExpression,
            Expression viewModelExpression,
            IObservable<TValue> changed,
            BindingDirection direction,
            IDisposable bindingDisposable)
        {
            View = view;
            ViewExpression = viewExpression;
            ViewModelExpression = viewModelExpression;
            Direction = direction;
            Changed = changed;

            _bindingDisposable = bindingDisposable;
        }

        /// <inheritdoc />
        [Obsolete("This property has been deprecated. Refer to ViewModelExpression for a representation of the bound view model.")]
        public TViewModel? ViewModel { get; }

        /// <inheritdoc />
        public Expression ViewModelExpression { get; }

        /// <inheritdoc />
        public TView View { get; }

        /// <inheritdoc />
        public Expression ViewExpression { get; }

        /// <inheritdoc />
        public IObservable<TValue> Changed { get; }

        /// <inheritdoc />
        public BindingDirection Direction { get; }

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
                _bindingDisposable?.Dispose();
            }
        }
    }
}
