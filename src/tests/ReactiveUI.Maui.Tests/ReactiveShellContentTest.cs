// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;
using ReactiveUI.Builder;
using ReactiveUI.Tests.Utilities.AppBuilder;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Maui.Tests;

/// <summary>Tests for <see cref="ReactiveShellContent{TViewModel}"/>.</summary>
public class ReactiveShellContentTest
{
    /// <summary>Tests that ViewModelProperty BindableProperty is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelProperty_IsRegistered() =>
        await Assert.That(ReactiveShellContent<TestViewModel>.ViewModelProperty).IsNotNull();

    /// <summary>Tests that ContractProperty BindableProperty is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ContractProperty_IsRegistered() =>
        await Assert.That(ReactiveShellContent<TestViewModel>.ContractProperty).IsNotNull();

    /// <summary>Tests that ViewModel property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_SetAndGet_WorksCorrectly()
    {
        var content = new ReactiveShellContent<TestViewModel>();
        var viewModel = new TestViewModel();

        content.ViewModel = viewModel;

        await Assert.That(content.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>Tests that Contract property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Contract_SetAndGet_WorksCorrectly()
    {
        const string contract = "TestContract";
        var content = new ReactiveShellContent<TestViewModel> { Contract = contract };

        await Assert.That(content.Contract).IsEqualTo(contract);
    }

    /// <summary>Tests that ViewModel can be null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_CanBeNull()
    {
        var content = new ReactiveShellContent<TestViewModel> { ViewModel = null };

        await Assert.That(content.ViewModel).IsNull();
    }

    /// <summary>Tests that Contract can be null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Contract_CanBeNull()
    {
        var content = new ReactiveShellContent<TestViewModel> { Contract = null };

        await Assert.That(content.Contract).IsNull();
    }

    /// <summary>Setting the view model resolves the registered view and assigns the content template.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<ReactiveShellContentTestExecutor>]
    public async Task ViewModelChange_WithRegisteredView_SetsContentTemplate()
    {
        var content = new ReactiveShellContent<TestViewModel> { ViewModel = new() };

        await Assert.That(content.ContentTemplate).IsNotNull();
    }

    /// <summary>Test executor that registers a view for the test view model.</summary>
    [NotInParallel]
    public sealed class ReactiveShellContentTestExecutor : MauiTestExecutor
    {
        /// <summary>The helper that configures and tears down the ReactiveUI app builder.</summary>
        private readonly AppBuilderTestHelper _helper = new();

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            _helper.Initialize(builder => _ = builder.WithMaui().WithCoreServices());

            AppLocator.CurrentMutable.Register<IViewFor<TestViewModel>>(static () => new TestView());
        }

        /// <inheritdoc/>
        protected override void CleanUp()
        {
            _helper.CleanUp();
            base.CleanUp();
        }
    }

    /// <summary>Test view model for testing.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "SST1436:Classes should not be empty", Justification = "Marker type for tests.")]
    private sealed class TestViewModel;

    /// <summary>Test view for the test view model.</summary>
    private sealed class TestView : ContentView, IViewFor<TestViewModel>
    {
        /// <inheritdoc/>
        public TestViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TestViewModel?)value;
        }
    }
}
