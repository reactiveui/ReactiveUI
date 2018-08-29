// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace ReactiveUI.Legacy
{
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
    internal class ReactiveDerivedCollectionFromObservable<T> : ReactiveDerivedCollection<T>
    {
        private SingleAssignmentDisposable _inner;

        public ReactiveDerivedCollectionFromObservable(
            IObservable<T> observable,
            TimeSpan? withDelay = null,
            Action<Exception> onError = null,
            IScheduler scheduler = null)
        {
            scheduler = scheduler ?? RxApp.MainThreadScheduler;
            _inner = new SingleAssignmentDisposable();

            onError = onError ?? (ex => RxApp.DefaultExceptionHandler.OnNext(ex));
            if (withDelay == null)
            {
                _inner.Disposable = observable.ObserveOn(scheduler).Subscribe(InternalAdd, onError);
                return;
            }

            // On a timer, dequeue items from queue if they are available
            var queue = new Queue<T>();
            var disconnect = Observable.Timer(withDelay.Value, withDelay.Value, scheduler)
                                       .Subscribe(_ =>
                                       {
                                           if (queue.Count > 0)
                                           {
                                               InternalAdd(queue.Dequeue());
                                           }
                                       });

            _inner.Disposable = disconnect;

            // When new items come in from the observable, stuff them in the queue.
            observable.ObserveOn(scheduler).Subscribe(queue.Enqueue, onError);

            // This is a bit clever - keep a running count of the items actually
            // added and compare them to the final count of items provided by the
            // Observable. Combine the two values, and when they're equal,
            // disconnect the timer
            ItemsAdded.Scan(0, (acc, _) => acc + 1).Zip(
                observable.Aggregate(0, (acc, _) => acc + 1),
                (l, r) => l == r).Where(x => x).Subscribe(_ => disconnect.Dispose());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disp = Interlocked.Exchange(ref _inner, null);
                if (disp == null)
                {
                    return;
                }

                disp.Dispose();
            }
        }
    }
}
