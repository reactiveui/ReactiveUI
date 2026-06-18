// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Testing;

/// <summary>Message bus testing extensions.</summary>
public static class MessageBusExtensions
{
    /// <summary>The lock object used to serialize access to the shared message bus while it is overridden.</summary>
    private static readonly object messageBusGate = 42;

    /// <summary>Provides testing extension members for <see cref="IMessageBus"/>.</summary>
    /// <param name="messageBus">The message bus to use for the block.</param>
    extension(IMessageBus messageBus)
    {
        /// <summary>Override the default Message Bus during the specified block.</summary>
        /// <typeparam name="TRet">The return type.</typeparam>
        /// <param name="block">The function to execute.</param>
        /// <returns>The return value of the function.</returns>
        public TRet With<TRet>(Func<TRet> block)
        {
            ArgumentExceptionHelper.ThrowIfNull(block);

            using (messageBus.WithMessageBus())
            {
                return block();
            }
        }

        /// <summary>Override the default Message Bus during the specified block.</summary>
        /// <param name="block">The action to execute.</param>
        public void With(Action block)
        {
            ArgumentExceptionHelper.ThrowIfNull(block);

            using (messageBus.WithMessageBus())
            {
                block();
            }
        }

        /// <summary>
        /// WithMessageBus allows you to override the default Message Bus
        /// implementation until the object returned is disposed. If a
        /// message bus is not specified, a default empty one is created.
        /// </summary>
        /// <returns>An object that when disposed, restores the original
        /// message bus.</returns>
        public IDisposable WithMessageBus()
        {
            var origMessageBus = MessageBus.Current;

            Monitor.Enter(messageBusGate);
            MessageBus.Current = messageBus;
            return new ActionDisposable(() =>
            {
                MessageBus.Current = origMessageBus;
                Monitor.Exit(messageBusGate);
            });
        }
    }
}
