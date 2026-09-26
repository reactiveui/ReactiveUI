// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>
/// The library module's catalog of tracks. A registrar only ever holds this behind a <see cref="Func{TResult}"/>
/// factory, so nothing in the compiled code calls <c>new TrackLibrary()</c> directly. A trimmed build can only
/// see that call by tracing through the factory delegate, which the linker does not do, so the attribute tells it
/// to keep the type anyway.
/// </summary>
[Preserve(AllMembers = true)]
[System.Diagnostics.DebuggerDisplay("TrackLibrary Titles = {Titles.Count}")]
public sealed class TrackLibrary : ITrackLibrary
{
    /// <inheritdoc/>
    public IReadOnlyList<string> Titles { get; } = ["Clair de Lune", "Take Five", "So What"];
}
