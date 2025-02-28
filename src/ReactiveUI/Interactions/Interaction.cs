// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
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
/// <remarks>
/// Initializes a new instance of the <see cref="Interaction{TInput, TOutput}"/> class.
/// </remarks>
/// <param name="handlerScheduler">
/// The scheduler to use when invoking handlers, which defaults to <c>CurrentThreadScheduler.Instance</c> if <see langword="null"/>.
/// </param>
public class Interaction<TInput, TOutput>(IScheduler? handlerScheduler = null) : IInteraction<TInput, TOutput>
{
    private readonly List<Func<IInteractionContext<TInput, TOutput>, IObservable<Unit>>> _handlers = [];
    private readonly object _sync = new();
    private readonly IScheduler _handlerScheduler = handlerScheduler ?? CurrentThreadScheduler.Instance;

    /// <inheritdoc/>
    public IDisposable RegisterHandler(Action<IInteractionContext<TInput, TOutput>> handler)
    {
        handler.ArgumentNullExceptionThrowIfNull(nameof(handler));

        return RegisterHandler(interaction =>
        {
            handler(interaction);
            return Observables.Unit;
        });
    }

    /// <inheritdoc />
    public IDisposable RegisterHandler(Func<IInteractionContext<TInput, TOutput>, Task> handler)
    {
        handler.ArgumentNullExceptionThrowIfNull(nameof(handler));

        return RegisterHandler(interaction => handler(interaction).ToObservable());
    }

    /// <inheritdoc />
    public IDisposable RegisterHandler<TDontCare>(Func<IInteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler)
    {
        handler.ArgumentNullExceptionThrowIfNull(nameof(handler));

        IObservable<Unit> ContentHandler(IInteractionContext<TInput, TOutput> context) => handler(context).Select(_ => Unit.Default);

        AddHandler(ContentHandler);
        return Disposable.Create(() => RemoveHandler(ContentHandler));
    }

    /// <inheritdoc />
    public virtual IObservable<TOutput> Handle(TInput input)
    {
        var context = GenerateContext(input);

        return Enumerable.Reverse(GetHandlers())
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
    protected Func<IInteractionContext<TInput, TOutput>, IObservable<Unit>>[] GetHandlers()
    {
        lock (_sync)
        {
            return [.. _handlers];
        }
    }

    /// <summary>
    /// Gets a interaction context which is used to provide information about the interaction.
    /// </summary>
    /// <param name="input">The input that is being passed in.</param>
    /// <returns>The interaction context.</returns>
    protected virtual IOutputContext<TInput, TOutput> GenerateContext(TInput input) => new InteractionContext<TInput, TOutput>(input);

    private void AddHandler(Func<IInteractionContext<TInput, TOutput>, IObservable<Unit>> handler)
    {
        lock (_sync)
        {
            _handlers.Add(handler);
        }
    }

    private void RemoveHandler(Func<IInteractionContext<TInput, TOutput>, IObservable<Unit>> handler)
    {
        lock (_sync)
        {
            _handlers.Remove(handler);
        }
    }
}
