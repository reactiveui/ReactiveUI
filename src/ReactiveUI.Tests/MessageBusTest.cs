// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using DynamicData;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    public class MessageBusTest
    {
        [Fact]
        public void MessageBusSmokeTest()
        {
            var input = new[] { 1, 2, 3, 4 };

            var result = (new TestScheduler()).With(sched => {
                var source = new Subject<int>();
                var fixture = new MessageBus();

                fixture.RegisterMessageSource(source, "Test");
                Assert.False(fixture.IsRegistered(typeof(int)));
                Assert.False(fixture.IsRegistered(typeof(int), "Foo"));

                fixture.Listen<int>("Test").ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var output).Subscribe();

                input.Run(source.OnNext);

                sched.Start();
                return output;
            });

            input.AssertAreEqual(result);
        }

        [Fact]
        public void ExplicitSendMessageShouldWorkEvenAfterRegisteringSource()
        {
            var fixture = new MessageBus();
            fixture.RegisterMessageSource(Observable<int>.Never);

            var messageReceived = false;
            fixture.Listen<int>().Subscribe(_ => messageReceived = true);

            fixture.SendMessage(42);
            Assert.True(messageReceived);
        }

        [Fact]
        public void ListeningBeforeRegisteringASourceShouldWork()
        {
            var fixture = new MessageBus();
            var result = -1;

            fixture.Listen<int>().Subscribe(x => result = x);

            Assert.Equal(-1, result);

            fixture.SendMessage(42);

            Assert.Equal(42, result);
        }

        [Fact]
        public void GCShouldNotKillMessageService()
        {
            var bus = new MessageBus();

            var recieved_message = false;
            var dispose = bus.Listen<int>().Subscribe(x => recieved_message = true);
            bus.SendMessage(1);
            Assert.True(recieved_message);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            recieved_message = false;
            bus.SendMessage(2);
            Assert.True(recieved_message);
        }

        [Fact]
        public void RegisteringSecondMessageSourceShouldMergeBothSources()
        {
            var bus = new MessageBus();
            var source1 = new Subject<int>();
            var source2 = new Subject<int>();
            var recieved_message1 = false;
            var recieved_message2 = false;

            bus.RegisterMessageSource(source1);
            bus.Listen<int>().Subscribe(x => recieved_message1 = true);

            bus.RegisterMessageSource(source2);
            bus.Listen<int>().Subscribe(x => recieved_message2 = true);

            source1.OnNext(1);
            Assert.True(recieved_message1);
            Assert.True(recieved_message2);

            recieved_message1 = false;
            recieved_message2 = false;

            source2.OnNext(2);
            Assert.True(recieved_message1);
            Assert.True(recieved_message2);
        }

        [Fact]
        public void MessageBusThreadingTest()
        {
            var mb = new MessageBus();
            int? listenedThreadId = null;
            int? otherThreadId = null;
            var thisThreadId = Thread.CurrentThread.ManagedThreadId;

            var otherThread = new Thread(new ThreadStart(() => {
                otherThreadId = Thread.CurrentThread.ManagedThreadId;
                mb.Listen<int>().Subscribe(_ => listenedThreadId = Thread.CurrentThread.ManagedThreadId);
                mb.SendMessage(42);
            }));

            otherThread.Start();
            otherThread.Join();

            Assert.NotEqual(listenedThreadId.Value, thisThreadId);
            Assert.Equal(listenedThreadId.Value, otherThreadId.Value);
        }
    }
}
