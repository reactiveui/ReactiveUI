// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace ReactiveUI.Blazor.Tests;

/// <summary>
/// Tests for the <see cref="ReactiveInjectableComponentBase{T}"/> class.
/// Verifies that ViewModels can be correctly injected via DI and that the component responds to changes.
/// </summary>
public class ReactiveInjectableComponentBaseTests : BunitContext
{
    /// <summary>
    /// Verifies that a ViewModel registered in the service container is correctly injected into the component
    /// and that property changes on that ViewModel trigger a re-render.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ViewModel_Injected_Works()
    {
        var viewModel = new TestViewModel();
        Services.AddSingleton(viewModel);

        var cut = Render<TestInjectableComponent>();

        // Verify injection was successful.
        await Assert.That(cut.Instance.ViewModel).IsEqualTo(viewModel);
        await Assert.That(cut.Instance.RenderCount).IsEqualTo(1);

        // Trigger a change to verify the component is listening.
        viewModel.SomeProperty = "Changed";

        await Task.Delay(100);
        await Assert.That(cut.Instance.RenderCount).IsGreaterThanOrEqualTo(2);
    }

    /// <summary>
    /// A simple ReactiveObject ViewModel for testing.
    /// </summary>
    public class TestViewModel : ReactiveObject
    {
        private string? _someProperty;

        /// <summary>
        /// Gets or sets a property that notifies on change.
        /// </summary>
        public string? SomeProperty
        {
            get => _someProperty;
            set => this.RaiseAndSetIfChanged(ref _someProperty, value);
        }
    }

    /// <summary>
    /// A concrete implementation of ReactiveInjectableComponentBase for testing.
    /// </summary>
    public class TestInjectableComponent : ReactiveInjectableComponentBase<TestViewModel>
    {
        /// <summary>
        /// Gets the number of times the component has rendered.
        /// </summary>
        public int RenderCount { get; private set; }

        /// <inheritdoc/>
        protected override void OnAfterRender(bool firstRender)
        {
            RenderCount++;
            base.OnAfterRender(firstRender);
        }
    }
}
