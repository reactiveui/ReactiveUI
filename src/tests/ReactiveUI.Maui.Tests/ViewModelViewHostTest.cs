// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Maui;

/// <summary>
/// Tests for <see cref="ViewModelViewHost"/>.
/// </summary>
public class ViewModelViewHostTest
{
    /// <summary>
    /// Tests that ViewModelProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered()
    {
        await Assert.That(ReactiveUI.Maui.ViewModelViewHost.ViewModelProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that DefaultContentProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultContentProperty_IsRegistered()
    {
        await Assert.That(ReactiveUI.Maui.ViewModelViewHost.DefaultContentProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that ViewContractObservableProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContractObservableProperty_IsRegistered()
    {
        await Assert.That(ReactiveUI.Maui.ViewModelViewHost.ViewContractObservableProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that ContractFallbackByPassProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractFallbackByPassProperty_IsRegistered()
    {
        await Assert.That(ReactiveUI.Maui.ViewModelViewHost.ContractFallbackByPassProperty).IsNotNull();
    }
}
