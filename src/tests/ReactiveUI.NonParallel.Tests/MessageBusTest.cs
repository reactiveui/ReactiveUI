// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;
using ReactiveUI.Tests.Infrastructure;
using ReactiveUI.Tests.Infrastructure.StaticState;

namespace ReactiveUI.Tests.Core;

[NotInParallel]
public class MessageBusTest : IDisposable
{
    private MessageBusScope? _messageBusScope;

    [Before(HookType.Test)]
    public void SetUp()
    {
        _messageBusScope = new MessageBusScope();
    }

    [After(HookType.Test)]
    public void TearDown()
    {
        _messageBusScope?.Dispose();
    }

    /// <summary>
    /// Smoke tests the MessageBus.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MessageBusSmokeTest()
    {
        var input = new[] { 1, 2, 3, 4 };

        var result = await new TestScheduler().With(async scheduler =>
        {
            var source = new Subject<int>();
            var fixture = new MessageBus();

            fixture.RegisterMessageSource(source, "Test");
            using (Assert.Multiple())
            {
                await Assert.That(fixture.IsRegistered(typeof(int))).IsFalse();
                await Assert.That(fixture.IsRegistered(typeof(int), "Foo")).IsFalse();
            }

            fixture.Listen<int>("Test").ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var output).Subscribe();

            input.Run(source.OnNext);

            scheduler.Start();
            return output;
        });

        await Assert.That(result).IsEquivalentTo(input);
    }

    /// <summary>
    /// Tests that explicits send message should work even after registering source.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExplicitSendMessageShouldWorkEvenAfterRegisteringSource()
    {
        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
        var fixture = new MessageBus();
        fixture.RegisterMessageSource(Observable<int>.Never);

        var messageReceived = false;
        fixture.Listen<int>().Subscribe(_ => messageReceived = true);

        fixture.SendMessage(42);
        await Assert.That(messageReceived).IsTrue();
    }

    /// <summary>
    /// Tests that listening before registering a source should work.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ListeningBeforeRegisteringASourceShouldWork()
    {
        var fixture = new MessageBus();
        var result = -1;

        fixture.Listen<int>().Subscribe(x => result = x);

        await Assert.That(result).IsEqualTo(-1);

        fixture.SendMessage(42);

        await Assert.That(result).IsEqualTo(42);
    }

    /// <summary>
    /// Tests that the Garbage Collector should not kill message service.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GcShouldNotKillMessageService()
    {
        var bus = new MessageBus();

        var receivedMessage = false;
        var dispose = bus.Listen<int>().Subscribe(_ => receivedMessage = true);
        bus.SendMessage(1);
        await Assert.That(receivedMessage).IsTrue();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        receivedMessage = false;
        bus.SendMessage(2);
        await Assert.That(receivedMessage).IsTrue();
    }

    /// <summary>
    /// Tests that Registering the second message source should merge both sources.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RegisteringSecondMessageSourceShouldMergeBothSources()
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
        using (Assert.Multiple())
        {
            await Assert.That(receivedMessage1).IsTrue();
            await Assert.That(receivedMessage2).IsTrue();
        }

        receivedMessage1 = false;
        receivedMessage2 = false;

        source2.OnNext(2);
        using (Assert.Multiple())
        {
            await Assert.That(receivedMessage1).IsTrue();
            await Assert.That(receivedMessage2).IsTrue();
        }
    }

    /// <summary>
    /// Tests the MessageBus threading.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MessageBusThreadingTest()
    {
        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
        var mb = new MessageBus();
        mb.RegisterScheduler<int>(ImmediateScheduler.Instance);
        int? listenedThreadId = null;
        int? otherThreadId = null;
        var thisThreadId = Environment.CurrentManagedThreadId;

        await Task.Run(() =>
        {
            otherThreadId = Environment.CurrentManagedThreadId;
            mb.Listen<int>().Subscribe(_ => listenedThreadId = Environment.CurrentManagedThreadId);
            mb.SendMessage(42);
        });

        using (Assert.Multiple())
        {
            await Assert.That(thisThreadId).IsNotEqualTo(listenedThreadId!.Value);
            await Assert.That(otherThreadId!.Value).IsEqualTo(listenedThreadId.Value);
        }
    }

    /// <summary>
    /// Tests MessageBus.RegisterScheduler method for complete coverage.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MessageBus_RegisterScheduler_ShouldWork()
    {
        // Arrange
        var messageBus = new MessageBus();
        var receivedMessages = new List<int>();

        // Act - Register scheduler without contract first
        messageBus.RegisterScheduler<int>(CurrentThreadScheduler.Instance);
        messageBus.Listen<int>().Subscribe(x => receivedMessages.Add(x));
        messageBus.SendMessage(42);

        // Assert
        await Assert.That(receivedMessages).Count().IsEqualTo(1);
        await Assert.That(receivedMessages[0]).IsEqualTo(42);
    }

    /// <summary>
    /// Tests MessageBus.ListenIncludeLatest method for complete coverage.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MessageBus_ListenIncludeLatest_ShouldIncludeLastMessage()
    {
        // Arrange
        var messageBus = new MessageBus();
        var receivedMessages = new List<int>();

        // Send a message first
        messageBus.SendMessage(42);

        // Act - Listen including latest should get the previously sent message
        messageBus.ListenIncludeLatest<int>().Subscribe(x => receivedMessages.Add(x));

        // Assert
        await Assert.That(receivedMessages).Count().IsEqualTo(1);
        await Assert.That(receivedMessages[0]).IsEqualTo(42);
    }

    /// <summary>
    /// Tests MessageBus.Current static property for complete coverage.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MessageBus_Current_ShouldBeAccessible()
    {
        // Act
        var current = MessageBus.Current;

        // Assert
        await Assert.That(current).IsNotNull();
        await Assert.That(current).IsAssignableTo<IMessageBus>();
    }

    /// <summary>
    /// Tests MessageBus with contracts to ensure message isolation.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MessageBus_WithContracts_ShouldIsolateMessages()
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
        await Assert.That(contract1Messages).Count().IsEqualTo(1);
        using (Assert.Multiple())
        {
            await Assert.That(contract1Messages[0]).IsEqualTo(1);

            await Assert.That(contract2Messages).Count().IsEqualTo(1);
        }

        using (Assert.Multiple())
        {
            await Assert.That(contract2Messages[0]).IsEqualTo(2);

            await Assert.That(noContractMessages).Count().IsEqualTo(1);
        }

        await Assert.That(noContractMessages[0]).IsEqualTo(3);
    }

    public void Dispose()
    {
        _messageBusScope?.Dispose();
        _messageBusScope = null;
    }
}
