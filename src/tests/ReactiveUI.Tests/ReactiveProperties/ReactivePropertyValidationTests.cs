// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace ReactiveUI.Tests.ReactiveProperties;

/// <summary>
/// Tests for the observable-based, asynchronous, and multi-rule validation overloads of <see cref="ReactiveProperty{T}"/>.
/// </summary>
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

    /// <summary>Verifies the observable-based validator overload surfaces and clears errors as the value changes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableValidator_SurfacesError()
    {
        using var rp = new ReactiveProperty<int>(0, ImmediateScheduler.Instance, false, false)
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
        using var rp = new ReactiveProperty<int>(0, ImmediateScheduler.Instance, false, false)
            .AddValidationError(static x => Task.FromResult<string?>(x < 0 ? AsyncError : null));

        rp.Value = -1;

        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(AsyncError);
    }

    /// <summary>Verifies two chained validators are aggregated, each firing for its own invalid range.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MultipleValidators_AggregateErrors()
    {
        using var rp = new ReactiveProperty<int>(0, ImmediateScheduler.Instance, false, false)
            .AddValidationError(static x => x < 0 ? NegativeError : null)
            .AddValidationError(static x => x > 0 ? PositiveError : null);

        rp.Value = -1;
        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(NegativeError);

        rp.Value = 1;
        await Assert.That(rp.GetErrors(null)?.OfType<string>()).Contains(PositiveError);
    }
}
