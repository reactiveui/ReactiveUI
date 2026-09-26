// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>The whole school timetable: every lesson, grouped into weekday summaries.</summary>
[System.Diagnostics.DebuggerDisplay("Lessons = {Lessons.Count}")]
public sealed class TimetableViewModel : ReactiveObject
{
    /// <summary>Initializes a new instance of the <see cref="TimetableViewModel"/> class with a sample week.</summary>
    public TimetableViewModel()
    {
        Lessons =
        [
            new() { Subject = "Mathematics", Room = "12", Weekday = "Monday" },
            new() { Subject = "Physics", Room = "7", Weekday = "Monday" },
            new() { Subject = "English", Room = "3", Weekday = "Tuesday" },
            new() { Subject = "History", Room = "9", Weekday = "Wednesday" },
            new() { Subject = "Chemistry", Room = "7", Weekday = "Thursday" },
            new() { Subject = "Art", Room = "1", Weekday = "Friday" },
        ];

        Weekdays =
        [
            .. Lessons
                .GroupBy(static lesson => lesson.Weekday)
                .Select(static group => new WeekdayViewModel { Name = group.Key, LessonCount = group.Count() }),
        ];
    }

    /// <summary>Gets every lesson on the timetable.</summary>
    public ObservableCollection<LessonViewModel> Lessons { get; }

    /// <summary>Gets the weekday summaries paged in the weekday pager.</summary>
    public ObservableCollection<WeekdayViewModel> Weekdays { get; }
}
