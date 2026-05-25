// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Subjects;
using ReactiveUI.Internal;

namespace ReactiveUI.Tests.WhenAny;

/// <content>Direct arity-2 <c>WhenAnyChangeSink</c> coverage.</content>
public partial class WhenAnySinkDirectTests
{
    /// <summary>Exercises every emit, error, completion, and selector-exception path of the arity-2 change sink.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangeSink2_AllPaths()
    {
        var s1 = new Subject<string>();
        var s2 = new Subject<string>();
        var rec = new Recorder<string>();
        using (new WhenAnyChangeSink<string, string, string>(s1, s2, (x1, x2) => x1 + x2).Subscribe(rec))
        {
            s1.OnNext("v1");
            s2.OnNext("v2");
            s1.OnNext("w1");
        }

        var ex = new InvalidOperationException("boom");
        var e1 = new Subject<string>();
        var e2 = new Subject<string>();
        var errRec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string>(e1, e2, (x1, x2) => x1 + x2).Subscribe(errRec);
        e1.OnError(ex);

        var k1 = new Subject<string>();
        var k2 = new Subject<string>();
        var cmpRec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string>(k1, k2, (x1, x2) => x1 + x2).Subscribe(cmpRec);
        k1.OnCompleted();
        k2.OnCompleted();

        var t1 = new Subject<string>();
        var t2 = new Subject<string>();
        var throwRec = new Recorder<string>();
        new WhenAnyChangeSink<string, string, string>(t1, t2, (_, _) => throw ex).Subscribe(throwRec);
        t1.OnNext("a");
        t2.OnNext("a");

        Exception[] expectedErrors = [ex];
        await Assert.That(rec.Values).IsNotEmpty();
        await Assert.That(errRec.Errors).IsEquivalentTo(expectedErrors);
        await Assert.That(cmpRec.Completed).IsEqualTo(1);
        await Assert.That(throwRec.Errors).IsEquivalentTo(expectedErrors);
    }
}
