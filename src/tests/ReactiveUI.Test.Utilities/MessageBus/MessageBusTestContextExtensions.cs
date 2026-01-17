// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities.MessageBus;

/// <summary>
///     Extensions for accessing message bus from TestContext.
/// </summary>
public static class MessageBusTestContextExtensions
{
    /// <summary>
    ///     Gets the message bus configured for this test.
    /// </summary>
    /// <param name="context">The test context.</param>
    /// <returns>The message bus instance.</returns>
    public static IMessageBus GetMessageBus(this TestContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return (IMessageBus)(context.StateBag.Items["MessageBus"] ?? new ReactiveUI.MessageBus());
    }
}
