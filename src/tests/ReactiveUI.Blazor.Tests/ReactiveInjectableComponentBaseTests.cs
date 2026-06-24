// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace ReactiveUI.Blazor.Tests;

/// <summary>
/// Tests for the <see cref="ReactiveInjectableComponentBase{T}"/> class.
/// Verifies that ViewModels can be correctly injected via DI and that the component responds to changes.
/// </summary>
public class ReactiveInjectableComponentBaseTests : BunitContext
{
    /// <summary>The expected number of renders after the initial render of the component.</summary>
    private const int ExpectedRenderCount = 2;

    /// <summary>The delay in milliseconds allowed for the asynchronous UI update to settle.</summary>
    private const int RenderDelayMilliseconds = 100;

    /// <summary>The delay in milliseconds used to confirm that no asynchronous re-render occurred.</summary>
    private const int NoRenderDelayMilliseconds = 50;

    /// <summary>
    /// Verifies that a ViewModel registered in the service container is correctly injected into the component
    /// and that property changes on that ViewModel trigger a re-render.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ViewModel_Injected_Works()
    {
        var viewModel = new TestViewModel();
        _ = Services.AddSingleton(viewModel);

        var cut = Render<TestInjectableComponent>();

        // Verify injection was successful.
        await Assert.That(cut.Instance.ViewModel).IsEqualTo(viewModel);
        await Assert.That(cut.Instance.RenderCount).IsEqualTo(ExpectedRenderCount);

        // Trigger a change to verify the component is listening.
        viewModel.SomeProperty = "Changed";

        await Task.Delay(RenderDelayMilliseconds);
        await Assert.That(cut.Instance.RenderCount).IsGreaterThanOrEqualTo(ExpectedRenderCount);
    }

    /// <summary>Verifies that setting the ViewModel to the same value doesn't trigger notifications.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ViewModel_Set_Same_Value_No_Notification()
    {
        var viewModel = new TestViewModel();
        _ = Services.AddSingleton(viewModel);

        var cut = Render<TestInjectableComponent>();

        var renderCount = cut.Instance.RenderCount;

        // Set the same ViewModel again
        cut.Instance.ViewModel = viewModel;

        await Task.Delay(NoRenderDelayMilliseconds);

        // Should not have triggered a re-render
        await Assert.That(cut.Instance.RenderCount).IsEqualTo(renderCount);
    }

    /// <summary>Verifies that the explicit interface implementation works correctly.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task IViewFor_ViewModel_Explicit_Interface_Works()
    {
        var viewModel = new TestViewModel();
        _ = Services.AddSingleton(viewModel);

        var cut = Render<TestInjectableComponent>();
        IViewFor viewFor = cut.Instance;

        // Get through explicit interface
        var vm = viewFor.ViewModel;
        await Assert.That(vm).IsEqualTo(viewModel);

        // Set through explicit interface
        var newViewModel = new TestViewModel { SomeProperty = "New" };
        viewFor.ViewModel = newViewModel;

        await Assert.That(cut.Instance.ViewModel).IsEqualTo(newViewModel);
    }

    /// <summary>Verifies that Activated and Deactivated observables are accessible.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Activated_Deactivated_Observables_Work()
    {
        var viewModel = new TestViewModel();
        _ = Services.AddSingleton(viewModel);

        var cut = Render<TestInjectableComponent>();
        var activatable = cut.Instance;

        await Assert.That(activatable.Activated).IsNotNull();
        await Assert.That(activatable.Deactivated).IsNotNull();
    }

    /// <summary>Verifies that IActivatableViewModel activation works correctly.</summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ActivatableViewModel_Works()
    {
        var viewModel = new TestActivatableViewModel();
        _ = Services.AddSingleton(viewModel);

        var cut = Render<TestActivatableInjectableComponent>();

        await Task.Delay(RenderDelayMilliseconds);

        // Verify that the ViewModel was activated
        await Assert.That(viewModel.IsActivated).IsTrue();

        // Dispose to trigger deactivation
        cut.Instance.Dispose();

        await Task.Delay(RenderDelayMilliseconds);

        // Verify that the ViewModel was deactivated
        await Assert.That(viewModel.IsActivated).IsFalse();
    }

    /// <summary>A simple ReactiveObject ViewModel for testing.</summary>
    public class TestViewModel : ReactiveObject
    {
        /// <summary>Gets or sets a property that notifies on change.</summary>
        public string? SomeProperty
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    /// <summary>An activatable ViewModel for testing IActivatableViewModel support.</summary>
    public class TestActivatableViewModel : ReactiveObject, IActivatableViewModel
    {
        /// <summary>Initializes a new instance of the <see cref="TestActivatableViewModel"/> class.</summary>
        public TestActivatableViewModel() =>
            this.WhenActivated(disposables =>
            {
                IsActivated = true;
                _ = new ActionDisposable(() => IsActivated = false).DisposeWith(disposables);
            });

        /// <summary>Gets the ViewModelActivator for this ViewModel.</summary>
        public ViewModelActivator Activator { get; } = new();

        /// <summary>Gets a value indicating whether the ViewModel is currently activated.</summary>
        public bool IsActivated { get; private set; }
    }

    /// <summary>A concrete implementation of ReactiveInjectableComponentBase for testing.</summary>
    public class TestInjectableComponent : ReactiveInjectableComponentBase<TestViewModel>
    {
        /// <summary>Gets the number of times the component has rendered.</summary>
        public int RenderCount { get; private set; }

        /// <inheritdoc/>
        protected override void OnAfterRender(bool firstRender)
        {
            RenderCount++;
            base.OnAfterRender(firstRender);
        }
    }

    /// <summary>A concrete implementation of ReactiveInjectableComponentBase for testing with activatable ViewModels.</summary>
    public class TestActivatableInjectableComponent : ReactiveInjectableComponentBase<TestActivatableViewModel>;
}
