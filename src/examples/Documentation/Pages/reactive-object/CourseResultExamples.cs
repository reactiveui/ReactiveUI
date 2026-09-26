// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveObjects;

/// <summary>Shows <see cref="ReactiveRecord"/>: a mostly-immutable record that still raises change notifications for the one property that can move.</summary>
public static class CourseResultExamples
{
    /// <summary>
    /// A <see cref="ReactiveRecord"/> prints like any other record: <c>ToString</c> lists the derived record's own
    /// public properties in declaration order, and none of the notification members it inherits.
    /// </summary>
    public static void CreateAndDisplayAResult()
    {
        CourseResult result = new() { StudentName = "Grace Hopper", Course = "Compilers", Grade = 95 };
        result.AddModeratorNote("Checked twice");

        Console.WriteLine(result);

        // Output:
        // CourseResult { StudentName = Grace Hopper, Course = Compilers, Grade = 95, ModeratorNotes = Checked twice }
    }

    /// <summary>Setting <c>ModeratorNotes</c> raises <c>Changing</c> then <c>Changed</c>, the same as a mutable property on a <see cref="ReactiveObject"/>.</summary>
    public static void RaiseOnModeratorNoteChange()
    {
        CourseResult result = new() { StudentName = "Grace Hopper", Course = "Compilers", Grade = 95 };
        List<string?> events = [];
        using IDisposable changingSubscription = result.Changing.Subscribe(args => events.Add($"changing {args.PropertyName}"));
        using IDisposable changedSubscription = result.Changed.Subscribe(args => events.Add($"changed {args.PropertyName}"));

        result.AddModeratorNote("Pending appeal");

        Console.WriteLine(string.Join(" -> ", events));

        // Output:
        // changing ModeratorNotes -> changed ModeratorNotes
    }

    /// <summary><c>SuppressChangeNotifications</c>, <c>AreChangeNotificationsEnabled</c> and <c>DelayChangeNotifications</c> work the same way on a <see cref="ReactiveRecord"/>.</summary>
    public static void SuppressAndDelayWhileEnteringNotes()
    {
        CourseResult result = new() { StudentName = "Alan Turing", Course = "Logic", Grade = 98 };
        List<string?> changed = [];
        using IDisposable subscription = result.Changed.Subscribe(args => changed.Add(args.PropertyName));

        using (result.SuppressChangeNotifications())
        {
            Console.WriteLine(result.AreChangeNotificationsEnabled());
            result.AddModeratorNote("Awaiting appeal");
        }

        using (result.DelayChangeNotifications())
        {
            result.AddModeratorNote("Appeal received");
            result.AddModeratorNote("Appeal upheld");
            Console.WriteLine(changed.Count);
        }

        Console.WriteLine(result.AreChangeNotificationsEnabled());
        Console.WriteLine(changed.Count);

        // Output:
        // False
        // 0
        // True
        // 1
    }

    /// <summary><c>with</c> clones a record and changes only the named properties; the clone is a separate instance with its own notification state.</summary>
    public static void CloneWithChanges()
    {
        CourseResult original = new() { StudentName = "Katherine Johnson", Course = "Orbital Mechanics", Grade = 99 };
        CourseResult corrected = original with { Grade = 100 };

        Console.WriteLine(original.Grade);
        Console.WriteLine(corrected.Grade);
        Console.WriteLine(original.StudentName == corrected.StudentName);

        // Output:
        // 99
        // 100
        // True
    }

    /// <summary>
    /// <c>ReactiveRecord.Equals(ReactiveRecord)</c> compares two records by the derived record's own values, even when
    /// both are only known through their base type, such as inside a routine that audits every record type in the
    /// school's data layer. Two separately created results with the same values are equal.
    /// </summary>
    public static void CompareTwoResultsAsBaseRecords()
    {
        CourseResult original = new() { StudentName = "Katherine Johnson", Course = "Orbital Mechanics", Grade = 99 };
        CourseResult enteredAgain = new() { StudentName = "Katherine Johnson", Course = "Orbital Mechanics", Grade = 99 };
        CourseResult correctedCopy = original with { Grade = 100 };
        ReactiveRecord originalAsRecord = original;

        Console.WriteLine(originalAsRecord.Equals(enteredAgain));
        Console.WriteLine(originalAsRecord.Equals(correctedCopy));
        Console.WriteLine(original == enteredAgain);

        // Output:
        // True
        // False
        // True
    }
}
