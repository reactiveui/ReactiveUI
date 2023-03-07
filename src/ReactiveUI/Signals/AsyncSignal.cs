// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace ReactiveUI.Signals;

/// <summary>
/// AsyncObservable.
/// </summary>
public static class AsyncSignal
{
    /// <summary>
    /// Creates the specified source.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="observableFactory">The observable factory.</param>
    /// <param name="scheduler">The scheduler.</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <returns>
    /// An AsyncObservable.
    /// </returns>
    /// <exception cref="ArgumentNullException">observableFactory.</exception>
    public static IAsyncSignal<TResult> Create<TResult>(Func<IAsyncSignal<TResult>, IObservable<TResult>> observableFactory, IScheduler? scheduler = null, CancellationTokenSource? cancellationTokenSource = null) =>
        Instance(observableFactory, scheduler, cancellationTokenSource);

    private static IAsyncSignal<TResult> Instance<TResult>(Func<IAsyncSignal<TResult>, IObservable<TResult>> observableFactory, IScheduler? scheduler, CancellationTokenSource? cancellationTokenSource)
    {
        if (observableFactory is null)
        {
            throw new ArgumentNullException(nameof(observableFactory));
        }

        return new AsyncSignal<TResult>(observableFactory, scheduler, cancellationTokenSource);
    }
}
