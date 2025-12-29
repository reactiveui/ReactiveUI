// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Maui;

/// <summary>
/// Tests for <see cref="ReactiveUI.Maui.ReactiveShellContent{TViewModel}"/>.
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
        await Assert.That(ReactiveUI.Maui.ReactiveShellContent<TestViewModel>.ViewModelProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that ContractProperty BindableProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractProperty_IsRegistered()
    {
        await Assert.That(ReactiveUI.Maui.ReactiveShellContent<TestViewModel>.ContractProperty).IsNotNull();
    }

    /// <summary>
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel
    {
    }
}
