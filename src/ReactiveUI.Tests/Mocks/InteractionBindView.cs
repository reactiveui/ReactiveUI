// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// A bind view.
/// </summary>
public class InteractionBindView : ReactiveObject, IViewFor<InteractionBindViewModel>
{
    private InteractionBindViewModel? _viewModel;

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (InteractionBindViewModel?)value;
    }

    /// <inheritdoc/>
    public InteractionBindViewModel? ViewModel
    {
        get => _viewModel;
        set => this.RaiseAndSetIfChanged(ref _viewModel, value);
    }
}
