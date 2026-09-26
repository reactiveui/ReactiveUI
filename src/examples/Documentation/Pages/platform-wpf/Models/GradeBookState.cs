// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>The state <see cref="AutoSuspendHelper"/> asks the app to persist when it goes idle or exits.</summary>
[System.Diagnostics.DebuggerDisplay("{LastCourseName}")]
public sealed class GradeBookState
{
    /// <summary>Gets or sets the name of the course the user last had open.</summary>
    public string? LastCourseName { get; set; }
}
