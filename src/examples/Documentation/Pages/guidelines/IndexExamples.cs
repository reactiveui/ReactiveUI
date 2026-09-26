// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>
/// Shows a few of the general guidelines from the top of the guidelines page: initializing with <c>RxAppBuilder</c>,
/// reading the modern <c>RxSchedulers</c> and <c>RxState</c> in place of the removed <c>RxApp</c>, handling a
/// command's errors, and awaiting instead of blocking.
/// </summary>
public static class IndexExamples
{
    /// <summary>
    /// <c>RxAppBuilder</c> configures dependency injection, schedulers and platform services through one fluent chain.
    /// Building a second, isolated instance like this exposes what was configured without touching the global
    /// schedulers <see cref="ExampleApp.Start()"/> already set up for this process.
    /// </summary>
    public static void PreferRxAppBuilderForInitialization()
    {
        ReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder();
        builder.WithMainThreadScheduler(Sequencer.Immediate, setRxApp: false);
        builder.WithTaskPoolScheduler(TaskPoolSequencer.Default, setRxApp: false);
        builder.WithCoreServices();
        IReactiveUIInstance instance = builder.BuildApp();

        Console.WriteLine(instance.MainThreadScheduler?.GetType().Name);

        // Output:
        // ImmediateSequencer
    }

    /// <summary>The static <c>RxApp</c> class is gone; read both schedulers, and the process-wide exception handler, through <see cref="RxSchedulers"/> and <see cref="RxState"/> instead.</summary>
    public static void PreferRxSchedulersOverTheRemovedRxApp()
    {
        Console.WriteLine(RxSchedulers.MainThreadScheduler.GetType().Name);
        Console.WriteLine(RxSchedulers.TaskpoolScheduler.GetType().Name);
        Console.WriteLine(RxState.DefaultExceptionHandler is not null);

        // Output:
        // ImmediateSequencer
        // TaskPoolSequencer
        // True
    }

    /// <summary>A command's <c>ThrownExceptions</c> stream is the place to turn a failure into a message for the screen, never the async body itself.</summary>
    /// <returns>A task that completes once the failing command has finished.</returns>
    public static async Task PreferHandlingThrownExceptions()
    {
        using ReactiveCommand<RxVoid, RxVoid> save = ReactiveCommand.CreateFromTask(
            static () => throw new InvalidOperationException("No connection"));
        string errorMessage = string.Empty;
        using IDisposable subscription = save.ThrownExceptions.Subscribe(_ => errorMessage = "Unable to save. Please try again.");

        try
        {
            await save.Execute();
        }
        catch (InvalidOperationException)
        {
            // The awaiting caller also sees the failure; ThrownExceptions is for a subscriber that only watches, such as the screen.
        }

        Console.WriteLine(errorMessage);

        // Output:
        // Unable to save. Please try again.
    }

    /// <summary>Awaiting a task keeps the calling thread free while the work runs, instead of blocking it on <c>Result</c> or <c>Wait()</c>.</summary>
    /// <returns>A task that completes once the grade has been fetched.</returns>
    public static async Task PreferAwaitingAsyncWork()
    {
        int grade = await FetchGradeAsync();
        Console.WriteLine(grade);

        // Output:
        // 91
    }

    /// <summary>Fetches a grade, standing in for a network or database call.</summary>
    /// <returns>A task that completes with the fetched grade.</returns>
    private static Task<int> FetchGradeAsync() => Task.FromResult(91);
}
