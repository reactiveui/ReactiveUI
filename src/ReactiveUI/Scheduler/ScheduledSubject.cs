// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;

using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>
/// A subject which dispatches all its events on the specified Scheduler.
/// </summary>
/// <typeparam name="T">The type of item being dispatched by the Subject.</typeparam>
public class ScheduledSubject<T> : IReactiveSubject<T>, IDisposable
{
    /// <summary>The observer that receives notifications when no other subscribers are active.</summary>
    private readonly IObserver<T>? _defaultObserver;

    /// <summary>The scheduler used to dispatch items to observers.</summary>
    private readonly IScheduler _scheduler;

    /// <summary>The underlying subject that items are forwarded through.</summary>
    private readonly IReactiveSubject<T> _subject;

    /// <summary>The current count of active observers.</summary>
    private int _observerRefCount;

    /// <summary>The subscription connecting the subject to the default observer.</summary>
    private IDisposable _defaultObserverSub = EmptyDisposable.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduledSubject{T}"/> class with only a scheduler.
    /// </summary>
    /// <param name="scheduler">The scheduler where to dispatch items to.</param>
    public ScheduledSubject(IScheduler scheduler)
        : this(scheduler, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduledSubject{T}"/> class with a scheduler and a default observer.
    /// </summary>
    /// <param name="scheduler">The scheduler where to dispatch items to.</param>
    /// <param name="defaultObserver">A default observer where notifications will be sent when no other subscribers are active.</param>
    public ScheduledSubject(IScheduler scheduler, IObserver<T>? defaultObserver)
        : this(scheduler, defaultObserver, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduledSubject{T}"/> class.
    /// </summary>
    /// <param name="scheduler">The scheduler where to dispatch items to.</param>
    /// <param name="defaultObserver">A optional default observer where notifications will be sent.</param>
    /// <param name="defaultSubject">A optional default subject which this Subject will wrap.</param>
    public ScheduledSubject(
        IScheduler scheduler,
        IObserver<T>? defaultObserver,
        IReactiveSubject<T>? defaultSubject)
    {
        _scheduler = scheduler;
        _defaultObserver = defaultObserver;
        _subject = defaultSubject ?? new BroadcastSubject<T>();

        if (defaultObserver is null)
        {
            return;
        }

        _defaultObserverSub = _subject.Subscribe(new SchedulingObserver<T>(defaultObserver, _scheduler));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public void OnCompleted() => _subject.OnCompleted();

    /// <inheritdoc/>
    public void OnError(Exception error) => _subject.OnError(error);

    /// <inheritdoc/>
    public void OnNext(T value) => _subject.OnNext(value);

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        Interlocked.Exchange(ref _defaultObserverSub, EmptyDisposable.Instance).Dispose();
        Interlocked.Increment(ref _observerRefCount);

        var inner = _subject.Subscribe(new SchedulingObserver<T>(observer, _scheduler));
        return new Subscription(this, inner);
    }

    /// <summary>
    /// Disposes of any managed resources in our class.
    /// </summary>
    /// <param name="isDisposing">If we are being called by the IDisposable method.</param>
    protected virtual void Dispose(bool isDisposing)
    {
        if (!isDisposing)
        {
            return;
        }

        if (_subject is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _defaultObserverSub.Dispose();
    }

    /// <summary>Re-attaches the default observer once the last real subscriber leaves.</summary>
    private void OnUnsubscribe()
    {
        if (Interlocked.Decrement(ref _observerRefCount) > 0 || _defaultObserver is null)
        {
            return;
        }

        _defaultObserverSub = _subject.Subscribe(new SchedulingObserver<T>(_defaultObserver, _scheduler));
    }

    /// <summary>Tears down a real subscription and restores the default observer when none remain.</summary>
    /// <param name="parent">The owning subject.</param>
    /// <param name="inner">The inner subject subscription.</param>
    private sealed class Subscription(ScheduledSubject<T> parent, IDisposable inner) : IDisposable
    {
        /// <inheritdoc/>
        public void Dispose()
        {
            inner.Dispose();
            parent.OnUnsubscribe();
        }
    }
}
