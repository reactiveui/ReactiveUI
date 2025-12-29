// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Testing;
using ReactiveUI.Tests.Infrastructure.StaticState;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for <see cref="MessageBusExtensions"/>.
/// </summary>
[NotInParallel]
public class MessageBusExtensionsTest
{
    private MessageBusScope? _messageBusScope;

    [Before(Test)]
    public void SetUp()
    {
        _messageBusScope = new MessageBusScope();
    }

    [After(Test)]
    public void TearDown()
    {
        _messageBusScope?.Dispose();
    }

    /// <summary>
    /// Tests that WithMessageBus overrides the message bus.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithMessageBus_OverridesMessageBus()
    {
        var originalBus = MessageBus.Current;
        var testBus = new MessageBus();

        using (testBus.WithMessageBus())
        {
            await Assert.That(MessageBus.Current).IsEqualTo(testBus);
        }

        await Assert.That(MessageBus.Current).IsEqualTo(originalBus);
    }

    /// <summary>
    /// Tests that With action executes the action.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task With_Action_ExecutesAction()
    {
        var testBus = new MessageBus();
        var executed = false;

        testBus.With(() => executed = true);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Tests that With function returns the result.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task With_Function_ReturnsResult()
    {
        var testBus = new MessageBus();

        var result = testBus.With(() => 42);

        await Assert.That(result).IsEqualTo(42);
    }

    /// <summary>
    /// Tests that With function throws for null function.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task With_NullFunction_Throws()
    {
        var testBus = new MessageBus();

        await Assert.That(() => testBus.With((Func<int>)null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that With action throws for null action.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task With_NullAction_Throws()
    {
        var testBus = new MessageBus();

        await Assert.That(() => testBus.With((Action)null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that With function overrides message bus during execution.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task With_Function_OverridesMessageBusDuringExecution()
    {
        var originalBus = MessageBus.Current;
        var testBus = new MessageBus();
        IMessageBus? capturedBus = null;

        testBus.With(() =>
        {
            capturedBus = MessageBus.Current;
            return 42;
        });

        await Assert.That(capturedBus).IsEqualTo(testBus);
        await Assert.That(MessageBus.Current).IsEqualTo(originalBus);
    }

    /// <summary>
    /// Tests that With action overrides message bus during execution.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task With_Action_OverridesMessageBusDuringExecution()
    {
        var originalBus = MessageBus.Current;
        var testBus = new MessageBus();
        IMessageBus? capturedBus = null;

        testBus.With(() =>
        {
            capturedBus = MessageBus.Current;
        });

        await Assert.That(capturedBus).IsEqualTo(testBus);
        await Assert.That(MessageBus.Current).IsEqualTo(originalBus);
    }
}
