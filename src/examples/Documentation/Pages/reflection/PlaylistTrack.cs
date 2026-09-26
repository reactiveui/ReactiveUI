// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;

namespace ReactiveUI.Documentation.Reflection;

/// <summary>
/// One track on a playlist. <c>AutoPersist</c> reflects over this class's <c>[DataContract]</c> and
/// <c>[DataMember]</c> attributes to find which properties should trigger a save.
/// </summary>
[DataContract]
[System.Diagnostics.DebuggerDisplay("Title = {Title}, IsFavorite = {IsFavorite}")]
public sealed class PlaylistTrack : ReactiveObject
{
    /// <summary>Initializes a new instance of the <see cref="PlaylistTrack"/> class.</summary>
    /// <param name="title">The track's title.</param>
    public PlaylistTrack(string title) => Title = title;

    /// <summary>Gets or sets the track's title. Persisted, because it carries <c>[DataMember]</c>.</summary>
    [DataMember]
    public string Title
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets a value indicating whether the track is a favorite. Also persisted.</summary>
    [DataMember]
    public bool IsFavorite
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets how many times the track has played this session. Not persisted: no <c>[DataMember]</c>.</summary>
    public int PlayCount
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
