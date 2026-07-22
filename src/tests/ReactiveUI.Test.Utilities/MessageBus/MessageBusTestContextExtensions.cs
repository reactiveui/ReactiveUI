// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
using MessageBusType = ReactiveUI.Reactive.MessageBus;
#else
using MessageBusType = ReactiveUI.MessageBus;
#endif

namespace ReactiveUI.Tests.Utilities.MessageBus;

/// <summary>Extensions for accessing message bus from TestContext.</summary>
public static class MessageBusTestContextExtensions
{
    /// <summary>Provides message bus accessors for <see cref="TestContext"/>.</summary>
    /// <param name="context">The test context.</param>
    extension(TestContext context)
    {
        /// <summary>Gets the message bus configured for this test.</summary>
        /// <returns>The message bus instance.</returns>
        public IMessageBus GetMessageBus()
        {
            ArgumentNullException.ThrowIfNull(context);
            return (IMessageBus)(context.StateBag.Items["MessageBus"] ?? new MessageBusType());
        }
    }
}
