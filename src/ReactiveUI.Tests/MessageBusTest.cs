// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

namespace ReactiveUI.Tests;

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
        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
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
        var dispose = bus.Listen<int>().Subscribe(_ => receivedMessage = true);
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
        bus.Listen<int>().Subscribe(_ => receivedMessage1 = true);

        bus.RegisterMessageSource(source2);
        bus.Listen<int>().Subscribe(_ => receivedMessage2 = true);

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
        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
        var mb = new MessageBus();
        int? listenedThreadId = null;
        int? otherThreadId = null;
        var thisThreadId = Environment.CurrentManagedThreadId;

        var otherThread = new Thread(new ThreadStart(() =>
        {
            otherThreadId = Environment.CurrentManagedThreadId;
            mb.Listen<int>().Subscribe(_ => listenedThreadId = Environment.CurrentManagedThreadId);
            mb.SendMessage(42);
        }));

        otherThread.Start();
        otherThread.Join();

        Assert.NotEqual(listenedThreadId!.Value, thisThreadId);
        Assert.Equal(listenedThreadId.Value, otherThreadId!.Value);
    }

    /// <summary>
    /// Tests MessageBus.RegisterScheduler method for complete coverage.
    /// </summary>
    [Fact]
    public void MessageBus_RegisterScheduler_ShouldWork()
    {
        // Arrange
        var messageBus = new MessageBus();
        var receivedMessages = new List<int>();

        // Act - Register scheduler without contract first
        messageBus.RegisterScheduler<int>(CurrentThreadScheduler.Instance);
        messageBus.Listen<int>().Subscribe(x => receivedMessages.Add(x));
        messageBus.SendMessage(42);

        // Assert
        Assert.Single(receivedMessages);
        Assert.Equal(42, receivedMessages[0]);
    }

    /// <summary>
    /// Tests MessageBus.ListenIncludeLatest method for complete coverage.
    /// </summary>
    [Fact]
    public void MessageBus_ListenIncludeLatest_ShouldIncludeLastMessage()
    {
        // Arrange
        var messageBus = new MessageBus();
        var receivedMessages = new List<int>();

        // Send a message first
        messageBus.SendMessage(42);

        // Act - Listen including latest should get the previously sent message
        messageBus.ListenIncludeLatest<int>().Subscribe(x => receivedMessages.Add(x));

        // Assert
        Assert.Single(receivedMessages);
        Assert.Equal(42, receivedMessages[0]);
    }

    /// <summary>
    /// Tests MessageBus.Current static property for complete coverage.
    /// </summary>
    [Fact]
    public void MessageBus_Current_ShouldBeAccessible()
    {
        // Act
        var current = MessageBus.Current;

        // Assert
        Assert.NotNull(current);
        Assert.IsAssignableFrom<IMessageBus>(current);
    }

    /// <summary>
    /// Tests MessageBus with contracts to ensure message isolation.
    /// </summary>
    [Fact]
    public void MessageBus_WithContracts_ShouldIsolateMessages()
    {
        // Arrange
        var messageBus = new MessageBus();
        var contract1Messages = new List<int>();
        var contract2Messages = new List<int>();
        var noContractMessages = new List<int>();

        // Act
        messageBus.Listen<int>("Contract1").Subscribe(x => contract1Messages.Add(x));
        messageBus.Listen<int>("Contract2").Subscribe(x => contract2Messages.Add(x));
        messageBus.Listen<int>().Subscribe(x => noContractMessages.Add(x));

        messageBus.SendMessage(1, "Contract1");
        messageBus.SendMessage(2, "Contract2");
        messageBus.SendMessage(3);

        // Assert
        Assert.Single(contract1Messages);
        Assert.Equal(1, contract1Messages[0]);

        Assert.Single(contract2Messages);
        Assert.Equal(2, contract2Messages[0]);

        Assert.Single(noContractMessages);
        Assert.Equal(3, noContractMessages[0]);
    }
}
