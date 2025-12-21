// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;
using ReactiveUI.Tests.Infrastructure.StaticState;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests the MessageBus class.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because some tests call
/// Locator.CurrentMutable.InitializeSplat() and Locator.CurrentMutable.InitializeReactiveUI(),
/// which mutate global service locator state. Other tests access MessageBus.Current static property.
/// These static states must not be mutated concurrently by parallel tests.
/// </remarks>
[TestFixture]
[NonParallelizable]
public class MessageBusTest : IDisposable
{
    private MessageBusScope? _messageBusScope;

    [SetUp]
    public void SetUp()
    {
        _messageBusScope = new MessageBusScope();
    }

    [TearDown]
    public void TearDown()
    {
        _messageBusScope?.Dispose();
    }

    /// <summary>
    /// Smoke tests the MessageBus.
    /// </summary>
    [Test]
    public void MessageBusSmokeTest()
    {
        var input = new[] { 1, 2, 3, 4 };

        var result = new TestScheduler().With(scheduler =>
        {
            var source = new Subject<int>();
            var fixture = new MessageBus();

            fixture.RegisterMessageSource(source, "Test");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(fixture.IsRegistered(typeof(int)), Is.False);
                Assert.That(fixture.IsRegistered(typeof(int), "Foo"), Is.False);
            }

            fixture.Listen<int>("Test").ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var output).Subscribe();

            input.Run(source.OnNext);

            scheduler.Start();
            return output;
        });

        Assert.That(result, Is.EqualTo(input));
    }

    /// <summary>
    /// Tests that explicits send message should work even after registering source.
    /// </summary>
    [Test]
    public void ExplicitSendMessageShouldWorkEvenAfterRegisteringSource()
    {
        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
        var fixture = new MessageBus();
        fixture.RegisterMessageSource(Observable<int>.Never);

        var messageReceived = false;
        fixture.Listen<int>().Subscribe(_ => messageReceived = true);

        fixture.SendMessage(42);
        Assert.That(messageReceived, Is.True);
    }

    /// <summary>
    /// Tests that listening before registering a source should work.
    /// </summary>
    [Test]
    public void ListeningBeforeRegisteringASourceShouldWork()
    {
        var fixture = new MessageBus();
        var result = -1;

        fixture.Listen<int>().Subscribe(x => result = x);

        Assert.That(result, Is.EqualTo(-1));

        fixture.SendMessage(42);

        Assert.That(result, Is.EqualTo(42));
    }

    /// <summary>
    /// Tests that the Garbage Collector should not kill message service.
    /// </summary>
    [Test]
    public void GcShouldNotKillMessageService()
    {
        var bus = new MessageBus();

        var receivedMessage = false;
        var dispose = bus.Listen<int>().Subscribe(_ => receivedMessage = true);
        bus.SendMessage(1);
        Assert.That(receivedMessage, Is.True);

        GC.Collect();
        GC.WaitForPendingFinalizers();

        receivedMessage = false;
        bus.SendMessage(2);
        Assert.That(receivedMessage, Is.True);
    }

    /// <summary>
    /// Tests that Registering the second message source should merge both sources.
    /// </summary>
    [Test]
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(receivedMessage1, Is.True);
            Assert.That(receivedMessage2, Is.True);
        }

        receivedMessage1 = false;
        receivedMessage2 = false;

        source2.OnNext(2);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(receivedMessage1, Is.True);
            Assert.That(receivedMessage2, Is.True);
        }
    }

    /// <summary>
    /// Tests the MessageBus threading.
    /// </summary>
    [Test]
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(thisThreadId, Is.Not.EqualTo(listenedThreadId!.Value));
            Assert.That(otherThreadId!.Value, Is.EqualTo(listenedThreadId.Value));
        }
    }

    /// <summary>
    /// Tests MessageBus.RegisterScheduler method for complete coverage.
    /// </summary>
    [Test]
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
        Assert.That(receivedMessages, Has.Exactly(1).Items);
        Assert.That(receivedMessages[0], Is.EqualTo(42));
    }

    /// <summary>
    /// Tests MessageBus.ListenIncludeLatest method for complete coverage.
    /// </summary>
    [Test]
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
        Assert.That(receivedMessages, Has.Exactly(1).Items);
        Assert.That(receivedMessages[0], Is.EqualTo(42));
    }

    /// <summary>
    /// Tests MessageBus.Current static property for complete coverage.
    /// </summary>
    [Test]
    public void MessageBus_Current_ShouldBeAccessible()
    {
        // Act
        var current = MessageBus.Current;

        // Assert
        Assert.That(current, Is.Not.Null);
        Assert.That(current, Is.InstanceOf<IMessageBus>());
    }

    /// <summary>
    /// Tests MessageBus with contracts to ensure message isolation.
    /// </summary>
    [Test]
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
        Assert.That(contract1Messages, Has.Exactly(1).Items);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(contract1Messages[0], Is.EqualTo(1));

            Assert.That(contract2Messages, Has.Exactly(1).Items);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(contract2Messages[0], Is.EqualTo(2));

            Assert.That(noContractMessages, Has.Exactly(1).Items);
        }

        Assert.That(noContractMessages[0], Is.EqualTo(3));
    }

    public void Dispose()
    {
        _messageBusScope?.Dispose();
        _messageBusScope = null;
    }
}
