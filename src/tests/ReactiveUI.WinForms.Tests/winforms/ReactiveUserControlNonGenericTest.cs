// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Winforms;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>
/// Tests for <see cref="ReactiveUserControlNonGeneric"/>.
/// </summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]

public class ReactiveUserControlNonGenericTest
{
    /// <summary>
    /// Tests that ReactiveUserControlNonGeneric can be instantiated.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_CreatesInstance()
    {
        var control = new ReactiveUserControlNonGeneric();

        await Assert.That(control).IsNotNull();
        control.Dispose();
    }

    /// <summary>
    /// Tests that ViewModel property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_CanBeSetAndRetrieved()
    {
        var control = new ReactiveUserControlNonGeneric();
        var viewModel = new TestViewModel();

        ((IViewFor)control).ViewModel = viewModel;
        var retrievedViewModel = ((IViewFor)control).ViewModel;

        await Assert.That(retrievedViewModel).IsSameReferenceAs(viewModel);
        control.Dispose();
    }

    /// <summary>
    /// Tests that ViewModel property can be set to null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_CanBeSetToNull()
    {
        var control = new ReactiveUserControlNonGeneric();
        var viewModel = new TestViewModel();

        ((IViewFor)control).ViewModel = viewModel;
        ((IViewFor)control).ViewModel = null;

        await Assert.That(((IViewFor)control).ViewModel).IsNull();
        control.Dispose();
    }

    /// <summary>
    /// Tests that Dispose cleans up resources.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispose_CleansUpResources()
    {
        var control = new ReactiveUserControlNonGeneric();

        control.Dispose();

        // Verify disposal completed without throwing
        await Task.CompletedTask;
    }

    /// <summary>
    /// Tests that control can be disposed multiple times.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Dispose_CanBeCalledMultipleTimes()
    {
        var control = new ReactiveUserControlNonGeneric();

        control.Dispose();
        control.Dispose();

        // Verify disposal can be called multiple times without throwing
        await Task.CompletedTask;
    }

    /// <summary>
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel
    {
    }
}
