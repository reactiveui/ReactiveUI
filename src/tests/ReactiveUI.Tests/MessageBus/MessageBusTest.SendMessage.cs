// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
using MessageBusType = ReactiveUI.Reactive.MessageBus;
#else
using MessageBusType = ReactiveUI.MessageBus;
#endif

namespace ReactiveUI.Tests.MessageBus;

/// <summary>SendMessage and Listen tests for the message bus.</summary>
public partial class MessageBusTest
{
    /// <summary>
    ///     Tests that SendMessage and Listen work together for basic message passing.
    ///     Verifies that messages sent via SendMessage are received by Listen subscribers.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_Listen_ReceivesMessage()
    {
        var messageBus = new MessageBusType();
        var messages = messageBus.Listen<string>().Collect();

        messageBus.SendMessage(HelloMessage);
        messageBus.SendMessage(WorldMessage);

        const int ExpectedCount = 2;
        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(ExpectedCount);
            await Assert.That(messages[0]).IsEqualTo(HelloMessage);
            await Assert.That(messages[1]).IsEqualTo(WorldMessage);
        }
    }

    /// <summary>
    ///     Tests that contracts distinguish between messages of the same type.
    ///     Verifies that messages with different contracts are delivered to separate subscribers.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_Listen_WithContract_DistinguishesMessages()
    {
        var messageBus = new MessageBusType();
        var messages1 = messageBus.Listen<string>(Contract1).Collect();
        var messages2 = messageBus.Listen<string>(Contract2).Collect();

        messageBus.SendMessage("Message1", Contract1);
        messageBus.SendMessage("Message2", Contract2);
        messageBus.SendMessage("Message3", Contract1);

        const int ExpectedCount = 2;
        using (Assert.Multiple())
        {
            await Assert.That(messages1).Count().IsEqualTo(ExpectedCount);
            await Assert.That(messages1[0]).IsEqualTo("Message1");
            await Assert.That(messages1[1]).IsEqualTo("Message3");
            await Assert.That(messages2).Count().IsEqualTo(1);
            await Assert.That(messages2[0]).IsEqualTo("Message2");
        }
    }

    /// <summary>
    ///     Tests that multiple subscribers receive the same message.
    ///     Verifies that the message bus supports multiple concurrent subscribers.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_MultipleSubscribers_AllReceiveMessage()
    {
        var messageBus = new MessageBusType();
        var subscriber1 = messageBus.Listen<int>().Collect();
        var subscriber2 = messageBus.Listen<int>().Collect();
        var subscriber3 = messageBus.Listen<int>().Collect();

        messageBus.SendMessage(MessageValue);

        using (Assert.Multiple())
        {
            await Assert.That(subscriber1).Count().IsEqualTo(1);
            await Assert.That(subscriber1[0]).IsEqualTo(MessageValue);
            await Assert.That(subscriber2).Count().IsEqualTo(1);
            await Assert.That(subscriber2[0]).IsEqualTo(MessageValue);
            await Assert.That(subscriber3).Count().IsEqualTo(1);
            await Assert.That(subscriber3[0]).IsEqualTo(MessageValue);
        }
    }

    /// <summary>Tests that nullable value types work correctly. Verifies support for Nullable&lt;T&gt; message types.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_NullableValueType_WorksCorrectly()
    {
        var messageBus = new MessageBusType();
        messageBus.RegisterScheduler<int?>(Sequencer.Immediate);
        var messages = new List<int?>();
        _ = messageBus.Listen<int?>().Subscribe(messages.Add);

        const int ExpectedCount = 3;
        const int ThirdIndex = 2;

        messageBus.SendMessage<int?>(MessageValue);
        messageBus.SendMessage<int?>(null);
        messageBus.SendMessage<int?>(SecondMessageValue);

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(ExpectedCount);
            await Assert.That(messages[0]).IsEqualTo(MessageValue);
            await Assert.That(messages[1]).IsNull();
            await Assert.That(messages[ThirdIndex]).IsEqualTo(SecondMessageValue);
        }
    }

    /// <summary>Tests that reference type null values work correctly. Verifies support for null reference type messages.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_NullReferenceType_WorksCorrectly()
    {
        var messageBus = new MessageBusType();
        messageBus.RegisterScheduler<string?>(Sequencer.Immediate);
        var messages = new List<string?>();
        _ = messageBus.Listen<string?>().Subscribe(messages.Add);

        messageBus.SendMessage<string?>(HelloMessage);
        messageBus.SendMessage<string?>(null);
        messageBus.SendMessage<string?>(WorldMessage);

        const int ExpectedCount = 3;
        const int ThirdIndex = 2;
        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(ExpectedCount);
            await Assert.That(messages[0]).IsEqualTo(HelloMessage);
            await Assert.That(messages[1]).IsNull();
            await Assert.That(messages[ThirdIndex]).IsEqualTo(WorldMessage);
        }
    }
}
