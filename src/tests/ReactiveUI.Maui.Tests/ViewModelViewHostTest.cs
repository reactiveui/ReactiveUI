// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Maui.Tests;

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
    /// Tests that ResolveViewForViewModel resolves the view and sets the content.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ResolveViewForViewModel_ResolvesAndSetsContent()
    {
        var host = new TestableViewModelViewHost();
        var viewModel = new TestViewModel();
        var view = new TestView();
        var locator = new MockViewLocator(view);

        host.ViewLocator = locator;
        host.ViewModel = viewModel;
        host.SimulateViewModelChange();

        await Assert.That(host.Content).IsEqualTo(view);
        await Assert.That(view.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that DefaultContent is shown when ViewModel is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultContent_IsShown_WhenViewModelIsNull()
    {
        var host = new TestableViewModelViewHost();
        var defaultContent = new Label();
        host.DefaultContent = defaultContent;

        // Trigger update
        host.ViewModel = new TestViewModel(); // First set to something
        host.ViewModel = null; // Then set to null
        host.SimulateViewModelChange();

        await Assert.That(host.Content).IsEqualTo(defaultContent);
    }

    /// <summary>
    /// Test view model for testing.
    /// </summary>
    private class TestViewModel
    {
    }

    private class TestView : ContentView, IViewFor<TestViewModel>
    {
        public TestViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }

    private class MockViewLocator : IViewLocator
    {
        private readonly IViewFor _view;

        public MockViewLocator(IViewFor view)
        {
            _view = view;
        }

        public IViewFor<T>? ResolveView<T>(string? contract = null)
            where T : class => _view as IViewFor<T>;

        [RequiresUnreferencedCode("This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
        [RequiresDynamicCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
        public IViewFor? ResolveView(object? viewModel, string? contract = null) => _view;
    }

    private class TestableViewModelViewHost : ViewModelViewHost
    {
        public void SimulateViewModelChange()
        {
            ResolveViewForViewModel(ViewModel, ViewContract);
        }
    }

    /// <summary>
    /// Test view locator for testing.
    /// </summary>
    private class TestViewLocator : IViewLocator
    {
        public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract = null)
            where TViewModel : class => null;

        [RequiresUnreferencedCode("This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
        [RequiresDynamicCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
        public IViewFor? ResolveView(object? instance, string? contract = null) => null;
    }
}