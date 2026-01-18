// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.MessageBus;

using TUnit.Core.Executors;

namespace ReactiveUI.Testing.Tests;

/// <summary>
///     Tests for <see cref="MessageBusExtensions"/> which provides testing utilities
///     for overriding the default MessageBus during tests.
/// </summary>
[NotInParallel]
[TestExecutor<WithMessageBusExecutor>]
public class MessageBusExtensionsTests
{
    [Test]
    public async Task WithMessageBus_RestoresOriginalMessageBus_WhenDisposed()
    {
        // Arrange
        var originalBus = MessageBus.Current;
        var testBus = new MessageBus();

        // Act
        using (testBus.WithMessageBus())
        {
            // Message bus should be changed
            await Assert.That(MessageBus.Current).IsSameReferenceAs(testBus);
        }

        // Assert - Original should be restored
        await Assert.That(MessageBus.Current).IsSameReferenceAs(originalBus);
    }

    [Test]
    public async Task WithMessageBus_AllowsMessageBusOperations()
    {
        // Arrange
        var testBus = new MessageBus();
        var messageReceived = false;

        using (testBus.WithMessageBus())
        {
            MessageBus.Current.Listen<string>().Subscribe(msg => messageReceived = true);

            // Act
            MessageBus.Current.SendMessage("test");
        }

        // Assert
        await Assert.That(messageReceived).IsTrue();
    }

    [Test]
    public async Task With_Action_ExecutesActionWithMessageBus()
    {
        // Arrange
        var testBus = new MessageBus();
        var executed = false;
        IMessageBus? capturedBus = null;

        // Act
        testBus.With(() =>
        {
            executed = true;
            capturedBus = MessageBus.Current;
        });

        // Assert
        await Assert.That(executed).IsTrue();
        await Assert.That(capturedBus).IsSameReferenceAs(testBus);
    }

    [Test]
    public async Task With_Action_RestoresOriginalMessageBus()
    {
        // Arrange
        var originalBus = MessageBus.Current;
        var testBus = new MessageBus();

        // Act
        testBus.With(() => { /* Do nothing */ });

        // Assert
        await Assert.That(MessageBus.Current).IsSameReferenceAs(originalBus);
    }

    [Test]
    public async Task With_Action_ThrowsArgumentNullException_WhenBlockIsNull()
    {
        // Arrange
        var testBus = new MessageBus();

        // Act & Assert
        await Assert.That(() => testBus.With((Action)null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task With_Function_ExecutesFunctionWithMessageBus()
    {
        // Arrange
        var testBus = new MessageBus();
        IMessageBus? capturedBus = null;

        // Act
        var result = testBus.With(() =>
        {
            capturedBus = MessageBus.Current;
            return 42;
        });

        // Assert
        await Assert.That(result).IsEqualTo(42);
        await Assert.That(capturedBus).IsSameReferenceAs(testBus);
    }

    [Test]
    public async Task With_Function_RestoresOriginalMessageBus()
    {
        // Arrange
        var originalBus = MessageBus.Current;
        var testBus = new MessageBus();

        // Act
        testBus.With(() => 42);

        // Assert
        await Assert.That(MessageBus.Current).IsSameReferenceAs(originalBus);
    }

    [Test]
    public async Task With_Function_ThrowsArgumentNullException_WhenBlockIsNull()
    {
        // Arrange
        var testBus = new MessageBus();

        // Act & Assert
        await Assert.That(() => testBus.With((Func<int>)null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task With_Function_ReturnsCorrectValue()
    {
        // Arrange
        var testBus = new MessageBus();

        // Act
        var result = testBus.With(() => "test result");

        // Assert
        await Assert.That(result).IsEqualTo("test result");
    }

    [Test]
    public async Task WithMessageBus_CanBeNested()
    {
        // Arrange
        var originalBus = MessageBus.Current;
        var testBus1 = new MessageBus();
        var testBus2 = new MessageBus();

        // Act
        using (testBus1.WithMessageBus())
        {
            await Assert.That(MessageBus.Current).IsSameReferenceAs(testBus1);

            using (testBus2.WithMessageBus())
            {
                await Assert.That(MessageBus.Current).IsSameReferenceAs(testBus2);
            }

            await Assert.That(MessageBus.Current).IsSameReferenceAs(testBus1);
        }

        // Assert
        await Assert.That(MessageBus.Current).IsSameReferenceAs(originalBus);
    }

    [Test]
    public async Task With_PropagatesExceptions()
    {
        // Arrange
        var testBus = new MessageBus();

        // Act & Assert
        await Assert.That(() => testBus.With(() =>
        {
            throw new InvalidOperationException("Test exception");
        })).Throws<InvalidOperationException>();
    }
}
