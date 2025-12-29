// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Maui;

/// <summary>
/// Tests for <see cref="ReactiveUI.Maui.ReactiveShellContent{TViewModel}"/>.
/// </summary>
/// <remarks>
/// Note: ReactiveShellContent inherits from Microsoft.Maui.Controls.ShellContent which requires MAUI runtime initialization.
/// These tests focus on type inspection and static members that don't require instantiation.
/// </remarks>
public class ReactiveShellContentTest
{
    /// <summary>
    /// Tests that ReactiveShellContent has ViewModel property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasViewModelProperty()
    {
        var type = typeof(ReactiveUI.Maui.ReactiveShellContent<TestViewModel>);
        var property = type.GetProperty("ViewModel");

        await Assert.That(property).IsNotNull();
        await Assert.That(property!.PropertyType).IsEqualTo(typeof(TestViewModel));
    }

    /// <summary>
    /// Tests that ReactiveShellContent has Contract property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasContractProperty()
    {
        var type = typeof(ReactiveUI.Maui.ReactiveShellContent<TestViewModel>);
        var property = type.GetProperty("Contract");

        await Assert.That(property).IsNotNull();
        await Assert.That(property!.PropertyType).IsEqualTo(typeof(string));
    }

    /// <summary>
    /// Tests that ReactiveShellContent implements IActivatableView.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ImplementsIActivatableView()
    {
        var type = typeof(ReactiveUI.Maui.ReactiveShellContent<TestViewModel>);
        var interfaces = type.GetInterfaces();

        await Assert.That(interfaces).Contains(typeof(IActivatableView));
    }

    /// <summary>
    /// Tests that ViewModelProperty BindableProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered()
    {
        var type = typeof(ReactiveUI.Maui.ReactiveShellContent<TestViewModel>);
        var field = type.GetField("ViewModelProperty", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        await Assert.That(field).IsNotNull();
    }

    /// <summary>
    /// Tests that ContractProperty BindableProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractProperty_IsRegistered()
    {
        var type = typeof(ReactiveUI.Maui.ReactiveShellContent<TestViewModel>);
        var field = type.GetField("ContractProperty", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        await Assert.That(field).IsNotNull();
    }

    /// <summary>
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel
    {
    }
}
