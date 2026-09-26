// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveObjects;

/// <summary>Shows how a <see cref="ReactiveObject"/> property raises change notifications from a registrar's edit form.</summary>
public static class StudentExamples
{
    /// <summary><c>RaiseAndSetIfChanged</c> raises <c>PropertyChanged</c> only when a value really changes.</summary>
    public static void EditNameAndCourseFromAForm()
    {
        Student student = new();
        List<string> changedProperties = [];
        student.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName ?? string.Empty);

        student.Name = "Ada Lovelace";
        student.Course = "Mathematics";
        student.Course = "Mathematics";

        Console.WriteLine(string.Join(", ", changedProperties));

        // Output:
        // Name, Course
    }

    /// <summary>
    /// <c>RaisePropertyChanging</c> and <c>RaisePropertyChanged</c> notify around a computed property that has no
    /// backing field, so <c>RaiseAndSetIfChanged</c> cannot raise them for it.
    /// </summary>
    public static void RaiseAroundAComputedProperty()
    {
        Student student = new() { Name = "Ada Lovelace" };
        List<string> changingProperties = [];
        List<string> changedProperties = [];
        student.PropertyChanging += (_, e) => changingProperties.Add(e.PropertyName ?? string.Empty);
        student.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName ?? string.Empty);

        student.AddGrade(88);
        student.AddGrade(92);

        Console.WriteLine(student.GradeAverage);
        Console.WriteLine(string.Join(", ", changingProperties));
        Console.WriteLine(string.Join(", ", changedProperties));

        // Output:
        // 90
        // GradeAverage, GradeAverage
        // GradeAverage, GradeAverage
    }
}
