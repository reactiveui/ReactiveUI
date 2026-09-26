// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;

namespace ReactiveUI.Documentation.DataPersistence;

/// <summary>One note in a notes app. Only the <c>[DataMember]</c> properties are worth saving.</summary>
[DataContract]
[System.Diagnostics.DebuggerDisplay("{Title}")]
public sealed class Note : ReactiveObject
{
    /// <summary>Gets or sets the note's title. Changing it should trigger a save.</summary>
    [DataMember]
    public string Title
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets or sets the note's text. Changing it should trigger a save.</summary>
    [DataMember]
    public string Body
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets or sets the last time the note was opened. Not saved: it is not a <c>[DataMember]</c>.</summary>
    public DateTimeOffset LastOpened
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
