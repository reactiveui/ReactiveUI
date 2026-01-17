// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;

namespace ReactiveUI.Tests.Utilities.SuspensionHost;

/// <summary>
/// Test executor that manages SuspensionHostExtensions static state for test isolation.
/// Saves and restores static fields before and after each test to prevent state leakage.
/// </summary>
/// <remarks>
/// This executor manages:
/// - SuspensionHostExtensions.EnsureLoadAppStateFunc
/// - SuspensionHostExtensions.SuspensionDriver
/// Tests using this executor should be marked with [NotInParallel] due to static state modifications.
/// </remarks>
public class SuspensionHostTestExecutor : ITestExecutor
{
    private Func<IObservable<Unit>>? _previousEnsureLoadAppStateFunc;
    private ISuspensionDriver? _previousSuspensionDriver;

    /// <inheritdoc/>
    public virtual async ValueTask ExecuteTest(TestContext context, Func<ValueTask> testAction)
    {
        ArgumentNullException.ThrowIfNull(testAction);

        SaveStaticState();

        try
        {
            await testAction();
        }
        finally
        {
            RestoreStaticState();
        }
    }

    /// <summary>
    /// Saves the current static state from SuspensionHostExtensions.
    /// </summary>
    protected virtual void SaveStaticState()
    {
        _previousEnsureLoadAppStateFunc = SuspensionHostExtensions.EnsureLoadAppStateFunc;
        _previousSuspensionDriver = SuspensionHostExtensions.SuspensionDriver;
    }

    /// <summary>
    /// Restores the previously saved static state to SuspensionHostExtensions.
    /// </summary>
    protected virtual void RestoreStaticState()
    {
        SuspensionHostExtensions.EnsureLoadAppStateFunc = _previousEnsureLoadAppStateFunc;
        SuspensionHostExtensions.SuspensionDriver = _previousSuspensionDriver;
    }
}
