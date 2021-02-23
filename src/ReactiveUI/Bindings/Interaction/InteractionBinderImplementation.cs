// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Provides methods to bind <see cref="Interaction{TInput, TOutput}"/>s to handlers.
    /// </summary>
    public class InteractionBinderImplementation : IInteractionBinderImplementation
    {
        /// <inheritdoc />
        public IDisposable BindInteraction<TViewModel, TView, TInput, TOutput>(
                TViewModel? viewModel,
                TView view,
                Expression<Func<TViewModel, Interaction<TInput, TOutput>>> propertyName,
                Func<InteractionContext<TInput, TOutput>, Task> handler)
            where TViewModel : class
            where TView : class, IViewFor
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var vmExpression = Reflection.Rewrite(propertyName.Body);

            var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Cast<Interaction<TInput, TOutput>>();

            var interactionDisposable = new SerialDisposable();

            return source
                .WhereNotNull()
                .Do(x => interactionDisposable.Disposable = x.RegisterHandler(handler))
                .Finally(() => interactionDisposable.Dispose())
                .Subscribe(_ => { }, ex => this.Log().Error(ex, $"{vmExpression} Interaction Binding received an Exception!"));
        }

        /// <inheritdoc />
        public IDisposable BindInteraction<TViewModel, TView, TInput, TOutput, TDontCare>(
                TViewModel? viewModel,
                TView view,
                Expression<Func<TViewModel, Interaction<TInput, TOutput>>> propertyName,
                Func<InteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler)
            where TViewModel : class
            where TView : class, IViewFor
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var vmExpression = Reflection.Rewrite(propertyName.Body);

            var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Cast<Interaction<TInput, TOutput>>();

            var interactionDisposable = new SerialDisposable();

            return source
                .Where(x => x is not null)
                .Do(x => interactionDisposable.Disposable = x.RegisterHandler(handler))
                .Finally(() => interactionDisposable.Dispose())
                .Subscribe(_ => { }, ex => this.Log().Error(ex, $"{vmExpression} Interaction Binding received an Exception!"));
        }
    }
}
