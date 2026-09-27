// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The application state <see cref="AppSupportJsonSuspensionDriver"/> saves under Application Support and
/// restores from it, so a relaunched app picks up where the last one left off.</summary>
[System.Diagnostics.DebuggerDisplay("{LastViewedBook}")]
public sealed class LibraryAppState
{
    /// <summary>Gets or sets the title of the book the member last looked at.</summary>
    public string LastViewedBook { get; set; } = string.Empty;
}
