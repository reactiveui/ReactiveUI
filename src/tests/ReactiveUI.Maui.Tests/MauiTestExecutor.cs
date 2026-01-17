// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Dispatching;
using TUnit.Core.Interfaces;

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Test executor that provides MAUI test isolation with dispatcher setup.
/// Sets up MAUI DispatcherProvider for test execution and restores state on cleanup.
/// </summary>
/// <remarks>
/// This executor provides:
/// - MAUI DispatcherProvider configuration for test execution
/// - Automatic cleanup and state restoration after test completion
/// Tests using this executor should be marked with [NotInParallel] if they modify shared state.
/// </remarks>
public class MauiTestExecutor : ITestExecutor
{
    private IDispatcherProvider? _previousProvider;

    /// <inheritdoc/>
    public virtual async ValueTask ExecuteTest(TestContext context, Func<ValueTask> testAction)
    {
        ArgumentNullException.ThrowIfNull(testAction);

        Initialize();

        try
        {
            await testAction();
        }
        finally
        {
            CleanUp();
        }
    }

    /// <summary>
    /// Initializes the MAUI test environment by setting up the test dispatcher provider.
    /// </summary>
    protected virtual void Initialize()
    {
        _previousProvider = DispatcherProvider.Current;
        DispatcherProvider.SetCurrent(new TestDispatcherProvider());
    }

    /// <summary>
    /// Cleans up the MAUI test environment by restoring the previous dispatcher provider.
    /// </summary>
    protected virtual void CleanUp()
    {
        if (_previousProvider is not null)
        {
            DispatcherProvider.SetCurrent(_previousProvider);
        }
    }
}
