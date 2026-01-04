// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;

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

    /// <summary>
    /// Tests that ViewModel property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetAndGet_WorksCorrectly()
    {
        var host = new ReactiveUI.Maui.ViewModelViewHost();
        var viewModel = new TestViewModel();

        host.ViewModel = viewModel;

        await Assert.That(host.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that DefaultContent property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultContent_SetAndGet_WorksCorrectly()
    {
        var host = new ReactiveUI.Maui.ViewModelViewHost();
        var defaultContent = new Label { Text = "Default" };

        host.DefaultContent = defaultContent;

        await Assert.That(host.DefaultContent).IsEqualTo(defaultContent);
    }

    /// <summary>
    /// Tests that ContractFallbackByPass property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractFallbackByPass_SetAndGet_WorksCorrectly()
    {
        var host = new ReactiveUI.Maui.ViewModelViewHost();

        host.ContractFallbackByPass = true;

        await Assert.That(host.ContractFallbackByPass).IsTrue();

        host.ContractFallbackByPass = false;

        await Assert.That(host.ContractFallbackByPass).IsFalse();
    }

    /// <summary>
    /// Tests that ViewLocator property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewLocator_SetAndGet_WorksCorrectly()
    {
        var host = new ReactiveUI.Maui.ViewModelViewHost();
        var viewLocator = new TestViewLocator();

        host.ViewLocator = viewLocator;

        await Assert.That(host.ViewLocator).IsEqualTo(viewLocator);
    }

    /// <summary>
    /// Tests that ViewContractObservable property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewContractObservable_SetAndGet_WorksCorrectly()
    {
        var host = new ReactiveUI.Maui.ViewModelViewHost();
        var observable = Observable.Return("contract");

        host.ViewContractObservable = observable;

        await Assert.That(host.ViewContractObservable).IsEqualTo(observable);
    }

    /// <summary>
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel
    {
    }

    /// <summary>
    /// Test view locator for testing.
    /// </summary>
    private class TestViewLocator : IViewLocator
    {
        public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract = null)
            where TViewModel : class => null;

        public IViewFor<object>? ResolveView(object? instance, string? contract = null) => null;
    }
}
