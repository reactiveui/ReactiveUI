// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

using Splat;

namespace ReactiveUI.Tests.Infrastructure.StaticState;

/// <summary>
/// A disposable scope that snapshots and restores Splat's Locator.Current static state.
/// Use this in test fixtures that read or modify Locator.CurrentMutable to ensure
/// static state is properly restored after tests complete.
/// </summary>
/// <remarks>
/// This helper is necessary because Splat's Locator maintains a static/global reference
/// that can leak between test executions, causing intermittent failures.
/// Tests using this scope should also be marked with [NotInParallel] to prevent
/// concurrent modifications to the shared state.
/// </remarks>
/// <example>
/// <code>
/// [NotInParallel]
/// public class MyTests
/// {
///     private LocatorScope? _locatorScope;
///
///     [Before(Test)]
///     public void SetUp()
///     {
///         _locatorScope = new LocatorScope();
///         // Now safe to use Locator.CurrentMutable
///     }
///
///     [After(Test)]
///     public void TearDown()
///     {
///         _locatorScope?.Dispose();
///     }
/// }
/// </code>
/// </example>
public sealed class LocatorScope : IDisposable
{
    private readonly IReadonlyDependencyResolver _previousLocator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocatorScope"/> class.
    /// Captures the current Locator state and sets up a fresh locator for testing.
    /// </summary>
    public LocatorScope()
    {
        // Save the current locator so we can restore it later
        _previousLocator = Locator.Current;

        // Replace with a new locator that tests can modify
        // Use the builder pattern to initialize ReactiveUI services
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();
    }

    /// <summary>
    /// Restores the Locator to its previous state.
    /// </summary>
    public void Dispose()
    {
        // Restore the previous locator
        // Cast is safe because we saved it from Locator.Current
        Locator.SetLocator((IDependencyResolver)_previousLocator);
    }
}
