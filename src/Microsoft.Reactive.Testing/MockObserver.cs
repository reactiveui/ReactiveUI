// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reactive;
using System;

namespace Microsoft.Reactive.Testing
{
    class MockObserver<T> : ITestableObserver<T>
    {
        TestScheduler scheduler;
        List<Recorded<Notification<T>>> messages;

        public MockObserver(TestScheduler scheduler)
        {
            if (scheduler == null)
                throw new ArgumentNullException("scheduler");

            this.scheduler = scheduler;
            this.messages = new List<Recorded<Notification<T>>>();
        }

        public void OnNext(T value)
        {
            messages.Add(new Recorded<Notification<T>>(scheduler.Clock, Notification.CreateOnNext<T>(value)));
        }

        public void OnError(Exception exception)
        {
            messages.Add(new Recorded<Notification<T>>(scheduler.Clock, Notification.CreateOnError<T>(exception)));
        }

        public void OnCompleted()
        {
            messages.Add(new Recorded<Notification<T>>(scheduler.Clock, Notification.CreateOnCompleted<T>()));
        }

        public IList<Recorded<Notification<T>>> Messages
        {
            get { return messages; }
        }
    }
}
