// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// Projects a single observed property's value through a selector. The fused
/// arity-1 form of the WhenAnyValue combine sink: no gate is needed for one source.
/// </summary>
/// <typeparam name="TSender">The type of the object whose properties are observed.</typeparam>
/// <typeparam name="T1">The value type of observed property 1.</typeparam>
/// <typeparam name="TResult">The type produced by the selector.</typeparam>
/// <param name="source">The source observable.</param>
/// <param name="selector">Projects the source element into the result.</param>
internal sealed class WhenAnyValueSink<TSender, T1, TResult>(
    IObservable<IObservedChange<TSender, T1>> source,
    Func<T1, TResult> selector) : IObservable<TResult>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<TResult> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        return source.Subscribe(new Sink(observer, selector));
    }

    /// <summary>
    /// Projects each source notification through the selector and forwards the result.
    /// </summary>
    /// <param name="downstream">The observer receiving the projected results.</param>
    /// <param name="selector">Projects the source element into the result.</param>
    private sealed class Sink(IObserver<TResult> downstream, Func<T1, TResult> selector) : IObserver<IObservedChange<TSender, T1>>
    {
        /// <inheritdoc/>
        public void OnNext(IObservedChange<TSender, T1> change)
        {
            TResult result;
            try
            {
                result = selector(change.Value);
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
