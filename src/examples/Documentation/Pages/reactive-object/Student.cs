// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveObjects;

/// <summary>A student record a registrar edits from an enrolment form.</summary>
[System.Diagnostics.DebuggerDisplay("{Name}: {Course}, GradeAverage = {GradeAverage}")]
public sealed class Student : ReactiveObject
{
    /// <summary>The grades recorded for the student so far.</summary>
    private readonly List<int> _grades = [];

    /// <summary>Gets or sets the student's full name.</summary>
    public string Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets or sets the course the student is enrolled in.</summary>
    public string Course
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets the mean of every recorded grade, or zero if none have been recorded.</summary>
    public double GradeAverage => _grades.Count == 0 ? 0 : _grades.Average();

    /// <summary>Records a new grade and updates <see cref="GradeAverage"/>.</summary>
    /// <param name="grade">The grade to add.</param>
    /// <remarks>
    /// <see cref="GradeAverage"/> has no backing field, so <c>RaiseAndSetIfChanged</c> does not apply. Raising the
    /// notifications by hand around the change that affects it is the pattern the library recommends instead.
    /// </remarks>
    public void AddGrade(int grade)
    {
        this.RaisePropertyChanging(nameof(GradeAverage));
        _grades.Add(grade);
        this.RaisePropertyChanged(nameof(GradeAverage));
    }
}
