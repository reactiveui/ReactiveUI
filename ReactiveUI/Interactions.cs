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
    /// Represents an interaction with the user.
    /// </summary>
    /// <remarks>
    /// <para>
    /// User interactions are used when view models need the user to answer some question before they can continue their work.
    /// For example, prior to deleting a file the user may be required to acquiesce. Naturally, this question needs to be
    /// answered in an asynchronous fashion, so that no blocking occurs while the view model waits for an answer.
    /// </para>
    /// <para>
    /// User interactions may be either local or global. Local interactions are typically exposed as properties on a view model.
    /// When the view model needs an answer to a question, it creates a new instance of the interaction and assigns it to the
    /// property. The corresponding view monitors that property for changes and, when a change is detected, prompts the user for
    /// their answer. When the user responds, the answer is pushed to the result of the user interaction, at which point the
    /// patiently-waiting view model will pick up its work.
    /// </para>
    /// <para>
    /// Global interactions, on the other hand, can be handled by any handler registered via the <see cref="RegisterGlobalHandler"/>
    /// methods. Each registered handler is given the opportunity to handle the interaction, but later handlers are given
    /// priority over handlers that are registered earlier. This means that it is possible to set up one or more "root" handlers
    /// to deal with common situations (such as recovering from errors), and temporary handlers registered by higher-level
    /// components can take precedence over the root handlers.
    /// </para>
    /// <para>
    /// It's important to understand that local interactions will not result in any handlers being invoked. Therefore, such
    /// interactions can only be handled by those components that have access to the interaction instances. Usually this means that
    /// the related view is responsible for handling the interaction. Any views further up the hierarchy are unlikely to hook into
    /// properties in a lower-level view model.
    /// </para>
    /// <para>
    /// The advantage of global interactions is that any application component can handle them, and handlers are queried in a
    /// logical order. The disadvantage is that they can make application logic harder to follow. Local interactions are therefore
    /// recommended wherever possible.
    /// </para>
    /// </remarks>
    public abstract class UserInteraction : ReactiveObject
    {
        private static readonly IList<Func<UserInteraction, IObservable<Unit>>> handlers = new List<Func<UserInteraction, IObservable<Unit>>>();
        private readonly AsyncSubject<object> result;
        private int resultSet;

        protected UserInteraction()
        {
            this.result = new AsyncSubject<object>();
        }

        /// <summary>
        /// Registers an observable-based asynchronous global handler for the specified interaction type.
        /// </summary>
        /// <typeparam name="TInteraction">
        /// The type of interaction being handled.
        /// </typeparam>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// A disposable instance that, when disposed, will unregister the global handler.
        /// </returns>
        public static IDisposable RegisterGlobalHandler<TInteraction>(Func<TInteraction, IObservable<Unit>> handler)
            where TInteraction : UserInteraction
        {
            var selectiveHandler = (Func<UserInteraction, IObservable<Unit>>)(interaction =>
            {
                var castInteraction = interaction as TInteraction;

                if (castInteraction == null)
                {
                    return Observable.Return(Unit.Default);
                }

                return handler(castInteraction);
            });

            handlers.Add(selectiveHandler);
            return Disposable.Create(() => handlers.Remove(selectiveHandler));
        }

        /// <summary>
        /// Registers a task-based asynchronous global handler for the specified interaction type.
        /// </summary>
        /// <typeparam name="TInteraction">
        /// The type of interaction being handled.
        /// </typeparam>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// A disposable instance that, when disposed, will unregister the global handler.
        /// </returns>
        public static IDisposable RegisterGlobalHandler<TInteraction>(Func<TInteraction, Task> handler)
            where TInteraction : UserInteraction
        {
            return RegisterGlobalHandler<TInteraction>(interaction => handler(interaction).ToObservable());
        }

        /// <summary>
        /// Registers a synchronous global handler for the specified interaction type.
        /// </summary>
        /// <remarks>
        /// Synchronous handlers cannot await user interactions. It is therefore more likely you want to use an asynchronous handler.
        /// </remarks>
        /// <typeparam name="TInteraction">
        /// The type of interaction being handled.
        /// </typeparam>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <returns>
        /// A disposable instance that, when disposed, will unregister the global handler.
        /// </returns>
        public static IDisposable RegisterGlobalHandler<TInteraction>(Action<TInteraction> handler)
            where TInteraction : UserInteraction
        {
            return RegisterGlobalHandler<TInteraction>(interaction =>
            {
                handler(interaction);
                return Observable.Return(Unit.Default);
            });
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
        /// Raises the interaction locally.
        /// </summary>
        /// <remarks>
        /// The result is untyped. Strong typing is introduced by the <see cref="UserInteraction{TResult}"/> class.
        /// </remarks>
        /// <returns>
        /// An observable that immediately ticks the result if it is already known, otherwise later when it is.
        /// </returns>
        protected IObservable<object> Raise()
        {
            return this.result;
        }

        /// <summary>
        /// Raises the interaction globally.
        /// </summary>
        /// <remarks>
        /// The result is untyped. Strong typing is introduced by the <see cref="UserInteraction{TResult}"/> class.
        /// </remarks>
        /// <returns>
        /// The result obtained from global interaction handlers.
        /// </returns>
        protected IObservable<object> RaiseGlobal()
        {
            return Observable
                .StartAsync(
                    async () =>
                    {
                        var handlers = UserInteraction.handlers.Reverse().ToArray();

                        foreach (var handler in handlers)
                        {
                            await handler(this);

                            if (this.IsHandled)
                            {
                                return this.result.GetResult();
                            }
                        }

                        throw new UnhandledUserInteractionException(this);
                    });
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
    }

    /// <summary>
    /// Implements a <see cref="UserInteraction"/> with a specific result type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides a means of interacting with users and obtaining a strongly-typed result. Typically, you will either
    /// use this class directly (e.g. a <c>UserInteraction&lt;bool&gt;</c> will allow you to obtain a yes/no answer from the user)
    /// or indirectly (by either subclassing or using an existing subclass).
    /// </para>
    /// </remarks>
    /// <typeparam name="TResult">
    /// The type of the interaction's result.
    /// </typeparam>
    public class UserInteraction<TResult> : UserInteraction
    {
        /// <summary>
        /// Raises the interaction locally.
        /// </summary>
        /// <remarks>
        /// <para>
        /// View models using local interactions would typically await a call to this method after making the interaction available
        /// for views to handle. If a global interaction is being used, you'd instead await the <see cref="RaiseGlobal"/> method.
        /// </para>
        /// </remarks>
        /// <returns>
        /// An observable that immediately ticks the interaction result if it is already known, otherwise later when it is.
        /// </returns>
        public new IObservable<TResult> Raise()
        {
            return base.Raise().Cast<TResult>();
        }

        /// <summary>
        /// Raises the interaction globally.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method passes the interaction through to all registered global handlers in reverse order of their registration.
        /// As soon as any handler supplies a result for the interaction, subsequent handlers are not called. That is, handlers 
        /// that are registered earlier are given the opportunity to handle the interaction only if handlers registered later
        /// have not already done so.
        /// </para>
        /// <para>
        /// If no handler handles the interaction, an <see cref="UnhandledUserInteractionException{TResult}"/> is thrown.
        /// </para>
        /// </remarks>
        public new IObservable<TResult> RaiseGlobal()
        {
            return base.RaiseGlobal().Cast<TResult>();
        }

        /// <summary>
        /// Assigns a result to the user interaction.
        /// </summary>
        /// <remarks>
        /// Handlers (be they for local or global interactions) call this method to assign a result to the interaction.
        /// </remarks>
        /// <param name="result">
        /// The interaction result.
        /// </param>
        public void SetResult(TResult result)
        {
            base.SetResult(result);
        }
    }

    /// <summary>
    /// An implementation of <see cref="UserInteraction{TResult}"/> that adds exception information.
    /// </summary>
    /// <typeparam name="TResult">
    /// The interaction result type.
    /// </typeparam>
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