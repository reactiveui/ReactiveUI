// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="ReactivePage{TViewModel}"/>.
/// </summary>
[NotInParallel]
public class ReactivePageTest
{
    /// <summary>
    /// Tests that ReactivePage can be instantiated.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_CreatesInstance()
    {
        var page = new ReactivePage<TestViewModel>();

        await Assert.That(page).IsNotNull();
        await Assert.That(page.ViewModel).IsNull();
    }

    /// <summary>
    /// Tests that ViewModel property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_CanBeSetAndRetrieved()
    {
        var page = new ReactivePage<TestViewModel>();
        var viewModel = new TestViewModel();

        page.ViewModel = viewModel;

        await Assert.That(page.ViewModel).IsSameReferenceAs(viewModel);
        await Assert.That(page.BindingRoot).IsSameReferenceAs(viewModel);
    }

    /// <summary>
    /// Tests that ViewModel property can be set to null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_CanBeSetToNull()
    {
        var page = new ReactivePage<TestViewModel>();
        var viewModel = new TestViewModel();

        page.ViewModel = viewModel;
        page.ViewModel = null;

        await Assert.That(page.ViewModel).IsNull();
        await Assert.That(page.BindingRoot).IsNull();
    }

    /// <summary>
    /// Tests that IViewFor.ViewModel property works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewForViewModel_CanBeSetAndRetrieved()
    {
        var page = new ReactivePage<TestViewModel>();
        var viewModel = new TestViewModel();

        ((IViewFor)page).ViewModel = viewModel;

        await Assert.That(((IViewFor)page).ViewModel).IsSameReferenceAs(viewModel);
        await Assert.That(page.ViewModel).IsSameReferenceAs(viewModel);
    }

    /// <summary>
    /// Tests that ViewModelProperty dependency property is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered()
    {
        await Assert.That(ReactivePage<TestViewModel>.ViewModelProperty).IsNotNull();
        await Assert.That(ReactivePage<TestViewModel>.ViewModelProperty.Name).IsEqualTo("ViewModel");
    }

    /// <summary>
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel
    {
    }
}
