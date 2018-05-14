using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

namespace ReactiveUI.Subjects
{
        /// <inheritdoc />
        /// <summary>
        /// This is a wrapper for independant IObserver and IObservable to be
        /// combined into an ISubject. All the methods are just forwarded
        /// to the respective object.
        /// </summary>
        public class TransformerSubject<T> : ISubject<T>
        {
            private readonly IObserver<T> _Observer;
            private readonly IObservable<T> _Observable;

            public TransformerSubject(IObserver<T> observer, IObservable<T> observable)
            {
                _Observer = observer;
                _Observable = observable;
            }

            public void OnCompleted()
            {
                _Observer.OnCompleted();
            }

            public void OnError(Exception error)
            {
                _Observer.OnError(error);
            }

            public void OnNext(T value)
            {
                _Observer.OnNext(value);
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return _Observable.Subscribe(observer);
            }
        }
}
