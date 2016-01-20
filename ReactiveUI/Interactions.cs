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
    public abstract class Interaction
    {
        private readonly AsyncSubject<object> result;
        private int resultSet;

        protected Interaction()
        {
            this.result = new AsyncSubject<object>();
        }

        public bool IsHandled
        {
            get { return this.resultSet == 1; }
        }

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

        public static InteractionSource CreateSource()
        {
            return new InteractionSource();
        }
    }

    public class Interaction<TResult> : Interaction
    {
        public void SetResult(TResult result)
        {
            base.SetResult(result);
        }

        internal new TResult GetResult()
        {
            return (TResult)base.GetResult();
        }

        public static new InteractionSource<Interaction<TResult>, TResult> CreateSource()
        {
            return new InteractionSource<Interaction<TResult>, TResult>();
        }
    }

    // an interaction source that doesn't care about the type of the interactions
    public class InteractionSource
    {
        // convenient broker that any component can hook into
        public static readonly InteractionSource Global = Interaction.CreateSource();

        private readonly IList<Func<Interaction, IObservable<Unit>>> handlers;

        public InteractionSource()
        {
            this.handlers = new List<Func<Interaction, IObservable<Unit>>>();
        }

        // normally only useful from unit tests
        public IDisposable RegisterHandler<TInteraction>(Action<TInteraction> handler)
            where TInteraction : Interaction
        {
            return RegisterHandler<TInteraction>(interaction => {
                handler(interaction);
                return Observable.Return(Unit.Default);
            });
        }

        public IDisposable RegisterHandler<TInteraction>(Func<TInteraction, Task> handler)
            where TInteraction : Interaction
        {
            return RegisterHandler<TInteraction>(interaction => handler(interaction).ToObservable());
        }

        public IDisposable RegisterHandler<TInteraction>(Func<TInteraction, IObservable<Unit>> handler)
            where TInteraction : Interaction
        {
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

        public IObservable<TResult> Raise<TResult>(Interaction<TResult> interaction)
        {
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

    // an interaction source that cares about the interaction type. NOTE: does NOT extend InteractionSource on purpose
    public class InteractionSource<TInteraction, TResult>
        where TInteraction : Interaction<TResult>
    {
        private readonly IList<Func<TInteraction, IObservable<Unit>>> handlers;

        public InteractionSource()
        {
            this.handlers = new List<Func<TInteraction, IObservable<Unit>>>();
        }

        // normally only useful from unit tests
        public IDisposable RegisterHandler(Action<TInteraction> handler)
        {
            return RegisterHandler(interaction => {
                handler(interaction);
                return Observable.Return(Unit.Default);
            });
        }

        public IDisposable RegisterHandler(Func<TInteraction, Task> handler)
        {
            return RegisterHandler(interaction => handler(interaction).ToObservable());
        }

        public IDisposable RegisterHandler(Func<TInteraction, IObservable<Unit>> handler)
        {
            handlers.Add(handler);
            return Disposable.Create(() => handlers.Remove(handler));
        }

        public IObservable<TResult> Raise(TInteraction interaction)
        {
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

    public class UnhandledInteractionException : Exception
    {
        private readonly Interaction interaction;

        public UnhandledInteractionException(Interaction interaction)
        {
            this.interaction = interaction;
        }

        public Interaction Interaction
        {
            get { return this.interaction; }
        }
    }



    /*
    /// <summary>
    /// Represents an interaction between a view model and some external component, often the user.
    /// </summary>
    /// <remarks>
    /// <para>
    /// interactions are used when view models need an answer to some question before they can continue their work.
    /// For example, prior to deleting a file the user may be required to acquiesce. Naturally, this question needs to be
    /// answered in an asynchronous fashion, so that no blocking occurs while the view model waits for an answer.
    /// </para>
    /// <para>
    /// Interactions may be either local or global. Local interactions are typically exposed as properties on a view model.
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
    public abstract class Interaction : ReactiveObject
    {
        private static readonly IList<Func<Interaction, IObservable<Unit>>> handlers = new List<Func<Interaction, IObservable<Unit>>>();
        private readonly AsyncSubject<object> result;
        private int resultSet;

        protected Interaction()
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
            where TInteraction : Interaction
        {
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
            where TInteraction : Interaction
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
            where TInteraction : Interaction
        {
            return RegisterGlobalHandler<TInteraction>(interaction => {
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
        /// The result is untyped. Strong typing is introduced by the <see cref="Interaction{TResult}"/> class.
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
        /// The result is untyped. Strong typing is introduced by the <see cref="Interaction{TResult}"/> class.
        /// </remarks>
        /// <returns>
        /// The result obtained from global interaction handlers.
        /// </returns>
        protected IObservable<object> RaiseGlobal()
        {
            return Observable
                .StartAsync(
                    (async () => {
                        var handlers = Enumerable.Reverse(Interaction.handlers).ToArray();

                        foreach (var handler in handlers) {
                            await handler(this);

                            if (this.IsHandled) {
                                return this.result.GetResult();
                            }
                        }

                        throw new UnhandledInteractionException(this);
                    }));
        }

        /// <summary>
        /// Sets the result of the interaction.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The result is untyped. Strong typing is introduced by the <see cref="Interaction{TResult}"/> class.
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
            if (Interlocked.CompareExchange(ref this.resultSet, 1, 0) != 0) {
                throw new InvalidOperationException("Result has already been set.");
            }

            this.result.OnNext(result);
            this.result.OnCompleted();
        }
    }

    /// <summary>
    /// Implements a <see cref="Interaction"/> with a specific result type.
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
    public class Interaction<TResult> : Interaction
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
    /// An implementation of <see cref="Interaction{TResult}"/> that adds exception information.
    /// </summary>
    /// <typeparam name="TResult">
    /// The interaction result type.
    /// </typeparam>
    public class ErrorInteraction<TResult> : Interaction<TResult>
    {
        private readonly Exception error;

        public ErrorInteraction(Exception error)
        {
            this.error = error;
        }

        public Exception Error
        {
            get { return this.error; }
        }
    }

    public class UnhandledInteractionException : Exception
    {
        private readonly Interaction userInteraction;

        public UnhandledInteractionException(Interaction userInteraction)
        {
            this.userInteraction = userInteraction;
        }

        public Interaction UserInteraction
        {
            get { return this.userInteraction; }
        }
    }
    */
}