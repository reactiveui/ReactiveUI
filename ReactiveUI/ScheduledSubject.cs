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
        public ScheduledSubject(IScheduler scheduler, IObserver<T> defaultObserver = null)
        {
            _scheduler = scheduler;
            _defaultObserver = defaultObserver;

            if (defaultObserver != null) {
                _defaultObserverSub = _subject.ObserveOn(_scheduler).Subscribe(_defaultObserver);
            }
        }

        readonly IObserver<T> _defaultObserver;
        readonly IScheduler _scheduler;
        readonly Subject<T> _subject = new Subject<T>();

        int _observerRefCount = 0;
        IDisposable _defaultObserverSub;

        public void Dispose()
        {
            _subject.Dispose();
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
            if (_defaultObserverSub != null) {
                _defaultObserverSub.Dispose();
                _defaultObserverSub = null;
            }

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