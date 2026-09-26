// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;
using UIKit;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests what the UIKit platform module registers and the main-thread sequencer it installs.</summary>
/// <remarks><see cref="AppleAssemblyHooks.StartReactiveUI"/> builds the app once, before these tests run.</remarks>
public class AppleRegistrationTests
{
    /// <summary>The delay the timed test schedules.</summary>
    private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(300);

    /// <summary>How much earlier than the delay the run loop may run the work, to allow for clock granularity.</summary>
    private static readonly TimeSpan ClockTolerance = TimeSpan.FromMilliseconds(20);

    /// <summary>The main-thread scheduler is the main run loop sequencer.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task MainThreadScheduler_IsTheMainRunloopSequencer() =>
        await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(NSRunloopSequencer.Main);

    /// <summary>A value emitted on the main run loop sequencer from a pool thread arrives on the main thread.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task NSRunloopSequencer_Emit_DeliversOnTheMainThread()
    {
        await Assert.That(MainThread.IsCurrent).IsFalse();

        var onMain = Signal.Emit(0, NSRunloopSequencer.Main).Select(static _ => MainThread.IsCurrent).FirstValueAsync(out var subscription);
        using (subscription)
        {
            await Assert.That(await onMain.WithTimeout("the emitted value")).IsTrue();
        }
    }

    /// <summary>Observing on the main run loop sequencer moves a pool-thread value onto the main thread.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task NSRunloopSequencer_ObserveOn_MovesDeliveryToTheMainThread()
    {
        using var source = new Signal<int>();
        var delivered = source.ObserveOn(NSRunloopSequencer.Main).Select(static _ => MainThread.IsCurrent).FirstValueAsync(out var subscription);
        using (subscription)
        {
            source.OnNext(1);

            await Assert.That(await delivered.WithTimeout("the observed value")).IsTrue();
        }
    }

    /// <summary>Delayed work on the main run loop waits for its due time and then runs on the main thread.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task NSRunloopSequencer_After_WaitsForTheDueTimeOnTheMainThread()
    {
        var started = System.Diagnostics.Stopwatch.GetTimestamp();

        var fired = Signal.After(Delay, NSRunloopSequencer.Main)
            .Select(_ => (OnMain: MainThread.IsCurrent, Elapsed: System.Diagnostics.Stopwatch.GetElapsedTime(started)))
            .FirstValueAsync(out var subscription);
        using (subscription)
        {
            var result = await fired.WithTimeout("the delayed value");

            await Assert.That(result.OnMain).IsTrue();
            await Assert.That(result.Elapsed).IsGreaterThanOrEqualTo(Delay - ClockTolerance);
        }
    }

    /// <summary>The platform operations service is the Apple implementation and reports a device orientation name.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task PlatformOperations_ReportsTheDeviceOrientation()
    {
        var operations = Locator.Current.GetService<IPlatformOperations>();

        await Assert.That(operations).IsTypeOf<PlatformOperations>();
        await Assert.That(Enum.GetNames<UIDeviceOrientation>()).Contains(operations!.GetOrientation()!);
    }

    /// <summary>The suspension driver is the Application Support JSON driver.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task SuspensionDriver_IsTheAppSupportDriver() =>
        await Assert.That(Locator.Current.GetService<ISuspensionDriver>()).IsTypeOf<AppSupportJsonSuspensionDriver>();
}
