// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Reactive.Disposables;

using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>
/// Provides methods to bind <see cref="Interaction{TInput, TOutput}"/>s to handlers.
/// </summary>
public class InteractionBinderImplementation : IInteractionBinderImplementation
{
    /// <inheritdoc />
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public IDisposable BindInteraction<TViewModel, TView, TInput, TOutput>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, IInteraction<TInput, TOutput>>> propertyName,
        Func<IInteractionContext<TInput, TOutput>, Task> handler)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(propertyName);
        ArgumentExceptionHelper.ThrowIfNull(handler);

        var vmExpression = Reflection.Rewrite(propertyName.Body);

        var vmNulls = new ChooseObservable<object?, IInteraction<TInput, TOutput>?>(
            view.WhenAnyValue(x => x.ViewModel),
            static x => x is null ? (true, (IInteraction<TInput, TOutput>?)null) : (false, null));
        var source = new MergeObservable<IInteraction<TInput, TOutput>?>(
        [
            new SelectObservable<object, IInteraction<TInput, TOutput>?>(
                Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression),
                static x => (IInteraction<TInput, TOutput>?)x),
            vmNulls,
        ]);

        var registration = new SwapDisposable();
        var subscription = source.Subscribe(new InteractionRegistrationObserver<TInput, TOutput>(
            registration,
            x => x is null ? EmptyDisposable.Instance : x.RegisterHandler(handler),
            this,
            $"{vmExpression} Interaction Binding received an Exception!"));
        return new CompositeDisposable(subscription, registration);
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public IDisposable BindInteraction<TViewModel, TView, TInput, TOutput, TDontCare>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, IInteraction<TInput, TOutput>>> propertyName,
        Func<IInteractionContext<TInput, TOutput>, IObservable<TDontCare>> handler)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(propertyName);
        ArgumentExceptionHelper.ThrowIfNull(handler);

        var vmExpression = Reflection.Rewrite(propertyName.Body);

        var vmNulls = new ChooseObservable<object?, IInteraction<TInput, TOutput>?>(
            view.WhenAnyValue(x => x.ViewModel),
            static x => x is null ? (true, (IInteraction<TInput, TOutput>?)null) : (false, null));
        var source = new MergeObservable<IInteraction<TInput, TOutput>?>(
        [
            new SelectObservable<object, IInteraction<TInput, TOutput>?>(
                Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression),
                static x => (IInteraction<TInput, TOutput>?)x),
            vmNulls,
        ]);

        var registration = new SwapDisposable();
        var subscription = source.Subscribe(new InteractionRegistrationObserver<TInput, TOutput>(
            registration,
            x => x is null ? EmptyDisposable.Instance : x.RegisterHandler(handler),
            this,
            $"{vmExpression} Interaction Binding received an Exception!"));
        return new CompositeDisposable(subscription, registration);
    }

    /// <summary>Projects each value of a source through a selector. Specialised interaction-binding projection.</summary>
    /// <typeparam name="TIn">The source element type.</typeparam>
    /// <typeparam name="TOut">The projected element type.</typeparam>
    /// <param name="source">The source observable.</param>
    /// <param name="selector">Projects a source value into a result.</param>
    private sealed class SelectObservable<TIn, TOut>(IObservable<TIn> source, Func<TIn, TOut> selector) : IObservable<TOut>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer, selector));
        }

        /// <summary>Applies the selector to each value and forwards the result.</summary>
        /// <param name="downstream">The observer receiving projected values.</param>
        /// <param name="selector">Projects a source value into a result.</param>
        private sealed class Sink(IObserver<TOut> downstream, Func<TIn, TOut> selector) : IObserver<TIn>
        {
            /// <inheritdoc/>
            public void OnNext(TIn value)
            {
                TOut result;
                try
                {
                    result = selector(value);
                }
                catch (Exception ex)
                {
                    downstream.OnError(ex);
                    return;
                }

                downstream.OnNext(result);
            }

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }

    /// <summary>Forwards only the values chosen by a chooser. Specialised interaction-binding filter-map.</summary>
    /// <typeparam name="TIn">The source element type.</typeparam>
    /// <typeparam name="TOut">The forwarded element type.</typeparam>
    /// <param name="source">The source observable.</param>
    /// <param name="chooser">Maps a source value to (forward, value); when forward is false the value is skipped.</param>
    private sealed class ChooseObservable<TIn, TOut>(IObservable<TIn> source, Func<TIn, (bool HasValue, TOut Value)> chooser) : IObservable<TOut>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer, chooser));
        }

        /// <summary>Applies the chooser to each value and forwards only the chosen ones.</summary>
        /// <param name="downstream">The observer receiving chosen values.</param>
        /// <param name="chooser">Maps a source value to (forward, value).</param>
        private sealed class Sink(IObserver<TOut> downstream, Func<TIn, (bool HasValue, TOut Value)> chooser) : IObserver<TIn>
        {
            /// <inheritdoc/>
            public void OnNext(TIn value)
            {
                (bool HasValue, TOut Value) result;
                try
                {
                    result = chooser(value);
                }
                catch (Exception ex)
                {
                    downstream.OnError(ex);
                    return;
                }

                if (!result.HasValue)
                {
                    return;
                }

                downstream.OnNext(result.Value);
            }

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }

    /// <summary>Forwards the values of every source and completes once all complete. Specialised interaction-binding merge.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="sources">The sources to merge.</param>
    private sealed class MergeObservable<T>(IObservable<T>[] sources) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Sink(observer, sources);
        }

        /// <summary>Forwards every source value under a gate and completes once all sources complete.</summary>
        private sealed class Sink : IDisposable
        {
            /// <summary>Guards downstream delivery and the completion counter.</summary>
            #if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
            #else
            private readonly object _gate = new();
            #endif

            /// <summary>The observer receiving the merged values.</summary>
            private readonly IObserver<T> _downstream;

            /// <summary>The subscriptions to each source.</summary>
            private readonly IDisposable?[] _subscriptions;

            /// <summary>The number of sources.</summary>
            private readonly int _count;

            /// <summary>The number of sources that have completed.</summary>
            private int _doneCount;

            /// <summary>Whether the downstream has terminated.</summary>
            private bool _stopped;

            /// <summary>Initializes a new instance of the <see cref="Sink"/> class and subscribes to every source.</summary>
            /// <param name="downstream">The observer receiving the merged values.</param>
            /// <param name="sources">The sources to merge.</param>
            public Sink(IObserver<T> downstream, IObservable<T>[] sources)
            {
                _downstream = downstream;
                _count = sources.Length;
                _subscriptions = new IDisposable?[sources.Length];
                for (var i = 0; i < sources.Length; i++)
                {
                    _subscriptions[i] = sources[i].Subscribe(new Element(this));
                }
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                for (var i = 0; i < _subscriptions.Length; i++)
                {
                    _subscriptions[i]?.Dispose();
                }
            }

            /// <summary>Forwards one source value to the downstream.</summary>
            /// <param name="value">The value to forward.</param>
            private void OnNextAt(T value)
            {
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _downstream.OnNext(value);
                }
            }

            /// <summary>Forwards an error from any source.</summary>
            /// <param name="error">The error to forward.</param>
            private void OnErrorAt(Exception error)
            {
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnError(error);
            }

            /// <summary>Completes the downstream once every source has completed.</summary>
            private void OnCompletedAt()
            {
                lock (_gate)
                {
                    if (_stopped || ++_doneCount < _count)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnCompleted();
            }

            /// <summary>Routes one source's notifications to the parent sink.</summary>
            /// <param name="parent">The owning sink.</param>
            private sealed class Element(Sink parent) : IObserver<T>
            {
                /// <inheritdoc/>
                public void OnNext(T value) => parent.OnNextAt(value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.OnErrorAt(error);

                /// <inheritdoc/>
                public void OnCompleted() => parent.OnCompletedAt();
            }
        }
    }

    /// <summary>
    /// Registers the latest interaction's handler (swapping out the previous registration), logs binding errors, and
    /// disposes the registration when the source terminates. Fuses the prior <c>Do</c> + <c>Finally</c> + <c>Subscribe</c>.
    /// </summary>
    /// <typeparam name="TInput">The interaction input type.</typeparam>
    /// <typeparam name="TOutput">The interaction output type.</typeparam>
    /// <param name="registration">Holds the current handler registration, disposing the previous on assignment.</param>
    /// <param name="register">Registers a handler for the supplied interaction (or a no-op for null).</param>
    /// <param name="logHost">The object used for logging.</param>
    /// <param name="errorMessage">Logged when the binding errors.</param>
    private sealed class InteractionRegistrationObserver<TInput, TOutput>(
        SwapDisposable registration,
        Func<IInteraction<TInput, TOutput>?, IDisposable> register,
        IEnableLogger logHost,
        string errorMessage) : IObserver<IInteraction<TInput, TOutput>?>
    {
        /// <inheritdoc/>
        public void OnNext(IInteraction<TInput, TOutput>? value) => registration.Disposable = register(value);

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
            logHost.Log().Error(error, errorMessage);
            registration.Dispose();
        }

        /// <inheritdoc/>
        public void OnCompleted() => registration.Dispose();
    }
}
