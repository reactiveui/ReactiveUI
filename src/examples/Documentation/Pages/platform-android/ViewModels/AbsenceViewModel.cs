// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>Tracks whether a student has been marked absent from a lesson.</summary>
[System.Diagnostics.DebuggerDisplay("{Subject}, Reported = {Reported}")]
public sealed class AbsenceViewModel : ReactiveObject
{
    /// <summary>Gets or sets the subject the absence applies to.</summary>
    public string Subject
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets or sets a value indicating whether the absence has been reported to the attendance service.</summary>
    public bool Reported
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
