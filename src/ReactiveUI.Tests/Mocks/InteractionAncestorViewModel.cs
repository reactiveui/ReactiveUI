// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// A ancestor view model.
/// </summary>
/// <seealso cref="ReactiveUI.ReactiveObject" />
public class InteractionAncestorViewModel : ReactiveObject
{
    private InteractionBindViewModel _interactionBindViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionAncestorViewModel"/> class.
    /// </summary>
    public InteractionAncestorViewModel()
    {
        _interactionBindViewModel = new InteractionBindViewModel();
    }

    /// <summary>
    /// Gets or sets the interaction view model.
    /// </summary>
    public InteractionBindViewModel InteractionViewModel
    {
        get => _interactionBindViewModel;
        set => this.RaiseAndSetIfChanged(ref _interactionBindViewModel, value);
    }
}
