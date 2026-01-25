// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Implementation logic for <see cref="Interaction{TInput, TOutput}"/> binding.
/// </summary>
public interface IInteractionBinderImplementation : IEnableLogger
{
    /// <summary>
    /// Binds an interaction on the specified view model to a handler on the view, enabling the view to respond to
    /// interaction requests from the view model.
    /// </summary>
    /// <remarks>This method enables the view to observe and handle interaction requests initiated by the view
    /// model, typically for user-driven workflows such as dialogs or prompts. The returned <see cref="IDisposable"/>
    /// should be disposed when the binding is no longer needed to prevent memory leaks. This method uses reflection and
    /// may not be compatible with trimming tools; see the <see cref="RequiresUnreferencedCodeAttribute"/> for
    /// details.</remarks>
    /// <typeparam name="TViewModel">The type of the view model containing the interaction property.</typeparam>
    /// <typeparam name="TView">The type of the view that will handle the interaction. Must implement <see cref="IViewFor"/>.</typeparam>
    /// <typeparam name="TInput">The type of the input parameter for the interaction.</typeparam>
    /// <typeparam name="TOutput">The type of the output parameter for the interaction.</typeparam>
    /// <param name="viewModel">The view model instance containing the interaction property. Can be <see langword="null"/> if the view is not
    /// currently bound to a view model.</param>
    /// <param name="view">The view that will handle the interaction. Must not be <see langword="null"/>.</param>
    /// <param name="propertyName">An expression identifying the interaction property on the view model to bind. Must not be <see
    /// langword="null"/>.</param>
    /// <param name="handler">A delegate that handles the interaction when it is triggered. Receives the interaction context and returns a
    /// <see cref="Task"/> representing the asynchronous operation. Must not be <see langword="null"/>.</param>
    /// <returns>An <see cref="IDisposable"/> that can be disposed to unbind the interaction and release associated resources.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    IDisposable BindInteraction<TViewModel, TView, TInput, TOutput>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, IInteraction<TInput, TOutput>>> propertyName,
        Func<IInteractionContext<TInput, TOutput>, Task> handler)
        where TViewModel : class
        where TView : class, IViewFor;

    /// <summary>
    /// Binds an interaction from a view model to a handler in the view, enabling the view to respond to interaction
    /// requests from the view model.
    /// </summary>
    /// <remarks>This method uses reflection to observe the specified interaction property, which may be
    /// affected by code trimming. The handler is invoked each time the interaction is triggered by the view model.
    /// Disposing the returned IDisposable will detach the handler and stop observing the interaction.</remarks>
    /// <typeparam name="TViewModel">The type of the view model containing the interaction property.</typeparam>
    /// <typeparam name="TView">The type of the view implementing the IViewFor interface.</typeparam>
    /// <typeparam name="TInput">The type of the input parameter for the interaction.</typeparam>
    /// <typeparam name="TOutput">The type of the output parameter for the interaction.</typeparam>
    /// <typeparam name="TDontCare">The type of the value produced by the handler observable. This value is ignored.</typeparam>
    /// <param name="viewModel">The view model instance containing the interaction property. Can be null if the view is not currently bound to a
    /// view model.</param>
    /// <param name="view">The view that will handle the interaction. Must not be null.</param>
    /// <param name="propertyName">An expression identifying the interaction property on the view model to bind.</param>
    /// <param name="handler">A function that handles the interaction by processing the interaction context and returning an observable
    /// sequence. The result of the observable is ignored.</param>
    /// <returns>An IDisposable that can be disposed to unbind the interaction and release associated resources.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    IDisposable BindInteraction<TViewModel, TView, TInput, TOutput, TDontCare>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, IInteraction<TInput, TOutput>>> propertyName,
        Func<IInteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler)
        where TViewModel : class
        where TView : class, IViewFor;
}
