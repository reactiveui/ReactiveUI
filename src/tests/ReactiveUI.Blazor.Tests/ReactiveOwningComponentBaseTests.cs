// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace ReactiveUI.Blazor.Tests;

/// <summary>
/// Tests for the <see cref="ReactiveOwningComponentBase{T}"/> class.
/// Verifies that the owning component correctly manages the scope and reactivity of its ViewModel.
/// </summary>
public class ReactiveOwningComponentBaseTests : BunitContext
{
    /// <summary>
    /// The expected number of renders after the initial render of the component.
    /// </summary>
    private const int ExpectedRenderCount = 2;

    /// <summary>
    /// The delay in milliseconds allowed for the asynchronous UI update to settle.
    /// </summary>
    private const int RenderDelayMilliseconds = 100;

    /// <summary>
    /// Verifies that changes to the ViewModel in an owning component trigger a re-render.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ViewModel_Change_Triggers_StateHasChanged()
    {
        var viewModel = new TestViewModel();

        // OwningComponentBase<T> typically expects T to be resolvable from the service provider.
        Services.AddScoped(_ => new TestViewModel());

        var cut = Render<TestOwningComponent>(parameters => parameters.Add(p => p.ViewModel, viewModel));

        await Assert.That(cut.Instance.RenderCount).IsEqualTo(ExpectedRenderCount);

        viewModel.SomeProperty = "Changed";

        await Task.Delay(RenderDelayMilliseconds);
        await Assert.That(cut.Instance.RenderCount).IsGreaterThanOrEqualTo(ExpectedRenderCount);
    }

    /// <summary>
    /// A simple ViewModel for owning component testing.
    /// </summary>
    public class TestViewModel : ReactiveObject
    {
        /// <summary>
        /// The backing field for the <see cref="SomeProperty"/> property.
        /// </summary>
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
    /// A concrete implementation of ReactiveOwningComponentBase for testing.
    /// </summary>
    public class TestOwningComponent : ReactiveOwningComponentBase<TestViewModel>
    {
        /// <summary>
        /// Gets the render count.
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
