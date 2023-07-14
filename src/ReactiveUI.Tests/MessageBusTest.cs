// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Tests the MessageBus class.
    /// </summary>
    public class MessageBusTest
    {
        /// <summary>
        /// Smoke tests the MessageBus.
        /// </summary>
        [Fact]
        public void MessageBusSmokeTest()
        {
            var input = new[] { 1, 2, 3, 4 };

            var result = new TestScheduler().With(scheduler =>
            {
                var source = new Subject<int>();
                var fixture = new MessageBus();

                fixture.RegisterMessageSource(source, "Test");
                Assert.False(fixture.IsRegistered(typeof(int)));
                Assert.False(fixture.IsRegistered(typeof(int), "Foo"));

                fixture.Listen<int>("Test").ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var output).Subscribe();

                input.Run(source.OnNext);

                scheduler.Start();
                return output;
            });

            input.AssertAreEqual(result);
        }

        /// <summary>
        /// Tests that explicits send message should work even after registering source.
        /// </summary>
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

        /// <summary>
        /// Tests that listening before registering a source should work.
        /// </summary>
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

        /// <summary>
        /// Tests that the Garbage Collector should not kill message service.
        /// </summary>
        [Fact]
        public void GcShouldNotKillMessageService()
        {
            var bus = new MessageBus();

            var receivedMessage = false;
            var dispose = bus.Listen<int>().Subscribe(x => receivedMessage = true);
            bus.SendMessage(1);
            Assert.True(receivedMessage);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            receivedMessage = false;
            bus.SendMessage(2);
            Assert.True(receivedMessage);
        }

        /// <summary>
        /// Tests that Registering the second message source should merge both sources.
        /// </summary>
        [Fact]
        public void RegisteringSecondMessageSourceShouldMergeBothSources()
        {
            var bus = new MessageBus();
            var source1 = new Subject<int>();
            var source2 = new Subject<int>();
            var receivedMessage1 = false;
            var receivedMessage2 = false;

            bus.RegisterMessageSource(source1);
            bus.Listen<int>().Subscribe(x => receivedMessage1 = true);

            bus.RegisterMessageSource(source2);
            bus.Listen<int>().Subscribe(x => receivedMessage2 = true);

            source1.OnNext(1);
            Assert.True(receivedMessage1);
            Assert.True(receivedMessage2);

            receivedMessage1 = false;
            receivedMessage2 = false;

            source2.OnNext(2);
            Assert.True(receivedMessage1);
            Assert.True(receivedMessage2);
        }

        /// <summary>
        /// Tests the MessageBus threading.
        /// </summary>
        [Fact]
        public void MessageBusThreadingTest()
        {
            var mb = new MessageBus();
            int? listenedThreadId = null;
            int? otherThreadId = null;
            var thisThreadId = Thread.CurrentThread.ManagedThreadId;

            var otherThread = new Thread(new ThreadStart(() =>
            {
                otherThreadId = Thread.CurrentThread.ManagedThreadId;
                mb.Listen<int>().Subscribe(_ => listenedThreadId = Thread.CurrentThread.ManagedThreadId);
                mb.SendMessage(42);
            }));

            otherThread.Start();
            otherThread.Join();

            Assert.NotEqual(listenedThreadId!.Value, thisThreadId);
            Assert.Equal(listenedThreadId.Value, otherThreadId!.Value);
        }
    }
}
