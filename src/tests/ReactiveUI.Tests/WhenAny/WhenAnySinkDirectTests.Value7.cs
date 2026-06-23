// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>
/// Direct tests for the internal <c>WhenAnyValueSink</c> and <c>WhenAnyChangeSink</c> combinators, exercising the
/// per-source emit branches, error forwarding, source-completion, and selector-exception paths that the public
/// <c>WhenAnyValue</c> API cannot reach (property-change observables never error or complete). Each arity lives in
/// its own partial-class file.
/// </summary>
public partial class WhenAnySinkDirectTests
{
    /// <summary>Verifies the arity-7 value sink emits when every source is ready and re-emits per later source update.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ValueSink7_Emits()
    {
        var s1 = new Signal<IObservedChange<object?, string>>();
        var s2 = new Signal<IObservedChange<object?, string>>();
        var s3 = new Signal<IObservedChange<object?, string>>();
        var s4 = new Signal<IObservedChange<object?, string>>();
        var s5 = new Signal<IObservedChange<object?, string>>();
        var s6 = new Signal<IObservedChange<object?, string>>();
        var s7 = new Signal<IObservedChange<object?, string>>();
        var rec = new Recorder<string>();
        using var sub = new WhenAnyValueSink<object?, string, string, string, string, string, string, string, string>(
            s1,
            s2,
            s3,
            s4,
            s5,
            s6,
            s7,
            (x1, x2, x3, x4, x5, x6, x7) => x1 + x2 + x3 + x4 + x5 + x6 + x7).Subscribe(rec);
        s1.OnNext(Ch("v1"));
        s2.OnNext(Ch("v2"));
        s3.OnNext(Ch("v3"));
        s4.OnNext(Ch("v4"));
        s5.OnNext(Ch("v5"));
        s6.OnNext(Ch("v6"));
        s7.OnNext(Ch("v7"));
        s1.OnNext(Ch("w1"));
        s2.OnNext(Ch("w2"));
        s3.OnNext(Ch("w3"));
        s4.OnNext(Ch("w4"));
        s5.OnNext(Ch("w5"));
        s6.OnNext(Ch("w6"));
        await Assert.That(rec.Values).IsNotEmpty();
    }

    /// <summary>Verifies the arity-7 value sink forwards a source error to the observer.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ValueSink7_ForwardsError()
    {
        var ex = new InvalidOperationException("boom");
        var e1 = new Signal<IObservedChange<object?, string>>();
        var e2 = new Signal<IObservedChange<object?, string>>();
        var e3 = new Signal<IObservedChange<object?, string>>();
        var e4 = new Signal<IObservedChange<object?, string>>();
        var e5 = new Signal<IObservedChange<object?, string>>();
        var e6 = new Signal<IObservedChange<object?, string>>();
        var e7 = new Signal<IObservedChange<object?, string>>();
        var rec = new Recorder<string>();
        _ = new WhenAnyValueSink<object?, string, string, string, string, string, string, string, string>(
            e1,
            e2,
            e3,
            e4,
            e5,
            e6,
            e7,
            (x1, x2, x3, x4, x5, x6, x7) => x1 + x2 + x3 + x4 + x5 + x6 + x7).Subscribe(rec);
        e1.OnError(ex);
        Exception[] expectedErrors = [ex];
        await Assert.That(rec.Errors).IsEquivalentTo(expectedErrors);
    }

    /// <summary>Verifies the arity-7 value sink completes once every source completes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ValueSink7_Completes()
    {
        var k1 = new Signal<IObservedChange<object?, string>>();
        var k2 = new Signal<IObservedChange<object?, string>>();
        var k3 = new Signal<IObservedChange<object?, string>>();
        var k4 = new Signal<IObservedChange<object?, string>>();
        var k5 = new Signal<IObservedChange<object?, string>>();
        var k6 = new Signal<IObservedChange<object?, string>>();
        var k7 = new Signal<IObservedChange<object?, string>>();
        var rec = new Recorder<string>();
        _ = new WhenAnyValueSink<object?, string, string, string, string, string, string, string, string>(
            k1,
            k2,
            k3,
            k4,
            k5,
            k6,
            k7,
            (x1, x2, x3, x4, x5, x6, x7) => x1 + x2 + x3 + x4 + x5 + x6 + x7).Subscribe(rec);
        k1.OnCompleted();
        k2.OnCompleted();
        k3.OnCompleted();
        k4.OnCompleted();
        k5.OnCompleted();
        k6.OnCompleted();
        k7.OnCompleted();
        await Assert.That(rec.Completed).IsEqualTo(1);
    }

    /// <summary>Verifies a throwing selector surfaces as an error from the arity-7 value sink.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ValueSink7_SelectorThrows()
    {
        var ex = new InvalidOperationException("selector");
        var t1 = new Signal<IObservedChange<object?, string>>();
        var t2 = new Signal<IObservedChange<object?, string>>();
        var t3 = new Signal<IObservedChange<object?, string>>();
        var t4 = new Signal<IObservedChange<object?, string>>();
        var t5 = new Signal<IObservedChange<object?, string>>();
        var t6 = new Signal<IObservedChange<object?, string>>();
        var t7 = new Signal<IObservedChange<object?, string>>();
        var rec = new Recorder<string>();
        _ = new WhenAnyValueSink<object?, string, string, string, string, string, string, string, string>(
            t1,
            t2,
            t3,
            t4,
            t5,
            t6,
            t7,
            (_, _, _, _, _, _, _) => throw ex).Subscribe(rec);
        t1.OnNext(Ch("a"));
        t2.OnNext(Ch("a"));
        t3.OnNext(Ch("a"));
        t4.OnNext(Ch("a"));
        t5.OnNext(Ch("a"));
        t6.OnNext(Ch("a"));
        t7.OnNext(Ch("a"));
        Exception[] expectedErrors = [ex];
        await Assert.That(rec.Errors).IsEquivalentTo(expectedErrors);
    }
}
