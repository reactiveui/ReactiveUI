// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>A student enrolled in one course, with a grade and an enrollment state that both change over a term.</summary>
[System.Diagnostics.DebuggerDisplay("Name = {Name}, Grade = {Grade}")]
public sealed class Student : ReactiveObject
{
    /// <summary>Initializes a new instance of the <see cref="Student"/> class.</summary>
    /// <param name="name">The student's name.</param>
    /// <param name="course">The course the student is enrolled in.</param>
    public Student(string name, string course)
    {
        Name = name;
        Enrollment = new Enrollment(course);
    }

    /// <summary>Gets the student's name.</summary>
    public string Name { get; }

    /// <summary>Gets the student's enrollment in <see cref="Enrollment"/>'s course.</summary>
    public Enrollment Enrollment { get; }

    /// <summary>Gets or sets the student's grade.</summary>
    public int Grade
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
