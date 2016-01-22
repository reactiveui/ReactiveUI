using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI
{
    /// <summary>
    /// Represents an interaction between a view model and some other party, usually the user.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Interactions allow a view model to ask a question of some other party, and wait asynchronously for a
    /// response. Interactions pass through a broker (see <see cref="InteractionBroker"/> and
    /// <see cref="InteractionBroker{TInteraction, TResult}"/>).
    /// </para>
    /// <para>
    /// This base class is not generally used directly. Instead, you would either create an instance of the
    /// generic subclass, <see cref="Interaction{TResult}"/>, or derive your own interaction class from it
    /// and create that.
    /// </para>
    /// </remarks>
    public abstract class Interaction
    {
        private readonly AsyncSubject<object> result;
        private int resultSet;

        protected Interaction()
        {
            this.result = new AsyncSubject<object>();
        }

        /// <summary>
        /// Gets a value indicating whether this interaction has been handled. That is, a result has been set.
        /// </summary>
        public bool IsHandled
        {
            get { return this.resultSet == 1; }
        }

        /// <summary>
        /// Subclasses can call this method to provide the result of the interaction.
        /// </summary>
        /// <param name="result">
        /// The interaction result.
        /// </param>
        protected void SetResult(object result)
        {
            if (Interlocked.CompareExchange(ref this.resultSet, 1, 0) != 0) {
                throw new InvalidOperationException("Result has already been set.");
            }

            this.result.OnNext(result);
            this.result.OnCompleted();
        }

        internal object GetResult()
        {
            Debug.Assert(this.IsHandled);
            return this.result.GetResult();
        }

        /// <summary>
        /// Creates a new <see cref="InterationBroker"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By convention, interaction implementations expose a static <c>CreateBroker</c> method that creates
        /// an appropriate broker for that interaction. It is generally easier to use this method rather than
        /// creating brokers directly.
        /// </para>
        /// </remarks>
        /// <returns>
        /// The new <c>InterationBroker</c>.
        /// </returns>
        public static InteractionBroker CreateBroker()
        {
            return new InteractionBroker();
        }
    }

    /// <summary>
    /// Represents an interaction between a view model and some other party, usually the user.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Interactions allow a view model to ask a question of some other party, and wait asynchronously for a
    /// response. Interactions pass through a broker (see <see cref="InteractionBroker"/> and
    /// <see cref="InteractionBroker{TInteraction, TResult}"/>).
    /// </para>
    /// <para>
    /// This generic class allows you to specify the type of the interaction's result. For example, an
    /// <c>Interaction&lt;bool&gt;</c> would be useful in asking a yes/no question.
    /// </para>
    /// <para>
    /// You can use this class directly, or derive your own interaction classes from it. Derivation would be
    /// useful if you wish to add extra data that the handlers of the interaction can utilize in performing
    /// their job. <see cref="ErrorInteraction{TResult}"/> is a good example of this, though you might also
    /// want your subclass to close the generic type.
    /// </para>
    /// <para>
    /// Typically view models create interaction instances and raise them against an interaction broker.
    /// Views, which register handlers via the same broker, can choose whether or not to handle that
    /// interaction. That is, they may or may not set a result.
    /// </para>
    /// </remarks>
    /// <typeparam name="TResult">
    /// The interaction's result type.
    /// </typeparam>
    public class Interaction<TResult> : Interaction
    {
        /// <summary>
        /// Assigns a result to the interaction.
        /// </summary>
        /// <param name="result">
        /// The result of the interaction.
        /// </param>
        public void SetResult(TResult result)
        {
            base.SetResult(result);
        }

        internal new TResult GetResult()
        {
            return (TResult)base.GetResult();
        }

        /// <summary>
        /// Creates an instance of <see cref="InteractionBroker{TInteraction, TResult}"/> via which interactions
        /// of this type can be handled.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By convention, interaction implementations expose a static <c>CreateBroker</c> method that creates
        /// an appropriate broker for that interaction. It is generally easier to use this method rather than
        /// creating brokers directly.
        /// </para>
        /// </remarks>
        /// <returns>
        /// The new <c>InterationBroker</c>.
        /// </returns>
        public static new InteractionBroker<Interaction<TResult>, TResult> CreateBroker()
        {
            return new InteractionBroker<Interaction<TResult>, TResult>();
        }
    }

    /// <summary>
    /// An interaction that includes information on an underlying error.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Interactions of this type (or a derived type) are useful when the view model needs to ask another party
    /// how to recover from or error.
    /// </para>
    /// </remarks>
    /// <typeparam name="TResult">
    /// The interaction's result type.
    /// </typeparam>
    public class ErrorInteraction<TResult> : Interaction<TResult>
    {
        private readonly Exception error;

        public ErrorInteraction(Exception error)
        {
            this.error = error;
        }

        /// <summary>
        /// Gets the exception object that is the underlying cause of the error.
        /// </summary>
        public Exception Error
        {
            get { return this.error; }
        }
    }

    /// <summary>
    /// Facilitates the distribution of heterogeneous interactions so that collaborating parties can ask
    /// questions of each other and asynchronously wait for an answer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Interaction brokers are the means by which interactions are passed between collaborators of the
    /// interaction. Interactions are passed in via the <see cref="Raise{TResult}(Interaction{TResult})"/>
    /// method, and handlers are registered via the <see cref="RegisterHandler"/> methods.
    /// </para>
    /// <para>
    /// This broker allows for heterogeneous interactions. That is, it can broker interactions of differing
    /// types; any <see cref="Interaction"/> instance can pass through this broker. Such a broker is most
    /// useful when it is being shared by a variety of components in a loosely-defined manner. Normally it
    /// suffices to use the instance exposed by <see cref="Global"/>.
    /// </para>
    /// <para>
    /// Handlers are invoked in reverse order of registration. That is, handlers registered later are
    /// given the opportunity to handle interactions before handlers that were registered earlier. This
    /// chaining mechanism enables handlers to be registered temporarily in a specific context, such that
    /// interactions can be handled in a different manner.
    /// </para>
    /// </remarks>
    public class InteractionBroker
    {
        // convenient broker that any component can hook into
        public static readonly InteractionBroker Global = Interaction.CreateBroker();

        private readonly IList<Func<Interaction, IObservable<Unit>>> handlers;

        public InteractionBroker()
        {
            this.handlers = new List<Func<Interaction, IObservable<Unit>>>();
        }

        /// <summary>
        /// Registers a synchronous handler for interactions of type <typeparamref name="TInteraction"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This overload of <c>RegisterHandler</c> is only useful if the handler can handle the interaction
        /// immediately. That is, it does not need to wait for a user or some other collaborating component.
        /// </para>
        /// </remarks>
        /// <typeparam name="TInteraction">
        /// The type interactions that the handler should receive from the broker.
        /// </typeparam>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// A disposable which, when disposed, will unregister the handler.
        /// </returns>
        public IDisposable RegisterHandler<TInteraction>(Action<TInteraction> handler)
            where TInteraction : Interaction
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            return RegisterHandler<TInteraction>(interaction => {
                handler(interaction);
                return Observable.Return(Unit.Default);
            });
        }

        /// <summary>
        /// Registers a task-based asynchronous handler for interactions of type <typeparamref name="TInteraction"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This overload of <c>RegisterHandler</c> is useful if the handler needs to perform some asynchronous
        /// operation, such as displaying a dialog and waiting for the user's response.
        /// </para>
        /// </remarks>
        /// <typeparam name="TInteraction">
        /// The type interactions that the handler should receive from the broker.
        /// </typeparam>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// A disposable which, when disposed, will unregister the handler.
        /// </returns>
        public IDisposable RegisterHandler<TInteraction>(Func<TInteraction, Task> handler)
            where TInteraction : Interaction
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            return RegisterHandler<TInteraction>(interaction => handler(interaction).ToObservable());
        }

        /// <summary>
        /// Registers an observable-based asynchronous handler for interactions of type <typeparamref name="TInteraction"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This overload of <c>RegisterHandler</c> is useful if the handler needs to perform some asynchronous
        /// operation, such as displaying a dialog and waiting for the user's response.
        /// </para>
        /// </remarks>
        /// <typeparam name="TInteraction">
        /// The type interactions that the handler should receive from the broker.
        /// </typeparam>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// A disposable which, when disposed, will unregister the handler.
        /// </returns>
        public IDisposable RegisterHandler<TInteraction>(Func<TInteraction, IObservable<Unit>> handler)
            where TInteraction : Interaction
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            var selectiveHandler = (Func<Interaction, IObservable<Unit>>)(interaction => {
                var castInteraction = interaction as TInteraction;

                if (castInteraction == null) {
                    return Observable.Return(Unit.Default);
                }

                return handler(castInteraction);
            });

            handlers.Add(selectiveHandler);
            return Disposable.Create(() => handlers.Remove(selectiveHandler));
        }

        /// <summary>
        /// Raises an interaction and asynchronously returns the result.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Raising an interaction passes it through to relevant handlers in reverse order of registration,
        /// ceasing once any handler handles the interaction. If the interaction remains unhandled after all
        /// relevant handlers have executed, an <see cref="UnhandledInteractionException"/> is thrown.
        /// </para>
        /// </remarks>
        /// <typeparam name="TResult">
        /// The interaction's result type.
        /// </typeparam>
        /// <param name="interaction">
        /// The interaction to raise.
        /// </param>
        /// <returns>
        /// An observable that ticks the interaction's result, or an <see cref="UnhandledInteractionException"/>
        /// if no handler handles the interaction.
        /// </returns>
        public IObservable<TResult> Raise<TResult>(Interaction<TResult> interaction)
        {
            if (interaction == null) {
                throw new ArgumentNullException("interaction");
            }

            return Enumerable
                .Reverse(this.handlers)
                .ToArray()
                .ToObservable()
                .Select(handler => Observable.Defer(() => handler(interaction)))
                .Concat()
                .TakeWhile(_ => !interaction.IsHandled)
                .IgnoreElements()
                .Select(_ => default(TResult))
                .Concat(Observable.Defer(() => interaction.IsHandled ? Observable.Return(interaction.GetResult()) : Observable.Throw<TResult>(new UnhandledInteractionException(interaction))));
        }
    }

    /// <summary>
    /// Facilitates the distribution of homogeneous interactions so that collaborating parties can ask
    /// questions of each other and asynchronously wait for an answer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Interaction brokers are the means by which interactions are passed between collaborators of the
    /// interaction. Interactions are passed in via the <see cref="Raise(TInteraction)"/> method, and
    /// handlers are registered via the <see cref="RegisterHandler"/> methods.
    /// </para>
    /// <para>
    /// This broker allows for homogenous interactions. Specifically, all interactions must be of type
    /// <typeparam name="TInteraction"/> (or subclasses thereof). Such a broker is most useful when the
    /// collaborating components are closely related (like between a view model and its view).
    /// </para>
    /// <para>
    /// Handlers are invoked in reverse order of registration. That is, handlers registered later are
    /// given the opportunity to handle interactions before handlers that were registered earlier. This
    /// chaining mechanism enables handlers to be registered temporarily in a specific context, such that
    /// interactions can be handled in a different manner.
    /// </para>
    /// </remarks>
    public class InteractionBroker<TInteraction, TResult>
        where TInteraction : Interaction<TResult>
    {
        private readonly IList<Func<TInteraction, IObservable<Unit>>> handlers;

        public InteractionBroker()
        {
            this.handlers = new List<Func<TInteraction, IObservable<Unit>>>();
        }

        /// <summary>
        /// Registers a task-based asynchronous handler for interactions of type <typeparamref name="TInteraction"/>.
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
        public IDisposable RegisterHandler(Action<TInteraction> handler)
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            return RegisterHandler(interaction => {
                handler(interaction);
                return Observable.Return(Unit.Default);
            });
        }

        /// <summary>
        /// Registers a task-based asynchronous handler for interactions of type <typeparamref name="TInteraction"/>.
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
        public IDisposable RegisterHandler(Func<TInteraction, Task> handler)
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            return RegisterHandler(interaction => handler(interaction).ToObservable());
        }

        /// <summary>
        /// Registers an observable-based asynchronous handler for interactions of type <typeparamref name="TInteraction"/>.
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
        public IDisposable RegisterHandler(Func<TInteraction, IObservable<Unit>> handler)
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            handlers.Add(handler);
            return Disposable.Create(() => handlers.Remove(handler));
        }

        /// <summary>
        /// Raises an interaction and asynchronously returns the result.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Raising an interaction passes it through to relevant handlers in reverse order of registration,
        /// ceasing once any handler handles the interaction. If the interaction remains unhandled after all
        /// relevant handlers have executed, an <see cref="UnhandledInteractionException"/> is thrown.
        /// </para>
        /// </remarks>
        /// <param name="interaction">
        /// The interaction to raise.
        /// </param>
        /// <returns>
        /// An observable that ticks the interaction's result, or an <see cref="UnhandledInteractionException"/>
        /// if no handler handles the interaction.
        /// </returns>
        public IObservable<TResult> Raise(TInteraction interaction)
        {
            if (interaction == null) {
                throw new ArgumentNullException("interaction");
            }

            return Enumerable
                .Reverse(this.handlers)
                .ToArray()
                .ToObservable()
                .Select(handler => Observable.Defer(() => handler(interaction)))
                .Concat()
                .TakeWhile(_ => !interaction.IsHandled)
                .IgnoreElements()
                .Select(_ => default(TResult))
                .Concat(Observable.Defer(() => interaction.IsHandled ? Observable.Return(interaction.GetResult()) : Observable.Throw<TResult>(new UnhandledInteractionException(interaction))));
        }
    }

    /// <summary>
    /// Indicates that an interaction passed into an interaction broker's <c>Raise</c> method was not handled.
    /// </summary>
    public class UnhandledInteractionException : Exception
    {
        private readonly Interaction interaction;

        public UnhandledInteractionException(Interaction interaction)
        {
            this.interaction = interaction;
        }

        /// <summary>
        /// Gets the interaction that was not handled.
        /// </summary>
        public Interaction Interaction
        {
            get { return this.interaction; }
        }
    }
}