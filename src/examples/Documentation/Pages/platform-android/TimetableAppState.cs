// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>The application state <see cref="BundleSuspensionDriver"/> saves into the activity bundle and restores
/// from it, so a recreated process picks up where the last one left off.</summary>
[System.Diagnostics.DebuggerDisplay("{LastViewedSubject}")]
public sealed class TimetableAppState
{
    /// <summary>Gets or sets the subject of the lesson the student last looked at.</summary>
    public string LastViewedSubject { get; set; } = string.Empty;
}
