// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;
using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>
/// Extension methods for subscribing to observables that emit other observables,
/// automatically switching to new inner observables when the source emits.
/// </summary>
/// <remarks>
/// <para>
/// These methods are particularly useful when working with reactive properties that
/// can be replaced, such as command properties. They ensure subscriptions follow
/// the property value changes instead of remaining attached to the old instance.
/// </para>
/// <para>
/// Example: If you have a ViewModel property <c>Command</c> that can be replaced with
/// a new ReactiveCommand instance, using SwitchSubscribe ensures your
/// subscription follows the new command rather than staying attached to the old one.
/// </para>
/// </remarks>
public static class SwitchSubscribeMixin
{
    /// <summary>
    /// Subscribes to the inner observables emitted by the source, automatically switching
    /// to new inner observables when the source emits a new value.
    /// </summary>
    /// <typeparam name="T">The type of values emitted by the inner observables.</typeparam>
    /// <param name="source">An observable that emits other observables.</param>
    /// <param name="onNext">Action to invoke for each element in the inner observable sequences.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="onNext"/> is null.</exception>
    /// <example>
    /// <code>
    /// // Subscribe to values from an observable property that can change
    /// this.WhenAnyValue(x => x.SomeObservableProperty)
    ///     .SwitchSubscribe(value => Console.WriteLine($"Value: {value}"));
    /// </code>
    /// </example>
    public static IDisposable SwitchSubscribe<T>(
        this IObservable<IObservable<T>?> source,
        Action<T> onNext)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(onNext);

        return new SwitchSelectObservable<IObservable<T>, T>(source, static x => x)
            .Subscribe(new DelegateObserver<T>(onNext));
    }

    /// <summary>
    /// Subscribes to the inner observables emitted by the source with error and completion handlers,
    /// automatically switching to new inner observables when the source emits a new value.
    /// </summary>
    /// <typeparam name="T">The type of values emitted by the inner observables.</typeparam>
    /// <param name="source">An observable that emits other observables.</param>
    /// <param name="onNext">Action to invoke for each element in the inner observable sequences.</param>
    /// <param name="onError">Action to invoke upon exceptional termination.</param>
    /// <param name="onCompleted">Action to invoke upon graceful termination.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public static IDisposable SwitchSubscribe<T>(
        this IObservable<IObservable<T>?> source,
        Action<T> onNext,
        Action<Exception> onError,
        Action onCompleted)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(onNext);
        ArgumentExceptionHelper.ThrowIfNull(onError);
        ArgumentExceptionHelper.ThrowIfNull(onCompleted);

        return new SwitchSelectObservable<IObservable<T>, T>(source, static x => x)
            .Subscribe(new DelegateObserver<T>(onNext, onError, onCompleted));
    }

    /// <summary>
    /// Projects each inner observable emitted by the source using the specified selector,
    /// then switches to the projected observable and subscribes with the provided action.
    /// </summary>
    /// <typeparam name="TSource">The type of the source inner observables.</typeparam>
    /// <typeparam name="TResult">The type of values in the projected observables.</typeparam>
    /// <param name="source">An observable that emits other observables.</param>
    /// <param name="selector">A transform function to apply to each inner observable.</param>
    /// <param name="onNext">Action to invoke for each element in the projected observable sequences.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <example>
    /// <code>
    /// // Subscribe to IsExecuting from a command property that can change
    /// this.WhenAnyValue(x => x.Command)
    ///     .SwitchSubscribe(
    ///         cmd => cmd.IsExecuting,
    ///         isExecuting => IsBusy = isExecuting
    ///     );
    /// </code>
    /// </example>
    public static IDisposable SwitchSubscribe<TSource, TResult>(
        this IObservable<TSource?> source,
        Func<TSource, IObservable<TResult>> selector,
        Action<TResult> onNext)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(selector);
        ArgumentExceptionHelper.ThrowIfNull(onNext);

        return new SwitchSelectObservable<TSource, TResult>(source, selector)
            .Subscribe(new DelegateObserver<TResult>(onNext));
    }

    /// <summary>
    /// Projects each inner observable emitted by the source using the specified selector,
    /// then switches to the projected observable and subscribes with the provided handlers.
    /// </summary>
    /// <typeparam name="TSource">The type of the source inner observables.</typeparam>
    /// <typeparam name="TResult">The type of values in the projected observables.</typeparam>
    /// <param name="source">An observable that emits other observables.</param>
    /// <param name="selector">A transform function to apply to each inner observable.</param>
    /// <param name="onNext">Action to invoke for each element in the projected observable sequences.</param>
    /// <param name="onError">Action to invoke upon exceptional termination.</param>
    /// <param name="onCompleted">Action to invoke upon graceful termination.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public static IDisposable SwitchSubscribe<TSource, TResult>(
        this IObservable<TSource?> source,
        Func<TSource, IObservable<TResult>> selector,
        Action<TResult> onNext,
        Action<Exception> onError,
        Action onCompleted)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(selector);
        ArgumentExceptionHelper.ThrowIfNull(onNext);
        ArgumentExceptionHelper.ThrowIfNull(onError);
        ArgumentExceptionHelper.ThrowIfNull(onCompleted);

        return new SwitchSelectObservable<TSource, TResult>(source, selector)
            .Subscribe(new DelegateObserver<TResult>(onNext, onError, onCompleted));
    }

    /// <summary>
    /// Subscribes to command execution results from a command property,
    /// automatically switching to new command instances when the property changes.
    /// </summary>
    /// <typeparam name="TParam">The command parameter type.</typeparam>
    /// <typeparam name="TResult">The command result type.</typeparam>
    /// <param name="source">An observable that emits ReactiveCommand instances.</param>
    /// <param name="onNext">Action to invoke for each command execution result.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="onNext"/> is null.</exception>
    /// <example>
    /// <code>
    /// // Subscribe to command results, following command property changes
    /// this.WhenAnyValue(x => x.SaveCommand)
    ///     .SwitchSubscribe(result => Console.WriteLine($"Saved: {result}"));
    /// </code>
    /// </example>
    public static IDisposable SwitchSubscribe<TParam, TResult>(
        this IObservable<IReactiveCommand<TParam, TResult>?> source,
        Action<TResult> onNext)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(onNext);

        return new SwitchSelectObservable<IReactiveCommand<TParam, TResult>, TResult>(source, static cmd => cmd)
            .Subscribe(new DelegateObserver<TResult>(onNext));
    }

    /// <summary>
    /// Subscribes to command execution results from a command property with error and completion handlers,
    /// automatically switching to new command instances when the property changes.
    /// </summary>
    /// <typeparam name="TParam">The command parameter type.</typeparam>
    /// <typeparam name="TResult">The command result type.</typeparam>
    /// <param name="source">An observable that emits ReactiveCommand instances.</param>
    /// <param name="onNext">Action to invoke for each command execution result.</param>
    /// <param name="onError">Action to invoke upon exceptional termination.</param>
    /// <param name="onCompleted">Action to invoke upon graceful termination.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public static IDisposable SwitchSubscribe<TParam, TResult>(
        this IObservable<IReactiveCommand<TParam, TResult>?> source,
        Action<TResult> onNext,
        Action<Exception> onError,
        Action onCompleted)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(onNext);
        ArgumentExceptionHelper.ThrowIfNull(onError);
        ArgumentExceptionHelper.ThrowIfNull(onCompleted);

        return new SwitchSelectObservable<IReactiveCommand<TParam, TResult>, TResult>(source, static cmd => cmd)
            .Subscribe(new DelegateObserver<TResult>(onNext, onError, onCompleted));
    }

    /// <summary>
    /// Projects a command property to one of its observables and subscribes with the provided action,
    /// automatically switching when the command property changes.
    /// </summary>
    /// <typeparam name="TParam">The command parameter type.</typeparam>
    /// <typeparam name="TResult">The command result type.</typeparam>
    /// <typeparam name="TValue">The type of values emitted by the selected observable.</typeparam>
    /// <param name="source">An observable that emits ReactiveCommand instances.</param>
    /// <param name="selector">A function to select an observable from the command.</param>
    /// <param name="onNext">Action to invoke for each value from the selected observable.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <example>
    /// <code>
    /// // Subscribe to IsExecuting, following command property changes
    /// this.WhenAnyValue(x => x.LoadCommand)
    ///     .SwitchSubscribe(
    ///         cmd => cmd.IsExecuting,
    ///         isExecuting => IsLoading = isExecuting
    ///     );
    /// </code>
    /// </example>
    public static IDisposable SwitchSubscribe<TParam, TResult, TValue>(
        this IObservable<IReactiveCommand<TParam, TResult>?> source,
        Func<IReactiveCommand<TParam, TResult>, IObservable<TValue>> selector,
        Action<TValue> onNext)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(selector);
        ArgumentExceptionHelper.ThrowIfNull(onNext);

        return new SwitchSelectObservable<IReactiveCommand<TParam, TResult>, TValue>(source, selector)
            .Subscribe(new DelegateObserver<TValue>(onNext));
    }

    /// <summary>
    /// Projects a command property to one of its observables and subscribes with the provided handlers,
    /// automatically switching when the command property changes.
    /// </summary>
    /// <typeparam name="TParam">The command parameter type.</typeparam>
    /// <typeparam name="TResult">The command result type.</typeparam>
    /// <typeparam name="TValue">The type of values emitted by the selected observable.</typeparam>
    /// <param name="source">An observable that emits ReactiveCommand instances.</param>
    /// <param name="selector">A function to select an observable from the command.</param>
    /// <param name="onNext">Action to invoke for each value from the selected observable.</param>
    /// <param name="onError">Action to invoke upon exceptional termination.</param>
    /// <param name="onCompleted">Action to invoke upon graceful termination.</param>
    /// <returns>A disposable that stops the subscription when disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public static IDisposable SwitchSubscribe<TParam, TResult, TValue>(
        this IObservable<IReactiveCommand<TParam, TResult>?> source,
        Func<IReactiveCommand<TParam, TResult>, IObservable<TValue>> selector,
        Action<TValue> onNext,
        Action<Exception> onError,
        Action onCompleted)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(selector);
        ArgumentExceptionHelper.ThrowIfNull(onNext);
        ArgumentExceptionHelper.ThrowIfNull(onError);
        ArgumentExceptionHelper.ThrowIfNull(onCompleted);

        return new SwitchSelectObservable<IReactiveCommand<TParam, TResult>, TValue>(source, selector)
            .Subscribe(new DelegateObserver<TValue>(onNext, onError, onCompleted));
    }

    /// <summary>
    /// Projects each inner observable emitted by the source using the specified selector,
    /// then switches to the projected observable.
    /// </summary>
    /// <typeparam name="TSource">The type of the source inner observables.</typeparam>
    /// <typeparam name="TResult">The type of values in the projected observables.</typeparam>
    /// <param name="source">An observable that emits other observables.</param>
    /// <param name="selector">A transform function to apply to each inner observable.</param>
    /// <returns>An observable sequence whose elements are the result of invoking the transform function on each inner observable and switching to it.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    /// <example>
    /// <code>
    /// // Subscribe to IsExecuting from a command property that can change
    /// this.WhenAnyValue(x => x.Command)
    ///     .SwitchSubscribe(
    ///         cmd => cmd.IsExecuting,
    ///         isExecuting => IsBusy = isExecuting
    ///     );
    ///
    /// // Or use with ToProperty
    /// _isBusy = this.WhenAnyValue(x => x.Command)
    ///     .SwitchSelect(cmd => cmd.IsExecuting)
    ///     .ToProperty(this, x => x.IsBusy);
    /// </code>
    /// </example>
    public static IObservable<TResult> SwitchSelect<TSource, TResult>(
        this IObservable<TSource?> source,
        Func<TSource, IObservable<TResult>> selector)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(selector);

        return new SwitchSelectObservable<TSource, TResult>(source, selector);
    }

    /// <summary>
    /// Projects a command property to one of its observables (e.g., IsExecuting, CanExecute),
    /// automatically switching when the command property changes.
    /// </summary>
    /// <typeparam name="TParam">The command parameter type.</typeparam>
    /// <typeparam name="TResult">The command result type.</typeparam>
    /// <typeparam name="TValue">The type of values emitted by the selected observable.</typeparam>
    /// <param name="source">An observable that emits ReactiveCommand instances.</param>
    /// <param name="selector">A function to select an observable from the command (e.g., <c>cmd => cmd.IsExecuting</c>).</param>
    /// <returns>An observable sequence that switches to the selected observable whenever the command changes.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    /// <example>
    /// <code>
    /// // Use with ToProperty to track IsExecuting from a replaceable command
    /// _isBusy = this.WhenAnyValue(x => x.SaveCommand)
    ///     .SwitchSelect(cmd => cmd.IsExecuting)
    ///     .ToProperty(this, x => x.IsBusy);
    ///
    /// // Or subscribe directly
    /// this.WhenAnyValue(x => x.DeleteCommand)
    ///     .SwitchSubscribe(
    ///         cmd => cmd.CanExecute,
    ///         canExecute => DeleteButtonEnabled = canExecute
    ///     );
    /// </code>
    /// </example>
    public static IObservable<TValue> SwitchSelect<TParam, TResult, TValue>(
        this IObservable<IReactiveCommand<TParam, TResult>?> source,
        Func<IReactiveCommand<TParam, TResult>, IObservable<TValue>> selector)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(selector);

        return new SwitchSelectObservable<IReactiveCommand<TParam, TResult>, TValue>(source, selector);
    }

    /// <summary>
    /// A fused sink that filters out null source values, projects each remaining value to an inner observable via
    /// <paramref name="selector"/>, and switches to the latest inner observable — replacing the
    /// <c>WhereNotNull().Select(selector).Switch()</c> chain with a single allocation-tuned operator.
    /// </summary>
    /// <typeparam name="TSource">The (nullable) source element type.</typeparam>
    /// <typeparam name="TResult">The element type of the projected inner observables.</typeparam>
    /// <param name="source">The source observable.</param>
    /// <param name="selector">Projects each non-null source value to an inner observable.</param>
    private sealed class SwitchSelectObservable<TSource, TResult>(
        IObservable<TSource?> source,
        Func<TSource, IObservable<TResult>> selector) : IObservable<TResult>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TResult> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            var sink = new Sink(selector, observer);
            sink.Run(source);
            return sink;
        }

        /// <summary>Subscribes to the source, switching the active inner subscription on each non-null value.</summary>
        private sealed class Sink(Func<TSource, IObservable<TResult>> selector, IObserver<TResult> downstream)
            : IObserver<TSource?>, IDisposable
        {
            /// <summary>Guards the switching state so outer and inner notifications stay consistent.</summary>
            #if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
            #else
            private readonly object _gate = new();
            #endif

            /// <summary>The outer (source) subscription.</summary>
            private readonly OnceDisposable _outer = new();

            /// <summary>The active inner subscription; assigning a new value disposes the previous one.</summary>
            private readonly SwapDisposable _inner = new();

            /// <summary>Generation id of the most recent inner observable; stale inner notifications are ignored.</summary>
            private ulong _latest;

            /// <summary>Whether an inner subscription is currently active.</summary>
            private bool _hasInner;

            /// <summary>Whether the outer source has completed.</summary>
            private bool _outerCompleted;

            /// <summary>Whether this sink has been disposed.</summary>
            private bool _disposed;

            /// <summary>Begins observing the source.</summary>
            /// <param name="source">The source observable.</param>
            public void Run(IObservable<TSource?> source) => _outer.Disposable = source.Subscribe(this);

            /// <inheritdoc/>
            public void OnNext(TSource? value)
            {
                if (value is null)
                {
                    return;
                }

                IObservable<TResult> inner;
                try
                {
                    inner = selector(value);
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    return;
                }

                ulong id;
                lock (_gate)
                {
                    if (_disposed)
                    {
                        return;
                    }

                    id = ++_latest;
                    _hasInner = true;
                }

                _inner.Disposable = inner.Subscribe(new InnerObserver(this, id));
            }

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                lock (_gate)
                {
                    if (_disposed)
                    {
                        return;
                    }
                }

                downstream.OnError(error);
                Dispose();
            }

            /// <inheritdoc/>
            public void OnCompleted()
            {
                bool complete;
                lock (_gate)
                {
                    if (_disposed)
                    {
                        return;
                    }

                    _outerCompleted = true;
                    complete = !_hasInner;
                }

                if (!complete)
                {
                    return;
                }

                downstream.OnCompleted();
                Dispose();
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                lock (_gate)
                {
                    if (_disposed)
                    {
                        return;
                    }

                    _disposed = true;
                }

                _outer.Dispose();
                _inner.Dispose();
            }

            /// <summary>Forwards an inner value to the downstream observer if it belongs to the active inner subscription.</summary>
            /// <param name="id">The generation id of the inner subscription that produced the value.</param>
            /// <param name="value">The value to forward.</param>
            private void InnerOnNext(ulong id, TResult value)
            {
                lock (_gate)
                {
                    if (_disposed || id != _latest)
                    {
                        return;
                    }
                }

                downstream.OnNext(value);
            }

            /// <summary>Forwards an inner error to the downstream observer if it belongs to the active inner subscription.</summary>
            /// <param name="id">The generation id of the inner subscription that errored.</param>
            /// <param name="error">The error to forward.</param>
            private void InnerOnError(ulong id, Exception error)
            {
                lock (_gate)
                {
                    if (_disposed || id != _latest)
                    {
                        return;
                    }
                }

                downstream.OnError(error);
                Dispose();
            }

            /// <summary>Clears the active inner subscription; completes downstream only if the outer has also completed.</summary>
            /// <param name="id">The generation id of the inner subscription that completed.</param>
            private void InnerOnCompleted(ulong id)
            {
                bool complete;
                lock (_gate)
                {
                    if (_disposed || id != _latest)
                    {
                        return;
                    }

                    _hasInner = false;
                    complete = _outerCompleted;
                }

                if (!complete)
                {
                    return;
                }

                downstream.OnCompleted();
                Dispose();
            }

            /// <summary>Forwards a single inner subscription's notifications, tagged with its generation id.</summary>
            private sealed class InnerObserver(Sink parent, ulong id) : IObserver<TResult>
            {
                /// <inheritdoc/>
                public void OnNext(TResult value) => parent.InnerOnNext(id, value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.InnerOnError(id, error);

                /// <inheritdoc/>
                public void OnCompleted() => parent.InnerOnCompleted(id);
            }
        }
    }
}
