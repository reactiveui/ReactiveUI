// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;
using Splat;

namespace ReactiveUI.Tests.Utilities.Logging;

/// <summary>
///     Provides a test execution scope that configures a default logging infrastructure for unit tests requiring
///     ReactiveUI
///     logging services.
/// </summary>
/// <remarks>
///     This class ensures that a default ILogManager and logger are registered for the duration of a test,
///     allowing tests to run without requiring explicit logging setup. The logging configuration is reset before and after
///     each test execution to prevent side effects between tests.
/// </remarks>
public class LoggingRegistrationExecutor : BaseAppBuilderTestExecutor
{
    /// <inheritdoc />
    protected override void ConfigureAppBuilder(IReactiveUIBuilder builder, TestContext context)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(context);

        var logger = new TestLogger();
        var fullLogger = new WrappingFullLogger(logger);

        // Register a default ILogManager for tests
        // This ensures ILogManager is available even if tests don't set up their own
        var currentLogManager = new TestLogManager(fullLogger);

        // Store test utilities in context
        context.StateBag["TestLogger"] = logger;
        context.StateBag["TestLogManager"] = currentLogManager;

        // Configure builder with logging registration and core services
        builder
            .WithRegistration(r => r.Register<ILogManager>(() => currentLogManager))
            .WithCoreServices();
    }
}
