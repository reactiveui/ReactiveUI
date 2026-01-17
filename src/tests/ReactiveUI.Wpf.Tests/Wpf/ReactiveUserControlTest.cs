// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="ReactiveUserControl{TViewModel}"/>.
/// </summary>
/// <remarks>
/// Note: ReactiveUserControl inherits from System.Windows.Controls.UserControl which requires a WPF Application to be running.
/// These tests focus on static members and type inspection that don't require instantiation.
/// Coverage is provided through integration testing scenarios.
/// </remarks>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class ReactiveUserControlTest
{
    /// <summary>
    /// Tests that ViewModelProperty dependency property is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered()
    {
        await Assert.That(ReactiveUserControl<TestViewModel>.ViewModelProperty).IsNotNull();
        await Assert.That(ReactiveUserControl<TestViewModel>.ViewModelProperty.Name).IsEqualTo("ViewModel");
        await Assert.That(ReactiveUserControl<TestViewModel>.ViewModelProperty.PropertyType).IsEqualTo(typeof(TestViewModel));
        await Assert.That(ReactiveUserControl<TestViewModel>.ViewModelProperty.OwnerType).IsEqualTo(typeof(ReactiveUserControl<TestViewModel>));
    }

    /// <summary>
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel
    {
    }
}
