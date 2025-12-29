// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Maui;

/// <summary>
/// Tests for <see cref="ReactivePage{TViewModel}"/>.
/// </summary>
/// <remarks>
/// Note: ReactivePage inherits from Microsoft.Maui.Controls.Page which requires MAUI runtime initialization.
/// These tests focus on type inspection and static members that don't require instantiation.
/// </remarks>
public class ReactivePageTest
{
    /// <summary>
    /// Tests that ReactivePage has ViewModel property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasViewModelProperty()
    {
        var type = typeof(ReactivePage<TestViewModel>);
        var property = type.GetProperty("ViewModel");

        await Assert.That(property).IsNotNull();
        await Assert.That(property!.PropertyType).IsEqualTo(typeof(TestViewModel));
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
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel
    {
    }
}
