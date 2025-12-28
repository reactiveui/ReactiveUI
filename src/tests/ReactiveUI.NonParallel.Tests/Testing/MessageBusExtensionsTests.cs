// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Testing;

namespace ReactiveUI.Tests.Testing;

/// <summary>
/// Tests for MessageBusExtensions.
/// </summary>
[NotInParallel]
public class MessageBusExtensionsTests
{
    private MessageBusScope _scope = null!;

    [Before(Test)]
    public void SetUp() => _scope = new MessageBusScope();

    [After(Test)]
    public void TearDown() => _scope.Dispose();

    [Test]
    public async Task With_Action_Executes_Block_With_Custom_MessageBus()
    {
        var customBus = new MessageBus();
        var executed = false;

        customBus.With(() => executed = true);

        await Assert.That(executed).IsTrue();
    }

    [Test]
    public async Task With_Func_Executes_Block_With_Custom_MessageBus_And_Returns_Value()
    {
        var customBus = new MessageBus();
        var result = customBus.With(() => 42);

        await Assert.That(result).IsEqualTo(42);
    }

    [Test]
    public void With_Action_Throws_When_Block_Is_Null()
    {
        var customBus = new MessageBus();
        Assert.Throws<ArgumentNullException>(() => customBus.With((Action)null!));
    }

    [Test]
    public void With_Func_Throws_When_Block_Is_Null()
    {
        var customBus = new MessageBus();
        Assert.Throws<ArgumentNullException>(() => customBus.With((Func<int>)null!));
    }

    [Test]
    public async Task WithMessageBus_Restores_Original_MessageBus_After_Disposal()
    {
        var originalBus = MessageBus.Current;
        var customBus = new MessageBus();

        using (customBus.WithMessageBus())
        {
            await Assert.That(MessageBus.Current).IsSameReferenceAs(customBus);
        }

        await Assert.That(MessageBus.Current).IsSameReferenceAs(originalBus);
    }

    private sealed class MessageBusScope : IDisposable
    {
        private readonly IMessageBus _originalMessageBus;

        public MessageBusScope() => _originalMessageBus = MessageBus.Current;

        public void Dispose() => MessageBus.Current = _originalMessageBus;
    }
}
