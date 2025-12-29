// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Tests for ReactiveMultiPage.
/// </summary>
public class ReactiveMultiPageTests
{
    private readonly MauiTestFixture _fixture = new();

    /// <summary>
    /// Sets up the test dispatcher for MAUI controls.
    /// </summary>
    [Before(Test)]
    public void Setup() => _fixture.Setup();

    /// <summary>
    /// Restores the previous dispatcher provider.
    /// </summary>
    [After(Test)]
    public void Teardown() => _fixture.Teardown();

    /// <summary>
    /// Tests that ViewModel property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetAndGet_WorksCorrectly()
    {
        var page = new TestMultiPage();
        var viewModel = new TestViewModel { Name = "Test" };

        page.ViewModel = viewModel;

        await Assert.That(page.ViewModel).IsEqualTo(viewModel);
        await Assert.That(page.ViewModel?.Name).IsEqualTo("Test");
    }

    /// <summary>
    /// Tests that IViewFor.ViewModel property works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewForViewModel_SetAndGet_WorksCorrectly()
    {
        var page = new TestMultiPage();
        var viewModel = new TestViewModel { Name = "Test" };

        ((IViewFor)page).ViewModel = viewModel;

        await Assert.That(((IViewFor)page).ViewModel).IsEqualTo(viewModel);
        await Assert.That(page.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that setting ViewModel updates BindingContext.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_WhenSet_UpdatesBindingContext()
    {
        var page = new TestMultiPage();
        var viewModel = new TestViewModel { Name = "Test" };

        page.ViewModel = viewModel;

        await Assert.That(page.BindingContext).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that ViewModel can be set to null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetToNull_WorksCorrectly()
    {
        var page = new TestMultiPage();
        var viewModel = new TestViewModel { Name = "Test" };

        page.ViewModel = viewModel;
        page.ViewModel = null;

        await Assert.That(page.ViewModel).IsNull();
        await Assert.That(page.BindingContext).IsNull();
    }

    /// <summary>
    /// Concrete implementation of ReactiveMultiPage for testing.
    /// </summary>
    private class TestMultiPage : ReactiveMultiPage<ContentPage, TestViewModel>
    {
        /// <inheritdoc/>
        protected override ContentPage CreateDefault(object item) => new ContentPage();
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
