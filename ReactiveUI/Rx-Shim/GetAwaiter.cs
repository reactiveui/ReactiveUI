using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.

/* This file is substantially copied from http://rx.codeplex.com/SourceControl/changeset/view/ef6a42709f49#Rx.NET/System.Reactive.Linq/Reactive/Linq/QueryLanguage.Awaiter.cs
 * Check LICENSE in this folder for licensing information */

namespace ReactiveUI
{
    public static class ObservableAwaiter
    {
        public static AwaitableAsyncSubject<TSource> GetAwaiter<TSource>(this IObservable<TSource> source)
        {
            var s = new AwaitableAsyncSubject<TSource>();
            source.SubscribeSafe(s);
            return s;
        }

        public static AwaitableAsyncSubject<TSource> GetAwaiter<TSource>(this IConnectableObservable<TSource> source)
        {
            var s = new AwaitableAsyncSubject<TSource>();
            source.SubscribeSafe(s);
            source.Connect();
            return s;
        }

        public static AwaitableAsyncSubject<TSource> RunAsync<TSource>(this IObservable<TSource> source, CancellationToken cancellationToken)
        {
            var s = new AwaitableAsyncSubject<TSource>();

            var cancel = new Action(() => s.OnError(new OperationCanceledException()));
            if (cancellationToken.IsCancellationRequested)
            {
                cancel();
                return s;
            }

            var d = source.SubscribeSafe(s);
            cancellationToken.Register(d.Dispose);
            cancellationToken.Register(cancel);

            return s;
        }

        public static AwaitableAsyncSubject<TSource> RunAsync<TSource>(this IConnectableObservable<TSource> source, CancellationToken cancellationToken)
        {
            var s = new AwaitableAsyncSubject<TSource>();

            var cancel = new Action(() => s.OnError(new OperationCanceledException()));
            if (cancellationToken.IsCancellationRequested)
            {
                cancel();
                return s;
            }

            var d = new CompositeDisposable(source.SubscribeSafe(s), source.Connect());
            cancellationToken.Register(d.Dispose);
            cancellationToken.Register(cancel);

            return s;
        }
    }
}