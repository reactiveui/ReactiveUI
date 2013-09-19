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

        readonly IObserver<T> _defaultObserver;
        readonly IScheduler _scheduler;
        readonly ISubject<T> _subject;

        int _observerRefCount = 0;
        IDisposable _defaultObserverSub = Disposable.Empty;

        public void Dispose()
        {
            if (_subject is IDisposable)
            {
                ((IDisposable)_subject).Dispose();
            }
        }

        public void OnCompleted()
        {
            _subject.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _subject.OnError(error);
        }

        public void OnNext(T value)
        {
            _subject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Interlocked.Exchange(ref _defaultObserverSub, Disposable.Empty).Dispose();

            Interlocked.Increment(ref _observerRefCount);

            return new CompositeDisposable(
                _subject.ObserveOn(_scheduler).Subscribe(observer),
                Disposable.Create(() => {
                    if (Interlocked.Decrement(ref _observerRefCount) <= 0 && _defaultObserver != null) {
                        _defaultObserverSub = _subject.ObserveOn(_scheduler).Subscribe(_defaultObserver);
                    }
                }));
        }
    }
}
