// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>A one-line "now playing" strip for the playlist screen, shown under the "compact" contract.</summary>
[System.Diagnostics.DebuggerDisplay("CompactPlaylistView ViewModel = {ViewModel}")]
public sealed class CompactPlaylistView : IViewFor<PlaylistViewModel>
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
