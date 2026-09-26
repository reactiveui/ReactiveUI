// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveObjects;

/// <summary>Shows the classic <see cref="System.ComponentModel.INotifyPropertyChanged"/> events, exception reporting, and the
/// remaining equality members a <see cref="ReactiveRecord"/> such as <see cref="CourseResult"/> inherits.</summary>
public static class RecordPlumbingExamples
{
    /// <summary>
    /// Subscribing to the classic <see cref="System.ComponentModel.INotifyPropertyChanging.PropertyChanging"/> and
    /// <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/> events on a <see cref="ReactiveRecord"/>
    /// works the same way as on a <see cref="ReactiveObject"/>; a control that only knows the classic events, rather
    /// than <c>Changing</c>/<c>Changed</c>, still gets notified.
    /// </summary>
    public static void SubscribeToClassicEventsOnAResult()
    {
        CourseResult result = new() { StudentName = "Ada Lovelace", Course = "Algorithms", Grade = 91 };
        int classicChanging = 0;
        int classicChanged = 0;
        result.PropertyChanging += (_, _) => classicChanging++;
        result.PropertyChanged += (_, _) => classicChanged++;

        result.AddModeratorNote("Reviewed");

        Console.WriteLine(classicChanging);
        Console.WriteLine(classicChanged);

        // Output:
        // 1
        // 1
    }

    /// <summary>An exception thrown by a <c>Changed</c> subscriber on a <see cref="ReactiveRecord"/> is caught and forwarded
    /// to <see cref="ReactiveRecord.ThrownExceptions"/> instead of crashing the caller.</summary>
    public static void ObserveThrownExceptionsOnAResult()
    {
        CourseResult result = new() { StudentName = "Grace Hopper", Course = "Compilers", Grade = 95 };
        List<Exception> errors = [];
        using IDisposable errorSubscription = result.ThrownExceptions.Subscribe(errors.Add);
        using IDisposable changeSubscription = result.Changed.Subscribe(
            static _ => throw new InvalidOperationException("Moderator notes must go through the review queue."));

        result.AddModeratorNote("Pending appeal");

        Console.WriteLine(errors.Count);
        Console.WriteLine(errors[0].Message);

        // Output:
        // 1
        // Moderator notes must go through the review queue.
    }

    /// <summary>
    /// <c>ReactiveRecord</c> declares its own <c>==</c> and <c>!=</c>; comparing through two variables declared as
    /// <see cref="ReactiveRecord"/>, such as inside a routine that audits every record type in the school's data
    /// layer, reaches them instead of <see cref="CourseResult"/>'s own operators. <c>GetHashCode</c> combines the
    /// base record's own hash with the derived record's members.
    /// </summary>
    public static void CompareAsBaseRecordsAndHash()
    {
        CourseResult original = new() { StudentName = "Katherine Johnson", Course = "Orbital Mechanics", Grade = 99 };
        CourseResult enteredAgain = new() { StudentName = "Katherine Johnson", Course = "Orbital Mechanics", Grade = 99 };
        CourseResult correctedCopy = original with { Grade = 100 };
        ReactiveRecord originalAsRecord = original;
        ReactiveRecord enteredAgainAsRecord = enteredAgain;
        ReactiveRecord correctedCopyAsRecord = correctedCopy;

        Console.WriteLine(originalAsRecord == enteredAgainAsRecord);
        Console.WriteLine(originalAsRecord != correctedCopyAsRecord);
        Console.WriteLine(original.GetHashCode() == enteredAgain.GetHashCode());

        // Output:
        // True
        // True
        // True
    }
}
