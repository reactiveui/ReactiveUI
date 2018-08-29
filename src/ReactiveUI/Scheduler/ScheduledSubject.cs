// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace ReactiveUI
{
    public class ScheduledSubject<T> : ISubject<T>
    {
        public ScheduledSubject(IScheduler scheduler, IObserver<T> defaultObserver = null, ISubject<T> defaultSubject = null)
        {
            _scheduler = scheduler;
            _defaultObserver = defaultObserver;
            _subject = defaultSubject ?? new Subject<T>();

            if (defaultObserver != null)
            {
                _defaultObserverSub = _subject.ObserveOn(_scheduler).Subscribe(_defaultObserver);
            }
        }

        private readonly IObserver<T> _defaultObserver;
        private readonly IScheduler _scheduler;
        private readonly ISubject<T> _subject;
        private int _observerRefCount;
        private IDisposable _defaultObserverSub = Disposable.Empty;

        public void Dispose()
        {
            if (_subject is IDisposable)
            {
                ((IDisposable)_subject).Dispose();
            }
        }

        /// <inheritdoc/>
        public void OnCompleted()
        {
            _subject.OnCompleted();
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
            _subject.OnError(error);
        }

        /// <inheritdoc/>
        public void OnNext(T value)
        {
            _subject.OnNext(value);
        }

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            Interlocked.Exchange(ref _defaultObserverSub, Disposable.Empty).Dispose();

            Interlocked.Increment(ref _observerRefCount);

            return new CompositeDisposable(
                _subject.ObserveOn(_scheduler).Subscribe(observer),
                Disposable.Create(() =>
                {
                    if (Interlocked.Decrement(ref _observerRefCount) <= 0 && _defaultObserver != null)
                    {
                        _defaultObserverSub = _subject.ObserveOn(_scheduler).Subscribe(_defaultObserver);
                    }
                }));
        }
    }
}
