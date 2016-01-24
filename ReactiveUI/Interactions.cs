using System;
using System.Collections.Generic;
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
    /// Represents an interaction between a view model and some other party, often the user.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Interactions allow a view model to ask a question of some other party, and wait asynchronously for a
    /// response. Typically, interactions pass through a broker (see <see cref="InteractionBroker{TInteraction}"/>).
    /// </para>
    /// <para>
    /// This base class is not generally used directly. Instead, you would either create an instance of the
    /// generic subclass, <see cref="Interaction{TResult}"/>, or derive your own interaction class from it
    /// and create that.
    /// </para>
    /// </remarks>
    public abstract class Interaction : ReactiveObject
    {
        /// <summary>
        /// Gets a value indicating whether this interaction has been handled. That is, a result has been set.
        /// </summary>
        public abstract bool IsHandled
        {
            get;
        }
    }

    /// <summary>
    /// Represents an interaction between a view model and some other party, often the user.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Interactions allow a view model to ask a question of some other party, and wait asynchronously for a
    /// response. Typically, interactions pass through a broker (see <see cref="InteractionBroker{TInteraction}"/>).
    /// </para>
    /// <para>
    /// This generic class allows you to specify the type of the interaction's result. For example, an
    /// <c>Interaction&lt;bool&gt;</c> would be useful in asking a yes/no question.
    /// </para>
    /// <para>
    /// You can use this class directly, or derive your own interaction classes from it. Derivation would be
    /// useful if you wish to add extra data that the handlers of the interaction can utilize in performing
    /// their job. <see cref="ErrorInteraction{TResult}"/> is a good example of this, though you might also
    /// want a subclass simply to close the generic type.
    /// </para>
    /// <para>
    /// Typically view models create interaction instances and raise them against an interaction broker.
    /// Views, which register handlers against the same broker, can choose whether or not to handle that
    /// interaction. That is, they may or may not set a result.
    /// </para>
    /// </remarks>
    /// <typeparam name="TResult">
    /// The interaction's result type.
    /// </typeparam>
    public class Interaction<TResult> : Interaction
    {
        private readonly AsyncSubject<TResult> result;
        private int resultSet;

        public Interaction()
        {
            this.result = new AsyncSubject<TResult>();
        }

        /// <inheritdoc/>
        public override bool IsHandled
        {
            get { return this.resultSet == 1; }
        }

        /// <summary>
        /// Sets the result of the interaction.
        /// </summary>
        /// <param name="result">
        /// The interaction's result.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If a result has already been set.
        /// </exception>
        public void SetResult(TResult result)
        {
            if (Interlocked.CompareExchange(ref this.resultSet, 1, 0) != 0) {
                throw new InvalidOperationException("Result has already been set.");
            }

            this.result.OnNext(result);
            this.result.OnCompleted();
        }

        /// <summary>
        /// Gets the result of the interaction.
        /// </summary>
        /// <returns>
        /// The interaction's result.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If no result has been set.
        /// </exception>
        public TResult GetResult()
        {
            if (this.resultSet == 0) {
                throw new InvalidOperationException("Result has not been set.");
            }

            return this.result.GetResult();
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

    // TODO: not convinced we should have this any more. Could just allow application code to define the static/shared brokers they want
    ///// <summary>
    ///// Provides a means of obtaining the global interaction broker.
    ///// </summary>
    ///// <remarks>
    ///// <para>
    ///// This static class provides a means of obtaining the global interaction broker. This broker is agnostic about the
    ///// types of interactions that it brokers. In other words, it is heterogeneous.
    ///// </para>
    ///// <para>
    ///// The global broker can be useful when multiple application components can be the source of a 
    ///// </para>
    ///// </remarks>
    //public static class InteractionBroker
    //{
    //    public static readonly InteractionBroker<Interaction> Global = new InteractionBroker<Interaction>();
    //}

    /// <summary>
    /// Facilitates the distribution of interactions so that collaborating parties can ask questions of
    /// each other and asynchronously wait for an answer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Interaction brokers are the means by which interactions are passed between collaborators of the
    /// interaction. Interactions are passed in via the <see cref="Raise(TInteraction)"/> method, and
    /// handlers are registered via the various <see cref="RegisterHandler"/> methods.
    /// </para>
    /// <para>
    /// By default, handlers are invoked in reverse order of registration. That is, handlers registered later
    /// are given the opportunity to handle interactions before handlers that were registered earlier. This
    /// chaining mechanism enables handlers to be registered temporarily in a specific context, such that
    /// interactions can be handled in a different manner. Subclasses may modify this behavior by overriding
    /// the <see cref="Raise"/> method.
    /// </para>
    /// </remarks>
    public class InteractionBroker<TInteraction>
        where TInteraction : Interaction
    {
        private readonly IList<Func<TInteraction, IObservable<Unit>>> handlers;
        private readonly object sync;

        public InteractionBroker()
        {
            this.handlers = new List<Func<TInteraction, IObservable<Unit>>>();
            this.sync = new object();
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

            this.AddHandler(handler);
            return Disposable.Create(() => this.RemoveHandler(handler));
        }

        /// <summary>
        /// Registers a synchronous handler for interactions of type <typeparamref name="T"/>, which must be
        /// derived from <typeparamref name="TInteraction"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This overload of <c>RegisterHandler</c> is only useful if the handler can handle the interaction
        /// immediately. That is, it does not need to wait for a user or some other collaborating component.
        /// </para>
        /// <para>
        /// This interaction broker is capable of distributing interactions of type
        /// <typeparamref name="TInteraction"/>. However, if a handler only wishes to deal with specific
        /// subclasses of <typeparamref name="TInteraction"/> then this method can be called.
        /// </para>
        /// </remarks>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// A disposable which, when disposed, will unregister the handler.
        /// </returns>
        public IDisposable RegisterHandler<T>(Action<T> handler)
            where T : TInteraction
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            return RegisterHandler<T>(interaction => {
                handler(interaction);
                return Observable.Return(Unit.Default);
            });
        }

        /// <summary>
        /// Registers a task-based asynchronous handler for interactions of type <typeparamref name="T"/>, which must be
        /// derived from <typeparamref name="TInteraction"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This overload of <c>RegisterHandler</c> is useful if the handler needs to perform some asynchronous
        /// operation, such as displaying a dialog and waiting for the user's response.
        /// </para>
        /// <para>
        /// This interaction broker is capable of distributing interactions of type
        /// <typeparamref name="TInteraction"/>. However, if a handler only wishes to deal with specific
        /// subclasses of <typeparamref name="TInteraction"/> then this method can be called.
        /// </para>
        /// </remarks>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// A disposable which, when disposed, will unregister the handler.
        /// </returns>
        public IDisposable RegisterHandler<T>(Func<T, Task> handler)
            where T : TInteraction
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            return RegisterHandler<T>(interaction => handler(interaction).ToObservable());
        }

        /// <summary>
        /// Registers an observable-based asynchronous handler for interactions of type <typeparamref name="TInteraction"/>,
        /// which must be derived from <typeparamref name="TInteraction"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This overload of <c>RegisterHandler</c> is useful if the handler needs to perform some asynchronous
        /// operation, such as displaying a dialog and waiting for the user's response.
        /// </para>
        /// <para>
        /// This interaction broker is capable of distributing interactions of type
        /// <typeparamref name="TInteraction"/>. However, if a handler only wishes to deal with specific
        /// subclasses of <typeparamref name="TInteraction"/> then this method can be called.
        /// </para>
        /// </remarks>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// A disposable which, when disposed, will unregister the handler.
        /// </returns>
        public IDisposable RegisterHandler<T>(Func<T, IObservable<Unit>> handler)
            where T : TInteraction
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            var selectiveHandler = (Func<TInteraction, IObservable<Unit>>)(interaction => {
                var castInteraction = interaction as T;

                if (castInteraction == null) {
                    return Observable.Return(Unit.Default);
                }

                return handler(castInteraction);
            });

            this.AddHandler(selectiveHandler);
            return Disposable.Create(() => this.RemoveHandler(selectiveHandler));
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
        public virtual IObservable<Unit> Raise(TInteraction interaction)
        {
            if (interaction == null) {
                throw new ArgumentNullException("interaction");
            }

            return this
                .GetHandlers()
                .Reverse()
                .ToObservable()
                .Select(handler => Observable.Defer(() => handler(interaction)))
                .Concat()
                .TakeWhile(_ => !interaction.IsHandled)
                .IgnoreElements()
                .Concat(Observable.Defer(() => interaction.IsHandled ? Observable.Return(Unit.Default) : Observable.Throw<Unit>(new UnhandledInteractionException(interaction))));
        }

        /// <summary>
        /// Gets all registered handlers in order of their registration.
        /// </summary>
        /// <returns>
        /// All registered handlers.
        /// </returns>
        protected Func<TInteraction, IObservable<Unit>>[] GetHandlers()
        {
            lock (this.sync) {
                return this.handlers.ToArray();
            }
        }

        private void AddHandler(Func<TInteraction, IObservable<Unit>> handler)
        {
            lock (this.sync) {
                this.handlers.Add(handler);
            }
        }

        private void RemoveHandler(Func<TInteraction, IObservable<Unit>> handler)
        {
            lock (this.sync) {
                this.handlers.Remove(handler);
            }
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