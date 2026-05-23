// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Concurrency;

using ReactiveUI.Internal;

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
/// Handlers receive an <see cref="IInteractionContext{TInput, TOutput}"/>, which exposes the request via
/// <see cref="IInteractionContext{TInput, TOutput}.Input"/> and lets the handler respond by calling
/// <see cref="IInteractionContext{TInput, TOutput}.SetOutput(TOutput)"/>.
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
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// public class DeleteCustomerViewModel : ReactiveObject
/// {
///     public Interaction<string, bool> ConfirmDelete { get; } = new();
///
///     public async Task<bool> TryDeleteAsync(string customerName)
///     {
///         var approved = await ConfirmDelete.Handle($"Delete {customerName}?");
///         return approved;
///     }
/// }
///
/// public partial class DeleteCustomerView : ReactiveUserControl<DeleteCustomerViewModel>
/// {
///     public DeleteCustomerView()
///     {
///         this.WhenActivated(disposables =>
///             ViewModel!.ConfirmDelete.RegisterHandler(async context =>
///             {
///                 var approved = await dialogService.ShowAsync(context.Input);
///                 context.SetOutput(approved);
///             }).DisposeWith(disposables));
///     }
/// }
/// ]]>
/// </code>
/// </example>
/// <typeparam name="TInput">
/// The interaction's input type.
/// </typeparam>
/// <typeparam name="TOutput">
/// The interaction's output type.
/// </typeparam>
/// <param name="handlerScheduler">
/// The scheduler to use when invoking handlers, which defaults to <c>CurrentThreadScheduler.Instance</c> if <see langword="null"/>.
/// </param>
public class Interaction<TInput, TOutput>(IScheduler? handlerScheduler = null) : IInteraction<TInput, TOutput>
{
    /// <summary>The ordered list of registered interaction handlers.</summary>
    private readonly List<Func<IInteractionContext<TInput, TOutput>, IObservable<Unit>>> _handlers = [];

    /// <summary>Lock object used to synchronize access to the handler list.</summary>
    #if NET9_0_OR_GREATER
    private readonly Lock _sync = new();
    #else
    private readonly object _sync = new();
    #endif

    /// <summary>The scheduler on which handlers are invoked.</summary>
    private readonly IScheduler _handlerScheduler = handlerScheduler ?? CurrentThreadScheduler.Instance;

    /// <inheritdoc/>
    public IDisposable RegisterHandler(Action<IInteractionContext<TInput, TOutput>> handler)
    {
        ArgumentExceptionHelper.ThrowIfNull(handler);

        return RegisterHandlerCore(ContentHandler);

        IObservable<Unit> ContentHandler(IInteractionContext<TInput, TOutput> interaction)
        {
            handler(interaction);
            return SingleValueObservable.Unit;
        }
    }

    /// <inheritdoc />
    public IDisposable RegisterHandler(Func<IInteractionContext<TInput, TOutput>, Task> handler)
    {
        ArgumentExceptionHelper.ThrowIfNull(handler);

        return RegisterHandlerCore(ContentHandler);

        // Yield before invoking the async handler so it is not run inside the current scheduler
        // trampoline (see #4351).
        IObservable<Unit> ContentHandler(IInteractionContext<TInput, TOutput> interaction) =>
            new TaskUnitObservable(InvokeAsync(interaction));

        async Task InvokeAsync(IInteractionContext<TInput, TOutput> interaction)
        {
            await YieldToCurrentContext().ConfigureAwait(false);
            await handler(interaction).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public IDisposable RegisterHandler<TDontCare>(
        Func<IInteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler)
    {
        ArgumentExceptionHelper.ThrowIfNull(handler);

        return RegisterHandlerCore(ContentHandler);

        IObservable<Unit> ContentHandler(IInteractionContext<TInput, TOutput> context) =>
            new ToUnitObservable<TDontCare>(handler(context));
    }

    /// <inheritdoc />
    public virtual IObservable<TOutput> Handle(TInput input)
    {
        var context = GenerateContext(input);
        return new InteractionHandleObservable<TInput, TOutput>(GetHandlers(), context, _handlerScheduler, this, input);
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
    /// Gets an interaction context which is used to provide information about the interaction.
    /// </summary>
    /// <param name="input">The input that is being passed in.</param>
    /// <returns>The interaction context.</returns>
    protected virtual IOutputContext<TInput, TOutput> GenerateContext(TInput input) =>
        new InteractionContext<TInput, TOutput>(input);

    /// <summary>Yields once so asynchronous handlers are not invoked inside the current scheduler trampoline.</summary>
    /// <returns>A task that completes after the current context has yielded.</returns>
    private static async Task YieldToCurrentContext() => await Task.Yield();

    /// <summary>Registers a normalized interaction handler that produces a <see cref="Unit"/> stream.</summary>
    /// <param name="contentHandler">The normalized handler.</param>
    /// <returns>A disposable which unregisters the handler.</returns>
    private ActionDisposable RegisterHandlerCore(Func<IInteractionContext<TInput, TOutput>, IObservable<Unit>> contentHandler)
    {
        ArgumentExceptionHelper.ThrowIfNull(contentHandler);
        AddHandler(contentHandler);
        return new ActionDisposable(() => RemoveHandler(contentHandler));
    }

    /// <summary>
    /// Adds a handler delegate to be invoked for interaction contexts.
    /// </summary>
    /// <param name="handler">A delegate that processes an interaction context and returns an observable sequence representing the handler's
    /// completion. Cannot be null.</param>
    private void AddHandler(Func<IInteractionContext<TInput, TOutput>, IObservable<Unit>> handler)
    {
        lock (_sync)
        {
            _handlers.Add(handler);
        }
    }

    /// <summary>
    /// Removes the specified interaction handler from the collection of registered handlers.
    /// </summary>
    /// <param name="handler">The handler delegate to remove. Represents a function that processes an interaction context and returns an
    /// observable sequence.</param>
    private void RemoveHandler(Func<IInteractionContext<TInput, TOutput>, IObservable<Unit>> handler)
    {
        lock (_sync)
        {
            _handlers.Remove(handler);
        }
    }
}
