// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
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

        subject.Subscribe(async _ =>
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
        });

        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(0);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(0);
        }

        // Trigger first advance from subscription
        tcs = new TaskCompletionSource<bool>();
        subject.OnNext(Unit.Default);

        // Wait briefly for async handler to start
        await Task.Delay(10);

        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(1);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(0);
        }

        // Complete Phase 1 from main thread
        await testSequencer.AdvancePhaseAsync("Phase 1");
        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(1);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(1);
        }

        // Trigger second advance from subscription
        tcs = new TaskCompletionSource<bool>();
        subject.OnNext(Unit.Default);

        // Wait briefly for async handler to start
        await Task.Delay(10);

        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(2);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(1);
        }

        // Complete Phase 2 from main thread
        await testSequencer.AdvancePhaseAsync("Phase 2");
        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(2);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(2);
        }
    }
}
