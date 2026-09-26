// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveObjects;

/// <summary>A bulk import writes many students at once; suppressing notifications avoids a flood of UI updates, and a small audit trail records what happened instead.</summary>
public static class BulkImportExamples
{
    /// <summary>Each row is imported under <c>SuppressChangeNotifications</c>, then a hand-built audit entry replaces the live notification the UI never saw.</summary>
    public static void ImportRosterWithoutLiveNotifications()
    {
        List<Student> roster =
        [
            new Student { Name = "Ada Lovelace", Course = "Mathematics" },
            new Student { Name = "Grace Hopper", Course = "Computer Science" },
        ];
        int[] importedGrades = [88, 92];
        List<string> auditLog = [];

        for (int i = 0; i < roster.Count; i++)
        {
            Student student = roster[i];
            using (student.SuppressChangeNotifications())
            {
                student.AddGrade(importedGrades[i]);
            }

            ReactivePropertyChangingEventArgs<Student> preview = new(student, nameof(Student.GradeAverage));
            ReactivePropertyChangedEventArgs<Student> applied = new(student, nameof(Student.GradeAverage));
            auditLog.Add(DescribeImport(preview));
            auditLog.Add(DescribeImport(applied));
        }

        foreach (string entry in auditLog)
        {
            Console.WriteLine(entry);
        }

        // Output:
        // about to import GradeAverage for Ada Lovelace
        // imported GradeAverage for Ada Lovelace
        // about to import GradeAverage for Grace Hopper
        // imported GradeAverage for Grace Hopper
    }

    /// <summary>Describes one audit entry; it accepts either a changing or a changed notification through the shared interface.</summary>
    /// <param name="args">The notification to describe.</param>
    /// <returns>A one-line description naming the sender and the property.</returns>
    private static string DescribeImport(IReactivePropertyChangedEventArgs<Student> args) =>
        args is ReactivePropertyChangingEventArgs<Student>
            ? $"about to import {args.PropertyName} for {args.Sender.Name}"
            : $"imported {args.PropertyName} for {args.Sender.Name}";
}
