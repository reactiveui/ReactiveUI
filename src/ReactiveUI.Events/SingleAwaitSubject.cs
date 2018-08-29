// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveUI.Events
{
    internal class SingleAwaitSubject<T> : ISubject<T>
    {
        private readonly Subject<T> _inner = new Subject<T>();

        public AsyncSubject<T> GetAwaiter()
        {
            return _inner.Take(1).GetAwaiter();
        }

        public void OnNext(T value)
        {
            _inner.OnNext(value);
        }

        public void OnError(Exception error)
        {
            _inner.OnError(error);
        }

        public void OnCompleted()
        {
            _inner.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _inner.Subscribe(observer);
        }
    }
}
