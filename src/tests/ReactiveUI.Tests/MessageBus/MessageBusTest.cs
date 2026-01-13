// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;
using ReactiveUI.Tests.Utilities.MessageBus;

namespace ReactiveUI.Tests.MessageBus;

/// <summary>
///     Comprehensive test suite for MessageBus.
///     Tests cover all public methods, contracts, scheduling, and edge cases.
/// </summary>
[NotInParallel]
[TestExecutor<WithMessageBusExecutor>]
public class MessageBusTest
{
    /// <summary>
    ///     Tests that MessageBus.Current property can be get and set.
    ///     Verifies the static Current property accessor functionality.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Current_CanGetAndSet()
    {
        var customBus = new ReactiveUI.MessageBus();
        var original = ReactiveUI.MessageBus.Current;

        try
        {
            ReactiveUI.MessageBus.Current = customBus;
            await Assert.That(ReactiveUI.MessageBus.Current).IsEqualTo(customBus);
        }
        finally
        {
            ReactiveUI.MessageBus.Current = original;
        }
    }

    /// <summary>
    ///     Tests that IsRegistered returns true after Listen is called.
    ///     Verifies that subscribing via Listen registers the type.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task IsRegistered_AfterListen_ReturnsTrue()
    {
        var messageBus = new ReactiveUI.MessageBus();

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
        var messageBus = new ReactiveUI.MessageBus();

        messageBus.SendMessage("Test");

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
        var messageBus = new ReactiveUI.MessageBus();

        await Assert.That(messageBus.IsRegistered(typeof(string))).IsFalse();
    }

    /// <summary>
    ///     Tests that IsRegistered returns false for different types.
    ///     Verifies that registration is type-specific.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task IsRegistered_DifferentType_ReturnsFalse()
    {
        var messageBus = new ReactiveUI.MessageBus();

        messageBus.SendMessage(42);

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
        var messageBus = new ReactiveUI.MessageBus();

        messageBus.SendMessage("Test", "Contract1");

        using (Assert.Multiple())
        {
            await Assert.That(messageBus.IsRegistered(typeof(string), "Contract1")).IsTrue();
            await Assert.That(messageBus.IsRegistered(typeof(string), "Contract2")).IsFalse();
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
        var messageBus = new ReactiveUI.MessageBus();

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
        var messageBus = new ReactiveUI.MessageBus();

        messageBus.SendMessage("Before1");
        messageBus.SendMessage("Before2");

        messageBus.Listen<string>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var messages)
            .Subscribe();

        messageBus.SendMessage("After");

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo("After");
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
        var messageBus = new ReactiveUI.MessageBus();
        messageBus.Listen<int>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var messages).Subscribe();

        await Assert.That(messages).IsEmpty();

        messageBus.SendMessage(42);
        await Assert.That(messages).Count().IsEqualTo(1);
        await Assert.That(messages[0]).IsEqualTo(42);
    }

    /// <summary>
    ///     Tests that unsubscribing from Listen stops receiving messages.
    ///     Verifies proper subscription disposal and cleanup.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Listen_Unsubscribe_StopsReceivingMessages()
    {
        var messageBus = new ReactiveUI.MessageBus();
        var subscription = messageBus.Listen<string>().ToObservableChangeSet(ImmediateScheduler.Instance)
            .Bind(out var messages).Subscribe();

        messageBus.SendMessage("Before");
        subscription.Dispose();
        messageBus.SendMessage("After");

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo("Before");
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
        var messageBus = new ReactiveUI.MessageBus();

        messageBus.SendMessage("First");
        messageBus.SendMessage("Second");
        messageBus.SendMessage("Third");

        messageBus.ListenIncludeLatest<string>().ToObservableChangeSet(ImmediateScheduler.Instance)
            .Bind(out var messages).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo("Third");
        }

        messageBus.SendMessage("Fourth");

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(2);
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
        var messageBus = new ReactiveUI.MessageBus();
        messageBus.ListenIncludeLatest<int>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var messages)
            .Subscribe();

        await Assert.That(messages).Count().IsEqualTo(1);
        await Assert.That(messages[0]).IsEqualTo(0);

        messageBus.SendMessage(42);

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(2);
            await Assert.That(messages[0]).IsEqualTo(0);
            await Assert.That(messages[1]).IsEqualTo(42);
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
        var messageBus = new ReactiveUI.MessageBus();

        messageBus.SendMessage("First");
        messageBus.SendMessage("Second");

        messageBus.ListenIncludeLatest<string>().ToObservableChangeSet(ImmediateScheduler.Instance)
            .Bind(out var messages).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo("Second");
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
        var messageBus = new ReactiveUI.MessageBus();
        var source = new Subject<string>();
        messageBus.Listen<string>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var messages)
            .Subscribe();

        var subscription = messageBus.RegisterMessageSource(source);

        source.OnNext("Before");
        subscription.Dispose();
        source.OnNext("After");

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo("Before");
        }
    }

    /// <summary>
    ///     Tests that RegisterMessageSource throws on null source.
    ///     Verifies proper argument validation in RegisterMessageSource.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterMessageSource_NullSource_ThrowsArgumentNullException()
    {
        var messageBus = new ReactiveUI.MessageBus();

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
        var messageBus = new ReactiveUI.MessageBus();
        var source = new Subject<string>();
        messageBus.Listen<string>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var messages)
            .Subscribe();

        messageBus.RegisterMessageSource(source);

        source.OnNext("Before");
        source.OnCompleted();

        messageBus.SendMessage("After");

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(2);
            await Assert.That(messages[0]).IsEqualTo("Before");
            await Assert.That(messages[1]).IsEqualTo("After");
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
        var messageBus = new ReactiveUI.MessageBus();
        var source = new Subject<string>();
        messageBus.Listen<string>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var messages)
            .Subscribe();

        messageBus.RegisterMessageSource(source);

        source.OnNext("Before");
        source.OnError(new InvalidOperationException("Test error"));

        messageBus.SendMessage("After");

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(2);
            await Assert.That(messages[0]).IsEqualTo("Before");
            await Assert.That(messages[1]).IsEqualTo("After");
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
        var messageBus = new ReactiveUI.MessageBus();
        var source = new Subject<string>();
        messageBus.Listen<string>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var messages)
            .Subscribe();

        messageBus.RegisterMessageSource(source);

        source.OnNext("First");
        source.OnNext("Second");
        source.OnNext("Third");

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(3);
            await Assert.That(messages[0]).IsEqualTo("First");
            await Assert.That(messages[1]).IsEqualTo("Second");
            await Assert.That(messages[2]).IsEqualTo("Third");
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
        var messageBus = new ReactiveUI.MessageBus();
        var source = new Subject<int>();
        messageBus.Listen<int>("MyContract").ToObservableChangeSet(ImmediateScheduler.Instance)
            .Bind(out var contractMessages).Subscribe();
        messageBus.Listen<int>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var noContractMessages)
            .Subscribe();

        messageBus.RegisterMessageSource(source, "MyContract");

        source.OnNext(1);
        source.OnNext(2);

        using (Assert.Multiple())
        {
            await Assert.That(contractMessages).Count().IsEqualTo(2);
            await Assert.That(contractMessages[0]).IsEqualTo(1);
            await Assert.That(contractMessages[1]).IsEqualTo(2);
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
        var messageBus = new ReactiveUI.MessageBus();
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        messageBus.RegisterScheduler<string>(scheduler);
        messageBus.Listen<string>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var messages)
            .Subscribe();

        messageBus.SendMessage("Test");

        await Assert.That(messages).IsEmpty();

        scheduler.AdvanceBy(TimeSpan.FromTicks(1));

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo("Test");
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
        var messageBus = new ReactiveUI.MessageBus();
        var scheduler1 = TestContext.Current.GetVirtualTimeScheduler();
        var scheduler2 = TestContext.Current.GetVirtualTimeScheduler();

        messageBus.RegisterScheduler<string>(scheduler1);
        messageBus.RegisterScheduler<string>(scheduler2);

        messageBus.Listen<string>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var messages)
            .Subscribe();

        messageBus.SendMessage("Test");

        await Assert.That(messages).IsEmpty();

        scheduler1.AdvanceBy(TimeSpan.FromTicks(1));
        await Assert.That(messages).IsEmpty();

        scheduler2.AdvanceBy(TimeSpan.FromTicks(1));

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(1);
            await Assert.That(messages[0]).IsEqualTo("Test");
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
        var messageBus = new ReactiveUI.MessageBus();
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        messageBus.RegisterScheduler<string>(scheduler, "TestContract");
        messageBus.Listen<string>("TestContract").ToObservableChangeSet(ImmediateScheduler.Instance)
            .Bind(out var contractMessages).Subscribe();
        messageBus.Listen<string>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var normalMessages)
            .Subscribe();

        messageBus.SendMessage("Contract", "TestContract");
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

    /// <summary>
    ///     Tests that complex objects work as messages.
    ///     Verifies support for custom class types as messages.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_ComplexObject_WorksCorrectly()
    {
        var messageBus = new ReactiveUI.MessageBus();
        messageBus.Listen<TestMessage>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var messages)
            .Subscribe();

        var msg1 = new TestMessage { Id = 1, Text = "First" };
        var msg2 = new TestMessage { Id = 2, Text = "Second" };

        messageBus.SendMessage(msg1);
        messageBus.SendMessage(msg2);

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(2);
            await Assert.That(messages[0]).IsEqualTo(msg1);
            await Assert.That(messages[1]).IsEqualTo(msg2);
            await Assert.That(messages[0].Id).IsEqualTo(1);
            await Assert.That(messages[0].Text).IsEqualTo("First");
            await Assert.That(messages[1].Id).IsEqualTo(2);
            await Assert.That(messages[1].Text).IsEqualTo("Second");
        }
    }

    /// <summary>
    ///     Tests concurrent SendMessage calls from multiple threads.
    ///     Verifies thread-safety of message bus operations.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_ConcurrentCalls_ThreadSafe()
    {
        var messageBus = new ReactiveUI.MessageBus();
        messageBus.Listen<int>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var messages).Subscribe();

        var tasks = Enumerable.Range(0, 100).Select(i => Task.Run(() => messageBus.SendMessage(i))).ToArray();

        await Task.WhenAll(tasks);

        await Assert.That(messages).Count().IsEqualTo(100);
    }

    /// <summary>
    ///     Tests that different message types are independent.
    ///     Verifies that messages of different types are delivered to their respective subscribers only.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_DifferentTypes_AreIndependent()
    {
        var messageBus = new ReactiveUI.MessageBus();
        messageBus.Listen<string>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var stringMessages)
            .Subscribe();
        messageBus.Listen<int>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var intMessages)
            .Subscribe();

        messageBus.SendMessage("Hello");
        messageBus.SendMessage(42);
        messageBus.SendMessage("World");
        messageBus.SendMessage(100);

        using (Assert.Multiple())
        {
            await Assert.That(stringMessages).Count().IsEqualTo(2);
            await Assert.That(stringMessages[0]).IsEqualTo("Hello");
            await Assert.That(stringMessages[1]).IsEqualTo("World");
            await Assert.That(intMessages).Count().IsEqualTo(2);
            await Assert.That(intMessages[0]).IsEqualTo(42);
            await Assert.That(intMessages[1]).IsEqualTo(100);
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
        var messageBus = new ReactiveUI.MessageBus();
        messageBus.Listen<string>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var nullMessages)
            .Subscribe();
        messageBus.Listen<string>(string.Empty).ToObservableChangeSet(ImmediateScheduler.Instance)
            .Bind(out var emptyMessages).Subscribe();

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
        var messageBus = new ReactiveUI.MessageBus();
        messageBus.Listen<string>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var messages)
            .Subscribe();

        messageBus.SendMessage("Hello");
        messageBus.SendMessage("World");

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(2);
            await Assert.That(messages[0]).IsEqualTo("Hello");
            await Assert.That(messages[1]).IsEqualTo("World");
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
        var messageBus = new ReactiveUI.MessageBus();
        messageBus.Listen<string>("Contract1").ToObservableChangeSet(ImmediateScheduler.Instance)
            .Bind(out var messages1).Subscribe();
        messageBus.Listen<string>("Contract2").ToObservableChangeSet(ImmediateScheduler.Instance)
            .Bind(out var messages2).Subscribe();

        messageBus.SendMessage("Message1", "Contract1");
        messageBus.SendMessage("Message2", "Contract2");
        messageBus.SendMessage("Message3", "Contract1");

        using (Assert.Multiple())
        {
            await Assert.That(messages1).Count().IsEqualTo(2);
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
        var messageBus = new ReactiveUI.MessageBus();
        messageBus.Listen<int>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var subscriber1)
            .Subscribe();
        messageBus.Listen<int>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var subscriber2)
            .Subscribe();
        messageBus.Listen<int>().ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var subscriber3)
            .Subscribe();

        messageBus.SendMessage(42);

        using (Assert.Multiple())
        {
            await Assert.That(subscriber1).Count().IsEqualTo(1);
            await Assert.That(subscriber1[0]).IsEqualTo(42);
            await Assert.That(subscriber2).Count().IsEqualTo(1);
            await Assert.That(subscriber2[0]).IsEqualTo(42);
            await Assert.That(subscriber3).Count().IsEqualTo(1);
            await Assert.That(subscriber3[0]).IsEqualTo(42);
        }
    }

    /// <summary>
    ///     Tests that nullable value types work correctly.
    ///     Verifies support for Nullable&lt;T&gt; message types.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_NullableValueType_WorksCorrectly()
    {
        var messageBus = new ReactiveUI.MessageBus();
        messageBus.RegisterScheduler<int?>(ImmediateScheduler.Instance);
        var messages = new List<int?>();
        messageBus.Listen<int?>().Subscribe(messages.Add);

        messageBus.SendMessage<int?>(42);
        messageBus.SendMessage<int?>(null);
        messageBus.SendMessage<int?>(100);

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(3);
            await Assert.That(messages[0]).IsEqualTo(42);
            await Assert.That(messages[1]).IsNull();
            await Assert.That(messages[2]).IsEqualTo(100);
        }
    }

    /// <summary>
    ///     Tests that reference type null values work correctly.
    ///     Verifies support for null reference type messages.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SendMessage_NullReferenceType_WorksCorrectly()
    {
        var messageBus = new ReactiveUI.MessageBus();
        messageBus.RegisterScheduler<string?>(ImmediateScheduler.Instance);
        var messages = new List<string?>();
        messageBus.Listen<string?>().Subscribe(messages.Add);

        messageBus.SendMessage<string?>("Hello");
        messageBus.SendMessage<string?>(null);
        messageBus.SendMessage<string?>("World");

        using (Assert.Multiple())
        {
            await Assert.That(messages).Count().IsEqualTo(3);
            await Assert.That(messages[0]).IsEqualTo("Hello");
            await Assert.That(messages[1]).IsNull();
            await Assert.That(messages[2]).IsEqualTo("World");
        }
    }

    /// <summary>
    ///     Test message class for complex object testing.
    /// </summary>
    private class TestMessage
    {
        /// <summary>
        ///     Gets or sets the message identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the message text content.
        /// </summary>
        public string? Text { get; set; }
    }
}
