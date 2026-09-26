// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>One page of the weekday pager, summarizing a single day of the timetable.</summary>
[System.Diagnostics.DebuggerDisplay("{Name}, LessonCount = {LessonCount}")]
public sealed class WeekdayViewModel : ReactiveObject
{
    /// <summary>Gets or sets the weekday name shown on the page.</summary>
    public string Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets or sets how many lessons fall on this day.</summary>
    public int LessonCount
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
