// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Testing;

namespace ReactiveUI.Documentation.Testing;

/// <summary>Shows how a test swaps in its own scheduler for the main-thread and task-pool schedulers.</summary>
public static class SchedulerExtensionsExamples
{
    /// <summary><c>WithScheduler</c> installs a scheduler until the returned token is disposed, then restores the previous ones.</summary>
    public static void SwapSchedulersForABlock()
    {
        ISequencer originalMainThread = RxSchedulers.MainThreadScheduler;

        using (Sequencer.CurrentThread.WithScheduler())
        {
            Console.WriteLine(ReferenceEquals(RxSchedulers.MainThreadScheduler, Sequencer.CurrentThread));
            Console.WriteLine(ReferenceEquals(RxSchedulers.TaskpoolScheduler, Sequencer.CurrentThread));
        }

        Console.WriteLine(ReferenceEquals(RxSchedulers.MainThreadScheduler, originalMainThread));

        // Output:
        // True
        // True
        // True
    }

    /// <summary>
    /// The <c>Func&lt;T, TRet&gt;</c> overload of <c>With</c> runs a function under the test scheduler and returns its
    /// result once the previous schedulers are restored.
    /// </summary>
    public static void ComputeAverageUnderATestScheduler()
    {
        double average = Sequencer.CurrentThread.With(static scheduler =>
        {
            using GradeCalculatorViewModel viewModel = new StudentBuilder()
                .WithName("Ada Lovelace")
                .WithGrades([88, 92, 79])
                .Build();

            return viewModel.Average;
        });

        Console.WriteLine(average);

        // Output:
        // 86.33333333333333
    }

    /// <summary>The <c>Action&lt;T&gt;</c> overload of <c>With</c> runs a side effect under the test scheduler.</summary>
    public static void RecordAGradeUnderATestScheduler()
    {
        List<GradeRecorded> recorded = [];

        using GradeCalculatorViewModel viewModel = new StudentBuilder().WithName("Grace Hopper").Build();
        using IDisposable subscription = viewModel.RecordGrade.Subscribe(recorded.Add);

        Sequencer.CurrentThread.With(_ =>
        {
            using IDisposable execution = viewModel.RecordGrade.Execute(97).Subscribe();
        });

        Console.WriteLine(recorded.Count);
        Console.WriteLine(recorded[0].Grade);

        // Output:
        // 1
        // 97
    }

    /// <summary>The <c>Func&lt;T, Task&lt;TRet&gt;&gt;</c> overload of <c>WithAsync</c> runs an asynchronous function under the test scheduler.</summary>
    /// <returns>A task that completes once the average has been computed.</returns>
    public static async Task ComputeAverageAsyncUnderATestScheduler()
    {
        double average = await Sequencer.CurrentThread.WithAsync(static async scheduler =>
        {
            await Task.Yield();

            using GradeCalculatorViewModel viewModel = new StudentBuilder()
                .WithName("Katherine Johnson")
                .WithGrades([95, 89])
                .Build();

            return viewModel.Average;
        });

        Console.WriteLine(average);

        // Output:
        // 92
    }

    /// <summary>The <c>Func&lt;T, Task&gt;</c> overload of <c>WithAsync</c> runs an asynchronous side effect under the test scheduler.</summary>
    /// <returns>A task that completes once the grade has been recorded.</returns>
    public static async Task RecordAGradeAsyncUnderATestScheduler()
    {
        List<GradeRecorded> recorded = [];

        using GradeCalculatorViewModel viewModel = new StudentBuilder().WithName("Rosalind Franklin").Build();
        using IDisposable subscription = viewModel.RecordGrade.Subscribe(recorded.Add);

        await Sequencer.CurrentThread.WithAsync(async scheduler =>
        {
            await Task.Yield();
            await viewModel.RecordGrade.Execute(84);
        });

        Console.WriteLine(recorded.Count);
        Console.WriteLine(recorded[0].StudentName);

        // Output:
        // 1
        // Rosalind Franklin
    }
}
