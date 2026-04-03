// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;
using TUnit.Core.Interfaces;

namespace ReactiveUI.Builder.Tests.Executors;

/// <summary>
/// Base test executor for Builder tests. Creates a fresh resolver, resets all
/// static state, and bootstraps services before each test. Override
/// <see cref="ConfigureBuilder"/> to customise what gets registered.
/// </summary>
public class BuilderTestExecutorBase : ITestExecutor
{
    /// <inheritdoc/>
    public async ValueTask ExecuteTest(TestContext context, Func<ValueTask> testAction)
    {
        ArgumentNullException.ThrowIfNull(testAction);

        ResetState();
        ConfigureBuilder();

        try
        {
            await testAction();
        }
        finally
        {
            ResetState();
        }
    }

    /// <summary>
    /// Resets all static state. Override to reset additional state holders.
    /// </summary>
    protected virtual void ResetState()
    {
        RxAppBuilder.ResetForTesting();
        AppBuilder.ResetBuilderStateForTests();
    }

    /// <summary>
    /// Creates the builder and registers services. Override to change registration.
    /// </summary>
    protected virtual void ConfigureBuilder() =>
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();
}
