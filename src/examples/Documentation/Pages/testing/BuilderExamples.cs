// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Testing;

namespace ReactiveUI.Documentation.Testing;

/// <summary>Shows the fluent <c>With</c> overloads a test-data builder gets from implementing <see cref="IBuilder"/>.</summary>
public static class BuilderExamples
{
    /// <summary>The <c>With(out TField, TField)</c> overload sets a single field and returns the builder.</summary>
    public static void SetASingleField()
    {
        StudentBuilder builder = new StudentBuilder().WithName("Ada Lovelace");
        GradeCalculatorViewModel student = builder.Build();

        Console.WriteLine(student.StudentName);

        // Output:
        // Ada Lovelace
    }

    /// <summary>The two <c>List&lt;TField&gt;</c> overloads add one grade at a time or add several grades at once.</summary>
    public static void AddGradesOneAtATimeOrAllAtOnce()
    {
        StudentBuilder builder = new StudentBuilder()
            .WithName("Grace Hopper")
            .WithGrade(88)
            .WithGrades([92, 79]);

        GradeCalculatorViewModel student = builder.Build();

        Console.WriteLine(student.Grades.Count);
        Console.WriteLine(string.Join(", ", student.Grades));

        // Output:
        // 3
        // 88, 92, 79
    }

    /// <summary>The three dictionary overloads add a key and value, add a key/value pair, or replace the whole dictionary.</summary>
    public static void SetCourseCreditsThreeWays()
    {
        StudentBuilder builder = new StudentBuilder()
            .WithName("Katherine Johnson")
            .WithCourseCredit("Calculus", 4)
            .WithCourseCredit(new KeyValuePair<string, int>("Physics", 3));

        GradeCalculatorViewModel beforeReplace = builder.Build();
        Console.WriteLine(beforeReplace.CourseCredits.Count);

        builder.WithCourseCredits(new Dictionary<string, int> { ["Statistics"] = 4 });
        GradeCalculatorViewModel afterReplace = builder.Build();

        Console.WriteLine(afterReplace.CourseCredits.Count);
        Console.WriteLine(afterReplace.CourseCredits["Statistics"]);

        // Output:
        // 2
        // 1
        // 4
    }
}
