// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Core.Executors;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>Tests for <see cref="ReactiveUserControl{TViewModel}"/>.</summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]

public class ReactiveUserControlTest
{
    /// <summary>Tests that ReactiveUserControl can be instantiated.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_CreatesInstance()
    {
        var control = new ReactiveUserControl<TestViewModel>();

        await Assert.That(control).IsNotNull();
    }

    /// <summary>Tests that ViewModel property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_CanBeSetAndRetrieved()
    {
        var control = new ReactiveUserControl<TestViewModel>();
        var viewModel = new TestViewModel();

        control.ViewModel = viewModel;

        await Assert.That(control.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>Tests that IViewFor.ViewModel can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewForViewModel_CanBeSetAndRetrieved()
    {
        IViewFor control = new ReactiveUserControl<TestViewModel>();
        var viewModel = new TestViewModel();

        control.ViewModel = viewModel;

        await Assert.That(control.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>Tests that ViewModel property can be set to null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_CanBeSetToNull()
    {
        var control = new ReactiveUserControl<TestViewModel>
        {
            ViewModel = new(),
        };
        control.ViewModel = null;

        await Assert.That(control.ViewModel).IsNull();
    }

    /// <summary>Tests that IViewFor.ViewModel returns null after being set to null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewForViewModel_ReturnsNullAfterSettingToNull()
    {
        IViewFor control = new ReactiveUserControl<TestViewModel> { ViewModel = null };

        await Assert.That(control.ViewModel).IsNull();
    }

    /// <summary>Tests that IViewFor.ViewModel can cast from object.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IViewForViewModel_CanCastFromObject()
    {
        var control = new ReactiveUserControl<TestViewModel>();
        IViewFor viewForControl = control;
        object viewModel = new TestViewModel();

        viewForControl.ViewModel = viewModel;

        var typedViewModel = control.ViewModel!;

        await Assert.That(typedViewModel).IsNotNull();
        await Assert.That(viewForControl.ViewModel).IsEqualTo(viewModel);
        await Assert.That(typedViewModel == viewForControl.ViewModel).IsTrue();
    }

    /// <summary>Tests that control initializes components on construction.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_InitializesComponents()
    {
        var control = new ReactiveUserControl<TestViewModel>();

        // Verify the control was initialized (components should not be null after InitializeComponent)
        await Assert.That(control).IsNotNull();
        await Assert.That(control.ViewModel).IsNull(); // Handle not created yet
    }

    /// <summary>Test view model for testing.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "SST1436:Classes should not be empty", Justification = "Marker type for tests.")]
    private sealed class TestViewModel;
}
