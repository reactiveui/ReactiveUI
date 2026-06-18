// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.WhenAny;

/// <summary>
/// Direct tests for the internal <c>WhenAnyValueSink</c> and <c>WhenAnyChangeSink</c> combinators, exercising the
/// per-source emit branches, error forwarding, source-completion, and selector-exception paths that the public
/// <c>WhenAnyValue</c> API cannot reach (property-change observables never error or complete). Each arity lives in
/// its own partial-class file.
/// </summary>
[SuppressMessage("Major Code Smell", "S107", Justification = "Variadic selectors intentionally accept more than seven parameters.")]
public partial class WhenAnySinkDirectTests
{
    /// <summary>Verifies the arity-11 change sink emits when every source is ready and re-emits per later source update.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangeSink11_Emits()
    {
        var s1 = new Signal<string>();
        var s2 = new Signal<string>();
        var s3 = new Signal<string>();
        var s4 = new Signal<string>();
        var s5 = new Signal<string>();
        var s6 = new Signal<string>();
        var s7 = new Signal<string>();
        var s8 = new Signal<string>();
        var s9 = new Signal<string>();
        var s10 = new Signal<string>();
        var s11 = new Signal<string>();
        var rec = new Recorder<string>();
        using var sub = new WhenAnyChangeSink<string, string, string, string, string, string, string, string, string, string, string, string>(
            s1,
            s2,
            s3,
            s4,
            s5,
            s6,
            s7,
            s8,
            s9,
            s10,
            s11,
            (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11) => x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9 + x10 + x11).Subscribe(rec);
        s1.OnNext("v1");
        s2.OnNext("v2");
        s3.OnNext("v3");
        s4.OnNext("v4");
        s5.OnNext("v5");
        s6.OnNext("v6");
        s7.OnNext("v7");
        s8.OnNext("v8");
        s9.OnNext("v9");
        s10.OnNext("v10");
        s11.OnNext("v11");
        s1.OnNext("w1");
        s2.OnNext("w2");
        s3.OnNext("w3");
        s4.OnNext("w4");
        s5.OnNext("w5");
        s6.OnNext("w6");
        s7.OnNext("w7");
        s8.OnNext("w8");
        s9.OnNext("w9");
        s10.OnNext("w10");
        await Assert.That(rec.Values).IsNotEmpty();
    }

    /// <summary>Verifies the arity-11 change sink forwards a source error to the observer.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangeSink11_ForwardsError()
    {
        var ex = new InvalidOperationException("boom");
        var e1 = new Signal<string>();
        var e2 = new Signal<string>();
        var e3 = new Signal<string>();
        var e4 = new Signal<string>();
        var e5 = new Signal<string>();
        var e6 = new Signal<string>();
        var e7 = new Signal<string>();
        var e8 = new Signal<string>();
        var e9 = new Signal<string>();
        var e10 = new Signal<string>();
        var e11 = new Signal<string>();
        var rec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string, string, string, string, string, string, string, string, string, string>(
            e1,
            e2,
            e3,
            e4,
            e5,
            e6,
            e7,
            e8,
            e9,
            e10,
            e11,
            (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11) => x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9 + x10 + x11).Subscribe(rec);
        e1.OnError(ex);
        Exception[] expectedErrors = [ex];
        await Assert.That(rec.Errors).IsEquivalentTo(expectedErrors);
    }

    /// <summary>Verifies the arity-11 change sink completes once every source completes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangeSink11_Completes()
    {
        var k1 = new Signal<string>();
        var k2 = new Signal<string>();
        var k3 = new Signal<string>();
        var k4 = new Signal<string>();
        var k5 = new Signal<string>();
        var k6 = new Signal<string>();
        var k7 = new Signal<string>();
        var k8 = new Signal<string>();
        var k9 = new Signal<string>();
        var k10 = new Signal<string>();
        var k11 = new Signal<string>();
        var rec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string, string, string, string, string, string, string, string, string, string>(
            k1,
            k2,
            k3,
            k4,
            k5,
            k6,
            k7,
            k8,
            k9,
            k10,
            k11,
            (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11) => x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9 + x10 + x11).Subscribe(rec);
        k1.OnCompleted();
        k2.OnCompleted();
        k3.OnCompleted();
        k4.OnCompleted();
        k5.OnCompleted();
        k6.OnCompleted();
        k7.OnCompleted();
        k8.OnCompleted();
        k9.OnCompleted();
        k10.OnCompleted();
        k11.OnCompleted();
        await Assert.That(rec.Completed).IsEqualTo(1);
    }

    /// <summary>Verifies a throwing selector surfaces as an error from the arity-11 change sink.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangeSink11_SelectorThrows()
    {
        var ex = new InvalidOperationException("selector");
        var t1 = new Signal<string>();
        var t2 = new Signal<string>();
        var t3 = new Signal<string>();
        var t4 = new Signal<string>();
        var t5 = new Signal<string>();
        var t6 = new Signal<string>();
        var t7 = new Signal<string>();
        var t8 = new Signal<string>();
        var t9 = new Signal<string>();
        var t10 = new Signal<string>();
        var t11 = new Signal<string>();
        var rec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string, string, string, string, string, string, string, string, string, string>(
            t1,
            t2,
            t3,
            t4,
            t5,
            t6,
            t7,
            t8,
            t9,
            t10,
            t11,
            (_, _, _, _, _, _, _, _, _, _, _) => throw ex).Subscribe(rec);
        t1.OnNext("a");
        t2.OnNext("a");
        t3.OnNext("a");
        t4.OnNext("a");
        t5.OnNext("a");
        t6.OnNext("a");
        t7.OnNext("a");
        t8.OnNext("a");
        t9.OnNext("a");
        t10.OnNext("a");
        t11.OnNext("a");
        Exception[] expectedErrors = [ex];
        await Assert.That(rec.Errors).IsEquivalentTo(expectedErrors);
    }
}
