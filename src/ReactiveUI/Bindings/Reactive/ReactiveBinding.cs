// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
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

        /// <inheritdoc />
        public TViewModel ViewModel { get; private set; }

        /// <inheritdoc />
        public Expression ViewModelExpression { get; private set; }

        /// <inheritdoc />
        public TView View { get; private set; }

        /// <inheritdoc />
        public Expression ViewExpression { get; private set; }

        /// <inheritdoc />
        public IObservable<TValue> Changed { get; private set; }

        /// <inheritdoc />
        public BindingDirection Direction { get; private set; }

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
