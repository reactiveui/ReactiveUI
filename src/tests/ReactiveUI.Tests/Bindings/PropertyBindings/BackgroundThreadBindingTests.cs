// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.PropertyBindings;

/// <summary>
/// Verifies that a view model property change raised on a background thread still propagates through a two-way
/// binding to the view. This exercises the platform-agnostic binding pipeline (observation, switch onto the active
/// view model, change projection and the view-value setter) using the default synchronous scheduler, so it documents
/// the core contract that the WPF dispatcher-marshalled binder builds on.
/// </summary>
public class BackgroundThreadBindingTests
{
    /// <summary>
    /// A view model property assignment from a background thread must reach the bound view property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BackgroundThreadViewModelChangePropagatesToView()
    {
        var viewModel = new BackgroundBindViewModel();
        var view = new BackgroundBindView { ViewModel = viewModel };
        using var binding = view.Bind(view.ViewModel, static x => x.Text, static x => x.ViewText);

        await Task.Run(() => viewModel.Text = "background update");

        await Assert.That(view.ViewText).IsEqualTo("background update");
    }

    /// <summary>A minimal view model exposing a single reactive string property.</summary>
    public class BackgroundBindViewModel : ReactiveObject
    {
        private string? _text;

        /// <summary>Gets or sets the bound text.</summary>
        public string? Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }
    }

    /// <summary>A minimal view holding a reactive view model and a bound text property.</summary>
    public class BackgroundBindView : ReactiveObject, IViewFor<BackgroundBindViewModel>
    {
        private string? _viewText;
        private BackgroundBindViewModel? _viewModel;

        /// <summary>Gets or sets the value mirrored from the view model.</summary>
        public string? ViewText
        {
            get => _viewText;
            set => this.RaiseAndSetIfChanged(ref _viewText, value);
        }

        /// <inheritdoc/>
        public BackgroundBindViewModel? ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => _viewModel;
            set => ViewModel = (BackgroundBindViewModel?)value;
        }
    }
}
