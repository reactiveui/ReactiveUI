// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mocks;

/// <summary>
///     A mock model used by tests.
/// </summary>
public class Foo
{
    /// <summary>
    ///     The initial value assigned in the constructor.
    /// </summary>
    private const int InitialValue = 42;

    /// <summary>
    ///     The delay, in milliseconds, used by <see cref="SetValueAsync" />.
    /// </summary>
    private const int SetValueDelayMilliseconds = 10;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Foo" /> class.
    /// </summary>
    public Foo() => Value = InitialValue;

    /// <summary>
    ///     Gets the value.
    /// </summary>
    public int Value { get; private set; }

    /// <summary>
    ///     Sets the value asynchronously.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A task that completes when the value is set.</returns>
    public async Task<Unit> SetValueAsync(int value)
    {
        await RxSchedulers.TaskpoolScheduler.Sleep(TimeSpan.FromMilliseconds(SetValueDelayMilliseconds));
        Value = value;
        return Unit.Default;
    }
}
