// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Controls;

namespace ReactiveUI.Documentation.GitHub;

/// <summary>
/// The repository search screen: a search box, the results and an error line. It is left out of the views the generator
/// registers, so an app maps it explicitly, the way a view from another assembly or a plug-in is registered.
/// </summary>
[ExcludeFromViewRegistration]
[System.Diagnostics.DebuggerDisplay("RepositorySearchView ViewModel = {ViewModel}")]
public sealed class RepositorySearchView : ReactiveObject, IViewFor<RepositorySearchViewModel>
{
    /// <summary>Gets the box the user types a search into.</summary>
    public TextBox SearchBox { get; } = new();

    /// <summary>Gets the list of repositories found.</summary>
    public ListBox<Repository> ResultList { get; } = new();

    /// <summary>Gets the label that shows the last error.</summary>
    public Label ErrorLabel { get; } = new();

    /// <summary>Gets or sets the view model the screen shows.</summary>
    public RepositorySearchViewModel? ViewModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (RepositorySearchViewModel?)value;
    }
}
