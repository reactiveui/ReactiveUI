// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Winforms;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Winforms;

/// <summary>
/// Tests for <see cref="ReactiveUserControl{TViewModel}"/>.
/// </summary>
[NotInParallel]
public class ReactiveUserControlTest
{
    /// <summary>
    /// Tests that ReactiveUserControl can be instantiated.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task Constructor_CreatesInstance()
    {
        var control = new ReactiveUserControl<TestViewModel>();

        await Assert.That(control).IsNotNull();
    }

    /// <summary>
    /// Tests that ViewModel property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ViewModel_CanBeSetAndRetrieved()
    {
        var control = new ReactiveUserControl<TestViewModel>();
        var viewModel = new TestViewModel();

        control.ViewModel = viewModel;

        await Assert.That(control.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that IViewFor.ViewModel can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task IViewForViewModel_CanBeSetAndRetrieved()
    {
        IViewFor control = new ReactiveUserControl<TestViewModel>();
        var viewModel = new TestViewModel();

        control.ViewModel = viewModel;

        await Assert.That(control.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that ViewModel property can be set to null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ViewModel_CanBeSetToNull()
    {
        var control = new ReactiveUserControl<TestViewModel>();
        var viewModel = new TestViewModel();

        control.ViewModel = viewModel;
        control.ViewModel = null;

        await Assert.That(control.ViewModel).IsNull();
    }

    /// <summary>
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel
    {
    }
}
