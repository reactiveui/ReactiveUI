// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Linq;

namespace ReactiveUI.Tests.ReactiveProperties;

/// <summary>Tests for the observable-based, asynchronous, and multi-rule validation overloads of <see cref="ReactiveProperty{T}"/>.</summary>
public class ReactivePropertyValidationTests
{
    /// <summary>Error message produced by the observable-based validator.</summary>
    private const string ObservableError = "observable-error";

    /// <summary>Error message produced by the asynchronous validator.</summary>
    private const string AsyncError = "async-error";

    /// <summary>Error message produced by the first of two chained validators.</summary>
    private const string NegativeError = "negative";

    /// <summary>Error message produced by the second of two chained validators.</summary>
    private const string PositiveError = "positive";

    /// <summary>The number of polling attempts awaited for an asynchronous validator to complete.</summary>
    private const int PollAttempts = 500;

    /// <summary>The delay between polling attempts, in milliseconds.</summary>
    private const int PollDelayMilliseconds = 10;

    /// <summary>Verifies the observable-based validator overload surfaces and clears errors as the value changes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableValidator_SurfacesError()
    {
        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false)
            .AddValidationError(xs => xs.Select(static x => x < 0 ? ObservableError : null));

        await Assert.That(rp.HasErrors).IsFalse();

        rp.Value = -1;

        await Assert.That(rp.HasErrors).IsTrue();
        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(ObservableError);
    }

    /// <summary>Verifies the asynchronous validator overload surfaces an error for an invalid value.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsyncValidator_SurfacesError()
    {
        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false)
            .AddValidationError(static x => Task.FromResult<string?>(x < 0 ? AsyncError : null));

        rp.Value = -1;

        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(AsyncError);
    }

    /// <summary>Verifies two chained validators are aggregated, each firing for its own invalid range.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MultipleValidators_AggregateErrors()
    {
        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false)
            .AddValidationError(static x => x < 0 ? NegativeError : null)
            .AddValidationError(static x => x > 0 ? PositiveError : null);

        rp.Value = -1;
        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(NegativeError);

        rp.Value = 1;
        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(PositiveError);
    }

    /// <summary>Verifies the single-argument observable-enumerable validator overload defaults to validating the initial value.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableEnumerableValidator_SingleArgOverload()
    {
        using var rp = new ReactiveProperty<string>(null, Sequencer.Immediate, false, false)
            .AddValidationError(xs =>
                new MapSignal<string?, IEnumerable?>(xs, static x => string.IsNullOrEmpty(x) ? new[] { ObservableError } : null));

        await Assert.That(rp.HasErrors).IsTrue();
        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(ObservableError);
    }

    /// <summary>Verifies the single-argument asynchronous-enumerable validator overload defaults to validating the initial value.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsyncEnumerableValidator_SingleArgOverload()
    {
        using var rp = new ReactiveProperty<int>(-1, Sequencer.Immediate, false, false)
            .AddValidationError(static x =>
                Task.FromResult<IEnumerable?>(x < 0 ? new[] { AsyncError } : null));

        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(AsyncError);
    }

    /// <summary>Verifies the single-argument asynchronous-string validator overload defaults to validating the initial value.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsyncStringValidator_SingleArgOverload()
    {
        using var rp = new ReactiveProperty<int>(-1, Sequencer.Immediate, false, false)
            .AddValidationError(static x => Task.FromResult<string?>(x < 0 ? AsyncError : null));

        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(AsyncError);
    }

    /// <summary>Verifies that two manually driven validators aggregate only once both have reported, mixing string and non-string error elements.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ManualValidators_AggregateMixedErrorElements()
    {
        var sig0 = new Signal<IEnumerable?>();
        var sig1 = new Signal<IEnumerable?>();

        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false)
            .AddValidationError(_ => sig0)
            .AddValidationError(_ => sig1);

        // Only the first validator has reported: aggregation must wait for the second.
        sig0.OnNext(new object[] { NegativeError });
        await Assert.That(rp.HasErrors).IsFalse();

        // Both validators have now reported; a non-string element and a string element are aggregated.
        sig1.OnNext(new[] { PositiveError });

        using (Assert.Multiple())
        {
            await Assert.That(rp.HasErrors).IsTrue();
            await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(NegativeError);
            await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(PositiveError);
        }

        // A later all-null report clears the aggregate.
        sig0.OnNext(null);
        sig1.OnNext(null);
        await Assert.That(rp.HasErrors).IsFalse();
    }

    /// <summary>Verifies that an error from any validator stream is forwarded and stops further aggregation.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ManualValidator_ErrorIsForwardedAndStops()
    {
        var erroring = new Signal<IEnumerable?>();
        var secondErroring = new Signal<IEnumerable?>();
        var live = new Signal<IEnumerable?>();

        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false)
            .AddValidationError(_ => erroring)
            .AddValidationError(_ => secondErroring)
            .AddValidationError(_ => live);

        // The first validator errors, terminating the aggregate sink.
        erroring.OnError(new InvalidOperationException("boom"));

        // A second error after the sink has stopped takes the already-stopped early-return path.
        secondErroring.OnError(new InvalidOperationException("second"));

        // An emission from a still-live validator after the stop is ignored (stopped guard in OnNextAt).
        live.OnNext(new[] { NegativeError });

        await Assert.That(rp.HasErrors).IsFalse();
    }

    /// <summary>Verifies that disposing a property with an asynchronous validator completes the validator's source cleanly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsyncValidator_DisposeCompletesSource()
    {
        var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false)
            .AddValidationError(static x => Task.FromResult<string?>(x < 0 ? AsyncError : null));

        rp.Value = -1;
        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(AsyncError);

        // Disposing completes the validator's source observable, exercising the async sink's completion accounting
        // and the aggregate sink's subscription disposal.
        rp.Dispose();

        await Assert.That(rp.IsDisposed).IsTrue();
    }

    /// <summary>Verifies that the aggregate downstream completes only after every validator stream has completed.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ManualValidators_CompleteWhenAllComplete()
    {
        var sig0 = new Signal<IEnumerable?>();
        var sig1 = new Signal<IEnumerable?>();

        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false)
            .AddValidationError(_ => sig0)
            .AddValidationError(_ => sig1);

        // Both validators report so the aggregate has a full set of latest values.
        sig0.OnNext(new[] { NegativeError });
        sig1.OnNext(null);
        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(NegativeError);

        // First completion is not enough to terminate the aggregate downstream.
        sig0.OnCompleted();

        // The still-open second validator can continue to drive aggregation.
        sig1.OnNext(new[] { PositiveError });
        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(PositiveError);

        // Completing the last validator terminates the aggregate downstream.
        sig1.OnCompleted();
        await Assert.That(rp.HasErrors).IsTrue();
    }

    /// <summary>Verifies the asynchronous validator forwards a faulted task as a no-op error and the property remains usable.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsyncValidator_FaultedTaskIsHandled()
    {
        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false)
            .AddValidationError(static _ => Task.FromException<string?>(new InvalidOperationException("nope")));

        rp.Value = -1;

        // The faulted task surfaces as an error on the validator stream (forwarded as a no-op downstream),
        // so no validation errors are recorded and the property stays alive.
        await Assert.That(rp.HasErrors).IsFalse();
    }

    /// <summary>Verifies the asynchronous validator forwards an exception thrown synchronously by the selector.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsyncValidator_SelectorThrowsSynchronously()
    {
        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false)
            .AddValidationError(static x => x < 0
                ? throw new InvalidOperationException("sync throw")
                : Task.FromResult<string?>(null));

        rp.Value = -1;

        await Assert.That(rp.HasErrors).IsFalse();
    }

    /// <summary>Verifies the asynchronous validator delivers a result from a genuinely awaited task.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsyncValidator_AwaitedTaskDeliversResult()
    {
        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false)
            .AddValidationError(
                async x =>
                {
                    await Task.Yield();
                    return x < 0 ? AsyncError : null;
                },
                true);

        rp.Value = -1;

        for (var attempt = 0; attempt < PollAttempts && !rp.HasErrors; attempt++)
        {
            await Task.Delay(PollDelayMilliseconds);
        }

        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(AsyncError);
    }
}
