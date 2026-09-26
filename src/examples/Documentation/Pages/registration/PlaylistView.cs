// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>The full playlist screen: cover art, track list and transport controls.</summary>
[System.Diagnostics.DebuggerDisplay("PlaylistView ViewModel = {ViewModel}")]
public sealed class PlaylistView : IViewFor<PlaylistViewModel>
{
    /// <inheritdoc/>
    public PlaylistViewModel? ViewModel { get; set; }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (PlaylistViewModel?)value;
    }
}
