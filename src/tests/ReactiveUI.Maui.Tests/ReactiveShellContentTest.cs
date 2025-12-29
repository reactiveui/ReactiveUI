// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Tests.Maui;

/// <summary>
/// Tests for <see cref="ReactiveShellContent{TViewModel}"/>.
/// </summary>
public class ReactiveShellContentTest
{
    /// <summary>
    /// Tests that ViewModelProperty BindableProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered()
    {
        await Assert.That(ReactiveShellContent<TestViewModel>.ViewModelProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that ContractProperty BindableProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractProperty_IsRegistered()
    {
        await Assert.That(ReactiveShellContent<TestViewModel>.ContractProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that ViewModel property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetAndGet_WorksCorrectly()
    {
        var content = new ReactiveShellContent<TestViewModel>();
        var viewModel = new TestViewModel();

        content.ViewModel = viewModel;

        await Assert.That(content.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that Contract property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Contract_SetAndGet_WorksCorrectly()
    {
        var content = new ReactiveShellContent<TestViewModel>();
        var contract = "TestContract";

        content.Contract = contract;

        await Assert.That(content.Contract).IsEqualTo(contract);
    }

    /// <summary>
    /// Tests that ViewModel can be null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_CanBeNull()
    {
        var content = new ReactiveShellContent<TestViewModel> { ViewModel = null };

        await Assert.That(content.ViewModel).IsNull();
    }

    /// <summary>
    /// Tests that Contract can be null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Contract_CanBeNull()
    {
        var content = new ReactiveShellContent<TestViewModel> { Contract = null };

        await Assert.That(content.Contract).IsNull();
    }

    /// <summary>
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel
    {
    }
}
