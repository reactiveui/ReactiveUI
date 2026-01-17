// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities.AppBuilder;

/// <summary>
///     Base test executor that provides AppBuilder lifecycle management with customizable registration hooks.
///     Derived executors can override <see cref="ConfigureAppBuilder"/> to add custom AppBuilder registrations.
/// </summary>
/// <remarks>
/// This base class uses <see cref="AppBuilderTestHelper"/> to handle:
/// <list type="bullet">
/// <item><description>AppBuilder state reset before and after test execution</description></item>
/// <item><description>Core services registration</description></item>
/// <item><description>Scheduler restoration after test completion</description></item>
/// <item><description>Customizable AppBuilder configuration via virtual method</description></item>
/// </list>
/// </remarks>
public abstract class BaseAppBuilderTestExecutor : ITestExecutor
{
    private readonly AppBuilderTestHelper _helper = new();

    /// <inheritdoc />
    public virtual async ValueTask ExecuteTest(TestContext context, Func<ValueTask> testAction)
    {
        ArgumentNullException.ThrowIfNull(testAction);

        _helper.Initialize(builder => ConfigureAppBuilder(builder, context));

        try
        {
            // Execute actual test
            await testAction();
        }
        finally
        {
            _helper.CleanUp();
        }
    }

    /// <summary>
    /// Configures the AppBuilder with custom registrations and services.
    /// Derived classes should override this method to add their specific configuration.
    /// </summary>
    /// <param name="builder">The ReactiveUI builder to configure.</param>
    /// <param name="context">The test context for storing state.</param>
    /// <remarks>
    /// The base implementation registers core services only. Derived classes should call the base method
    /// or ensure they register core services if they override without calling base.
    /// </remarks>
    protected virtual void ConfigureAppBuilder(IReactiveUIBuilder builder, TestContext context)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(context);

        // Default implementation: just register core services
        builder.WithCoreServices();
    }
}
