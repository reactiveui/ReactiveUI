// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Activation;

/// <summary>
/// Tests for the activation-signal plumbing the platform view types share. Those types extend unrelated platform
/// base classes, so this helper is linked into each platform assembly instead of inherited from a common base.
/// </summary>
public class ActivationSignalsTests
{
    /// <summary>An arbitrary value used to prove the additional signal still delivers.</summary>
    private const int SentinelValue = 42;

    /// <summary>Raising a signal notifies its subscribers.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Raise_NotifiesSubscribers()
    {
        using var signal = new Signal<RxVoid>();
        var raised = 0;
        using var subscription = signal.Subscribe(_ => raised++);

        ActivationSignals.Raise(signal);

        await Assert.That(raised).IsEqualTo(1);
    }

    /// <summary>A non-deterministic dispose leaves the signals usable, so finalization never closes them.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DisposeWhen_NotDisposing_LeavesTheSignalsUsable()
    {
        using var activated = new Signal<RxVoid>();
        using var deactivated = new Signal<RxVoid>();
        var raised = 0;
        using var subscription = activated.Subscribe(_ => raised++);

        ActivationSignals.DisposeWhen(false, activated, deactivated);
        activated.OnNext(RxVoid.Default);

        await Assert.That(raised).IsEqualTo(1);
    }

    /// <summary>A deterministic dispose closes both activation signals.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DisposeWhen_Disposing_ClosesBothSignals()
    {
        var activated = new Signal<RxVoid>();
        var deactivated = new Signal<RxVoid>();

        ActivationSignals.DisposeWhen(true, activated, deactivated);

        using (Assert.Multiple())
        {
            await Assert.That(() => activated.OnNext(RxVoid.Default)).Throws<ObjectDisposedException>();
            await Assert.That(() => deactivated.OnNext(RxVoid.Default)).Throws<ObjectDisposedException>();
        }
    }

    /// <summary>The overload carrying an extra signal also leaves everything usable when not disposing.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DisposeWhenWithAdditionalSignal_NotDisposing_LeavesEverythingUsable()
    {
        using var activated = new Signal<RxVoid>();
        using var deactivated = new Signal<RxVoid>();
        using var additional = new Signal<int>();
        var received = 0;
        using var subscription = additional.Subscribe(value => received = value);

        ActivationSignals.DisposeWhen(false, activated, deactivated, additional);
        additional.OnNext(SentinelValue);

        await Assert.That(received).IsEqualTo(SentinelValue);
    }

    /// <summary>The overload carrying an extra signal closes that signal too.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DisposeWhenWithAdditionalSignal_Disposing_ClosesTheAdditionalSignal()
    {
        var activated = new Signal<RxVoid>();
        var deactivated = new Signal<RxVoid>();
        var additional = new Signal<int>();

        ActivationSignals.DisposeWhen(true, activated, deactivated, additional);

        using (Assert.Multiple())
        {
            await Assert.That(() => activated.OnNext(RxVoid.Default)).Throws<ObjectDisposedException>();
            await Assert.That(() => deactivated.OnNext(RxVoid.Default)).Throws<ObjectDisposedException>();
            await Assert.That(() => additional.OnNext(1)).Throws<ObjectDisposedException>();
        }
    }

    /// <summary>Forcing activation raises the activated signal, and only that one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ScheduleActivation_WhenActivating_RaisesOnlyTheActivatedSignal()
    {
        var (activations, deactivations) = await ScheduleAndCount(true);

        using (Assert.Multiple())
        {
            await Assert.That(activations).IsEqualTo(1);
            await Assert.That(deactivations).IsEqualTo(0);
        }
    }

    /// <summary>Forcing deactivation raises the deactivated signal, and only that one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ScheduleActivation_WhenDeactivating_RaisesOnlyTheDeactivatedSignal()
    {
        var (activations, deactivations) = await ScheduleAndCount(false);

        using (Assert.Multiple())
        {
            await Assert.That(activations).IsEqualTo(0);
            await Assert.That(deactivations).IsEqualTo(1);
        }
    }

    /// <summary>
    /// Runs <c>ScheduleActivation</c> against an immediate main-thread scheduler so the notification is delivered
    /// synchronously, and reports how many times each signal fired.
    /// </summary>
    /// <param name="isActivating">Whether to force activation rather than deactivation.</param>
    /// <returns>The activation and deactivation counts.</returns>
    private static async Task<(int Activations, int Deactivations)> ScheduleAndCount(bool isActivating)
    {
        var original = RxSchedulers.MainThreadScheduler;
        try
        {
            RxSchedulers.MainThreadScheduler = Sequencer.Immediate;

            using var activated = new Signal<RxVoid>();
            using var deactivated = new Signal<RxVoid>();
            var activations = 0;
            var deactivations = 0;
            using var activatedSubscription = activated.Subscribe(_ => activations++);
            using var deactivatedSubscription = deactivated.Subscribe(_ => deactivations++);

            ActivationSignals.ScheduleActivation(isActivating, activated, deactivated);

            await Task.Yield();
            return (activations, deactivations);
        }
        finally
        {
            RxSchedulers.MainThreadScheduler = original;
        }
    }
}
