// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Represents an interaction between collaborating application components.
/// </summary>
/// <remarks>
/// <para>
/// Interactions allow collaborating components in an application to ask each other questions. Typically,
/// interactions allow a view model to get the user's confirmation from the view before proceeding with
/// some operation. The view provides the interaction's confirmation interface in a handler registered
/// for the interaction.
/// </para>
/// <para>
/// Interactions have both an input and an output. Interaction inputs and outputs use generic type parameters.
/// The interaction's input provides handlers the information they require to ask a question. The handler
/// then provides the interaction with an output as the answer to the question.
/// </para>
/// </remarks>
/// <typeparam name="TInput">
/// The interaction's input type.
/// </typeparam>
/// <typeparam name="TOutput">
/// The interaction's output type.
/// </typeparam>
public interface IInteraction<TInput, TOutput>
{
    /// <summary>
    /// Registers a synchronous interaction handler.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This overload of <c>RegisterHandler</c> is only useful if the handler can handle the interaction
    /// immediately. That is, it does not need to wait for the user or some other collaborating component.
    /// </para>
    /// </remarks>
    /// <param name="handler">
    /// The handler.
    /// </param>
    /// <returns>
    /// A disposable which, when disposed, will unregister the handler.
    /// </returns>
    IDisposable RegisterHandler(Action<IInteractionContext<TInput, TOutput>> handler);

    /// <summary>
    /// Registers a task-based asynchronous interaction handler.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This overload of <c>RegisterHandler</c> is useful if the handler needs to perform some asynchronous
    /// operation, such as displaying a dialog and waiting for the user's response.
    /// </para>
    /// </remarks>
    /// <param name="handler">
    /// The handler.
    /// </param>
    /// <returns>
    /// A disposable which, when disposed, will unregister the handler.
    /// </returns>
    IDisposable RegisterHandler(Func<IInteractionContext<TInput, TOutput>, Task> handler);

    /// <summary>
    /// Registers an observable-based asynchronous interaction handler.
    /// </summary>
    /// <typeparam name="TDontCare">The signal type.</typeparam>
    /// <remarks>
    /// <para>
    /// This overload of <c>RegisterHandler</c> is useful if the handler needs to perform some asynchronous
    /// operation, such as displaying a dialog and waiting for the user's response.
    /// </para>
    /// </remarks>
    /// <param name="handler">
    /// The handler.
    /// </param>
    /// <returns>
    /// A disposable which, when disposed, will unregister the handler.
    /// </returns>
    IDisposable RegisterHandler<TDontCare>(Func<IInteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler);

    /// <summary>
    /// Handles an interaction and asynchronously returns the result.
    /// </summary>
    /// <param name="input">
    /// The input for the interaction.
    /// </param>
    /// <remarks>
    /// <para>
    /// This method passes the interaction in turn to its registered handlers in reverse order of registration
    /// until one of them handles the interaction. If the interaction remains unhandled after all
    /// its registered handlers have executed, an <see cref="UnhandledInteractionException{TInput, TOutput}"/> is thrown.
    /// </para>
    /// </remarks>
    /// <returns>
    /// An observable that ticks when the interaction completes.
    /// </returns>
    IObservable<TOutput> Handle(TInput input);
}
