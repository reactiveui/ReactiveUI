// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Infrastructure.StaticState;

/// <summary>
/// A disposable scope that snapshots and restores MessageBus.Current static state.
/// Use this in test fixtures that read or modify MessageBus.Current to ensure
/// static state is properly restored after tests complete.
/// </summary>
/// <remarks>
/// This helper is necessary because MessageBus.Current maintains a static/global reference
/// that can leak between parallel test executions, causing intermittent failures.
/// Tests using this scope should also be marked with [NonParallelizable] to prevent
/// concurrent modifications to the shared state.
/// </remarks>
/// <example>
/// <code>
/// [TestFixture]
/// [NonParallelizable]
/// public class MyTests
/// {
///     private MessageBusScope _messageBusScope;
///
///     [SetUp]
///     public void SetUp()
///     {
///         _messageBusScope = new MessageBusScope();
///         // Now safe to use MessageBus.Current
///     }
///
///     [TearDown]
///     public void TearDown()
///     {
///         _messageBusScope?.Dispose();
///     }
/// }
/// </code>
/// </example>
public sealed class MessageBusScope : IDisposable
{
    private readonly IMessageBus _previousMessageBus;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBusScope"/> class.
    /// Snapshots the current MessageBus.Current state.
    /// </summary>
    public MessageBusScope()
    {
        _previousMessageBus = MessageBus.Current;
    }

    /// <summary>
    /// Restores the MessageBus.Current state to what it was when this scope was created.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        MessageBus.Current = _previousMessageBus;
        _disposed = true;
    }
}
