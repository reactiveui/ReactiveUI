using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace ReactiveUI
{
    /// <summary>
    /// Represents an interaction with the user.
    /// </summary>
    /// <remarks>
    /// <para>
    /// User interactions are used when view models need the user to answer some question before they can continue their work.
    /// For example, prior to deleting a file the user may be required to acquiesce.
    /// </para>
    /// <para>
    /// User interactions may be either propagated or not. Non-propagated interactions are typically exposed as properties on
    /// a view model. When the view model needs an answer to a question, it creates a new instance of the interaction and assigns
    /// it to the property. The corresponding view monitors that property for changes and, when a change is detected, prompts the
    /// user for their answer. When the user responds, the answer is pushed to the result of the user interaction, at which point
    /// the patiently-waiting view model will pick up its work.
    /// </para>
    /// <para>
    /// It's important to understand that non-propagated exceptions will not tick through the <see cref="PropagatedInteractions"/>
    /// observable. Therefore, such interactions can only be handled by those components that have access to the interactions.
    /// Usually this means that the related view is responsible for handling the interaction. Any views further up the hierarchy
    /// are unlikely to hook into properties in a lower-level view model.
    /// </para>
    /// <para>
    /// Propagated interactions, on the other hand, do tick through <see cref="PropagatedInteractions"/> and so can be handled
    /// anywhere in the application. Every subscriber of the <see cref="PropagatedInteractions"/> observable is given the
    /// opportunity to handle the interaction. Later subscribers are given priority over earlier subscribers. This means that it
    /// is possible to set up one or more "root" handlers to deal with common situations, such as recovering from errors. And
    /// temporary subscriptions from higher-level components can take precedence over the root handlers.
    /// </para>
    /// <para>
    /// The advantage of propagated exceptions is that any application component can handle them, and handlers are queried in a
    /// logical order. The disadvantage is that they can make application logic harder to follow. Non-propagated interactions are
    /// therefore recommended wherever possible.
    /// </para>
    /// </remarks>
    public abstract class UserInteraction
    {
        private static readonly PropagatedInteractionsObservable propagatedInteractionsObservable = new PropagatedInteractionsObservable();
        private readonly AsyncSubject<object> result;
        private int resultSet;

        protected UserInteraction()
        {
            this.result = new AsyncSubject<object>();
        }

        /// <summary>
        /// An observable of all propagated user interactions.
        /// </summary>
        /// <remarks>
        /// Any user interaction that is propagated will tick through this observable. Typically, handlers will use <c>OfType</c> to filter
        /// to only those interactions they potentially wish to handle.
        /// </remarks>
        public static IObservable<UserInteraction> PropagatedInteractions
        {
            get { return propagatedInteractionsObservable.ObserveOn(RxApp.MainThreadScheduler); }
        }

        /// <summary>
        /// Gets a value indicating whether this user interaction has been handled.
        /// </summary>
        /// <remarks>
        /// This property returns <see langword="true"/> if the user interaction has been handled, or <see langword="false"/> if it
        /// hasn't. The definition of "handled" is that a call to <see cref="SetResult"/> has been made.
        /// </remarks>
        public bool IsHandled
        {
            get { return this.resultSet == 1; }
        }

        /// <summary>
        /// Gets the result of the interaction. That is, the "answer" provided by the user.
        /// </summary>
        /// <remarks>
        /// The result is untyped. Strong typing is introduced by the <see cref="UserInteraction{TResult}"/> class.
        /// </remarks>
        /// <returns>
        /// An observable that immediately ticks the result if it is already known, otherwise later when it is.
        /// </returns>
        protected IObservable<object> GetResult()
        {
            return this.result;
        }

        /// <summary>
        /// Sets the result of the interaction.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The result is untyped. Strong typing is introduced by the <see cref="UserInteraction{TResult}"/> class.
        /// </para>
        /// <para>
        /// This method can only be called once. Any subsequent attempt to supply a result will cause an exception to be thrown.
        /// </para>
        /// </remarks>
        /// <param name="result">
        /// The result.
        /// </param>
        protected void SetResult(object result)
        {
            if (Interlocked.CompareExchange(ref this.resultSet, 1, 0) != 0)
            {
                throw new InvalidOperationException("Result has already been set.");
            }

            this.result.OnNext(result);
            this.result.OnCompleted();
        }

        /// <summary>
        /// Propagates this interaction.
        /// </summary>
        /// <remarks>
        /// The result is untyped. Strong typing is introduced by the <see cref="UserInteraction{TResult}"/> class.
        /// </remarks>
        /// <returns>
        /// The result obtained from propagated interaction handlers.
        /// </returns>
        protected IObservable<object> Propagate()
        {
            return propagatedInteractionsObservable.Propagate(this);
        }

        // a custom implementation of IObservable<UserIntraction> that visits observers in reverse order, and ceases
        // visiting observers if any prior observer handles the interaction
        private sealed class PropagatedInteractionsObservable : IObservable<UserInteraction>
        {
            private readonly IList<IObserver<UserInteraction>> observers;
            private readonly object sync;

            public PropagatedInteractionsObservable()
            {
                this.observers = new List<IObserver<UserInteraction>>();
                this.sync = new object();
            }

            public IDisposable Subscribe(IObserver<UserInteraction> observer)
            {
                lock (this.sync)
                {
                    this.observers.Add(observer);
                }

                return Disposable.Create(() => this.Unsubscribe(observer));
            }

            private void Unsubscribe(IObserver<UserInteraction> observer)
            {
                lock (this.sync)
                {
                    this.observers.Remove(observer);
                }
            }

            public IObservable<object> Propagate(UserInteraction userInteraction)
            {
                IObserver<UserInteraction>[] handlers = null;

                lock (this.sync)
                {
                    handlers = this
                        .observers
                        .Reverse()
                        .ToArray();
                }

                // AAAAAAGGGGGGGGGGGGHHHHHHHHHHHHHHHHHHHHHHH!!!!!!!!!!!!!!!!!!!
                // need to wait for the handler to do its thing before checking if interaction is handled
                // but there's no way of achieving that with current design because handlers are not asynchronous
                return Observable
                    .StartAsync(
                        async () =>
                        {
                            foreach (var handler in handlers)
                            {
                                handler.OnNext(userInteraction);

                                if (userInteraction.IsHandled)
                                {
                                    return await userInteraction.GetResult();
                                }
                            }

                            throw new UnhandledUserInteractionException(userInteraction);
                        });
            }
        }
    }

    public class UserInteraction<TResult> : UserInteraction
    {
        public new IObservable<TResult> GetResult()
        {
            return base.GetResult().Cast<TResult>();
        }

        public void SetResult(TResult result)
        {
            base.SetResult(result);
        }

        /// <summary>
        /// Propagate this user interaction.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method propagates this user interaction until one of the <see cref="PropagatedInteractions"/> observers handles it.
        /// If any observer handles the interaction, no subsequent observers receive it. Observers are traversed in reverse order.
        /// That is, earlier subscribers are given the opportunity to handle the interaction only if later subscribers have not
        /// already handled it.
        /// </para>
        /// <para>
        /// If no observer handles the interaction, an <see cref="UnhandledUserInteractionException{TResult}"/> is thrown.
        /// </para>
        /// </remarks>
        public new IObservable<TResult> Propagate()
        {
            return base.Propagate().Cast<TResult>();
        }
    }

    public class UserErrorInteraction<TResult> : UserInteraction<TResult>
    {
        private readonly Exception error;

        public UserErrorInteraction(Exception error)
        {
            this.error = error;
        }

        public Exception Error
        {
            get { return this.error; }
        }
    }

    public class UnhandledUserInteractionException : Exception
    {
        private readonly UserInteraction userInteraction;

        public UnhandledUserInteractionException(UserInteraction userInteraction)
        {
            this.userInteraction = userInteraction;
        }

        public UserInteraction UserInteraction
        {
            get { return this.userInteraction; }
        }
    }
}