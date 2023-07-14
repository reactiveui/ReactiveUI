// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// A subject which dispatches all its events on the specified Scheduler.
/// </summary>
/// <typeparam name="T">The type of item being dispatched by the Subject.</typeparam>
public class ScheduledSubject<T> : ISubject<T>, IDisposable
{
    private readonly IObserver<T> _defaultObserver;
    private readonly IScheduler _scheduler;
    private readonly ISubject<T> _subject;
    private int _observerRefCount;
    private IDisposable _defaultObserverSub = Disposable.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduledSubject{T}"/> class.
    /// </summary>
    /// <param name="scheduler">The scheduler where to dispatch items to.</param>
    /// <param name="defaultObserver">A optional default observer where notifications will be sent.</param>
    /// <param name="defaultSubject">A optional default subject which this Subject will wrap.</param>
    public ScheduledSubject(IScheduler scheduler, IObserver<T>? defaultObserver = null, ISubject<T>? defaultSubject = null)
    {
        _scheduler = scheduler;
        _defaultObserver = defaultObserver ?? new Subject<T>();
        _subject = defaultSubject ?? new Subject<T>();

        if (defaultObserver is not null)
        {
            _defaultObserverSub = _subject.ObserveOn(_scheduler).Subscribe(_defaultObserver);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public void OnCompleted() => _subject.OnCompleted(); // TODO: Create Test

    /// <inheritdoc/>
    public void OnError(Exception error) => _subject.OnError(error); // TODO: Create Test

    /// <inheritdoc/>
    public void OnNext(T value) => _subject.OnNext(value);

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        Interlocked.Exchange(ref _defaultObserverSub, Disposable.Empty).Dispose();

        Interlocked.Increment(ref _observerRefCount);

        return new CompositeDisposable(
                                       _subject.ObserveOn(_scheduler).Subscribe(observer),
                                       Disposable.Create(() =>
                                       {
                                           if (Interlocked.Decrement(ref _observerRefCount) <= 0)
                                           {
                                               _defaultObserverSub = _subject.ObserveOn(_scheduler).Subscribe(_defaultObserver);
                                           }
                                       }));
    }

    /// <summary>
    /// Disposes of any managed resources in our class.
    /// </summary>
    /// <param name="isDisposing">If we are being called by the IDisposable method.</param>
    protected virtual void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            if (_subject is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _defaultObserverSub.Dispose();
        }
    }
}