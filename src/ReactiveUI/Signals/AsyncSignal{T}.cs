// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace ReactiveUI.Signals;

/// <summary>
/// AsyncSignal.
/// </summary>
/// <typeparam name="T">The object that provides notification information.</typeparam>
internal class AsyncSignal<T> : IAsyncSignal<T>
{
    private readonly IScheduler _scheduler;
    private readonly CompositeDisposable _cleanUp = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncSignal{T}" /> class.
    /// </summary>
    /// <param name="observableFactory">The observable factory.</param>
    /// <param name="scheduler">The scheduler.</param>
    public AsyncSignal(Func<IAsyncSignal<T>, IObservable<T>> observableFactory, IScheduler? scheduler = null)
    {
        _scheduler = scheduler ?? CurrentThreadScheduler.Instance;
        Source = observableFactory(this);
    }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    /// <value>
    /// The source.
    /// </value>
    public IObservable<T>? Source { get; set; }

    /// <summary>
    /// Gets the cancellation token source.
    /// </summary>
    /// <value>
    /// The cancellation token source.
    /// </value>
    public CancellationTokenSource? CancellationTokenSource { get; } = new();

    /// <summary>
    /// Gets a value indicating whether this instance is cancellation requested.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is cancellation requested; otherwise, <c>false</c>.
    /// </value>
    public bool IsCancellationRequested => CancellationTokenSource?.IsCancellationRequested == true;

    /// <summary>
    /// Gets a value indicating whether gets a value that indicates whether the object is disposed.
    /// </summary>
    public bool IsDisposed => _cleanUp.IsDisposed;

    /// <summary>
    /// Gets the operation canceled.
    /// </summary>
    /// <param name="observer">The observer.</param>
    public void GetOperationCanceled(IObserver<Exception> observer) =>
        CancellationTokenSource?.Token.Register(() => observer.OnNext(new OperationCanceledException())).DisposeWith(_cleanUp);

    /// <summary>
    /// Subscribes the specified observer.
    /// </summary>
    /// <param name="observer">The observer.</param>
    /// <returns>A Disposable.</returns>
    public IDisposable Subscribe(IObserver<T> observer) =>
        Source!.ObserveOn(_scheduler).Subscribe(observer).DisposeWith(_cleanUp);

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_cleanUp.IsDisposed && disposing)
        {
            CancellationTokenSource?.Cancel();
            _cleanUp.Dispose();
            CancellationTokenSource?.Dispose();
        }
    }
}
