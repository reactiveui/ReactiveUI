// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Mocks;

public class Foo
{
    public Foo() => Value = 42;

    public int Value { get; private set; }

    public async Task<Unit> SetValueAsync(int value)
    {
        await RxApp.TaskpoolScheduler.Sleep(TimeSpan.FromMilliseconds(10));
        Value = value;
        return Unit.Default;
    }
}
