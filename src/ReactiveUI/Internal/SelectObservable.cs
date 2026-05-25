// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Projects each value of a source through a selector (the no-allocation fused replacement for Rx <c>Select</c>),
/// forwarding selector exceptions as <c>OnError</c>. Shared across the binding and routing internals.
/// </summary>
/// <typeparam name="TIn">The source element type.</typeparam>
/// <typeparam name="TOut">The projected element type.</typeparam>
/// <param name="source">The source observable.</param>
/// <param name="selector">Projects a source value into a result.</param>
internal sealed class SelectObservable<TIn, TOut>(IObservable<TIn> source, Func<TIn, TOut> selector) : IObservable<TOut>
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
