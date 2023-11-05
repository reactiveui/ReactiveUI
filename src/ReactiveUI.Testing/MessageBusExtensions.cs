// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Testing;

/// <summary>
/// Message bus testing extensions.
/// </summary>
public static class MessageBusExtensions
{
    private static readonly object mbGate = 42;

    /// <summary>
    /// Override the default Message Bus during the specified block.
    /// </summary>
    /// <typeparam name="TRet">The return type.</typeparam>
    /// <param name="messageBus">The message bus to use for the block.</param>
    /// <param name="block">The function to execute.</param>
    /// <returns>The return value of the function.</returns>
    public static TRet With<TRet>(this IMessageBus messageBus, Func<TRet> block)
    {
        if (block is null)
        {
            throw new ArgumentNullException(nameof(block));
        }

        using (messageBus.WithMessageBus())
        {
            return block();
        }
    }

    /// <summary>
    /// WithMessageBus allows you to override the default Message Bus
    /// implementation until the object returned is disposed. If a
    /// message bus is not specified, a default empty one is created.
    /// </summary>
    /// <param name="messageBus">The message bus to use, or null to create
    /// a new one using the default implementation.</param>
    /// <returns>An object that when disposed, restores the original
    /// message bus.</returns>
    public static IDisposable WithMessageBus(this IMessageBus messageBus)
    {
        var origMessageBus = MessageBus.Current;

        Monitor.Enter(mbGate);
        MessageBus.Current = messageBus;
        return Disposable.Create(() =>
        {
            MessageBus.Current = origMessageBus;
            Monitor.Exit(mbGate);
        });
    }

    /// <summary>
    /// Override the default Message Bus during the specified block.
    /// </summary>
    /// <param name="messageBus">The message bus to use for the block.</param>
    /// <param name="block">The action to execute.</param>
    public static void With(this IMessageBus messageBus, Action block)
    {
        if (block is null)
        {
            throw new ArgumentNullException(nameof(block));
        }

        using (messageBus.WithMessageBus())
        {
            block();
        }
    }
}