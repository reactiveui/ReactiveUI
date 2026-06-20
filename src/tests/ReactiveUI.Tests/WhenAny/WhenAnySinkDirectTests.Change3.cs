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
    /// <summary>Exercises every emit, error, completion, and selector-exception path of the arity-3 change sink.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangeSink3_AllPaths()
    {
        var s1 = new Signal<string>();
        var s2 = new Signal<string>();
        var s3 = new Signal<string>();
        var rec = new Recorder<string>();
        using (new WhenAnyChangeSink<string, string, string, string>(s1, s2, s3, (x1, x2, x3) => x1 + x2 + x3).Subscribe(rec))
        {
            s1.OnNext("v1");
            s2.OnNext("v2");
            s3.OnNext("v3");
            s1.OnNext("w1");
            s2.OnNext("w2");
        }

        var ex = new InvalidOperationException("boom");
        var e1 = new Signal<string>();
        var e2 = new Signal<string>();
        var e3 = new Signal<string>();
        var errRec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string, string>(e1, e2, e3, (x1, x2, x3) => x1 + x2 + x3).Subscribe(errRec);
        e1.OnError(ex);

        var k1 = new Signal<string>();
        var k2 = new Signal<string>();
        var k3 = new Signal<string>();
        var cmpRec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string, string>(k1, k2, k3, (x1, x2, x3) => x1 + x2 + x3).Subscribe(cmpRec);
        k1.OnCompleted();
        k2.OnCompleted();
        k3.OnCompleted();

        var t1 = new Signal<string>();
        var t2 = new Signal<string>();
        var t3 = new Signal<string>();
        var throwRec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string, string>(t1, t2, t3, (_, _, _) => throw ex).Subscribe(throwRec);
        t1.OnNext("a");
        t2.OnNext("a");
        t3.OnNext("a");

        Exception[] expectedErrors = [ex];
        await Assert.That(rec.Values).IsNotEmpty();
        await Assert.That(errRec.Errors).IsEquivalentTo(expectedErrors);
        await Assert.That(cmpRec.Completed).IsEqualTo(1);
        await Assert.That(throwRec.Errors).IsEquivalentTo(expectedErrors);
    }
}
