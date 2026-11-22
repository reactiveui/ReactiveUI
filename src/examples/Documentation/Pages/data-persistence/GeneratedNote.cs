// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;
using ReactiveUI.SourceGenerators;

namespace ReactiveUI.Documentation.DataPersistence;

/// <summary>A note whose <c>AutoPersistMetadata</c> is generated at compile time by <see cref="AutoPersistAttribute"/>.</summary>
[DataContract]
[AutoPersist]
[System.Diagnostics.DebuggerDisplay("{Title}")]
public sealed partial class GeneratedNote : ReactiveObject
{
    /// <summary>Gets or sets the note's title. Changing it requests a save.</summary>
    [DataMember]
    public string Title
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets or sets the note's text. Changing it requests a save.</summary>
    [DataMember]
    public string Body
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets or sets the last time the note was opened. Not persisted because it has no <c>[DataMember]</c>.</summary>
    public DateTimeOffset LastOpened
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
