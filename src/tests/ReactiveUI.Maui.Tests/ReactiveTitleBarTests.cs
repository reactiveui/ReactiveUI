// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Core.Executors;

namespace ReactiveUI.Maui.Tests;

/// <summary>Tests for <see cref="ReactiveTitleBar{TViewModel}"/>.</summary>
[TestExecutor<MauiTestExecutor>]
public class ReactiveTitleBarTests
{
    /// <summary>Tests that the ViewModel bindable property is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered() =>
        await Assert.That(ReactiveTitleBar<TestViewModel>.ViewModelProperty).IsNotNull();

    /// <summary>Tests that ViewModel property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetAndGet_WorksCorrectly()
    {
        var titleBar = new ReactiveTitleBar<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        titleBar.ViewModel = viewModel;

        using (Assert.Multiple())
        {
            await Assert.That(titleBar.ViewModel).IsEqualTo(viewModel);
            await Assert.That(titleBar.ViewModel?.Name).IsEqualTo("Test");
        }
    }

    /// <summary>Tests that the IViewFor.ViewModel property works correctly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewForViewModel_SetAndGet_WorksCorrectly()
    {
        var titleBar = new ReactiveTitleBar<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        ((IViewFor)titleBar).ViewModel = viewModel;

        using (Assert.Multiple())
        {
            await Assert.That(((IViewFor)titleBar).ViewModel).IsEqualTo(viewModel);
            await Assert.That(titleBar.ViewModel).IsEqualTo(viewModel);
        }
    }

    /// <summary>Tests that setting the ViewModel updates the BindingContext.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_WhenSet_UpdatesBindingContext()
    {
        var titleBar = new ReactiveTitleBar<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        titleBar.ViewModel = viewModel;

        await Assert.That(titleBar.BindingContext).IsEqualTo(viewModel);
    }

    /// <summary>Tests that setting the BindingContext updates the ViewModel.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindingContext_WhenSet_UpdatesViewModel()
    {
        var titleBar = new ReactiveTitleBar<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        titleBar.BindingContext = viewModel;

        await Assert.That(titleBar.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>Tests that the ViewModel can be set to null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetToNull_WorksCorrectly()
    {
        var titleBar = new ReactiveTitleBar<TestViewModel> { ViewModel = new() { Name = "Test" } };

        titleBar.ViewModel = null;

        using (Assert.Multiple())
        {
            await Assert.That(titleBar.ViewModel).IsNull();
            await Assert.That(titleBar.BindingContext).IsNull();
        }
    }

    /// <summary>Test view model.</summary>
    private sealed class TestViewModel
    {
        /// <summary>Gets or sets the name.</summary>
        public string? Name { get; set; }
    }
}
