// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Tests.Infrastructure.StaticState;

/// <summary>
/// A disposable scope that snapshots and restores Splat's Locator.Current static state
/// with WPF-specific service registrations.
/// Use this in WPF test fixtures that read or modify Locator.CurrentMutable to ensure
/// static state is properly restored after tests complete.
/// </summary>
/// <remarks>
/// This is the WPF-specific version of LocatorScope that includes WPF platform services
/// like view locator, activation fetcher, and platform operations.
/// Tests using this scope should also be marked with [NotInParallel] to prevent
/// concurrent modifications to the shared state.
/// </remarks>
public sealed class WpfLocatorScope : IDisposable
{
    private readonly IReadonlyDependencyResolver _previousLocator;

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfLocatorScope"/> class.
    /// Captures the current Locator state and sets up a fresh locator for WPF testing.
    /// </summary>
    public WpfLocatorScope()
    {
        // Save the current locator so we can restore it later
        _previousLocator = Locator.Current;

        // Replace with a new locator that tests can modify
        // Include WPF platform services to ensure view locator, activation, etc. work
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .WithCoreServices()
            .BuildApp();
    }

    /// <summary>
    /// Restores the Locator to its previous state.
    /// </summary>
    public void Dispose()
    {
        RxAppBuilder.ResetForTesting();

        // Restore the previous locator
        // Cast is safe because we saved it from Locator.Current
        Locator.SetLocator((IDependencyResolver)_previousLocator);
    }
}
