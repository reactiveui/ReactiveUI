// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading;

namespace ReactiveUI.Testing.Tests;

/// <summary>
/// A series of tests associated with the test sequencer.
/// </summary>
public class TestSequencerTests
{
    private const int InitialPhase = 0;
    private const int FirstPhase = 1;
    private const int SecondPhase = 2;
    private const int HandlerStartDelayMilliseconds = 10;

    /// <summary>
    /// Shoulds the execute tests in order.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Should_Execute_Tests_In_Order()
    {
        using var testSequencer = new TestSequencer();
        var subject = new Subject<Unit>();

        // Track async operations to ensure proper coordination
        var tcs = new TaskCompletionSource<bool>();
        var advanceCount = 0;

        subject.SelectMany(async _ =>
        {
            try
            {
                await testSequencer.AdvancePhaseAsync();
                Interlocked.Increment(ref advanceCount);
                tcs.TrySetResult(true);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            return Unit.Default;
        }).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(InitialPhase);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(InitialPhase);
        }

        // Trigger first advance from subscription
        tcs = new();
        subject.OnNext(Unit.Default);

        // Wait briefly for async handler to start
        await Task.Delay(HandlerStartDelayMilliseconds);

        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(FirstPhase);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(InitialPhase);
        }

        // Complete Phase 1 from main thread
        await testSequencer.AdvancePhaseAsync("Phase 1");
        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(FirstPhase);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(FirstPhase);
        }

        // Trigger second advance from subscription
        tcs = new();
        subject.OnNext(Unit.Default);

        // Wait briefly for async handler to start
        await Task.Delay(HandlerStartDelayMilliseconds);

        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(SecondPhase);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(FirstPhase);
        }

        // Complete Phase 2 from main thread
        await testSequencer.AdvancePhaseAsync("Phase 2");
        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(SecondPhase);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(SecondPhase);
        }
    }
}
