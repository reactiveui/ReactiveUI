// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace ReactiveUI.Tests.ObservableForProperty;

/// <summary>
/// Exercises the full matrix of <see cref="OAPHCreationHelperMixin"/> <c>ToProperty</c> overloads (expression vs string
/// property, with and without an initial value, deferral flag, and scheduler) to cover the delegating overload bodies.
/// </summary>
public class OAPHCreationHelperOverloadsTests
{
    /// <summary>The initial value supplied to the initial-value overloads.</summary>
    private const string InitialValue = "initial";

    /// <summary>The value produced by the source observable.</summary>
    private const string SourceValue = "source";

    /// <summary>Covers the expression-based <c>ToProperty</c> overloads across the deferral/scheduler/initial-value matrix.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_ExpressionOverloads()
    {
        var helpers = new List<IDisposable>();
        IScheduler scheduler = ImmediateScheduler.Instance;
        Expression<Func<OverloadFixture, string?>> property = x => x.Text;

        helpers.Add(Source().ToProperty(NewFixture(), property));
        helpers.Add(Source().ToProperty(NewFixture(), property, true));
        helpers.Add(Source().ToProperty(NewFixture(), property, scheduler));
        helpers.Add(Source().ToProperty(NewFixture(), property, true, scheduler));
        helpers.Add(Source().ToProperty(NewFixture(), property, InitialValue));
        helpers.Add(Source().ToProperty(NewFixture(), property, InitialValue, scheduler));
        helpers.Add(Source().ToProperty(NewFixture(), property, InitialValue, true));
        helpers.Add(Source().ToProperty(NewFixture(), property, InitialValue, true, scheduler));

        foreach (var helper in helpers)
        {
            helper.Dispose();
        }

        await Assert.That(helpers).IsNotEmpty();
    }

    /// <summary>Covers the string-property <c>ToProperty</c> overloads across the deferral/scheduler/initial-value matrix.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_StringPropertyOverloads()
    {
        var helpers = new List<IDisposable>();
        IScheduler scheduler = ImmediateScheduler.Instance;
        const string name = nameof(OverloadFixture.Text);

        helpers.Add(Source().ToProperty(NewFixture(), name));
        helpers.Add(Source().ToProperty(NewFixture(), name, true));
        helpers.Add(Source().ToProperty(NewFixture(), name, scheduler));
        helpers.Add(Source().ToProperty(NewFixture(), name, true, scheduler));
        helpers.Add(Source().ToProperty(NewFixture(), name, InitialValue));
        helpers.Add(Source().ToProperty(NewFixture(), name, InitialValue, scheduler));
        helpers.Add(Source().ToProperty(NewFixture(), name, InitialValue, true));
        helpers.Add(Source().ToProperty(NewFixture(), name, InitialValue, true, scheduler));

        foreach (var helper in helpers)
        {
            helper.Dispose();
        }

        await Assert.That(helpers).IsNotEmpty();
    }

    /// <summary>Covers the <c>out</c>-parameter and <c>getInitialValue</c> <c>ToProperty</c> overloads.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_OutAndGetInitialValueOverloads()
    {
        var helpers = new List<IDisposable>();
        IScheduler scheduler = ImmediateScheduler.Instance;
        Expression<Func<OverloadFixture, string?>> property = x => x.Text;
        const string name = nameof(OverloadFixture.Text);

        helpers.Add(Source().ToProperty(NewFixture(), property, out var e1, () => InitialValue, true, scheduler));
        helpers.Add(Source().ToProperty(NewFixture(), name, out var s1, () => InitialValue));
        helpers.Add(Source().ToProperty(NewFixture(), name, out var s2, () => InitialValue, true));
        helpers.Add(Source().ToProperty(NewFixture(), name, out var s3, () => InitialValue, true, scheduler));
        helpers.Add(Source().ToProperty(NewFixture(), name, out var s4, true));

        await Assert.That(e1).IsNotNull();
        await Assert.That(s1).IsNotNull();
        await Assert.That(s2).IsNotNull();
        await Assert.That(s3).IsNotNull();
        await Assert.That(s4).IsNotNull();

        foreach (var helper in helpers)
        {
            helper.Dispose();
        }
    }

    /// <summary>Covers the expression-based <c>getInitialValue</c> and <c>out</c> overloads.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_ExpressionGetInitialValueAndOutOverloads()
    {
        var helpers = new List<IDisposable>();
        IScheduler scheduler = ImmediateScheduler.Instance;
        Expression<Func<OverloadFixture, string?>> property = x => x.Text;

        helpers.Add(Source().ToProperty(NewFixture(), property, () => InitialValue));
        helpers.Add(Source().ToProperty(NewFixture(), property, () => InitialValue, true));
        helpers.Add(Source().ToProperty(NewFixture(), property, out _, true));
        helpers.Add(Source().ToProperty(NewFixture(), property, out _, InitialValue));
        helpers.Add(Source().ToProperty(NewFixture(), property, out _, InitialValue, true));
        helpers.Add(Source().ToProperty(NewFixture(), property, out _, InitialValue, true, scheduler));
        helpers.Add(Source().ToProperty(NewFixture(), property, out _, () => InitialValue));
        helpers.Add(Source().ToProperty(NewFixture(), property, out _, () => InitialValue, true));

        await Assert.That(helpers).IsNotEmpty();

        foreach (var helper in helpers)
        {
            helper.Dispose();
        }
    }

    /// <summary>Covers the string-property <c>getInitialValue</c> overloads.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_StringGetInitialValueOverloads()
    {
        const string name = nameof(OverloadFixture.Text);
        var first = Source().ToProperty(NewFixture(), name, () => InitialValue);
        var second = Source().ToProperty(NewFixture(), name, () => InitialValue, true);

        await Assert.That(first).IsNotNull();
        await Assert.That(second).IsNotNull();

        first.Dispose();
        second.Dispose();
    }

    /// <summary>Creates a fresh source observable emitting a single value on the immediate scheduler.</summary>
    /// <returns>A source observable.</returns>
    private static IObservable<string?> Source() => Observable.Return<string?>(SourceValue).ObserveOn(ImmediateScheduler.Instance);

    /// <summary>Creates a fresh fixture to back each helper.</summary>
    /// <returns>A reactive fixture.</returns>
    private static OverloadFixture NewFixture() => new();

    /// <summary>A minimal reactive object exposing a single string property for the overload tests.</summary>
    private sealed class OverloadFixture : ReactiveObject
    {
        /// <summary>The backing field for <see cref="Text"/>.</summary>
        private string? _text;

        /// <summary>Gets or sets the text property surfaced by the helpers.</summary>
        public string? Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }
    }
}
