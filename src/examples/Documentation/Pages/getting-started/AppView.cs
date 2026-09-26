// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Controls;

namespace ReactiveUI.Documentation.GettingStarted;

/// <summary>
/// The console stand-in for the compelling example's main window: a search box, a label that shows whether results are
/// available, and the list of results.
/// </summary>
[ExcludeFromViewRegistration]
[System.Diagnostics.DebuggerDisplay("AppView ViewModel = {ViewModel}")]
public sealed class AppView : ReactiveObject, IViewFor<AppViewModel>
{
    /// <summary>Gets the box the user types a search into.</summary>
    public TextBox SearchBox { get; } = new();

    /// <summary>Gets the label shown while search results are available.</summary>
    public Label AvailableLabel { get; } = new();

    /// <summary>Gets the list of repositories found.</summary>
    public ListBox<RepositoryDetailsViewModel> ResultList { get; } = new();

    /// <summary>Gets or sets the view model the window shows.</summary>
    public AppViewModel? ViewModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (AppViewModel?)value;
    }
}
