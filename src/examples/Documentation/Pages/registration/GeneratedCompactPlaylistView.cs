// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.SourceGenerators;

namespace ReactiveUI.Documentation.Registration;

/// <summary>A compact singleton playlist view registered at compile time under the <c>"compact"</c> contract.</summary>
[RegisterView<GeneratedPlaylistViewModel>(Contract = "compact", IsSingleton = true)]
[System.Diagnostics.DebuggerDisplay("GeneratedCompactPlaylistView ViewModel = {ViewModel}")]
public sealed class GeneratedCompactPlaylistView : IViewFor<GeneratedPlaylistViewModel>
{
    /// <inheritdoc/>
    public GeneratedPlaylistViewModel? ViewModel { get; set; }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (GeneratedPlaylistViewModel?)value;
    }
}
