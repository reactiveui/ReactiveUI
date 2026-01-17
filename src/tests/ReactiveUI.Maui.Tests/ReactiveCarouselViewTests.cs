// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Tests for ReactiveCarouselView.
/// </summary>
[TestExecutor<MauiTestExecutor>]
public class ReactiveCarouselViewTests
{
    /// <summary>
    /// Tests that ViewModel property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetAndGet_WorksCorrectly()
    {
        var view = new ReactiveCarouselView<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        view.ViewModel = viewModel;

        await Assert.That(view.ViewModel).IsEqualTo(viewModel);
        await Assert.That(view.ViewModel?.Name).IsEqualTo("Test");
    }

    /// <summary>
    /// Tests that IViewFor.ViewModel property works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewForViewModel_SetAndGet_WorksCorrectly()
    {
        var view = new ReactiveCarouselView<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        ((IViewFor)view).ViewModel = viewModel;

        await Assert.That(((IViewFor)view).ViewModel).IsEqualTo(viewModel);
        await Assert.That(view.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that setting ViewModel updates BindingContext.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_WhenSet_UpdatesBindingContext()
    {
        var view = new ReactiveCarouselView<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        view.ViewModel = viewModel;

        await Assert.That(view.BindingContext).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that ViewModel can be set to null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetToNull_WorksCorrectly()
    {
        var view = new ReactiveCarouselView<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        view.ViewModel = viewModel;
        view.ViewModel = null;

        await Assert.That(view.ViewModel).IsNull();
        await Assert.That(view.BindingContext).IsNull();
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
