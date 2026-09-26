// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace ReactiveUI.Documentation.Reflection;

/// <summary>The view model behind a form for adding a track to a playlist.</summary>
[System.Diagnostics.DebuggerDisplay("Title = {Title.Value}")]
public sealed class PlaylistTrackFormViewModel
{
    /// <summary>Initializes a new instance of the <see cref="PlaylistTrackFormViewModel"/> class.</summary>
    public PlaylistTrackFormViewModel() =>
        Title = new ReactiveProperty<string>(string.Empty, Sequencer.Immediate, false, false)
            .AddValidation(() => Title);

    /// <summary>Gets the title the user is typing. <c>[Required]</c> drives the validation <c>AddValidation</c> reads by reflection.</summary>
    [Required(ErrorMessage = "A track needs a title.")]
    public ReactiveProperty<string> Title { get; }
}
