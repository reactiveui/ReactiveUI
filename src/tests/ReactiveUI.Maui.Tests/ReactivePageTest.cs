// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Tests for <see cref="ReactivePage{TViewModel}"/>.
/// </summary>
public class ReactivePageTest
{
    /// <summary>
    /// Tests that ViewModelProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered()
    {
        await Assert.That(ReactivePage<TestViewModel>.ViewModelProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that ViewModel property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetAndGet_WorksCorrectly()
    {
        var page = new ReactivePage<TestViewModel>();
        var viewModel = new TestViewModel();

        page.ViewModel = viewModel;

        await Assert.That(page.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that IViewFor.ViewModel works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewForViewModel_SetAndGet_WorksCorrectly()
    {
        IViewFor page = new ReactivePage<TestViewModel>();
        var viewModel = new TestViewModel();

        page.ViewModel = viewModel;

        await Assert.That(page.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that BindingRoot returns the ViewModel.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindingRoot_ReturnsViewModel()
    {
        var page = new ReactivePage<TestViewModel>();
        var viewModel = new TestViewModel();

        page.ViewModel = viewModel;

        await Assert.That(page.BindingRoot).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that ViewModel can be null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_CanBeNull()
    {
        var page = new ReactivePage<TestViewModel> { ViewModel = null };

        await Assert.That(page.ViewModel).IsNull();
        await Assert.That(page.BindingRoot).IsNull();
    }

    /// <summary>
    /// Tests that setting BindingContext updates ViewModel.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindingContext_UpdatesViewModel()
    {
        var page = new ReactivePage<TestViewModel>();
        var viewModel = new TestViewModel();

        page.BindingContext = viewModel;

        await Assert.That(page.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that setting ViewModel updates BindingContext.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_UpdatesBindingContext()
    {
        var page = new ReactivePage<TestViewModel>();
        var viewModel = new TestViewModel();

        page.ViewModel = viewModel;

        await Assert.That(page.BindingContext).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel
    {
    }
}
