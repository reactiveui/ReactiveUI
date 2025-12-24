// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
        subject.Subscribe(async void (_) => await testSequencer.AdvancePhaseAsync());

        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(0);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(0);
        }

        subject.OnNext(Unit.Default);
        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(1);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(0);
        }

        await testSequencer.AdvancePhaseAsync("Phase 1");
        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(1);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(1);
        }

        subject.OnNext(Unit.Default);
        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(2);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(1);
        }

        await testSequencer.AdvancePhaseAsync("Phase 2");
        using (Assert.Multiple())
        {
            await Assert.That(testSequencer.CurrentPhase).IsEqualTo(2);
            await Assert.That(testSequencer.CompletedPhases).IsEqualTo(2);
        }
    }
}
