// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities.AppBuilder;

/// <summary>
///     Helper class that provides common AppBuilder lifecycle management for test executors.
///     Can be used by both STA and non-STA test executors to ensure consistent behavior.
/// </summary>
/// <remarks>
/// This helper handles:
/// <list type="bullet">
/// <item><description>AppBuilder state reset during initialization and cleanup</description></item>
/// <item><description>Scheduler saving and restoration</description></item>
/// <item><description>Customizable AppBuilder configuration via Action delegate</description></item>
/// </list>
/// Usage:
/// <code>
/// private readonly AppBuilderTestHelper _helper = new();
///
/// // In Initialize or before test:
/// _helper.Initialize(builder =>
/// {
///     builder.WithWpf().WithCoreServices();
/// });
///
/// // In CleanUp or after test:
/// _helper.CleanUp();
/// </code>
/// </remarks>
public sealed class AppBuilderTestHelper
{
    private IScheduler? _originalMainThreadScheduler;
    private IScheduler? _originalTaskpoolScheduler;

    /// <summary>
    /// Initializes the AppBuilder with custom configuration.
    /// Saves current schedulers, resets AppBuilder state, and configures the builder using the provided action.
    /// </summary>
    /// <param name="configureBuilder">
    /// Action to configure the ReactiveUI builder. This is where you add platform services,
    /// schedulers, and other registrations. The action should call .WithCoreServices() and .BuildApp() will be called automatically.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if configureBuilder is null.</exception>
    /// <example>
    /// <code>
    /// _helper.Initialize(builder =>
    /// {
    ///     var scheduler = ImmediateScheduler.Instance;
    ///     builder
    ///         .WithMainThreadScheduler(scheduler)
    ///         .WithTaskPoolScheduler(scheduler)
    ///         .WithCoreServices();
    /// });
    /// </code>
    /// </example>
    public void Initialize(Action<IReactiveUIBuilder> configureBuilder)
    {
        ArgumentNullException.ThrowIfNull(configureBuilder);

        // Save the current schedulers so we can restore them later
        _originalMainThreadScheduler = RxSchedulers.MainThreadScheduler;
        _originalTaskpoolScheduler = RxSchedulers.TaskpoolScheduler;

        // Force-reset any previous builder state to avoid waiting deadlocks
        RxAppBuilder.ResetForTesting();
        Splat.Builder.AppBuilder.ResetBuilderStateForTests();

        // Create builder and apply custom configuration
        var builder = RxAppBuilder.CreateReactiveUIBuilder();
        configureBuilder(builder);

        // Build the app with configured services
        _ = builder.BuildApp();
    }

    /// <summary>
    /// Cleans up AppBuilder state and restores original schedulers.
    /// Should be called in the finally block or CleanUp method of test executors.
    /// </summary>
    /// <remarks>
    /// This method:
    /// <list type="bullet">
    /// <item><description>Restores original MainThreadScheduler and TaskpoolScheduler</description></item>
    /// <item><description>Resets AppBuilder state</description></item>
    /// <item><description>Rebuilds with core services to ensure clean state for next test</description></item>
    /// </list>
    /// </remarks>
    public void CleanUp()
    {
        // Restore original schedulers before resetting
        if (_originalMainThreadScheduler is not null)
        {
            RxSchedulers.MainThreadScheduler = _originalMainThreadScheduler;
        }

        if (_originalTaskpoolScheduler is not null)
        {
            RxSchedulers.TaskpoolScheduler = _originalTaskpoolScheduler;
        }

        // Reset AppBuilder state to clean up test-specific registrations
        RxAppBuilder.ResetForTesting();

        // Rebuild to ensure completely clean state for next test
        _ = RxAppBuilder.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();
    }
}
