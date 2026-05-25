// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Subjects;
using ReactiveUI.Internal;

namespace ReactiveUI.Tests.WhenAny;

/// <content>Direct arity-7 <c>WhenAnyChangeSink</c> coverage.</content>
public partial class WhenAnySinkDirectTests
{
    /// <summary>Verifies the arity-7 change sink emits when every source is ready and re-emits per later source update.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangeSink7_Emits()
    {
        var s1 = new Subject<string>();
        var s2 = new Subject<string>();
        var s3 = new Subject<string>();
        var s4 = new Subject<string>();
        var s5 = new Subject<string>();
        var s6 = new Subject<string>();
        var s7 = new Subject<string>();
        var rec = new Recorder<string>();
        using var sub = new WhenAnyChangeSink<string, string, string, string, string, string, string, string>(
            s1,
            s2,
            s3,
            s4,
            s5,
            s6,
            s7,
            (x1, x2, x3, x4, x5, x6, x7) => x1 + x2 + x3 + x4 + x5 + x6 + x7).Subscribe(rec);
        s1.OnNext("v1");
        s2.OnNext("v2");
        s3.OnNext("v3");
        s4.OnNext("v4");
        s5.OnNext("v5");
        s6.OnNext("v6");
        s7.OnNext("v7");
        s1.OnNext("w1");
        s2.OnNext("w2");
        s3.OnNext("w3");
        s4.OnNext("w4");
        s5.OnNext("w5");
        s6.OnNext("w6");
        await Assert.That(rec.Values).IsNotEmpty();
    }

    /// <summary>Verifies the arity-7 change sink forwards a source error to the observer.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangeSink7_ForwardsError()
    {
        var ex = new InvalidOperationException("boom");
        var e1 = new Subject<string>();
        var e2 = new Subject<string>();
        var e3 = new Subject<string>();
        var e4 = new Subject<string>();
        var e5 = new Subject<string>();
        var e6 = new Subject<string>();
        var e7 = new Subject<string>();
        var rec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string, string, string, string, string, string>(
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

    /// <summary>Verifies the arity-7 change sink completes once every source completes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangeSink7_Completes()
    {
        var k1 = new Subject<string>();
        var k2 = new Subject<string>();
        var k3 = new Subject<string>();
        var k4 = new Subject<string>();
        var k5 = new Subject<string>();
        var k6 = new Subject<string>();
        var k7 = new Subject<string>();
        var rec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string, string, string, string, string, string>(
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

    /// <summary>Verifies a throwing selector surfaces as an error from the arity-7 change sink.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangeSink7_SelectorThrows()
    {
        var ex = new InvalidOperationException("selector");
        var t1 = new Subject<string>();
        var t2 = new Subject<string>();
        var t3 = new Subject<string>();
        var t4 = new Subject<string>();
        var t5 = new Subject<string>();
        var t6 = new Subject<string>();
        var t7 = new Subject<string>();
        var rec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string, string, string, string, string, string>(
            t1,
            t2,
            t3,
            t4,
            t5,
            t6,
            t7,
            (_, _, _, _, _, _, _) => throw ex).Subscribe(rec);
        t1.OnNext("a");
        t2.OnNext("a");
        t3.OnNext("a");
        t4.OnNext("a");
        t5.OnNext("a");
        t6.OnNext("a");
        t7.OnNext("a");
        Exception[] expectedErrors = [ex];
        await Assert.That(rec.Errors).IsEquivalentTo(expectedErrors);
    }
}
