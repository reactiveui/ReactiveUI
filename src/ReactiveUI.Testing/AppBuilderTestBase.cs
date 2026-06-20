// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Testing;

/// <summary>Provides a base class for tests that exercise the application builder lifecycle, offering helpers to run a test body within a serialized AppBuilder test context.</summary>
public abstract class AppBuilderTestBase
{
    /// <summary>Runs the application builder test asynchronous.</summary>
    /// <param name="testBody">The test body.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected static Task RunAppBuilderTestAsync(Func<Task> testBody) =>
        RxTest.AppBuilderTestAsync(testBody);

    /// <summary>Runs the application builder test asynchronous.</summary>
    /// <param name="testBody">The test body.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected static Task RunAppBuilderTestAsync(Action testBody) =>
        RxTest.AppBuilderTestAsync(() =>
        {
            testBody();
            return Task.CompletedTask;
        });
}
