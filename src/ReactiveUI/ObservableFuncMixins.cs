// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>
/// Provides extension methods for creating observables from expression-based property accessors on view model
/// instances.
/// </summary>
/// <remarks>These extension methods enable reactive observation of property changes by converting expression
/// trees into observable sequences. This is useful for scenarios where you want to monitor changes to properties in
/// view models and react to those changes in a composable, declarative manner. The methods in this class rely on
/// reflection and may be affected by trimming in certain deployment scenarios.</remarks>
public static class ObservableFuncMixins
{
    /// <summary>
    /// Converts a property expression to an observable sequence using default options.
    /// </summary>
    /// <typeparam name="TSource">The type of the view model.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <param name="source">The view model.</param>
    /// <returns>An observable sequence of property values.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public static IObservable<TResult?> ToObservable<TSource, TResult>(
        this Expression<Func<TSource, TResult?>> expression,
        TSource? source) =>
        expression.ToObservable(source, false, false);

    /// <summary>
    /// Converts a property expression to an observable sequence, optionally observing values before change.
    /// </summary>
    /// <typeparam name="TSource">The type of the view model.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <param name="source">The view model.</param>
    /// <param name="beforeChange">If true, emits the value before the property changes rather than after.</param>
    /// <returns>An observable sequence of property values.</returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public static IObservable<TResult?> ToObservable<TSource, TResult>(
        this Expression<Func<TSource, TResult?>> expression,
        TSource? source,
        bool beforeChange) =>
        expression.ToObservable(source, beforeChange, false);

    /// <summary>
    /// Converts to observable.
    /// </summary>
    /// <typeparam name="TSource">The type of the view model.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <param name="source">The view model.</param>
    /// <param name="beforeChange">If true, emits the value before the property changes rather than after.</param>
    /// <param name="skipInitial">If true, skips emitting the initial value when subscribing.</param>
    /// <returns>
    /// An observable Result.
    /// </returns>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public static IObservable<TResult?> ToObservable<TSource, TResult>(
        this Expression<Func<TSource, TResult?>> expression,
        TSource? source,
        bool beforeChange,
        bool skipInitial)
    {
        ArgumentExceptionHelper.ThrowIfNull(expression);

        var sParam = Reflection.Rewrite(expression.Body);
        var changes = source.SubscribeToExpressionChain<TSource, TResult?>(
            sParam,
            beforeChange,
            skipInitial,
            RxSchedulers.SuppressViewCommandBindingMessage);
        return new ProjectedRetryObservable<TSource, TResult>(changes);
    }

    /// <summary>
    /// A fused sink that projects each observed change to its current value and retries (resubscribes to the source) on
    /// any error — replacing the <c>Select(x =&gt; x.GetValue()).Retry()</c> tail of
    /// <see cref="ToObservable{TSource, TResult}(Expression{Func{TSource, TResult}}, TSource, bool, bool)"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the observed object.</typeparam>
    /// <typeparam name="TResult">The projected value type.</typeparam>
    /// <param name="source">The source stream of observed changes.</param>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    private sealed class ProjectedRetryObservable<TSource, TResult>(
        IObservable<IObservedChange<TSource, TResult?>> source) : IObservable<TResult?>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TResult?> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            var sink = new Sink(source, observer);
            sink.Run();
            return sink;
        }

        /// <summary>Projects each change via <c>GetValue</c>; on any error, resubscribes to the source (infinite retry).</summary>
        [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
        private sealed class Sink(IObservable<IObservedChange<TSource, TResult?>> source, IObserver<TResult?> downstream)
            : IObserver<IObservedChange<TSource, TResult?>>, IDisposable
        {
            /// <summary>The current source subscription; reassigned on each retry.</summary>
            private readonly SwapDisposable _subscription = new();

            /// <summary>Whether this sink has been disposed; stops further retries.</summary>
            private int _disposed;

            /// <summary>Begins observing the source.</summary>
            public void Run() => _subscription.Disposable = source.Subscribe(this);

            /// <inheritdoc/>
            public void OnNext(IObservedChange<TSource, TResult?> value)
            {
                TResult? projected;
                try
                {
                    projected = value.GetValue();
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    return;
                }

                downstream.OnNext(projected);
            }

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                if (Volatile.Read(ref _disposed) != 0)
                {
                    return;
                }

                // Retry semantics: resubscribe to the source instead of forwarding the error.
                _subscription.Disposable = source.Subscribe(this);
            }

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();

            /// <inheritdoc/>
            public void Dispose()
            {
                Interlocked.Exchange(ref _disposed, 1);
                _subscription.Dispose();
            }
        }
    }
}
