// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using Android.OS;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests <see cref="HandlerSequencer"/>, the sequencer ReactiveUI uses as the Android main-thread scheduler.</summary>
/// <remarks>Test bodies run on a pool thread, so work that lands on the main thread was really moved there.</remarks>
public class HandlerSequencerTests
{
    /// <summary>The delay the timed test schedules.</summary>
    private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(300);

    /// <summary>How much earlier than the delay the looper may run the work, to allow for clock granularity.</summary>
    private static readonly TimeSpan ClockTolerance = TimeSpan.FromMilliseconds(20);

    /// <summary>A value emitted on the main sequencer arrives on the main thread.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task Emit_OnMain_DeliversOnTheMainThread()
    {
        await Assert.That(MainThread.IsCurrent).IsFalse();

        var onMain = Signal.Emit(0, HandlerSequencer.Main).Select(static _ => MainThread.IsCurrent).FirstValueAsync(out var subscription);
        using (subscription)
        {
            await Assert.That(await onMain.WithTimeout("the emitted value")).IsTrue();
        }
    }

    /// <summary>Observing on the main sequencer moves a pool-thread value onto the main thread.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task ObserveOn_Main_MovesDeliveryToTheMainThread()
    {
        using var source = new Signal<int>();
        var delivered = source.ObserveOn(HandlerSequencer.Main).Select(static _ => MainThread.IsCurrent).FirstValueAsync(out var subscription);
        using (subscription)
        {
            source.OnNext(1);

            await Assert.That(await delivered.WithTimeout("the observed value")).IsTrue();
        }
    }

    /// <summary>Delayed work on the main sequencer waits for its due time and then runs on the main thread.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task After_OnMain_WaitsForTheDueTimeOnTheMainThread()
    {
        var started = Stopwatch.GetTimestamp();

        var fired = Signal.After(Delay, HandlerSequencer.Main)
            .Select(_ => (OnMain: MainThread.IsCurrent, Elapsed: Stopwatch.GetElapsedTime(started)))
            .FirstValueAsync(out var subscription);
        using (subscription)
        {
            var result = await fired.WithTimeout("the delayed value");

            await Assert.That(result.OnMain).IsTrue();
            await Assert.That(result.Elapsed).IsGreaterThanOrEqualTo(Delay - ClockTolerance);
        }
    }

    /// <summary>A sequencer over a background handler thread runs its work on that thread.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task CustomHandler_RunsWorkOnItsLooperThread()
    {
        using var thread = new HandlerThread("rxui-device-tests");
        thread.Start();
        try
        {
            var looper = thread.Looper!;
            var sequencer = new HandlerSequencer(new(looper));

            var onThread = Signal.Emit(0, sequencer)
                .Select(_ => looper.IsCurrentThread)
                .FirstValueAsync(out var subscription);
            using (subscription)
            {
                await Assert.That(await onThread.WithTimeout("the handler-thread value")).IsTrue();
                await Assert.That(sequencer.Handler.Looper).IsSameReferenceAs(looper);
            }
        }
        finally
        {
            _ = thread.QuitSafely();
        }
    }
}
