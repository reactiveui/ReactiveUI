// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="ReactiveWindow{TViewModel}"/>.
/// </summary>
/// <remarks>
/// Note: ReactiveWindow inherits from System.Windows.Window which requires a WPF Application to be running.
/// These tests focus on static members and type inspection that don't require instantiation.
/// Coverage is provided through integration testing scenarios.
/// </remarks>
[NotInParallel]
public class ReactiveWindowTest
{
    /// <summary>
    /// Tests that ViewModelProperty dependency property is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered()
    {
        await Assert.That(ReactiveWindow<TestViewModel>.ViewModelProperty).IsNotNull();
        await Assert.That(ReactiveWindow<TestViewModel>.ViewModelProperty.Name).IsEqualTo("ViewModel");
        await Assert.That(ReactiveWindow<TestViewModel>.ViewModelProperty.PropertyType).IsEqualTo(typeof(TestViewModel));
        await Assert.That(ReactiveWindow<TestViewModel>.ViewModelProperty.OwnerType).IsEqualTo(typeof(ReactiveWindow<TestViewModel>));
    }

    /// <summary>
    /// Tests that ReactiveWindow implements IViewFor.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ImplementsIViewFor()
    {
        var type = typeof(ReactiveWindow<TestViewModel>);
        var interfaces = type.GetInterfaces();

        await Assert.That(interfaces).Contains(typeof(IViewFor<TestViewModel>));
        await Assert.That(interfaces).Contains(typeof(IViewFor));
    }

    /// <summary>
    /// Tests that ReactiveWindow has BindingRoot property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasBindingRootProperty()
    {
        var type = typeof(ReactiveWindow<TestViewModel>);
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
