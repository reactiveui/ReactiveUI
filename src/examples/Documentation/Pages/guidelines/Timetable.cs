// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>A school's timetable: a long-lived dependency several screens read, standing in for a singleton service.</summary>
[System.Diagnostics.DebuggerDisplay("NextClass = {NextClass}")]
public sealed class Timetable : ReactiveObject
{
    /// <summary>Gets or sets the next class on the timetable.</summary>
    public string NextClass
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;
}
