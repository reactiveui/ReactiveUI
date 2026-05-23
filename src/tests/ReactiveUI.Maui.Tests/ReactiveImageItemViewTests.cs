// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Core.Executors;

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Tests for ReactiveImageItemView.
/// </summary>
[TestExecutor<MauiTestExecutor>]
public class ReactiveImageItemViewTests
{
    /// <summary>
    /// Tests that ViewModel property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetAndGet_WorksCorrectly()
    {
        var view = new ReactiveImageItemView<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        view.ViewModel = viewModel;

        await Assert.That(view.ViewModel).IsEqualTo(viewModel);
        await Assert.That(view.ViewModel?.Name).IsEqualTo("Test");
    }

    /// <summary>
    /// Tests that Text property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Text_SetAndGet_WorksCorrectly()
    {
        var view = new ReactiveImageItemView<TestViewModel> { Text = "Test Text" };

        await Assert.That(view.Text).IsEqualTo("Test Text");
    }

    /// <summary>
    /// Tests that Detail property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Detail_SetAndGet_WorksCorrectly()
    {
        var view = new ReactiveImageItemView<TestViewModel> { Detail = "Test Detail" };

        await Assert.That(view.Detail).IsEqualTo("Test Detail");
    }

    /// <summary>
    /// Tests that ImageSource property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ImageSource_SetAndGet_WorksCorrectly()
    {
        var imageSource = ImageSource.FromFile("test.png");
        var view = new ReactiveImageItemView<TestViewModel> { ImageSource = imageSource };

        await Assert.That(view.ImageSource).IsEqualTo(imageSource);
    }

    /// <summary>
    /// Tests that setting ViewModel updates BindingContext.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_WhenSet_UpdatesBindingContext()
    {
        var view = new ReactiveImageItemView<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        view.ViewModel = viewModel;

        await Assert.That(view.BindingContext).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Test view model.
    /// </summary>
    private sealed class TestViewModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string? Name { get; set; }
    }
}
