// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Tests for <see cref="AutoSuspendHelper"/>.</summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class AutoSuspendHelperTest
{
    /// <summary>The default idle timeout, in seconds, reported by a freshly created helper.</summary>
    private const double DefaultIdleTimeoutSeconds = 15.0;

    /// <summary>A custom idle timeout, in seconds, used to verify the property round-trips.</summary>
    private const double CustomIdleTimeoutSeconds = 30.0;

    /// <summary>Tests that AutoSuspendHelper can be created with Application.Current.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_CreatesInstanceWithApplication()
    {
        if (Application.Current is null)
        {
            _ = new Application();
        }

        var helper = new AutoSuspendHelper(Application.Current!);

        await Assert.That(helper).IsNotNull();
        await Assert.That(helper.IdleTimeout).IsEqualTo(TimeSpan.FromSeconds(DefaultIdleTimeoutSeconds));
    }

    /// <summary>Tests that IdleTimeout property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IdleTimeout_CanBeSetAndRetrieved()
    {
        if (Application.Current is null)
        {
            _ = new Application();
        }

        var helper = new AutoSuspendHelper(Application.Current!)
        {
            IdleTimeout = TimeSpan.FromSeconds(CustomIdleTimeoutSeconds)
        };

        await Assert.That(helper.IdleTimeout).IsEqualTo(TimeSpan.FromSeconds(CustomIdleTimeoutSeconds));
    }
}
