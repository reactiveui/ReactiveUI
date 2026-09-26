// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>Shows marshaling a fetched value to <c>RxSchedulers.MainThreadScheduler</c> at the boundary where it updates a bound property.</summary>
public static class UiThreadAndSchedulersExamples
{
    /// <summary>
    /// Subscribing directly leaves the update on whatever thread the fetch's continuation runs on. A console has no
    /// UI thread to protect, so this is safe here, but the same code updating a bound view model property on a UI
    /// platform can crash or corrupt the view.
    /// </summary>
    /// <returns>A task that completes once the grade has been fetched and set.</returns>
    public static async Task AvoidUpdatingWithoutMarshaling()
    {
        Student student = new("Ada", "Robotics Club");
        int grade = await FetchLatestGradeAsync();
        student.Grade = grade;

        Console.WriteLine(student.Grade);

        // Output:
        // 91
    }

    /// <summary><c>WitnessOn(RxSchedulers.MainThreadScheduler)</c> marks the boundary explicitly: everything after it is guaranteed to run where the UI thread runs.</summary>
    /// <returns>A task that completes once the grade has been fetched and set.</returns>
    public static async Task PreferWitnessOnAtTheBoundary()
    {
        Student student = new("Ada", "Robotics Club");
        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using IDisposable subscription = Signal.FromAsync(FetchLatestGradeAsync)
            .WitnessOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(grade =>
            {
                student.Grade = grade;
                completion.SetResult();
            });

        await completion.Task;
        Console.WriteLine(student.Grade);

        // Output:
        // 91
    }

    /// <summary>Passing the scheduler into the asynchronous operation itself, so the operation lands its result there instead of a separate <c>WitnessOn</c> step.</summary>
    /// <returns>A task that completes once the grade has been fetched and set.</returns>
    public static async Task PreferPassingTheSchedulerToTheOperation()
    {
        Student student = new("Ada", "Robotics Club");
        int grade = await FetchLatestGradeOnAsync(RxSchedulers.MainThreadScheduler);
        student.Grade = grade;

        Console.WriteLine(student.Grade);

        // Output:
        // 91
    }

    /// <summary>Fetches the grade, then hands the result back through the given scheduler, the way a real fetch posts its continuation there.</summary>
    /// <param name="scheduler">The scheduler the result is delivered on.</param>
    /// <returns>A task that completes with the fetched grade.</returns>
    private static async Task<int> FetchLatestGradeOnAsync(ISequencer scheduler)
    {
        int grade = await FetchLatestGradeAsync();
        TaskCompletionSource<int> delivered = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using IDisposable delivery = scheduler.Schedule((delivered, grade), static (_, state) =>
        {
            state.delivered.SetResult(state.grade);
            return EmptyDisposable.Instance;
        });

        return await delivered.Task;
    }

    /// <summary>Fetches a grade, standing in for a network or database call.</summary>
    /// <returns>A task that completes with the fetched grade.</returns>
    private static Task<int> FetchLatestGradeAsync() => Task.Run(static () => 91);
}
