// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides methods to bind <see cref="Interaction{TInput, TOutput}"/>s to handlers.
/// </summary>
public class InteractionBinderImplementation : IInteractionBinderImplementation
{
    /// <inheritdoc />
    public IDisposable BindInteraction<TViewModel, TView, TInput, TOutput>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, IInteraction<TInput, TOutput>>> propertyName,
        Func<IInteractionContext<TInput, TOutput>, Task> handler) // TODO: Create Test
            where TViewModel : class
            where TView : class, IViewFor
    {
        propertyName.ArgumentNullExceptionThrowIfNull(nameof(propertyName));
        handler.ArgumentNullExceptionThrowIfNull(nameof(handler));

        var vmExpression = Reflection.Rewrite(propertyName.Body);

        var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Cast<IInteraction<TInput, TOutput>>();

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
        Expression<Func<TViewModel, IInteraction<TInput, TOutput>>> propertyName,
        Func<IInteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler) // TODO: Create Test
            where TViewModel : class
            where TView : class, IViewFor
    {
        propertyName.ArgumentNullExceptionThrowIfNull(nameof(propertyName));
        handler.ArgumentNullExceptionThrowIfNull(nameof(handler));

        var vmExpression = Reflection.Rewrite(propertyName.Body);

        var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Cast<IInteraction<TInput, TOutput>>();

        var interactionDisposable = new SerialDisposable();

        return source
               .Where(x => x is not null)
               .Do(x => interactionDisposable.Disposable = x.RegisterHandler(handler))
               .Finally(() => interactionDisposable.Dispose())
               .Subscribe(_ => { }, ex => this.Log().Error(ex, $"{vmExpression} Interaction Binding received an Exception!"));
    }
}
