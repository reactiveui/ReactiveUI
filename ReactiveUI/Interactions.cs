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
    /// Encapsulates any data required for an interaction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This base class is not generally used directly. Instead, you would either create an instance of the
    /// generic subclass, <see cref="InteractionData{TResult}"/>, or derive your own interaction data class from
    /// it and create that.
    /// </para>
    /// </remarks>
    public abstract class InteractionData : ReactiveObject
    {
        /// <summary>
        /// Gets a value indicating whether the interaction has been handled. That is, a result has been set.
        /// </summary>
        public abstract bool IsHandled
        {
            get;
        }
    }

    /// <summary>
    /// Encapsulates any data required for an interaction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This generic class allows you to specify the type of the interaction's result. For example, an
    /// <c>InteractionData&lt;bool&gt;</c> would be useful in asking a yes/no question.
    /// </para>
    /// <para>
    /// You can use this class directly, or derive your own interaction data classes from it. Derivation is
    /// useful if you wish to add extra data for interaction handlers to examine.
    /// <see cref="ErrorInteractionData{TResult}"/> is a good example of this, though you might also want to
    /// subclass simply to close the generic type.
    /// </para>
    /// </remarks>
    /// <typeparam name="TResult">
    /// The interaction's result type.
    /// </typeparam>
    public class InteractionData<TResult> : InteractionData
    {
        private readonly AsyncSubject<TResult> result;
        private int resultSet;

        public InteractionData()
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
    /// Encapsulates data for an interaction that includes error information.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Instances of this type (or a derived type) are useful when the view model needs to ask another party
    /// how to recover from an error.
    /// </para>
    /// </remarks>
    /// <typeparam name="TResult">
    /// The interaction's result type.
    /// </typeparam>
    public class ErrorInteractionData<TResult> : InteractionData<TResult>
    {
        private readonly Exception error;

        public ErrorInteractionData(Exception error)
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
    /// Facilitates the distribution of interactions so that collaborating parties can ask questions of
    /// each other and asynchronously wait for an answer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Interactions allow collaborating components to asynchronously ask questions of each other. Typically,
    /// a view model wants to ask the user a question before proceeding with some operation. To do this, it
    /// creates and exposes an instance of this class. The view participates in the interaction by calling a
    /// <see cref="RegisterHandler"/> method. Whenever the view model wishes to ask a question, it creates an
    /// appropriate instance of <see cref="InteractionData"/> and passes it into the <see cref="Handle"/> method.
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
    /// will throw an <see cref="UnhandledInteractionException"/>.
    /// </para>
    /// </remarks>
    public class Interaction<TInteractionData>
        where TInteractionData : InteractionData
    {
        private readonly IList<Func<TInteractionData, IObservable<Unit>>> handlers;
        private readonly object sync;

        public Interaction()
        {
            this.handlers = new List<Func<TInteractionData, IObservable<Unit>>>();
            this.sync = new object();
        }

        /// <summary>
        /// Registers a synchronous handler for interactions of type <typeparamref name="TInteractionData"/>.
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
        public IDisposable RegisterHandler(Action<TInteractionData> handler)
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
        /// Registers a task-based asynchronous handler for interactions of type <typeparamref name="TInteractionData"/>.
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
        public IDisposable RegisterHandler(Func<TInteractionData, Task> handler)
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            return RegisterHandler(interaction => handler(interaction).ToObservable());
        }

        /// <summary>
        /// Registers an observable-based asynchronous handler for interactions of type <typeparamref name="TInteractionData"/>.
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
        public IDisposable RegisterHandler(Func<TInteractionData, IObservable<Unit>> handler)
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            this.AddHandler(handler);
            return Disposable.Create(() => this.RemoveHandler(handler));
        }

        /// <summary>
        /// Registers a synchronous handler for interactions of type <typeparamref name="T"/>, which must be
        /// derived from <typeparamref name="TInteractionData"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This overload of <c>RegisterHandler</c> is only useful if the handler can handle the interaction
        /// immediately. That is, it does not need to wait for a user or some other collaborating component.
        /// </para>
        /// <para>
        /// This interaction broker is capable of distributing interactions of type
        /// <typeparamref name="TInteractionData"/>. However, if a handler only wishes to deal with specific
        /// subclasses of <typeparamref name="TInteractionData"/> then this method can be called.
        /// </para>
        /// </remarks>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// A disposable which, when disposed, will unregister the handler.
        /// </returns>
        public IDisposable RegisterHandler<T>(Action<T> handler)
            where T : TInteractionData
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
        /// derived from <typeparamref name="TInteractionData"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This overload of <c>RegisterHandler</c> is useful if the handler needs to perform some asynchronous
        /// operation, such as displaying a dialog and waiting for the user's response.
        /// </para>
        /// <para>
        /// This interaction broker is capable of distributing interactions of type
        /// <typeparamref name="TInteractionData"/>. However, if a handler only wishes to deal with specific
        /// subclasses of <typeparamref name="TInteractionData"/> then this method can be called.
        /// </para>
        /// </remarks>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// A disposable which, when disposed, will unregister the handler.
        /// </returns>
        public IDisposable RegisterHandler<T>(Func<T, Task> handler)
            where T : TInteractionData
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            return RegisterHandler<T>(interaction => handler(interaction).ToObservable());
        }

        /// <summary>
        /// Registers an observable-based asynchronous handler for interactions of type <typeparamref name="TInteractionData"/>,
        /// which must be derived from <typeparamref name="TInteractionData"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This overload of <c>RegisterHandler</c> is useful if the handler needs to perform some asynchronous
        /// operation, such as displaying a dialog and waiting for the user's response.
        /// </para>
        /// <para>
        /// This interaction broker is capable of distributing interactions of type
        /// <typeparamref name="TInteractionData"/>. However, if a handler only wishes to deal with specific
        /// subclasses of <typeparamref name="TInteractionData"/> then this method can be called.
        /// </para>
        /// </remarks>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// A disposable which, when disposed, will unregister the handler.
        /// </returns>
        public IDisposable RegisterHandler<T>(Func<T, IObservable<Unit>> handler)
            where T : TInteractionData
        {
            if (handler == null) {
                throw new ArgumentNullException("handler");
            }

            var selectiveHandler = (Func<TInteractionData, IObservable<Unit>>)(interaction => {
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
        /// Handles an interaction and asynchronously returns the result.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method passes the interaction data through to relevant handlers in reverse order of registration,
        /// ceasing once any handler handles the interaction. If the interaction remains unhandled after all
        /// relevant handlers have executed, an <see cref="UnhandledInteractionException"/> is thrown.
        /// </para>
        /// </remarks>
        /// <param name="interactionData">
        /// The data for the interaction.
        /// </param>
        /// <returns>
        /// An observable that ticks when the interaction completes, or throws an
        /// <see cref="UnhandledInteractionException"/> if no handler handles the interaction.
        /// </returns>
        public virtual IObservable<Unit> Handle(TInteractionData interactionData)
        {
            if (interactionData == null) {
                throw new ArgumentNullException("interaction");
            }

            return this
                .GetHandlers()
                .Reverse()
                .ToObservable()
                .Select(handler => Observable.Defer(() => handler(interactionData)))
                .Concat()
                .TakeWhile(_ => !interactionData.IsHandled)
                .IgnoreElements()
                .Concat(
                    Observable.Defer(
                        () => interactionData.IsHandled
                            ? Observable.Return(Unit.Default)
                            : Observable.Throw<Unit>(new UnhandledInteractionException<TInteractionData>(this, interactionData))));
        }

        /// <summary>
        /// Gets all registered handlers in order of their registration.
        /// </summary>
        /// <returns>
        /// All registered handlers.
        /// </returns>
        protected Func<TInteractionData, IObservable<Unit>>[] GetHandlers()
        {
            lock (this.sync) {
                return this.handlers.ToArray();
            }
        }

        private void AddHandler(Func<TInteractionData, IObservable<Unit>> handler)
        {
            lock (this.sync) {
                this.handlers.Add(handler);
            }
        }

        private void RemoveHandler(Func<TInteractionData, IObservable<Unit>> handler)
        {
            lock (this.sync) {
                this.handlers.Remove(handler);
            }
        }
    }

    /// <summary>
    /// Indicates that an interaction passed into an interaction broker's <c>Raise</c> method was not handled.
    /// </summary>
    public class UnhandledInteractionException<TInteractionData> : Exception
        where TInteractionData : InteractionData
    {
        private readonly Interaction<TInteractionData> interaction;
        private readonly TInteractionData interactionData;

        public UnhandledInteractionException(Interaction<TInteractionData> interaction, TInteractionData interactionData)
        {
            this.interaction = interaction;
            this.interactionData = interactionData;
        }

        /// <summary>
        /// Gets the interaction that was not handled.
        /// </summary>
        public Interaction<TInteractionData> Interaction
        {
            get { return this.interaction; }
        }

        /// <summary>
        /// Gets the data for the interaction that was not handled.
        /// </summary>
        public TInteractionData InteractionData
        {
            get { return this.interactionData; }
        }
    }
}