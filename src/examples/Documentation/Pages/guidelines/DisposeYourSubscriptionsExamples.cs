// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>Shows disposing a subscription with <c>WhenActivated</c> and <c>DisposeWith</c>, and when a subscription needs no disposal at all.</summary>
public static class DisposeYourSubscriptionsExamples
{
    /// <summary>A subscription that is never kept anywhere cannot be disposed, so it keeps firing for as long as the student it watches is alive.</summary>
    public static void AvoidNotDisposingTheSubscription()
    {
        Student student = new("Grace", "Robotics Club") { Grade = 88 };
        AvoidGradeAnnouncer announcer = new(student); // prints the current grade immediately: 88

        student.Grade = 95;
        announcer.Dispose(); // there is nothing here to stop
        student.Grade = 99; // still announced: the subscription leaked

        // Output:
        // Avoid: Grace's grade is now 88
        // Avoid: Grace's grade is now 95
        // Avoid: Grace's grade is now 99
    }

    /// <summary><c>WhenActivated</c> plus <c>DisposeWith</c> ties the subscription to activation, so deactivating stops it.</summary>
    public static void PreferDisposingWithActivation()
    {
        Student student = new("Grace", "Robotics Club") { Grade = 88 };
        using PreferGradeAnnouncer announcer = new(student);
        IDisposable activation = announcer.Activator.Activate(); // prints the current grade immediately: 88

        student.Grade = 95;
        activation.Dispose(); // stops the subscription WhenActivated set up
        student.Grade = 99; // not announced

        // Output:
        // Prefer: Grace's grade is now 88
        // Prefer: Grace's grade is now 95
    }

    /// <summary>A view model that watches its own property does not need to dispose that subscription: the subscription is just the view model holding a reference to itself.</summary>
    public static void NoNeedToDisposeSelfObservation()
    {
        Student student = new("Grace", "Robotics Club");
        List<int> grades = [];

        // No DisposeWith: student's own PropertyChanged event holds this subscription, and dies with student itself.
        student.WhenAnyValue(x => x.Grade).Subscribe(grades.Add);

        student.Grade = 88;
        student.Grade = 95;

        Console.WriteLine(string.Join(", ", grades));

        // Output:
        // 0, 88, 95
    }
}
