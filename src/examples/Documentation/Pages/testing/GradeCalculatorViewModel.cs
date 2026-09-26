// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Testing;

/// <summary>Calculates a student's average grade and records each new grade on the message bus.</summary>
[System.Diagnostics.DebuggerDisplay("StudentName = {StudentName}, Average = {Average}")]
public sealed class GradeCalculatorViewModel : ReactiveObject, IDisposable
{
    /// <summary>Initializes a new instance of the <see cref="GradeCalculatorViewModel"/> class.</summary>
    /// <param name="studentName">The name of the student.</param>
    /// <param name="grades">The grades recorded so far.</param>
    /// <param name="courseCredits">The credit hours earned per course.</param>
    public GradeCalculatorViewModel(string studentName, IReadOnlyList<int> grades, IReadOnlyDictionary<string, int> courseCredits)
    {
        StudentName = studentName;
        Grades = grades;
        CourseCredits = courseCredits;
        Average = Grades.Count == 0 ? 0D : Grades.Average();

        RecordGrade = ReactiveCommand.Create<int, GradeRecorded>(grade =>
        {
            GradeRecorded recorded = new(StudentName, grade);
            MessageBus.Current.SendMessage(recorded);
            return recorded;
        });
    }

    /// <summary>Gets the student's name.</summary>
    public string StudentName { get; }

    /// <summary>Gets the grades recorded so far.</summary>
    public IReadOnlyList<int> Grades { get; }

    /// <summary>Gets the credit hours earned per course.</summary>
    public IReadOnlyDictionary<string, int> CourseCredits { get; }

    /// <summary>Gets the mean of <see cref="Grades"/>, or zero when there are none.</summary>
    public double Average { get; }

    /// <summary>Gets the command that records a new grade on the message bus.</summary>
    public ReactiveCommand<int, GradeRecorded> RecordGrade { get; }

    /// <inheritdoc/>
    public void Dispose() => RecordGrade.Dispose();
}
