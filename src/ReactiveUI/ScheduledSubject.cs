using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace ReactiveUI
{
    /// <summary>
    /// Scheduled Subject
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Reactive.Subjects.ISubject{T}"/>
    public class ScheduledSubject<T> : ISubject<T>
    {
        private readonly IObserver<T> _defaultObserver;

        private readonly IScheduler _scheduler;

        private readonly ISubject<T> _subject;

        private IDisposable _defaultObserverSub = Disposable.Empty;

        private int _observerRefCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledSubject{T}"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="defaultObserver">The default observer.</param>
        /// <param name="defaultSubject">The default subject.</param>
        public ScheduledSubject(IScheduler scheduler, IObserver<T> defaultObserver = null, ISubject<T> defaultSubject = null)
        {
            this._scheduler = scheduler;
            this._defaultObserver = defaultObserver;
            this._subject = defaultSubject ?? new Subject<T>();

            if (defaultObserver != null) {
                this._defaultObserverSub = this._subject.ObserveOn(this._scheduler).Subscribe(this._defaultObserver);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (this._subject is IDisposable) {
                ((IDisposable)this._subject).Dispose();
            }
        }

        /// <summary>
        /// Called when [completed].
        /// </summary>
        public void OnCompleted()
        {
            this._subject.OnCompleted();
        }

        /// <summary>
        /// Called when [error].
        /// </summary>
        /// <param name="error">The error.</param>
        public void OnError(Exception error)
        {
            this._subject.OnError(error);
        }

        /// <summary>
        /// Called when [next].
        /// </summary>
        /// <param name="value">The value.</param>
        public void OnNext(T value)
        {
            this._subject.OnNext(value);
        }

        /// <summary>
        /// Subscribes the specified observer.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            Interlocked.Exchange(ref this._defaultObserverSub, Disposable.Empty).Dispose();

            Interlocked.Increment(ref this._observerRefCount);

            return new CompositeDisposable(
                this._subject.ObserveOn(this._scheduler).Subscribe(observer),
                Disposable.Create(() => {
                    if (Interlocked.Decrement(ref this._observerRefCount) <= 0 && this._defaultObserver != null) {
                        this._defaultObserverSub = this._subject.ObserveOn(this._scheduler).Subscribe(this._defaultObserver);
                    }
                }));
        }
    }
}