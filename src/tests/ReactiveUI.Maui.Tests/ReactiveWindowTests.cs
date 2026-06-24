// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Core.Executors;

namespace ReactiveUI.Maui.Tests;

/// <summary>Tests for <see cref="ReactiveWindow{TViewModel}"/>.</summary>
[TestExecutor<MauiTestExecutor>]
public class ReactiveWindowTests
{
    /// <summary>Tests that the ViewModel bindable property is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered() =>
        await Assert.That(ReactiveWindow<TestViewModel>.ViewModelProperty).IsNotNull();

    /// <summary>Tests that ViewModel property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetAndGet_WorksCorrectly()
    {
        var window = new ReactiveWindow<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        window.ViewModel = viewModel;

        using (Assert.Multiple())
        {
            await Assert.That(window.ViewModel).IsEqualTo(viewModel);
            await Assert.That(window.ViewModel?.Name).IsEqualTo("Test");
        }
    }

    /// <summary>Tests that the IViewFor.ViewModel property works correctly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewForViewModel_SetAndGet_WorksCorrectly()
    {
        var window = new ReactiveWindow<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        ((IViewFor)window).ViewModel = viewModel;

        using (Assert.Multiple())
        {
            await Assert.That(((IViewFor)window).ViewModel).IsEqualTo(viewModel);
            await Assert.That(window.ViewModel).IsEqualTo(viewModel);
        }
    }

    /// <summary>Tests that setting the ViewModel updates the BindingContext.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_WhenSet_UpdatesBindingContext()
    {
        var window = new ReactiveWindow<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        window.ViewModel = viewModel;

        await Assert.That(window.BindingContext).IsEqualTo(viewModel);
    }

    /// <summary>Tests that setting the BindingContext updates the ViewModel.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindingContext_WhenSet_UpdatesViewModel()
    {
        var window = new ReactiveWindow<TestViewModel>();
        var viewModel = new TestViewModel { Name = "Test" };

        window.BindingContext = viewModel;

        await Assert.That(window.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>Tests that the ViewModel can be set to null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetToNull_WorksCorrectly()
    {
        var window = new ReactiveWindow<TestViewModel> { ViewModel = new() { Name = "Test" } };

        window.ViewModel = null;

        using (Assert.Multiple())
        {
            await Assert.That(window.ViewModel).IsNull();
            await Assert.That(window.BindingContext).IsNull();
        }
    }

    /// <summary>Test view model.</summary>
    private sealed class TestViewModel
    {
        /// <summary>Gets or sets the name.</summary>
        public string? Name { get; set; }
    }
}
