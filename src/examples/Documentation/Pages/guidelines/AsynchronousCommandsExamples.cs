// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>Shows why an async <c>ReactiveCommand</c> should do its own work, instead of a synchronous command that starts async work from <c>Subscribe</c>.</summary>
public static class AsynchronousCommandsExamples
{
    /// <summary>How many times <see cref="SubmitGradesAsync"/> has actually run.</summary>
    private static int _submitCount;

    /// <summary>
    /// <c>IsExecuting</c> never reports the async work, because the command's own action finishes synchronously; the
    /// work fired from <c>Subscribe</c> is invisible to it, so it already reads <see langword="false"/> while that
    /// work is still running.
    /// </summary>
    /// <returns>A task that completes once the fire-and-forget work below has finished.</returns>
    public static async Task AvoidStartingAsyncWorkFromSubscribe()
    {
        Volatile.Write(ref _submitCount, 0);

        using ReactiveCommand<RxVoid, RxVoid> submit = ReactiveCommand.Create(static () => { });
        using IDisposable subscription = submit.Subscribe(static unused => _ = SubmitGradesAsync());

        submit.Execute().Subscribe(static _ => { });
        bool executingRightAfterTheClick = await submit.IsExecuting.FirstAsync();
        await Task.Delay(50); // give the fire-and-forget work time to finish

        Console.WriteLine(executingRightAfterTheClick);
        Console.WriteLine(Volatile.Read(ref _submitCount));

        // Output:
        // False
        // 1
    }

    /// <summary>An async command reports <c>IsExecuting</c> as <see langword="true"/> for as long as its own work is running.</summary>
    /// <returns>A task that completes once the submit command has finished.</returns>
    public static async Task PreferAnAsyncCommand()
    {
        Volatile.Write(ref _submitCount, 0);

        using ReactiveCommand<RxVoid, RxVoid> submit = ReactiveCommand.CreateFromTask(SubmitGradesAsync);
        using IDisposable errorSubscription = submit.ThrownExceptions.Subscribe(static ex => Console.WriteLine($"error: {ex.Message}"));

        using IDisposable executeSubscription = submit.Execute().Subscribe(static _ => { });
        bool executingRightAfterTheClick = await submit.IsExecuting.FirstAsync();
        await Task.Delay(50);

        Console.WriteLine(executingRightAfterTheClick);
        Console.WriteLine(Volatile.Read(ref _submitCount));

        // Output:
        // True
        // 1
    }

    /// <summary>Simulates submitting the pending grades: counts each attempt, then takes a moment, the way a network call would.</summary>
    /// <returns>A task that completes once the simulated submission has finished.</returns>
    private static async Task SubmitGradesAsync()
    {
        Interlocked.Increment(ref _submitCount);
        await Task.Delay(20);
    }
}
