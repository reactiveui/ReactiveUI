using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI
{
    /// <summary>
    /// Contains contextual information for an interaction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Instances of this class are passed into interaction handlers. The <see cref="Input"/> property exposes
    /// the input to the interaction, whilst the <see cref="SetOutput"/> method allows a handler to provide the
    /// output.
    /// </para>
    /// </remarks>
    /// <typeparam name="TInput">
    /// The type of the interaction's input.
    /// </typeparam>
    /// <typeparam name="TOutput">
    /// The type of the interaction's output.
    /// </typeparam>
    public sealed class InteractionContext<TInput, TOutput>
    {
        private readonly TInput input;
        private TOutput output;
        private int outputSet;

        internal InteractionContext(TInput input)
        {
            this.input = input;
        }

        /// <summary>
        /// Gets the input for the interaction.
        /// </summary>
        public TInput Input
        {
            get { return this.input; }
        }

        /// <summary>
        /// Gets a value indicating whether the interaction is handled. That is, whether the output has been set.
        /// </summary>
        public bool IsHandled
        {
            get { return this.outputSet == 1; }
        }

        /// <summary>
        /// Sets the output for the interaction.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If the output has already been set.
        /// </exception>
        public void SetOutput(TOutput output)
        {
            if (Interlocked.CompareExchange(ref this.outputSet, 1, 0) != 0) {
                throw new InvalidOperationException("Output has already been set.");
            }

            this.output = output;
        }

        /// <summary>
        /// Gets the output of the interaction.
        /// </summary>
        /// <returns>
        /// The output.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If the output has not been set.
        /// </exception>
        public TOutput GetOutput()
        {
            if (this.outputSet == 0) {
                throw new InvalidOperationException("Output has not been set.");
            }

            return this.output;
        }
    }

    /// <summary>
    /// Represents an interaction between collaborating parties.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Interactions allow collaborating components to asynchronously ask questions of each other. Typically,
    /// a view model wants to ask the user a question before proceeding with some operation, and it's the view
    /// that provides the interface via which users can answer the question.
    /// </para>
    /// <para>
    /// Interactions have both an input and output, both of which are strongly-typed via generic type parameters.
    /// The input is passed into the interaction so that handlers have the information they require. The output
    /// is provided by a handler.
    /// </para>
    /// <para>
    /// By default, handlers are invoked in reverse order of registration. That is, handlers registered later
    /// are given the opportunity to handle interactions before handlers that were registered earlier. This
    /// chaining mechanism enables handlers to be registered temporarily in a specific context, such that
    /// interactions can be handled in a different manner. Subclasses may modify this behavior by overriding
    /// the <see cref="Handle"/> method.
    /// </para>
    /// <para>
    /// Note that handlers are not required to handle an interaction. They can choose to ignore it, leaving it
    /// for some other handler to handle. If no handler handles the interaction, the <see cref="Handle"/> method
    /// will throw an <see cref="UnhandledInteractionException{TInput, TOutput}"/>.
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
        private readonly IList<Func<InteractionContext<TInput, TOutput>, IObservable<Unit>>> handlers;
        private readonly object sync;
        private readonly IScheduler handlerScheduler;

        /// <summary>
        /// Creates a new interaction instance.
        /// </summary>
        /// <param name="handlerScheduler">
        /// The scheduler to use when invoking handlers, which defaults to <c>CurrentThreadScheduler.Instance</c> if <see langword="null"/>.
        /// </param>
        public Interaction(IScheduler handlerScheduler = null)
        {
            this.handlers = new List<Func<InteractionContext<TInput, TOutput>, IObservable<Unit>>>();
            this.sync = new object();
            this.handlerScheduler = handlerScheduler ?? CurrentThreadScheduler.Instance;
        }

        /// <summary>
        /// Registers a synchronous interaction handler.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This overload of <c>RegisterHandler</c> is only useful if the handler can handle the interaction
        /// immediately. That is, it does not need to wait for a user or some other collaborating component.
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
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            return RegisterHandler(interaction => {
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
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            return RegisterHandler(interaction => handler(interaction).ToObservable());
        }

        /// <summary>
        /// Registers an observable-based asynchronous interaction handler.
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
        public IDisposable RegisterHandler<TDontCare>(Func<InteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler)
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            Func<InteractionContext<TInput, TOutput>, IObservable<Unit>> unitHandler = context => handler(context).Select(_ => Unit.Default);

            this.AddHandler(unitHandler);
            return Disposable.Create(() => this.RemoveHandler(unitHandler));
        }

        /// <summary>
        /// Handles an interaction and asynchronously returns the result.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method passes the interaction through to relevant handlers in reverse order of registration,
        /// ceasing once any handler handles the interaction. If the interaction remains unhandled after all
        /// relevant handlers have executed, an <see cref="UnhandledInteractionException{TInput, TOutput}"/> is thrown.
        /// </para>
        /// </remarks>
        /// <param name="input">
        /// The input for the interaction.
        /// </param>
        /// <returns>
        /// An observable that ticks when the interaction completes, or throws an
        /// <see cref="UnhandledInteractionException{TInput, TOutput}"/> if no handler handles the interaction.
        /// </returns>
        public virtual IObservable<TOutput> Handle(TInput input)
        {
            var context = new InteractionContext<TInput, TOutput>(input);

            return this
                .GetHandlers()
                .Reverse()
                .ToObservable()
                .ObserveOn(this.handlerScheduler)
                .Select(handler => Observable.Defer(() => handler(context)))
                .Concat()
                .TakeWhile(_ => !context.IsHandled)
                .IgnoreElements()
                .Select(_ => default(TOutput))
                .Concat(
                    Observable.Defer(
                        () => context.IsHandled
                            ? Observable.Return(context.GetOutput())
                            : Observable.Throw<TOutput>(new UnhandledInteractionException<TInput, TOutput>(this, input))));
        }

        /// <summary>
        /// Gets all registered handlers in order of their registration.
        /// </summary>
        /// <returns>
        /// All registered handlers.
        /// </returns>
        protected Func<InteractionContext<TInput, TOutput>, IObservable<Unit>>[] GetHandlers()
        {
            lock (this.sync) {
                return this.handlers.ToArray();
            }
        }

        private void AddHandler(Func<InteractionContext<TInput, TOutput>, IObservable<Unit>> handler)
        {
            lock (this.sync) {
                this.handlers.Add(handler);
            }
        }

        private void RemoveHandler(Func<InteractionContext<TInput, TOutput>, IObservable<Unit>> handler)
        {
            lock (this.sync) {
                this.handlers.Remove(handler);
            }
        }
    }

    /// <summary>
    /// Indicates that an interaction has gone unhandled.
    /// </summary>
    /// <typeparam name="TInput">
    /// The type of the interaction's input.
    /// </typeparam>
    /// <typeparam name="TOutput">
    /// The type of the interaction's output.
    /// </typeparam>
    public class UnhandledInteractionException<TInput, TOutput> : Exception
    {
        private readonly Interaction<TInput, TOutput> interaction;
        private readonly TInput input;

        public UnhandledInteractionException(Interaction<TInput, TOutput> interaction, TInput input)
        {
            this.interaction = interaction;
            this.input = input;
        }

        /// <summary>
        /// Gets the interaction that was not handled.
        /// </summary>
        public Interaction<TInput, TOutput> Interaction
        {
            get { return this.interaction; }
        }

        /// <summary>
        /// Gets the input for the interaction that was not handled.
        /// </summary>
        public TInput Input
        {
            get { return this.input; }
        }
    }
}