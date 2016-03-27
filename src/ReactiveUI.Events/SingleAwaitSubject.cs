using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveUI.Events
{
    class SingleAwaitSubject<T> : ISubject<T>
    {
        readonly Subject<T> inner = new Subject<T>();

        public AsyncSubject<T> GetAwaiter()
        {
            return inner.Take(1).GetAwaiter();
        }

        public void OnNext(T value) { inner.OnNext(value); }
        public void OnError(Exception error) { inner.OnError(error); }
        public void OnCompleted() { inner.OnCompleted(); }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return inner.Subscribe(observer);
        }
    }
}

