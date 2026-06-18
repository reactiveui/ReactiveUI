// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mocks;

/// <summary>A mock model used by tests.</summary>
public class Foo
{
    /// <summary>The initial value assigned in the constructor.</summary>
    private const int InitialValue = 42;

    /// <summary>Initializes a new instance of the <see cref="Foo" /> class.</summary>
    public Foo() => Value = InitialValue;

    /// <summary>Gets the value.</summary>
    public int Value { get; private set; }

    /// <summary>Sets the value asynchronously.</summary>
    /// <param name="value">The value.</param>
    /// <returns>A completed task carrying <see cref="RxVoid.Default"/>.</returns>
    /// <remarks>The set itself is immediate; the operation's latency is modelled in <see cref="FooViewModel"/>'s
    /// pipeline by a scheduler-driven delay, which keeps virtual-time tests fully deterministic (a wall-clock or
    /// thread-pool-bridged delay here races the test's virtual scheduler).</remarks>
    public Task<RxVoid> SetValueAsync(int value)
    {
        Value = value;
        return Task.FromResult(RxVoid.Default);
    }
}
