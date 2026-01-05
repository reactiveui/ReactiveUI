// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using System.Windows;

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="AutoSuspendHelper"/>.
/// </summary>
[NotInParallel]
public class AutoSuspendHelperTest
{
    /// <summary>
    /// Tests that AutoSuspendHelper can be created with Application.Current.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_CreatesInstanceWithApplication()
    {
        if (Application.Current == null)
        {
            _ = new Application();
        }

        var helper = new AutoSuspendHelper(Application.Current!);

        await Assert.That(helper).IsNotNull();
        await Assert.That(helper.IdleTimeout).IsEqualTo(TimeSpan.FromSeconds(15.0));
    }

    /// <summary>
    /// Tests that IdleTimeout property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IdleTimeout_CanBeSetAndRetrieved()
    {
        if (Application.Current == null)
        {
            _ = new Application();
        }

        var helper = new AutoSuspendHelper(Application.Current!)
        {
            IdleTimeout = TimeSpan.FromSeconds(30.0)
        };

        await Assert.That(helper.IdleTimeout).IsEqualTo(TimeSpan.FromSeconds(30.0));
    }
}
