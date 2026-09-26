// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveProperty;

/// <summary>Shows the three- and four-argument constructors, and what their <c>skipCurrentValueOnSubscribe</c> and <c>allowDuplicateValues</c> flags do.</summary>
public static class SubscriptionBehaviorExamples
{
    /// <summary>The three-argument constructor takes both flags without a scheduler, using the default task-pool scheduler.</summary>
    /// <returns>A task that completes once the property has delivered its first value.</returns>
    public static async Task ConstructWithoutAnExplicitScheduler()
    {
        ReactiveProperty<string> club = new("Robotics Club", skipCurrentValueOnSubscribe: false, allowDuplicateValues: false);

        Console.WriteLine(await club.FirstAsync());

        club.Dispose();

        // Output:
        // Robotics Club
    }

    /// <summary><c>skipCurrentValueOnSubscribe</c> controls whether a new subscriber immediately receives the current value.</summary>
    public static void SkipCurrentValueOnSubscribe()
    {
        ReactiveProperty<string> repliesImmediately = new("Robotics Club", RxSchedulers.MainThreadScheduler, false, false);
        ReactiveProperty<string> waitsForAChange = new("Robotics Club", RxSchedulers.MainThreadScheduler, true, false);

        List<string> immediateReceived = [];
        List<string> waitingReceived = [];
        IDisposable immediateSubscription = repliesImmediately.Subscribe(value => immediateReceived.Add(value!));
        IDisposable waitingSubscription = waitsForAChange.Subscribe(value => waitingReceived.Add(value!));

        waitsForAChange.Value = "Chess Club";

        Console.WriteLine(string.Join(", ", immediateReceived));
        Console.WriteLine(string.Join(", ", waitingReceived));

        immediateSubscription.Dispose();
        waitingSubscription.Dispose();
        repliesImmediately.Dispose();
        waitsForAChange.Dispose();

        // Output:
        // Robotics Club
        // Chess Club
    }

    /// <summary><c>allowDuplicateValues</c> controls whether setting the same value again reaches subscribers.</summary>
    public static void AllowDuplicateValues()
    {
        ReactiveProperty<string> suppressesDuplicates = new("Chess Club", RxSchedulers.MainThreadScheduler, false, false);
        ReactiveProperty<string> allowsDuplicates = new("Chess Club", RxSchedulers.MainThreadScheduler, false, true);

        List<string> suppressed = [];
        List<string> allowed = [];
        IDisposable suppressedSubscription = suppressesDuplicates.Subscribe(value => suppressed.Add(value!));
        IDisposable allowedSubscription = allowsDuplicates.Subscribe(value => allowed.Add(value!));

        suppressesDuplicates.Value = "Chess Club";
        suppressesDuplicates.Value = "Robotics Club";
        allowsDuplicates.Value = "Chess Club";
        allowsDuplicates.Value = "Robotics Club";

        Console.WriteLine(string.Join(", ", suppressed));
        Console.WriteLine(string.Join(", ", allowed));

        suppressedSubscription.Dispose();
        allowedSubscription.Dispose();
        suppressesDuplicates.Dispose();
        allowsDuplicates.Dispose();

        // Output:
        // Chess Club, Robotics Club
        // Chess Club, Chess Club, Robotics Club
    }
}
