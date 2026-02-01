// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;

namespace ReactiveUI.Blazor.Tests;

/// <summary>
/// Tests for the <see cref="ReactiveComponentBase{T}"/> class.
/// These tests verify proper rendering, property change handling, and activation logic within a Blazor environment.
/// </summary>
public class ReactiveComponentBaseTests : BunitContext
{
    /// <summary>
    /// Verifies that changing a property on the bound ViewModel triggers the component to re-render.
    /// This ensures that the component is correctly observing ViewModel property changes.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ViewModel_Change_Triggers_StateHasChanged()
    {
        var viewModel = new TestViewModel();
        var cut = Render<TestComponent>(parameters => parameters.Add(p => p.ViewModel, viewModel));

        // Initial render should have occurred once.
        await Assert.That(cut.Instance.RenderCount).IsEqualTo(1);

        // Change a property on the ViewModel to trigger a notification.
        viewModel.SomeProperty = "Changed";

        // ReactiveComponentBase uses throttled/debounced logic for StateHasChanged.
        // Waiting briefly allows the asynchronous UI update to complete.
        await Task.Delay(100);
        await Assert.That(cut.Instance.RenderCount).IsGreaterThanOrEqualTo(2);
    }

    /// <summary>
    /// Verifies that replacing the ViewModel instance on the component triggers a re-render.
    /// This ensures that the component reacts to the binding change itself.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ViewModel_Instance_Change_Triggers_StateHasChanged()
    {
        var viewModel1 = new TestViewModel();
        var cut = Render<TestComponent>(parameters => parameters.Add(p => p.ViewModel, viewModel1));

        await Assert.That(cut.Instance.RenderCount).IsEqualTo(1);

        var viewModel2 = new TestViewModel();
        cut.Render(parameters => parameters.Add(p => p.ViewModel, viewModel2));

        // Wait for the asynchronous update triggered by the property setter.
        await Task.Delay(100);
        await Assert.That(cut.Instance.RenderCount).IsGreaterThanOrEqualTo(2);
    }

    /// <summary>
    /// Verifies that <see cref="IActivatableViewModel"/> ViewModels are activated when the component is initialized
    /// and deactivated when the component is disposed.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task Activation_Works()
    {
        var viewModel = new TestActivatableViewModel();
        var cut = Render<TestActivatableComponent>(parameters => parameters.Add(p => p.ViewModel, viewModel));

        // The ViewModel should be active immediately after the component is rendered.
        await Assert.That(viewModel.IsActive).IsTrue();

        // Disposing the component (simulating navigation away or removal) should deactivate the ViewModel.
        cut.Instance.Dispose();
        await Assert.That(viewModel.IsActive).IsFalse();
    }

    /// <summary>
    /// A simple ReactiveObject ViewModel for testing property change notifications.
    /// </summary>
    public class TestViewModel : ReactiveObject
    {
        private string? _someProperty;

        /// <summary>
        /// Gets or sets a test property that raises INotifyPropertyChanged events.
        /// </summary>
        public string? SomeProperty
        {
            get => _someProperty;
            set => this.RaiseAndSetIfChanged(ref _someProperty, value);
        }
    }

    /// <summary>
    /// A test ViewModel implementing IActivatableViewModel to verify activation lifecycle.
    /// </summary>
    public class TestActivatableViewModel : ReactiveObject, IActivatableViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestActivatableViewModel"/> class.
        /// Sets up the WhenActivated block to toggle the IsActive flag.
        /// </summary>
        public TestActivatableViewModel() =>
            this.WhenActivated(d =>
            {
                IsActive = true;
                Disposable.Create(() => IsActive = false).DisposeWith(d);
            });

        /// <summary>
        /// Gets the ViewModelActivator required for activation logic.
        /// </summary>
        public ViewModelActivator Activator { get; } = new();

        /// <summary>
        /// Gets a value indicating whether the ViewModel is currently active.
        /// </summary>
        public bool IsActive { get; private set; }
    }

    /// <summary>
    /// A concrete implementation of ReactiveComponentBase for testing purposes.
    /// Tracks the number of times it has been rendered.
    /// </summary>
    public class TestComponent : ReactiveComponentBase<TestViewModel>
    {
        /// <summary>
        /// Gets the number of times OnAfterRender has been called.
        /// </summary>
        public int RenderCount { get; private set; }

        /// <inheritdoc/>
        protected override void OnAfterRender(bool firstRender)
        {
            RenderCount++;
            base.OnAfterRender(firstRender);
        }
    }

    /// <summary>
    /// A concrete implementation of ReactiveComponentBase for testing activatable ViewModels.
    /// </summary>
    public class TestActivatableComponent : ReactiveComponentBase<TestActivatableViewModel>;
}
