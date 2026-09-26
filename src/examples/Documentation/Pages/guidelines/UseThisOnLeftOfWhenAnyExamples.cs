// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>Shows putting <c>this</c> on the left of a <c>WhenAny</c> call instead of a dependency whose lifetime the view model does not control.</summary>
public static class UseThisOnLeftOfWhenAnyExamples
{
    /// <summary>Reading a dependency's property through <c>this.WhenAny</c> ties the pipeline to the view model, not to how long the dependency lives.</summary>
    public static void PreferThisOnTheLeft()
    {
        Timetable timetable = new() { NextClass = "Robotics, 9am" };
        using PreferLessonReminderViewModel viewModel = new(timetable);

        Console.WriteLine(viewModel.NextClass);

        timetable.NextClass = "Chess, 11am";
        Console.WriteLine(viewModel.NextClass);

        // Output:
        // Robotics, 9am
        // Chess, 11am
    }

    /// <summary>Reading the dependency directly behaves the same here, but a long-lived <see cref="Timetable"/> singleton would keep this view model reachable for as long as it lives.</summary>
    public static void AvoidTheDependencyOnTheLeft()
    {
        Timetable timetable = new() { NextClass = "Robotics, 9am" };
        using AvoidLessonReminderViewModel viewModel = new(timetable);

        Console.WriteLine(viewModel.NextClass);

        timetable.NextClass = "Chess, 11am";
        Console.WriteLine(viewModel.NextClass);

        // Output:
        // Robotics, 9am
        // Chess, 11am
    }

    /// <summary>
    /// Putting <c>this</c> on the left avoids the singleton-holds-the-view-model leak, but a subscription rooted in a
    /// longer-lived dependency still needs disposing when the screen goes away.
    /// </summary>
    public static void PreferDisposingEvenWithThisOnTheLeft()
    {
        Timetable timetable = new() { NextClass = "Robotics, 9am" };
        List<string> announcements = [];

        MultipleDisposable disposables = [];
        timetable.WhenAnyValue(x => x.NextClass)
            .Subscribe(announcements.Add)
            .DisposeWith(disposables);

        timetable.NextClass = "Chess, 11am";
        disposables.Dispose();
        timetable.NextClass = "Art, 1pm";

        Console.WriteLine(string.Join(", ", announcements));

        // Output:
        // Robotics, 9am, Chess, 11am
    }
}
