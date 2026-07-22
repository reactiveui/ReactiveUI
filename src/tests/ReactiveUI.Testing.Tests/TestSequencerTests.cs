// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading;

namespace ReactiveUI.Testing.Tests;

/// <summary>A series of tests associated with the test sequencer.</summary>
public class TestSequencerTests
{
    /// <summary>The initial phase count before any phase has advanced.</summary>
    private const int InitialPhase = 0;

    /// <summary>The phase count after the first phase has advanced.</summary>
    private const int FirstPhase = 1;

    /// <summary>The phase count after the second phase has advanced.</summary>
    private const int SecondPhase = 2;

    /// <summary>The delay, in milliseconds, before the handler starts.</summary>
    private const int HandlerStartDelayMilliseconds = 10;

    /// <summary>Shoulds the execute tests in order.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Should_Execute_Tests_In_Order()
    {
        using var testSequencer = new TestSequencer();
        using var subject = new StateSignal<RxVoid>(RxVoid.Default);

        // Track async operations to ensure proper coordination
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var advanceCount = 0;

        _ = subject.Skip(1).Subscribe(Witness.Create<RxVoid>(ignored => _ = AdvancePhaseAsync()));

        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(InitialPhase);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(InitialPhase);
        }

        // Trigger first advance from subscription
        tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        subject.OnNext(RxVoid.Default);

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
        tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
        subject.OnNext(RxVoid.Default);

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

        async Task AdvancePhaseAsync()
        {
            try
            {
                await testSequencer.AdvancePhaseAsync();
                _ = Interlocked.Increment(ref advanceCount);
                _ = tcs.TrySetResult(true);
            }
            catch (Exception ex)
            {
                _ = tcs.TrySetException(ex);
            }
        }
    }
}
