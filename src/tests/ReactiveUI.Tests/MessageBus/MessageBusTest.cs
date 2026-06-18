// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.MessageBus;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

#if REACTIVE_SHIM
using MessageBusType = ReactiveUI.Reactive.MessageBus;
#else
using MessageBusType = ReactiveUI.MessageBus;
#endif

namespace ReactiveUI.Tests.MessageBus;

/// <summary>Comprehensive test suite for MessageBus. Tests cover all public methods, contracts, scheduling, and edge cases.</summary>
[NotInParallel]
[TestExecutor<WithMessageBusExecutor>]
public class MessageBusTest
{
    /// <summary>A representative integer message value used across the tests.</summary>
    private const int MessageValue = 42;

    /// <summary>A second representative integer message value used across the tests.</summary>
    private const int SecondMessageValue = 100;

    /// <summary>A representative string message used across the tests.</summary>
    private const string TestMessage1 = "Test";

    /// <summary>The first representative contract name used across the tests.</summary>
    private const string Contract1 = "Contract1";

    /// <summary>The second representative contract name used across the tests.</summary>
    private const string Contract2 = "Contract2";

    /// <summary>A representative string message sent before a state change.</summary>
    private const string BeforeMessage = "Before";

    /// <summary>A representative string message sent after a state change.</summary>
    private const string AfterMessage = "After";

    /// <summary>A representative first ordered string message.</summary>
    private const string FirstMessage = "First";

    /// <summary>A representative second ordered string message.</summary>
    private const string SecondTextMessage = "Second";

    /// <summary>A representative third ordered string message.</summary>
    private const string ThirdMessage = "Third";

    /// <summary>A representative greeting string message.</summary>
    private const string HelloMessage = "Hello";

    /// <summary>A representative world string message.</summary>
    private const string WorldMessage = "World";

    /// <summary>A representative contract name used for scheduler-scoping tests.</summary>
    private const string TestContract = "TestContract";

    /// <summary>Tests that MessageBus.Current property can be get and set. Verifies the static Current property accessor functionality.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Current_CanGetAndSet()
    {
        var customBus = new MessageBusType();
        var original = MessageBusType.Current;

        try
        {
            MessageBusType.Current = customBus;
            await Assert.That(MessageBusType.Current).IsEqualTo(customBus);
        }
        finally
        {
            MessageBusType.Current = original;
        }
    }

    /// <summary>Tests that IsRegistered returns true after Listen is called. Verifies that subscribing via Listen registers the type.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task IsRegistered_AfterListen_ReturnsTrue()
    {
        var messageBus = new MessageBusType();

        messageBus.Listen<int>().Subscribe();

        await Assert.That(messageBus.IsRegistered(typeof(int))).IsTrue();
    }

    /// <summary>
    ///     Tests that IsRegistered returns true after a message is sent.
    ///     Verifies that sending a message registers the type in the message bus.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task IsRegistered_AfterSendMessage_ReturnsTrue()
    {
        var messageBus = new MessageBusType();

        messageBus.SendMessage(TestMessage1);

        await Assert.That(messageBus.IsRegistered(typeof(string))).IsTrue();
    }

    /// <summary>
    ///     Tests that IsRegistered returns false before any messages are sent.
    ///     Verifies initial state of message bus registration tracking.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task IsRegistered_BeforeMessages_ReturnsFalse()
    {
        var messageBus = new MessageBusType();

        await Assert.That(messageBus.IsRegistered(typeof(string))).IsFalse();
    }

    /// <summary>Tests that IsRegistered returns false for different types. Verifies that registration is type-specific.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task IsRegistered_DifferentType_ReturnsFalse()
    {
        var messageBus = new MessageBusType();

        messageBus.SendMessage(MessageValue);

        using (Assert.Multiple())
        {
            await Assert.That(messageBus.IsRegistered(typeof(int))).IsTrue();
            await Assert.That(messageBus.IsRegistered(typeof(string))).IsFalse();
        }
    }

    /// <summary>
    ///     Tests that IsRegistered with contract distinguishes between different contracts.
    ///     Verifies that registration checking respects contract boundaries.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task IsRegistered_WithContract_DistinguishesContracts()
    {
        var messageBus = new MessageBusType();

        messageBus.SendMessage(TestMessage1, Contract1);

        using (Assert.Multiple())
        {
            await Assert.That(messageBus.IsRegistered(typeof(string), Contract1)).IsTrue();
            await Assert.That(messageBus.IsRegistered(typeof(string), Contract2)).IsFalse();
            await Assert.That(messageBus.IsRegistered(typeof(string))).IsFalse();
        }
    }

    /// <summary>
    ///     Tests that Listen observable is cold until subscribed.
    ///     Verifies that Listen doesn't cause side effects until subscription.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Listen_ColdObservable_NoSideEffects()
    {
        var messageBus = new MessageBusType();

        var observable = messageBus.Listen<string>();

        await Assert.That(messageBus.IsRegistered(typeof(string))).IsFalse();

        observable.Subscribe();

        await Assert.That(messageBus.IsRegistered(typeof(string))).IsTrue();
    }

    /// <summary>
    ///     Tests that messages sent before subscription are not received by Listen.
    ///     Verifies that Listen uses Skip(1) to exclude historical messages.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Listen_MessagesSentBeforeSubscription_AreNotReceived()
    {
        var messageBus = new MessageBusType();

        messageBus.SendMessage("Before1");
        messageBus.SendMessage("Before2");

        var messages = messageBus.Listen<string>().Collect();

        messageBus.SendMessage(AfterMessage);

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo(AfterMessage);
        }
    }

    /// <summary>
    ///     Tests that Listen does not receive the initial default value.
    ///     Verifies that Listen skips the initial BehaviorSubject value and only receives explicit messages.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Listen_SkipsInitialValue()
    {
        var messageBus = new MessageBusType();
        var messages = messageBus.Listen<int>().Collect();

        await Assert.That(messages).IsEmpty();

        messageBus.SendMessage(MessageValue);
        await Assert.That(messages).Count().IsEqualTo(1);
        await Assert.That(messages[0]).IsEqualTo(MessageValue);
    }

    /// <summary>Tests that unsubscribing from Listen stops receiving messages. Verifies proper subscription disposal and cleanup.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Listen_Unsubscribe_StopsReceivingMessages()
    {
        var messageBus = new MessageBusType();
        var messages = new List<string>();
        var subscription = messageBus.Listen<string>().Subscribe(messages.Add);

        messageBus.SendMessage(BeforeMessage);
        subscription.Dispose();
        messageBus.SendMessage(AfterMessage);

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo(BeforeMessage);
        }
    }

    /// <summary>
    ///     Tests that ListenIncludeLatest receives the latest message sent before subscription.
    ///     Verifies BehaviorSubject replay behavior of ListenIncludeLatest.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ListenIncludeLatest_MessagesSentBeforeSubscription_ReceivesLatest()
    {
        var messageBus = new MessageBusType();

        messageBus.SendMessage(FirstMessage);
        messageBus.SendMessage(SecondTextMessage);
        messageBus.SendMessage(ThirdMessage);

        var messages = messageBus.ListenIncludeLatest<string>().Collect();

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo(ThirdMessage);
        }

        messageBus.SendMessage("Fourth");

        const int ExpectedCount = 2;
        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(ExpectedCount);
            await Assert.That(messages[1]).IsEqualTo("Fourth");
        }
    }

    /// <summary>
    ///     Tests that ListenIncludeLatest receives the initial default value.
    ///     Verifies that ListenIncludeLatest includes the BehaviorSubject's initial value.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ListenIncludeLatest_ReceivesInitialValue()
    {
        var messageBus = new MessageBusType();
        var messages = messageBus.ListenIncludeLatest<int>().Collect();

        await Assert.That(messages).Count().IsEqualTo(1);
        await Assert.That(messages[0]).IsEqualTo(0);

        messageBus.SendMessage(MessageValue);

        const int ExpectedCount = 2;
        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(ExpectedCount);
            await Assert.That(messages[0]).IsEqualTo(0);
            await Assert.That(messages[1]).IsEqualTo(MessageValue);
        }
    }

    /// <summary>
    ///     Tests that ListenIncludeLatest receives the latest message when subscribing after a message was sent.
    ///     Verifies the BehaviorSubject replay semantics of ListenIncludeLatest.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ListenIncludeLatest_ReceivesLatestMessage()
    {
        var messageBus = new MessageBusType();

        messageBus.SendMessage(FirstMessage);
        messageBus.SendMessage(SecondTextMessage);

        var messages = messageBus.ListenIncludeLatest<string>().Collect();

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo(SecondTextMessage);
        }
    }

    /// <summary>
    ///     Tests that disposing the RegisterMessageSource subscription stops sending messages.
    ///     Verifies that the returned IDisposable properly unsubscribes from the source observable.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterMessageSource_Dispose_StopsSendingMessages()
    {
        var messageBus = new MessageBusType();
        var source = new Signal<string>();
        var messages = messageBus.Listen<string>().Collect();

        var subscription = messageBus.RegisterMessageSource(source);

        source.OnNext(BeforeMessage);
        subscription.Dispose();
        source.OnNext(AfterMessage);

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo(BeforeMessage);
        }
    }

    /// <summary>Tests that RegisterMessageSource throws on null source. Verifies proper argument validation in RegisterMessageSource.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterMessageSource_NullSource_ThrowsArgumentNullException()
    {
        var messageBus = new MessageBusType();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            messageBus.RegisterMessageSource<string>(null!);
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Tests that RegisterMessageSource handles observable completion.
    ///     Verifies that completing the source observable unsubscribes cleanly.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterMessageSource_ObservableComplete_UnsubscribesCorrectly()
    {
        var messageBus = new MessageBusType();
        var source = new Signal<string>();
        var messages = messageBus.Listen<string>().Collect();

        messageBus.RegisterMessageSource(source);

        source.OnNext(BeforeMessage);
        source.OnCompleted();

        messageBus.SendMessage(AfterMessage);

        const int ExpectedCount = 2;
        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(ExpectedCount);
            await Assert.That(messages[0]).IsEqualTo(BeforeMessage);
            await Assert.That(messages[1]).IsEqualTo(AfterMessage);
        }
    }

    /// <summary>
    ///     Tests that RegisterMessageSource handles observable errors gracefully.
    ///     Verifies that errors in the source observable don't break the message bus.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterMessageSource_ObservableError_DoesNotBreakBus()
    {
        var messageBus = new MessageBusType();
        var source = new Signal<string>();
        var messages = messageBus.Listen<string>().Collect();

        messageBus.RegisterMessageSource(source);

        source.OnNext(BeforeMessage);
        source.OnError(new InvalidOperationException("Test error"));

        messageBus.SendMessage(AfterMessage);

        const int ExpectedCount = 2;
        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(ExpectedCount);
            await Assert.That(messages[0]).IsEqualTo(BeforeMessage);
            await Assert.That(messages[1]).IsEqualTo(AfterMessage);
        }
    }

    /// <summary>
    ///     Tests that RegisterMessageSource subscribes to the source observable and sends messages.
    ///     Verifies that RegisterMessageSource properly bridges an observable to the message bus.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterMessageSource_SendsMessagesFromObservable()
    {
        var messageBus = new MessageBusType();
        var source = new Signal<string>();
        var messages = messageBus.Listen<string>().Collect();

        messageBus.RegisterMessageSource(source);

        source.OnNext(FirstMessage);
        source.OnNext(SecondTextMessage);
        source.OnNext(ThirdMessage);

        const int ExpectedCount = 3;
        const int ThirdIndex = 2;
        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(ExpectedCount);
            await Assert.That(messages[0]).IsEqualTo(FirstMessage);
            await Assert.That(messages[1]).IsEqualTo(SecondTextMessage);
            await Assert.That(messages[ThirdIndex]).IsEqualTo(ThirdMessage);
        }
    }

    /// <summary>
    ///     Tests that RegisterMessageSource with contract sends to the correct listeners.
    ///     Verifies that contract parameter is properly passed through when registering a message source.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterMessageSource_WithContract_SendsToCorrectListeners()
    {
        var messageBus = new MessageBusType();
        var source = new Signal<int>();
        var contractMessages = messageBus.Listen<int>("MyContract").Collect();
        var noContractMessages = messageBus.Listen<int>().Collect();

        const int SecondMessage = 2;
        const int ExpectedCount = 2;

        messageBus.RegisterMessageSource(source, "MyContract");

        source.OnNext(1);
        source.OnNext(SecondMessage);

        using (Assert.Multiple())
        {
            await Assert.That(contractMessages).Count().IsEqualTo(ExpectedCount);
            await Assert.That(contractMessages[0]).IsEqualTo(1);
            await Assert.That(contractMessages[1]).IsEqualTo(SecondMessage);
            await Assert.That(noContractMessages).IsEmpty();
        }
    }

    /// <summary>
    ///     Tests that RegisterScheduler affects message delivery scheduler.
    ///     Verifies that messages are delivered on the registered scheduler.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterScheduler_AffectsMessageDelivery()
    {
        var messageBus = new MessageBusType();
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        messageBus.RegisterScheduler<string>(scheduler);
        var messages = messageBus.Listen<string>().Collect();

        messageBus.SendMessage(TestMessage1);

        await Assert.That(messages).IsEmpty();

        scheduler.AdvanceBy(TimeSpan.FromTicks(1));

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo(TestMessage1);
        }
    }

    /// <summary>
    ///     Tests that multiple RegisterScheduler operations on the same type-contract overwrite previous registrations.
    ///     Verifies that scheduler registration follows last-wins semantics.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterScheduler_Multiple_OverwritesPrevious()
    {
        var messageBus = new MessageBusType();
        var scheduler1 = TestContext.Current.GetVirtualTimeScheduler();
        var scheduler2 = new VirtualTimeScheduler();

        messageBus.RegisterScheduler<string>(scheduler1);
        messageBus.RegisterScheduler<string>(scheduler2);

        var messages = messageBus.Listen<string>().Collect();

        messageBus.SendMessage(TestMessage1);

        await Assert.That(messages).IsEmpty();

        scheduler1.AdvanceBy(TimeSpan.FromTicks(1));
        await Assert.That(messages).IsEmpty();

        scheduler2.AdvanceBy(TimeSpan.FromTicks(1));

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo(TestMessage1);
        }
    }

    /// <summary>
    ///     Tests that RegisterScheduler with contract only affects messages with that contract.
    ///     Verifies that scheduler registration is scoped to specific type-contract combinations.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterScheduler_WithContract_OnlyAffectsContract()
    {
        var messageBus = new MessageBusType();
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        messageBus.RegisterScheduler<string>(scheduler, TestContract);
        var contractMessages = messageBus.Listen<string>(TestContract).Collect();
        var normalMessages = messageBus.Listen<string>().Collect();

        messageBus.SendMessage("Contract", TestContract);
        messageBus.SendMessage("Normal");

        using (Assert.Multiple())
        {
            await Assert.That(contractMessages).IsEmpty();
            await Assert.That(normalMessages).Count().IsEqualTo(1);
            await Assert.That(normalMessages[0]).IsEqualTo("Normal");
        }

        scheduler.AdvanceBy(TimeSpan.FromTicks(1));

        using (Assert.Multiple())
        {
            await Assert.That(contractMessages).Count().IsEqualTo(1);
            await Assert.That(contractMessages[0]).IsEqualTo("Contract");
        }
    }

    /// <summary>Tests that complex objects work as messages. Verifies support for custom class types as messages.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_ComplexObject_WorksCorrectly()
    {
        var messageBus = new MessageBusType();
        var messages = messageBus.Listen<TestMessage>().Collect();

        const int SecondId = 2;
        const int ExpectedCount = 2;

        var msg1 = new TestMessage { Id = 1, Text = FirstMessage };
        var msg2 = new TestMessage { Id = SecondId, Text = SecondTextMessage };

        messageBus.SendMessage(msg1);
        messageBus.SendMessage(msg2);

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(ExpectedCount);
            await Assert.That(messages[0]).IsEqualTo(msg1);
            await Assert.That(messages[1]).IsEqualTo(msg2);
            await Assert.That(messages[0].Id).IsEqualTo(1);
            await Assert.That(messages[0].Text).IsEqualTo(FirstMessage);
            await Assert.That(messages[1].Id).IsEqualTo(SecondId);
            await Assert.That(messages[1].Text).IsEqualTo(SecondTextMessage);
        }
    }

    /// <summary>Tests concurrent SendMessage calls from multiple threads. Verifies thread-safety of message bus operations.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_ConcurrentCalls_ThreadSafe()
    {
        var messageBus = new MessageBusType();
        var messages = messageBus.Listen<int>().Collect();

        const int MessageCount = 100;
        var tasks = Enumerable.Range(0, MessageCount).Select(i => Task.Run(() => messageBus.SendMessage(i))).ToArray();

        await Task.WhenAll(tasks);

        await Assert.That(messages).Count().IsEqualTo(MessageCount);
    }

    /// <summary>
    ///     Tests that different message types are independent.
    ///     Verifies that messages of different types are delivered to their respective subscribers only.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_DifferentTypes_AreIndependent()
    {
        var messageBus = new MessageBusType();
        var stringMessages = messageBus.Listen<string>().Collect();
        var intMessages = messageBus.Listen<int>().Collect();

        const int ExpectedCount = 2;

        messageBus.SendMessage(HelloMessage);
        messageBus.SendMessage(MessageValue);
        messageBus.SendMessage(WorldMessage);
        messageBus.SendMessage(SecondMessageValue);

        using (Assert.Multiple())
        {
            await Assert.That(stringMessages).Count().IsEqualTo(ExpectedCount);
            await Assert.That(stringMessages[0]).IsEqualTo(HelloMessage);
            await Assert.That(stringMessages[1]).IsEqualTo(WorldMessage);
            await Assert.That(intMessages).Count().IsEqualTo(ExpectedCount);
            await Assert.That(intMessages[0]).IsEqualTo(MessageValue);
            await Assert.That(intMessages[1]).IsEqualTo(SecondMessageValue);
        }
    }

    /// <summary>
    ///     Tests that null contract and empty string contract are different.
    ///     Verifies that null and empty string are treated as distinct contract values.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_Listen_NullVsEmptyContract_AreDifferent()
    {
        var messageBus = new MessageBusType();
        var nullMessages = messageBus.Listen<string>().Collect();
        var emptyMessages = messageBus.Listen<string>(string.Empty).Collect();

        messageBus.SendMessage("Null");
        messageBus.SendMessage("Empty", string.Empty);

        using (Assert.Multiple())
        {
            await Assert.That(nullMessages).Count().IsEqualTo(1);
            await Assert.That(nullMessages[0]).IsEqualTo("Null");
            await Assert.That(emptyMessages).Count().IsEqualTo(1);
            await Assert.That(emptyMessages[0]).IsEqualTo("Empty");
        }
    }

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
        messageBus.Listen<int?>().Subscribe(messages.Add);

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
        messageBus.Listen<string?>().Subscribe(messages.Add);

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

    /// <summary>Test message class for complex object testing.</summary>
    private sealed class TestMessage
    {
        /// <summary>Gets or sets the message identifier.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the message text content.</summary>
        public string? Text { get; set; }
    }
}
