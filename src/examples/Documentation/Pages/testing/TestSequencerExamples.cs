// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Testing;

namespace ReactiveUI.Documentation.Testing;

/// <summary>
/// Shows how <see cref="TestSequencer"/> lets a test and a background worker rendezvous phase by phase, so the test
/// can inspect state while the worker is paused mid-way through its work.
/// </summary>
public static class TestSequencerExamples
{
    /// <summary>The time a phase-change subscriber needs to reach its checkpoint before the test looks at the sequencer.</summary>
    private static readonly TimeSpan CheckpointDelay = TimeSpan.FromMilliseconds(50);

    /// <summary>
    /// <c>AdvancePhaseAsync</c> blocks until both the worker and the test have called it; <c>CurrentPhase</c> moves
    /// ahead of <c>CompletedPhases</c> while one participant is still waiting for the other.
    /// </summary>
    /// <returns>A task that completes once every grade has been recorded.</returns>
    public static async Task StepAGradeRecorderPhaseByPhase()
    {
        using TestSequencer sequencer = new();
        List<int> grades = [];

        Task recorder = Task.Run(async () =>
        {
            grades.Add(88);
            await sequencer.AdvancePhaseAsync("first grade recorded");

            grades.Add(92);
            await sequencer.AdvancePhaseAsync("second grade recorded");
        });

        await Task.Delay(CheckpointDelay);
        Console.WriteLine(sequencer.CurrentPhase);
        Console.WriteLine(sequencer.CompletedPhases);
        Console.WriteLine(grades.Count);

        await sequencer.AdvancePhaseAsync();

        await Task.Delay(CheckpointDelay);
        Console.WriteLine(sequencer.CurrentPhase);
        Console.WriteLine(sequencer.CompletedPhases);
        Console.WriteLine(grades.Count);

        await sequencer.AdvancePhaseAsync();
        await recorder;

        Console.WriteLine(sequencer.CompletedPhases);
        Console.WriteLine(grades.Count);

        // Output:
        // 1
        // 0
        // 1
        // 2
        // 1
        // 2
        // 2
        // 2
    }
}
