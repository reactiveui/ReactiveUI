// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Runs an interaction's handlers in reverse registration order, one at a time on the handler scheduler, stopping as
/// soon as a handler marks the interaction handled; then emits the output or errors with an
/// <see cref="UnhandledInteractionException{TInput, TOutput}"/>. Collapses the
/// <c>ToObservable</c>+<c>ObserveOn</c>+<c>Select(Defer)</c>+<c>Concat</c>+<c>TakeWhile</c>+<c>SelectMany</c>+
/// <c>Concat(Defer(Return/Throw))</c> pipeline into a single sequential runner.
/// </summary>
/// <typeparam name="TInput">The interaction's input type.</typeparam>
/// <typeparam name="TOutput">The interaction's output type.</typeparam>
/// <param name="handlers">The registered handlers in registration order (run from last to first).</param>
/// <param name="context">The interaction context, used to read the handled flag and the output.</param>
/// <param name="scheduler">The scheduler each handler is invoked on.</param>
/// <param name="interaction">The interaction, surfaced on the unhandled exception.</param>
/// <param name="input">The interaction input, surfaced on the unhandled exception.</param>
internal sealed class InteractionHandleObservable<TInput, TOutput>(
    Func<IInteractionContext<TInput, TOutput>, IObservable<Unit>>[] handlers,
    IOutputContext<TInput, TOutput> context,
    IScheduler scheduler,
    Interaction<TInput, TOutput> interaction,
    TInput input) : IObservable<TOutput>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<TOutput> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        var sink = new Sink(observer, handlers, context, scheduler, interaction, input);
        sink.Start();
        return sink;
    }

    /// <summary>Drives the sequential, scheduler-bound handler run for a single subscription.</summary>
    private sealed class Sink : IDisposable
    {
        /// <summary>The observer receiving the interaction output.</summary>
        private readonly IObserver<TOutput> _observer;

        /// <summary>The registered handlers in registration order.</summary>
        private readonly Func<IInteractionContext<TInput, TOutput>, IObservable<Unit>>[] _handlers;

        /// <summary>The interaction context (handled flag and output).</summary>
        private readonly IOutputContext<TInput, TOutput> _context;

        /// <summary>The scheduler each handler is invoked on.</summary>
        private readonly IScheduler _scheduler;

        /// <summary>The interaction, surfaced on the unhandled exception.</summary>
        private readonly Interaction<TInput, TOutput> _interaction;

        /// <summary>The interaction input, surfaced on the unhandled exception.</summary>
        private readonly TInput _input;

        /// <summary>Holds the current scheduled step or handler subscription; swapped as the run progresses.</summary>
        private readonly SwapDisposable _current = new();

        /// <summary>Latched once this run has been disposed.</summary>
        private bool _disposed;

        /// <summary>Initializes a new instance of the <see cref="Sink"/> class.</summary>
        /// <param name="observer">The observer receiving the interaction output.</param>
        /// <param name="handlers">The registered handlers in registration order.</param>
        /// <param name="context">The interaction context.</param>
        /// <param name="scheduler">The scheduler each handler is invoked on.</param>
        /// <param name="interaction">The interaction, surfaced on the unhandled exception.</param>
        /// <param name="input">The interaction input, surfaced on the unhandled exception.</param>
        public Sink(
            IObserver<TOutput> observer,
            Func<IInteractionContext<TInput, TOutput>, IObservable<Unit>>[] handlers,
            IOutputContext<TInput, TOutput> context,
            IScheduler scheduler,
            Interaction<TInput, TOutput> interaction,
            TInput input)
        {
            _observer = observer;
            _handlers = handlers;
            _context = context;
            _scheduler = scheduler;
            _interaction = interaction;
            _input = input;
        }

        /// <summary>Begins the run at the most-recently registered handler.</summary>
        public void Start() => RunFrom(_handlers.Length - 1);

        /// <inheritdoc/>
        public void Dispose()
        {
            _disposed = true;
            _current.Dispose();
        }

        /// <summary>Schedules the next handler step on the handler scheduler.</summary>
        /// <param name="index">The handler index to run (descending toward the first-registered handler).</param>
        private void RunFrom(int index) =>
            _current.Disposable = _scheduler.Schedule(
                (Self: this, Index: index),
                static (_, state) =>
                {
                    state.Self.Step(state.Index);
                    return EmptyDisposable.Instance;
                });

        /// <summary>Runs the handler at <paramref name="index"/>, or finishes when handled or exhausted.</summary>
        /// <param name="index">The handler index to run.</param>
        private void Step(int index)
        {
            if (_disposed)
            {
                return;
            }

            if (_context.IsHandled || index < 0)
            {
                Finish();
                return;
            }

            IObservable<Unit> handlerResult;
            try
            {
                handlerResult = _handlers[index](_context);
            }
            catch (Exception ex)
            {
                _observer.OnError(ex);
                return;
            }

            var handlerObserver = new HandlerObserver(this, index);
            var subscription = handlerResult.Subscribe(handlerObserver);

            // If the handler completed (or errored) synchronously during Subscribe, it has already advanced the run and
            // stored the next scheduled step in _current; overwriting that with this now-dead subscription would dispose
            // (cancel) the next step and stall the run. Only retain the subscription while the handler is still running.
            if (handlerObserver.IsCompleted)
            {
                return;
            }

            _current.Disposable = subscription;
        }

        /// <summary>Emits the output once handled, or the unhandled-interaction error otherwise.</summary>
        private void Finish()
        {
            if (_context.IsHandled)
            {
                TOutput output;
                try
                {
                    output = _context.GetOutput();
                }
                catch (Exception ex)
                {
                    _observer.OnError(ex);
                    return;
                }

                _observer.OnNext(output);
                _observer.OnCompleted();
                return;
            }

            _observer.OnError(new UnhandledInteractionException<TInput, TOutput>(_interaction, _input));
        }

        /// <summary>Advances to the next-older handler once the current one completes.</summary>
        /// <param name="index">The index of the handler that just completed.</param>
        private void OnHandlerCompleted(int index) => RunFrom(index - 1);

        /// <summary>Forwards a handler error to the observer.</summary>
        /// <param name="error">The error to forward.</param>
        private void OnHandlerError(Exception error) => _observer.OnError(error);

        /// <summary>Observes a single handler's completion, ignoring its values.</summary>
        /// <param name="sink">The owning run.</param>
        /// <param name="index">The index of the handler being observed.</param>
        private sealed class HandlerObserver(Sink sink, int index) : IObserver<Unit>
        {
            /// <summary>Gets a value indicating whether the handler has completed or errored.</summary>
            public bool IsCompleted { get; private set; }

            /// <inheritdoc/>
            public void OnNext(Unit value)
            {
            }

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                IsCompleted = true;
                sink.OnHandlerError(error);
            }

            /// <inheritdoc/>
            public void OnCompleted()
            {
                IsCompleted = true;
                sink.OnHandlerCompleted(index);
            }
        }
    }
}
