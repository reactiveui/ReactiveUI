// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>Shows narrowing <c>WitnessOn(RxSchedulers.MainThreadScheduler)</c> to the one place a value reaches a bound property, and letting a command marshal its own results.</summary>
public static class ThreadingExamples
{
    /// <summary>The "shotgun" fix: adding <c>WitnessOn</c> after every step stops a real crash on a UI platform, but it schedules three times more than it needs to.</summary>
    /// <returns>A task that completes once the grade has been fetched and set.</returns>
    public static async Task AvoidWitnessOnAfterEveryStep()
    {
        Student student = new("Ada", "Robotics Club");

        int fetched = await Signal.FromAsync(FetchLatestGradeAsync)
            .WitnessOn(RxSchedulers.MainThreadScheduler)
            .Select(static grade => grade)
            .WitnessOn(RxSchedulers.MainThreadScheduler)
            .Select(static grade => grade + 0)
            .WitnessOn(RxSchedulers.MainThreadScheduler)
            .FirstAsync();

        student.Grade = fetched;
        Console.WriteLine(student.Grade);

        // Output:
        // 91
    }

    /// <summary>Surgical precision: one <c>WitnessOn</c>, at the boundary where the value reaches the bound property.</summary>
    /// <returns>A task that completes once the grade has been fetched and set.</returns>
    public static async Task PreferWitnessOnAtTheBoundary()
    {
        Student student = new("Ada", "Robotics Club");

        int fetched = await Signal.FromAsync(FetchLatestGradeAsync)
            .Select(static grade => grade)
            .Select(static grade => grade + 0)
            .WitnessOn(RxSchedulers.MainThreadScheduler)
            .FirstAsync();

        student.Grade = fetched;
        Console.WriteLine(student.Grade);

        // Output:
        // 91
    }

    /// <summary>An async command marshals its results to <c>RxSchedulers.MainThreadScheduler</c> automatically, so a <c>Subscribe</c> on its results is already safe.</summary>
    /// <returns>A task that completes once the command has run.</returns>
    public static async Task PreferLettingTheCommandMarshalItsResults()
    {
        Student student = new("Ada", "Robotics Club");
        using ReactiveCommand<RxVoid, int> loadGrade = ReactiveCommand.CreateFromTask(FetchLatestGradeAsync);
        using IDisposable subscription = loadGrade.Subscribe(grade => student.Grade = grade);

        _ = await loadGrade.Execute();
        Console.WriteLine(student.Grade);

        // Output:
        // 91
    }

    /// <summary>Fetches a grade, standing in for a network or database call.</summary>
    /// <returns>A task that completes with the fetched grade.</returns>
    private static Task<int> FetchLatestGradeAsync() => Task.Run(static () => 91);
}
