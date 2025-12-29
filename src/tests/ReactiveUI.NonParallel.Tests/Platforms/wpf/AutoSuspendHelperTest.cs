// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="AutoSuspendHelper"/>.
/// </summary>
/// <remarks>
/// Note: AutoSuspendHelper requires a WPF Application instance which can only be created once per AppDomain.
/// These tests use Application.Current which is initialized by the test framework.
/// Coverage is provided through integration testing scenarios.
/// </remarks>
[NotInParallel]
public class AutoSuspendHelperTest
{
    /// <summary>
    /// Tests that AutoSuspendHelper type is accessible and can be referenced.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TypeIsAccessible()
    {
        var type = typeof(AutoSuspendHelper);
        await Assert.That(type).IsNotNull();
        await Assert.That(type.Name).IsEqualTo("AutoSuspendHelper");
    }

    /// <summary>
    /// Tests that AutoSuspendHelper has IdleTimeout property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasIdleTimeoutProperty()
    {
        var type = typeof(AutoSuspendHelper);
        var property = type.GetProperty("IdleTimeout");

        await Assert.That(property).IsNotNull();
        await Assert.That(property!.PropertyType).IsEqualTo(typeof(TimeSpan));
        await Assert.That(property.CanRead).IsTrue();
        await Assert.That(property.CanWrite).IsTrue();
    }

    /// <summary>
    /// Tests that AutoSuspendHelper has constructor taking Application parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasApplicationConstructor()
    {
        var type = typeof(AutoSuspendHelper);
        var constructor = type.GetConstructor([typeof(Application)]);

        await Assert.That(constructor).IsNotNull();
    }
}
