// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>
/// Shows the naming convention command-backed properties follow: a verb-based name with a <c>Command</c> suffix, and
/// an <c>Impl</c>-suffixed method behind the command.
/// </summary>
public static class CommandNamesExamples
{
    /// <summary>
    /// Every command property carries the <c>Command</c> suffix, so a reader can tell a command from a plain method
    /// at a glance; its implementation is a same-named method with the <c>Impl</c> suffix.
    /// </summary>
    /// <returns>A task that completes once the commands below have run.</returns>
    public static async Task PreferTheCommandSuffix()
    {
        Enrollment enrollment = new("Robotics Club") { IsEnabled = true };
        IObservable<bool> canSave = enrollment.WhenAnyValue(x => x.IsEnabled);

        using ReactiveCommand<RxVoid, RxVoid> saveCommand = ReactiveCommand.CreateFromTask(SaveImplAsync, canSave);
        using ReactiveCommand<string, RxVoid> searchCommand = ReactiveCommand.CreateFromTask<string, RxVoid>(SearchImplAsync);

        _ = await saveCommand.Execute();
        _ = await searchCommand.Execute("Robotics");

        enrollment.IsEnabled = false;
        Console.WriteLine(await saveCommand.CanExecute.FirstAsync());

        // Output:
        // Saved
        // Searched for Robotics
        // False
    }

    /// <summary>Saves the roster; the same-named command calls this through the <c>Impl</c> naming convention.</summary>
    /// <returns>A task that completes once the save has finished.</returns>
    private static async Task SaveImplAsync()
    {
        await Task.Delay(1);
        Console.WriteLine("Saved");
    }

    /// <summary>Searches the course catalogue; the same-named command calls this through the <c>Impl</c> naming convention.</summary>
    /// <param name="term">The text to search for.</param>
    /// <returns>A task that completes with <see cref="RxVoid.Default"/> once the search has finished.</returns>
    private static async Task<RxVoid> SearchImplAsync(string term)
    {
        await Task.Delay(1);
        Console.WriteLine($"Searched for {term}");
        return RxVoid.Default;
    }
}
