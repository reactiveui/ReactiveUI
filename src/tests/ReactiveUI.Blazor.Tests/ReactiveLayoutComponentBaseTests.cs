// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Blazor.Tests;

/// <summary>
/// Tests for the <see cref="ReactiveLayoutComponentBase{T}"/> class.
/// Verifies proper functioning of reactive features in Blazor layouts.
/// </summary>
public class ReactiveLayoutComponentBaseTests : BunitContext
{
    /// <summary>
    /// Verifies that changes to the ViewModel associated with a layout component trigger a re-render.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task ViewModel_Change_Triggers_StateHasChanged()
    {
        var viewModel = new TestViewModel();
        var cut = Render<TestLayoutComponent>(parameters => parameters.Add(p => p.ViewModel, viewModel));

        await Assert.That(cut.Instance.RenderCount).IsEqualTo(1);

        viewModel.SomeProperty = "Changed";

        await Task.Delay(100);
        await Assert.That(cut.Instance.RenderCount).IsGreaterThanOrEqualTo(2);
    }

    /// <summary>
    /// A simple ViewModel for layout testing.
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
    /// A concrete implementation of ReactiveLayoutComponentBase for testing.
    /// </summary>
    public class TestLayoutComponent : ReactiveLayoutComponentBase<TestViewModel>
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
