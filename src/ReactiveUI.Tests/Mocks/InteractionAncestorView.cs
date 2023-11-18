// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// A ancestor view.
/// </summary>
public class InteractionAncestorView : ReactiveObject, IViewFor<InteractionAncestorViewModel>
{
    private InteractionAncestorViewModel? _viewModel;

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (InteractionAncestorViewModel?)value;
    }

    /// <inheritdoc/>
    public InteractionAncestorViewModel? ViewModel
    {
        get => _viewModel;
        set => this.RaiseAndSetIfChanged(ref _viewModel, value);
    }
}
