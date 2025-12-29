// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="ReactiveWindow{TViewModel}"/>.
/// </summary>
[NotInParallel]
public class ReactiveWindowTest
{
    /// <summary>
    /// Tests that ReactiveWindow can be instantiated.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_CreatesInstance()
    {
        var window = new ReactiveWindow<TestViewModel>();

        await Assert.That(window).IsNotNull();
        await Assert.That(window.ViewModel).IsNull();
    }

    /// <summary>
    /// Tests that ViewModel property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_CanBeSetAndRetrieved()
    {
        var window = new ReactiveWindow<TestViewModel>();
        var viewModel = new TestViewModel();

        window.ViewModel = viewModel;

        await Assert.That(window.ViewModel).IsSameReferenceAs(viewModel);
        await Assert.That(window.BindingRoot).IsSameReferenceAs(viewModel);
    }

    /// <summary>
    /// Tests that ViewModel property can be set to null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_CanBeSetToNull()
    {
        var window = new ReactiveWindow<TestViewModel>();
        var viewModel = new TestViewModel();

        window.ViewModel = viewModel;
        window.ViewModel = null;

        await Assert.That(window.ViewModel).IsNull();
        await Assert.That(window.BindingRoot).IsNull();
    }

    /// <summary>
    /// Tests that IViewFor.ViewModel property works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewForViewModel_CanBeSetAndRetrieved()
    {
        var window = new ReactiveWindow<TestViewModel>();
        var viewModel = new TestViewModel();

        ((IViewFor)window).ViewModel = viewModel;

        await Assert.That(((IViewFor)window).ViewModel).IsSameReferenceAs(viewModel);
        await Assert.That(window.ViewModel).IsSameReferenceAs(viewModel);
    }

    /// <summary>
    /// Tests that ViewModelProperty dependency property is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered()
    {
        await Assert.That(ReactiveWindow<TestViewModel>.ViewModelProperty).IsNotNull();
        await Assert.That(ReactiveWindow<TestViewModel>.ViewModelProperty.Name).IsEqualTo("ViewModel");
    }

    /// <summary>
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel
    {
    }
}
