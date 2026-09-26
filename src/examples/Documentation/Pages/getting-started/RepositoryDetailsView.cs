// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Controls;

namespace ReactiveUI.Documentation.GettingStarted;

/// <summary>The console stand-in for the compelling example's per-row view: a title, a description and an open button.</summary>
[ExcludeFromViewRegistration]
[System.Diagnostics.DebuggerDisplay("RepositoryDetailsView ViewModel = {ViewModel}")]
public sealed class RepositoryDetailsView : ReactiveObject, IViewFor<RepositoryDetailsViewModel>
{
    /// <summary>Gets the label that shows the repository's full name.</summary>
    public Label TitleLabel { get; } = new();

    /// <summary>Gets the label that shows the repository's description.</summary>
    public Label DescriptionLabel { get; } = new();

    /// <summary>Gets the button that opens the repository's page.</summary>
    public Button OpenButton { get; } = new();

    /// <summary>Gets or sets the view model the row shows.</summary>
    public RepositoryDetailsViewModel? ViewModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (RepositoryDetailsViewModel?)value;
    }
}
