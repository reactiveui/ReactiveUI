// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Tests for ReactiveShell.
/// </summary>
[TestExecutor<MauiTestExecutor>]
public class ReactiveShellTests
{
    /// <summary>
    /// Tests that ViewModel property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetAndGet_WorksCorrectly()
    {
        var shell = new ReactiveShell<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        shell.ViewModel = viewModel;

        await Assert.That(shell.ViewModel).IsEqualTo(viewModel);
        await Assert.That(shell.ViewModel?.Name).IsEqualTo("Test");
    }

    /// <summary>
    /// Tests that IViewFor.ViewModel property works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewForViewModel_SetAndGet_WorksCorrectly()
    {
        var shell = new ReactiveShell<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        ((IViewFor)shell).ViewModel = viewModel;

        await Assert.That(((IViewFor)shell).ViewModel).IsEqualTo(viewModel);
        await Assert.That(shell.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that setting ViewModel updates BindingContext.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_WhenSet_UpdatesBindingContext()
    {
        var shell = new ReactiveShell<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        shell.ViewModel = viewModel;

        await Assert.That(shell.BindingContext).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that ViewModel can be set to null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetToNull_WorksCorrectly()
    {
        var shell = new ReactiveShell<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        shell.ViewModel = viewModel;
        shell.ViewModel = null;

        await Assert.That(shell.ViewModel).IsNull();
        await Assert.That(shell.BindingContext).IsNull();
    }

    /// <summary>
    /// Test view model.
    /// </summary>
    private class TestViewModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string? Name { get; set; }
    }
}
