// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="ReactivePage{TViewModel}"/>.
/// </summary>
/// <remarks>
/// Note: ReactivePage inherits from System.Windows.Controls.Page which requires a WPF Application to be running.
/// These tests focus on static members and type inspection that don't require instantiation.
/// Coverage is provided through integration testing scenarios.
/// </remarks>
[NotInParallel]
public class ReactivePageTest
{
    /// <summary>
    /// Tests that ViewModelProperty dependency property is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered()
    {
        await Assert.That(ReactivePage<TestViewModel>.ViewModelProperty).IsNotNull();
        await Assert.That(ReactivePage<TestViewModel>.ViewModelProperty.Name).IsEqualTo("ViewModel");
        await Assert.That(ReactivePage<TestViewModel>.ViewModelProperty.PropertyType).IsEqualTo(typeof(TestViewModel));
        await Assert.That(ReactivePage<TestViewModel>.ViewModelProperty.OwnerType).IsEqualTo(typeof(ReactivePage<TestViewModel>));
    }

    /// <summary>
    /// Tests that ReactivePage implements IViewFor.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ImplementsIViewFor()
    {
        var type = typeof(ReactivePage<TestViewModel>);
        var interfaces = type.GetInterfaces();

        await Assert.That(interfaces).Contains(typeof(IViewFor<TestViewModel>));
        await Assert.That(interfaces).Contains(typeof(IViewFor));
    }

    /// <summary>
    /// Tests that ReactivePage has BindingRoot property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasBindingRootProperty()
    {
        var type = typeof(ReactivePage<TestViewModel>);
        var property = type.GetProperty("BindingRoot");

        await Assert.That(property).IsNotNull();
        await Assert.That(property!.PropertyType).IsEqualTo(typeof(TestViewModel));
    }

    /// <summary>
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel
    {
    }
}
