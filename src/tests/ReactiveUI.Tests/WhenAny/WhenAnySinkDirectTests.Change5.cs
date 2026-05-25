// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Subjects;
using ReactiveUI.Internal;

namespace ReactiveUI.Tests.WhenAny;

/// <content>Direct arity-5 <c>WhenAnyChangeSink</c> coverage.</content>
public partial class WhenAnySinkDirectTests
{
    /// <summary>Exercises every emit, error, completion, and selector-exception path of the arity-5 change sink.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangeSink5_AllPaths()
    {
        var s1 = new Subject<string>();
        var s2 = new Subject<string>();
        var s3 = new Subject<string>();
        var s4 = new Subject<string>();
        var s5 = new Subject<string>();
        var rec = new Recorder<string>();
        using (new WhenAnyChangeSink<string, string, string, string, string, string>(s1, s2, s3, s4, s5, (x1, x2, x3, x4, x5) => x1 + x2 + x3 + x4 + x5).Subscribe(rec))
        {
            s1.OnNext("v1");
            s2.OnNext("v2");
            s3.OnNext("v3");
            s4.OnNext("v4");
            s5.OnNext("v5");
            s1.OnNext("w1");
            s2.OnNext("w2");
            s3.OnNext("w3");
            s4.OnNext("w4");
        }

        var ex = new InvalidOperationException("boom");
        var e1 = new Subject<string>();
        var e2 = new Subject<string>();
        var e3 = new Subject<string>();
        var e4 = new Subject<string>();
        var e5 = new Subject<string>();
        var errRec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string, string, string, string>(e1, e2, e3, e4, e5, (x1, x2, x3, x4, x5) => x1 + x2 + x3 + x4 + x5).Subscribe(errRec);
        e1.OnError(ex);

        var k1 = new Subject<string>();
        var k2 = new Subject<string>();
        var k3 = new Subject<string>();
        var k4 = new Subject<string>();
        var k5 = new Subject<string>();
        var cmpRec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string, string, string, string>(k1, k2, k3, k4, k5, (x1, x2, x3, x4, x5) => x1 + x2 + x3 + x4 + x5).Subscribe(cmpRec);
        k1.OnCompleted();
        k2.OnCompleted();
        k3.OnCompleted();
        k4.OnCompleted();
        k5.OnCompleted();

        var t1 = new Subject<string>();
        var t2 = new Subject<string>();
        var t3 = new Subject<string>();
        var t4 = new Subject<string>();
        var t5 = new Subject<string>();
        var throwRec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string, string, string, string>(t1, t2, t3, t4, t5, (_, _, _, _, _) => throw ex).Subscribe(throwRec);
        t1.OnNext("a");
        t2.OnNext("a");
        t3.OnNext("a");
        t4.OnNext("a");
        t5.OnNext("a");

        Exception[] expectedErrors = [ex];
        await Assert.That(rec.Values).IsNotEmpty();
        await Assert.That(errRec.Errors).IsEquivalentTo(expectedErrors);
        await Assert.That(cmpRec.Completed).IsEqualTo(1);
        await Assert.That(throwRec.Errors).IsEquivalentTo(expectedErrors);
    }
}
