// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// Projects the value of each observed change through a selector. Backs the <c>ObservableForProperty</c> overloads
/// that take a value selector, replacing a <c>.Select(x =&gt; selector(x.Value))</c> hop.
/// </summary>
/// <typeparam name="TSender">The observed-change sender type.</typeparam>
/// <typeparam name="TValue">The observed-change value type.</typeparam>
/// <typeparam name="TRet">The projected result type.</typeparam>
/// <param name="source">The source observed-change stream.</param>
/// <param name="selector">Projects an observed change's value into the result.</param>
internal sealed class ObservedChangeValueSelector<TSender, TValue, TRet>(
    IObservable<IObservedChange<TSender, TValue>> source,
    Func<TValue?, TRet> selector) : IObservable<TRet>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<TRet> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        return source.Subscribe(new Sink(observer, selector));
    }

    /// <summary>Applies the selector to each observed change's value and forwards the result.</summary>
    /// <param name="downstream">The observer receiving projected results.</param>
    /// <param name="selector">Projects an observed change's value into the result.</param>
    private sealed class Sink(IObserver<TRet> downstream, Func<TValue?, TRet> selector) : IObserver<IObservedChange<TSender, TValue>>
    {
        /// <inheritdoc/>
        public void OnNext(IObservedChange<TSender, TValue> value)
        {
            TRet result;
            try
            {
                result = selector(value.Value);
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
