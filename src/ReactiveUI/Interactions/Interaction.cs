// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

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
/// <para>
/// By default, handlers are invoked in reverse order of registration. That is, handlers registered later
/// are given the opportunity to handle interactions before handlers that were registered earlier. This
/// chaining mechanism enables handlers to be registered temporarily in a specific context, such that
/// interactions can be handled differently according to the situation. This behavior can be modified
/// by overriding the <see cref="Handle"/> method in a subclass.
/// </para>
/// <para>
/// Note that handlers are not required to handle an interaction. They can choose to ignore it, leaving it
/// for some other handler to handle. The interaction's <see cref="Handle"/> method will throw an
/// <see cref="UnhandledInteractionException{TInput, TOutput}"/> if no handler handles the interaction.
/// </para>
/// </remarks>
/// <typeparam name="TInput">
/// The interaction's input type.
/// </typeparam>
/// <typeparam name="TOutput">
/// The interaction's output type.
/// </typeparam>
public class Interaction<TInput, TOutput>
{
    private readonly IList<Func<InteractionContext<TInput, TOutput>, IObservable<Unit>>> _handlers;
    private readonly object _sync;
    private readonly IScheduler _handlerScheduler;

    /// <summary>
    /// Initializes a new instance of the <see cref="Interaction{TInput, TOutput}"/> class.
    /// </summary>
    /// <param name="handlerScheduler">
    /// The scheduler to use when invoking handlers, which defaults to <c>CurrentThreadScheduler.Instance</c> if <see langword="null"/>.
    /// </param>
    public Interaction(IScheduler? handlerScheduler = null)
    {
        _handlers = new List<Func<InteractionContext<TInput, TOutput>, IObservable<Unit>>>();
        _sync = new object();
        _handlerScheduler = handlerScheduler ?? CurrentThreadScheduler.Instance;
    }

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
    public IDisposable RegisterHandler(Action<InteractionContext<TInput, TOutput>> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        return RegisterHandler(interaction =>
        {
            handler(interaction);
            return Observables.Unit;
        });
    }

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
    public IDisposable RegisterHandler(Func<InteractionContext<TInput, TOutput>, Task> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        return RegisterHandler(interaction => handler(interaction).ToObservable());
    }

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
    public IDisposable RegisterHandler<TDontCare>(Func<InteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        IObservable<Unit> ContentHandler(InteractionContext<TInput, TOutput> context) => handler(context).Select(_ => Unit.Default);

        AddHandler(ContentHandler);
        return Disposable.Create(() => RemoveHandler(ContentHandler));
    }

    /// <summary>
    /// Handles an interaction and asynchronously returns the result.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method passes the interaction in turn to its registered handlers in reverse order of registration
    /// until one of them handles the interaction. If the interaction remains unhandled after all
    /// its registered handlers have executed, an <see cref="UnhandledInteractionException{TInput, TOutput}"/> is thrown.
    /// </para>
    /// </remarks>
    /// <param name="input">
    /// The input for the interaction.
    /// </param>
    /// <returns>
    /// An observable that ticks when the interaction completes.
    /// </returns>
    /// <exception cref="UnhandledInteractionException{TInput, TOutput}">Thrown when no handler handles the interaction.</exception>
    public virtual IObservable<TOutput> Handle(TInput input)
    {
        var context = new InteractionContext<TInput, TOutput>(input);

        return GetHandlers()
               .Reverse()
               .ToObservable()
               .ObserveOn(_handlerScheduler)
               .Select(handler => Observable.Defer(() => handler(context)))
               .Concat()
               .TakeWhile(_ => !context.IsHandled)
               .IgnoreElements()
               .Select(_ => default(TOutput)!)
               .Concat(
                       Observable.Defer(
                                        () => context.IsHandled
                                                  ? Observable.Return(context.GetOutput())
                                                  : Observable.Throw<TOutput>(new UnhandledInteractionException<TInput, TOutput>(this, input))));
    }

    /// <summary>
    /// Gets all registered handlers by order of registration.
    /// </summary>
    /// <returns>
    /// All registered handlers.
    /// </returns>
    protected Func<InteractionContext<TInput, TOutput>, IObservable<Unit>>[] GetHandlers()
    {
        lock (_sync)
        {
            return _handlers.ToArray();
        }
    }

    private void AddHandler(Func<InteractionContext<TInput, TOutput>, IObservable<Unit>> handler)
    {
        lock (_sync)
        {
            _handlers.Add(handler);
        }
    }

    private void RemoveHandler(Func<InteractionContext<TInput, TOutput>, IObservable<Unit>> handler)
    {
        lock (_sync)
        {
            _handlers.Remove(handler);
        }
    }
}