// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Testing;

namespace ReactiveUI.Documentation.Testing;

/// <summary>An <see cref="IBuilder"/> that assembles a <see cref="GradeCalculatorViewModel"/> one field at a time.</summary>
[System.Diagnostics.DebuggerDisplay("Name = {_name}")]
public sealed class StudentBuilder : IBuilder
{
    /// <summary>The student's name, set through <see cref="WithName"/>.</summary>
    private string _name = string.Empty;

    /// <summary>The grades added so far.</summary>
    private List<int>? _grades = [];

    /// <summary>The credit hours earned per course.</summary>
    private Dictionary<string, int> _courseCredits = [];

    /// <summary>Sets the student's name.</summary>
    /// <param name="name">The student's name.</param>
    /// <returns>This builder.</returns>
    public StudentBuilder WithName(string name) => this.With(out _name, name);

    /// <summary>Adds a single grade.</summary>
    /// <param name="grade">The grade to add.</param>
    /// <returns>This builder.</returns>
    public StudentBuilder WithGrade(int grade) => this.With(ref _grades, grade);

    /// <summary>Adds several grades at once.</summary>
    /// <param name="grades">The grades to add.</param>
    /// <returns>This builder.</returns>
    public StudentBuilder WithGrades(IEnumerable<int> grades) => this.With(ref _grades, grades);

    /// <summary>Adds a single course's credit hours.</summary>
    /// <param name="course">The course name.</param>
    /// <param name="credit">The credit hours earned.</param>
    /// <returns>This builder.</returns>
    public StudentBuilder WithCourseCredit(string course, int credit) => this.With(ref _courseCredits, course, credit);

    /// <summary>Adds a single course's credit hours from a key/value pair.</summary>
    /// <param name="courseCredit">The course and the credit hours earned.</param>
    /// <returns>This builder.</returns>
    public StudentBuilder WithCourseCredit(KeyValuePair<string, int> courseCredit) => this.With(ref _courseCredits, courseCredit);

    /// <summary>Replaces every course credit with the given dictionary.</summary>
    /// <param name="courseCredits">The course credits to use.</param>
    /// <returns>This builder.</returns>
    public StudentBuilder WithCourseCredits(IDictionary<string, int> courseCredits) => this.With(ref _courseCredits, courseCredits);

    /// <summary>Builds the view model from the fields set so far.</summary>
    /// <returns>The built view model.</returns>
    public GradeCalculatorViewModel Build() => new(_name, _grades ?? [], _courseCredits);
}
