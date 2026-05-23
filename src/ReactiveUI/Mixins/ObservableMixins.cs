// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;

using ReactiveUI.Builder;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>
/// Provides extension methods and utilities for working with observable sequences, including helpers for filtering null
/// values and converting asynchronous actions to observables.
/// </summary>
/// <remarks>This static class offers mixin methods to enhance the usability of IObservable{T} sequences,
/// particularly in scenarios involving nullability and asynchronous operations. All members are thread-safe and
/// intended for use with reactive programming patterns.</remarks>
public static class ObservableMixins
{
    /// <summary>
    /// Initializes static members of the <see cref="ObservableMixins"/> class.
    /// </summary>
    static ObservableMixins() => RxAppBuilder.EnsureInitialized();

    /// <summary>
    /// Returns only values that are not null.
    /// Converts the nullability.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the observable.</typeparam>
    /// <param name="observable">The observable that can contain nulls.</param>
    /// <returns>A non nullable version of the observable that only emits valid values.</returns>
    public static IObservable<T> WhereNotNull<T>(this IObservable<T?> observable) =>
        new WhereNotNullObservable<T>(observable);

    /// <summary>
    /// Converts an asynchronous action into an observable sequence. Each subscription
    ///     to the resulting sequence causes the action to be started. The CancellationToken
    ///     passed to the asynchronous action is tied to the observable sequence's subscription
    ///     that triggered the action's invocation and can be used for best-effort cancellation.
    /// </summary>
    /// <param name="actionAsync">Asynchronous action to convert.</param>
    /// <returns>An observable sequence exposing a Unit value upon completion of the action, or an exception.</returns>
    internal static IObservable<(IObservable<Unit> Result, Action Cancel)> FromAsyncWithAllNotifications(
        Func<CancellationToken, Task> actionAsync) =>
        new DeferredValueObservable<(IObservable<Unit> Result, Action Cancel)>(() =>
        {
            var cts = new CancellationTokenSource();
            var result = new FromAsyncObservable<Unit>(
                async ct =>
                {
                    await actionAsync(ct).ConfigureAwait(false);
                    return Unit.Default;
                },
                cts);
            return (result, () => cts.Cancel());
        });

    /// <summary>
    /// Converts an asynchronous action into an observable sequence. Each subscription
    ///     to the resulting sequence causes the action to be started. The CancellationToken
    ///     passed to the asynchronous action is tied to the observable sequence's subscription
    ///     that triggered the action's invocation and can be used for best-effort cancellation.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <param name="actionAsync">Asynchronous action to convert.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>An observable sequence exposing a Unit value upon completion of the action, or an exception.</returns>
    internal static IObservable<(IObservable<Unit> Result, Action Cancel)> FromAsyncWithAllNotifications<TParam>(
        Func<TParam, CancellationToken, Task> actionAsync,
        TParam param) =>
        new DeferredValueObservable<(IObservable<Unit> Result, Action Cancel)>(() =>
        {
            var cts = new CancellationTokenSource();
            var result = new FromAsyncObservable<Unit>(
                async ct =>
                {
                    await actionAsync(param, ct).ConfigureAwait(false);
                    return Unit.Default;
                },
                cts);
            return (result, () => cts.Cancel());
        });

    /// <summary>
    /// Converts an asynchronous action into an observable sequence. Each subscription
    ///     to the resulting sequence causes the action to be started. The CancellationToken
    ///     passed to the asynchronous action is tied to the observable sequence's subscription
    ///     that triggered the action's invocation and can be used for best-effort cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="actionAsync">Asynchronous action to convert.</param>
    /// <returns>An observable sequence exposing a Unit value upon completion of the action, or an exception.</returns>
    internal static IObservable<(IObservable<TResult> Result, Action Cancel)> FromAsyncWithAllNotifications<TResult>(
        Func<CancellationToken, Task<TResult>> actionAsync) =>
        new DeferredValueObservable<(IObservable<TResult> Result, Action Cancel)>(() =>
        {
            var cts = new CancellationTokenSource();
            var result = new FromAsyncObservable<TResult>(actionAsync, cts);
            return (result, () => cts.Cancel());
        });

    /// <summary>
    /// Converts an asynchronous action into an observable sequence. Each subscription
    ///     to the resulting sequence causes the action to be started. The CancellationToken
    ///     passed to the asynchronous action is tied to the observable sequence's subscription
    ///     that triggered the action's invocation and can be used for best-effort cancellation.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="actionAsync">Asynchronous action to convert.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>An observable sequence exposing a Unit value upon completion of the action, or an exception.</returns>
    internal static IObservable<(IObservable<TResult> Result, Action Cancel)> FromAsyncWithAllNotifications<
        TParam,
        TResult>(
        Func<TParam, CancellationToken, Task<TResult>> actionAsync,
        TParam param) =>
        new DeferredValueObservable<(IObservable<TResult> Result, Action Cancel)>(() =>
        {
            var cts = new CancellationTokenSource();
            var result = new FromAsyncObservable<TResult>(ct => actionAsync(param, ct), cts);
            return (result, () => cts.Cancel());
        });

    /// <summary>Forwards only the non-null values of a source, projected to the non-nullable type. Replaces <c>Where(x is not null).Select(x!)</c>.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source observable.</param>
    private sealed class WhereNotNullObservable<T>(IObservable<T?> source) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer));
        }

        /// <summary>Forwards each non-null value as its non-nullable type.</summary>
        /// <param name="downstream">The observer receiving non-null values.</param>
        private sealed class Sink(IObserver<T> downstream) : IObserver<T?>
        {
            /// <inheritdoc/>
            public void OnNext(T? value)
            {
                if (value is null)
                {
                    return;
                }

                downstream.OnNext(value);
            }

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }

    /// <summary>
    /// Builds a value at subscription time and emits it once, then completes. Replaces <c>Observable.Defer(() =&gt; Observable.Return(...))</c>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="factory">Builds the value to emit at subscription time.</param>
    private sealed class DeferredValueObservable<T>(Func<T> factory) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            T value;
            try
            {
                value = factory();
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
                return EmptyDisposable.Instance;
            }

            observer.OnNext(value);
            observer.OnCompleted();
            return EmptyDisposable.Instance;
        }
    }

    /// <summary>
    /// Runs an asynchronous factory on subscription (with a cancellation token linked to the outer source and the
    /// subscription) and emits its single result. Replaces <c>Observable.FromAsync(...).Finally(...)</c>.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="factory">The asynchronous factory, invoked with a linked cancellation token.</param>
    /// <param name="outerCancellation">The outer cancellation source, cancelled by the caller and disposed when the run ends.</param>
    private sealed class FromAsyncObservable<T>(Func<CancellationToken, Task<T>> factory, CancellationTokenSource outerCancellation) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            var run = new Run(observer, factory, outerCancellation);
            run.Start();
            return run;
        }

        /// <summary>Drives a single asynchronous execution, forwarding its result or error exactly once.</summary>
        private sealed class Run : IDisposable
        {
            /// <summary>The observer receiving the result.</summary>
            private readonly IObserver<T> _observer;

            /// <summary>The asynchronous factory.</summary>
            private readonly Func<CancellationToken, Task<T>> _factory;

            /// <summary>The outer cancellation source (caller-cancellable), disposed when the run ends.</summary>
            private readonly CancellationTokenSource _outerCancellation;

            /// <summary>The subscription cancellation source, cancelled on dispose.</summary>
            private readonly CancellationTokenSource _subscriptionCancellation = new();

            /// <summary>Zero until the result or error has been forwarded.</summary>
            private int _emitted;

            /// <summary>Zero until the cancellation sources have been cleaned up.</summary>
            private int _disposed;

            /// <summary>The linked token source passed to the factory; disposed before its source sources to avoid ObjectDisposedException.</summary>
            private CancellationTokenSource? _linked;

            /// <summary>Initializes a new instance of the <see cref="Run"/> class.</summary>
            /// <param name="observer">The observer receiving the result.</param>
            /// <param name="factory">The asynchronous factory.</param>
            /// <param name="outerCancellation">The outer cancellation source.</param>
            public Run(IObserver<T> observer, Func<CancellationToken, Task<T>> factory, CancellationTokenSource outerCancellation)
            {
                _observer = observer;
                _factory = factory;
                _outerCancellation = outerCancellation;
            }

            /// <summary>Starts the asynchronous execution.</summary>
            public void Start() => _ = RunAsync();

            /// <inheritdoc/>
            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) != 0)
                {
                    return;
                }

                _subscriptionCancellation.Cancel();

                // Dispose the linked source BEFORE its source sources: a linked token source deregisters from its
                // sources on dispose, which throws ObjectDisposedException if the sources were disposed first.
                Volatile.Read(ref _linked)?.Dispose();
                _subscriptionCancellation.Dispose();
                _outerCancellation.Dispose();
            }

            /// <summary>Awaits the factory and forwards the result or error, cleaning up afterwards.</summary>
            /// <returns>A task that completes when the result has been forwarded.</returns>
            private async Task RunAsync()
            {
                var linked = CancellationTokenSource.CreateLinkedTokenSource(_outerCancellation.Token, _subscriptionCancellation.Token);
                Volatile.Write(ref _linked, linked);
                try
                {
                    var value = await _factory(linked.Token).ConfigureAwait(false);
                    if (Interlocked.Exchange(ref _emitted, 1) == 0)
                    {
                        _observer.OnNext(value);
                        _observer.OnCompleted();
                    }
                }
                catch (Exception ex)
                {
                    if (Interlocked.Exchange(ref _emitted, 1) == 0)
                    {
                        _observer.OnError(ex);
                    }
                }
                finally
                {
                    Dispose();
                }
            }
        }
    }
}
