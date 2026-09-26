// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>The view model behind the "About" page, a plain WPF <see cref="System.Windows.Controls.Page"/> shown outside the router.</summary>
/// <param name="courseCount">The number of courses in the grade book.</param>
/// <param name="studentCount">The number of students across every course.</param>
[System.Diagnostics.DebuggerDisplay("{CourseCount} courses, {StudentCount} students")]
public sealed class CourseInfoPageViewModel(int courseCount, int studentCount) : ReactiveObject
{
    /// <summary>Gets the number of courses in the grade book.</summary>
    public int CourseCount { get; } = courseCount;

    /// <summary>Gets the number of students across every course.</summary>
    public int StudentCount { get; } = studentCount;
}
