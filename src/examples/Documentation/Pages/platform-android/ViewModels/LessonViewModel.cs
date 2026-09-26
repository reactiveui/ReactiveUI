// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>One lesson on the school timetable.</summary>
[System.Diagnostics.DebuggerDisplay("{Subject}, Room {Room}, {Weekday}")]
public sealed class LessonViewModel : ReactiveObject
{
    /// <summary>Gets or sets the subject taught in this lesson.</summary>
    public string Subject
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets or sets the room the lesson is held in.</summary>
    public string Room
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets or sets the weekday this lesson falls on.</summary>
    public string Weekday
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;
}
