// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI.Signals;
using Xunit;

namespace ReactiveUI.Tests;

public class SignalFromTaskTest
{
    [Fact]
    public async Task SignalFromTaskHandlesUserExceptions()
    {
        var statusTrail = new List<(int, string)>();
        var position = 0;
        Exception? exception = null;
        var fixture = Signal.FromTask(
             async (cts) =>
             {
                 statusTrail.Add((position++, "started command"));
                 await Task.Delay(10000, cts.Token).HandleCancellation(async () =>
                 {
                     // User Handles cancellation.
                     statusTrail.Add((position++, "starting cancelling command"));

                     // dummy cleanup
                     await Task.Delay(5000, CancellationToken.None).ConfigureAwait(false);
                     statusTrail.Add((position++, "finished cancelling command"));
                 }).ConfigureAwait(true);

                 if (!cts.IsCancellationRequested)
                 {
                     statusTrail.Add((position++, "finished command Normally"));
                 }

                 throw new Exception("break execution");
             }).Catch<Unit, Exception>(
            ex =>
            {
                exception = ex;
                statusTrail.Add((position++, "Exception Should Be here"));
                return Observable.Throw<Unit>(ex);
            }).Finally(() => statusTrail.Add((position++, "Should always come here.")));

        var result = false;
        var cancel = fixture.Subscribe(_ => result = true);
        await Task.Delay(500).ConfigureAwait(true);

        Assert.True(statusTrail.Select(x => x.Item2).Contains("started command"));

        await Task.Delay(10000).ConfigureAwait(true);
        cancel.Dispose();

        // Wait 6000 ms to allow execution and cleanup to complete
        await Task.Delay(6000).ConfigureAwait(false);

        Assert.False(statusTrail.Select(x => x.Item2).Contains("starting cancelling command"));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("Should always come here."));
        Assert.False(statusTrail.Select(x => x.Item2).Contains("finished cancelling command"));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("Exception Should Be here"));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("finished command Normally"));
        Assert.False(result);
        //// (0, "started command")
        //// (1, "finished command Normally")
        //// (2, "Exception Should Be here")
        //// (3, "Should always come here.")
    }

    [Fact]
    public async Task SignalFromTaskHandlesCancellation()
    {
        var statusTrail = new List<(int, string)>();
        var position = 0;
        Exception? exception = null;
        var fixture = Signal.FromTask(
             async (cts) =>
        {
            statusTrail.Add((position++, "started command"));
            await Task.Delay(10000, cts.Token).HandleCancellation(async () =>
            {
                // User Handles cancellation.
                statusTrail.Add((position++, "starting cancelling command"));

                // dummy cleanup
                await Task.Delay(5000, CancellationToken.None).ConfigureAwait(false);
                statusTrail.Add((position++, "finished cancelling command"));
            }).ConfigureAwait(true);

            if (!cts.IsCancellationRequested)
            {
                statusTrail.Add((position++, "finished command Normally"));
            }

            return Unit.Default;
        }).Catch<Unit, Exception>(
            ex =>
            {
                exception = ex;
                statusTrail.Add((position++, "Exception Should Be here"));
                return Observable.Throw<Unit>(ex);
            }).Finally(() => statusTrail.Add((position++, "Should always come here.")));

        var result = false;
        var cancel = fixture.Subscribe(_ => result = true);
        await Task.Delay(500).ConfigureAwait(true);

        Assert.True(statusTrail.Select(x => x.Item2).Contains("started command"));
        cancel.Dispose();

        // Wait 6000 ms to allow execution and cleanup to complete
        await Task.Delay(6000).ConfigureAwait(false);

        Assert.True(statusTrail.Select(x => x.Item2).Contains("starting cancelling command"));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("Should always come here."));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("finished cancelling command"));
        Assert.False(statusTrail.Select(x => x.Item2).Contains("finished command Normally"));
        Assert.False(result);
        //// (0, "started command")
        //// (1, "starting cancelling command")
        //// (2, "Should always come here.")
        //// (3, "finished cancelling command")
    }

    [Fact]
    public async Task SignalFromTaskHandlesTokenCancellation()
    {
        var statusTrail = new List<(int, string)>();
        var position = 0;
        Exception? exception = null;
        var fixture = Signal.FromTask(
             async (cts) =>
             {
                 statusTrail.Add((position++, "started command"));
                 await Task.Delay(1000, cts.Token).HandleCancellation();
                 _ = Task.Run(async () =>
                 {
                     // Wait for 1s then cancel
                     await Task.Delay(1000);
                     cts.Cancel();
                 });
                 await Task.Delay(5000, cts.Token).HandleCancellation(async () =>
                 {
                     // User Handles cancellation.
                     statusTrail.Add((position++, "starting cancelling command"));

                     // dummy cleanup
                     await Task.Delay(5000, CancellationToken.None).ConfigureAwait(false);
                     statusTrail.Add((position++, "finished cancelling command"));
                 }).ConfigureAwait(true);

                 if (!cts.IsCancellationRequested)
                 {
                     statusTrail.Add((position++, "finished command Normally"));
                 }

                 return Unit.Default;
             }).Catch<Unit, Exception>(
            ex =>
            {
                exception = ex;
                statusTrail.Add((position++, "Exception Should Be here"));
                return Observable.Throw<Unit>(ex);
            }).Finally(() => statusTrail.Add((position++, "Should always come here.")));

        var result = false;
        var cancel = fixture.Subscribe(_ => result = true);
        await Task.Delay(500).ConfigureAwait(true);

        Assert.True(statusTrail.Select(x => x.Item2).Contains("started command"));

        // Wait 8000 ms to allow execution and cleanup to complete
        await Task.Delay(8000).ConfigureAwait(false);

        Assert.True(statusTrail.Select(x => x.Item2).Contains("starting cancelling command"));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("Should always come here."));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("finished cancelling command"));
        Assert.False(statusTrail.Select(x => x.Item2).Contains("finished command Normally"));
        Assert.False(result);
        //// (0, "started command")
        //// (1, "starting cancelling command")
        //// (2, "Should always come here.")
        //// (3, "finished cancelling command")
    }

    [Fact]
    public async Task SignalFromTaskHandlesCancellationInBase()
    {
        var statusTrail = new List<(int, string)>();
        var position = 0;
        Exception? exception = null;
        var fixture = Signal.FromTask(
             async (cts) =>
             {
                 var ex = new Exception();
                 statusTrail.Add((position++, "started command"));
                 await Task.Delay(10000, cts.Token).ConfigureAwait(true);
                 if (!cts.IsCancellationRequested)
                 {
                     statusTrail.Add((position++, "finished command Normally"));
                 }

                 return Unit.Default;
             }).Catch<Unit, Exception>(
            ex =>
            {
                exception = ex;
                statusTrail.Add((position++, "Exception Should Be here"));
                return Observable.Throw<Unit>(ex);
            }).Finally(() => statusTrail.Add((position++, "Should always come here.")));

        var cancel = fixture.Subscribe();
        await Task.Delay(500).ConfigureAwait(true);
        Assert.True(statusTrail.Select(x => x.Item2).Contains("started command"));
        cancel.Dispose();

        // Wait 5050 ms to allow execution and cleanup to complete
        await Task.Delay(6000).ConfigureAwait(false);

        Assert.False(statusTrail.Select(x => x.Item2).Contains("finished command Normally"));
        Assert.Equal("Should always come here.", statusTrail.Last().Item2);

        //// (0, "started command")
        //// (1, "Should always come here.")
    }

    [Fact]
    public async Task SignalFromTaskHandlesCompletion()
    {
        var statusTrail = new List<(int, string)>();
        var position = 0;
        Exception? exception = null;
        var fixture = Signal.FromTask(
             async (cts) =>
             {
                 statusTrail.Add((position++, "started command"));
                 await Task.Delay(10000, cts.Token).HandleCancellation(async () =>
                 {
                     // NOT EXPECTED TO ENTER HERE

                     // User Handles cancellation.
                     statusTrail.Add((position++, "starting cancelling command"));

                     // dummy cleanup
                     await Task.Delay(5000, CancellationToken.None).ConfigureAwait(false);
                     statusTrail.Add((position++, "finished cancelling command"));
                 }).ConfigureAwait(true);

                 if (!cts.IsCancellationRequested)
                 {
                     statusTrail.Add((position++, "finished command Normally"));
                 }

                 return Unit.Default;
             }).Catch<Unit, Exception>(
            ex =>
            {
                exception = ex;
                statusTrail.Add((position++, "Exception Should Be here"));
                return Observable.Throw<Unit>(ex);
            }).Finally(() => statusTrail.Add((position++, "Should always come here.")));

        var result = false;
        var cancel = fixture.Subscribe(_ => result = true);
        await Task.Delay(500).ConfigureAwait(true);

        Assert.True(statusTrail.Select(x => x.Item2).Contains("started command"));

        // Wait 11000 ms to allow execution complete
        await Task.Delay(11000).ConfigureAwait(false);

        Assert.False(statusTrail.Select(x => x.Item2).Contains("starting cancelling command"));
        Assert.False(statusTrail.Select(x => x.Item2).Contains("finished cancelling command"));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("finished command Normally"));
        Assert.Equal("Should always come here.", statusTrail.Last().Item2);
        Assert.True(result);
        //// (0, "started command")
        //// (2, "finished command Normally")
        //// (1, "Should always come here.")
    }

    [Fact]
    public async Task SignalFromTask_T_HandlesUserExceptions()
    {
        var statusTrail = new List<(int, string)>();
        var position = 0;
        Exception? exception = null;
        var fixture = Signal.FromTask<Unit>(
             async (cts) =>
             {
                 statusTrail.Add((position++, "started command"));
                 await Task.Delay(10000, cts.Token).HandleCancellation(async () =>
                 {
                     // User Handles cancellation.
                     statusTrail.Add((position++, "starting cancelling command"));

                     // dummy cleanup
                     await Task.Delay(5000, CancellationToken.None).ConfigureAwait(false);
                     statusTrail.Add((position++, "finished cancelling command"));
                 }).ConfigureAwait(true);

                 if (!cts.IsCancellationRequested)
                 {
                     statusTrail.Add((position++, "finished command Normally"));
                 }

                 throw new Exception("break execution");
             }).Catch<Unit, Exception>(
            ex =>
            {
                exception = ex;
                statusTrail.Add((position++, "Exception Should Be here"));
                return Observable.Throw<Unit>(ex);
            }).Finally(() => statusTrail.Add((position++, "Should always come here.")));

        var result = false;
        var cancel = fixture.Subscribe(_ => result = true);
        await Task.Delay(500).ConfigureAwait(true);

        Assert.True(statusTrail.Select(x => x.Item2).Contains("started command"));

        await Task.Delay(10000).ConfigureAwait(true);
        cancel.Dispose();

        // Wait 6000 ms to allow execution and cleanup to complete
        await Task.Delay(6000).ConfigureAwait(false);

        Assert.False(statusTrail.Select(x => x.Item2).Contains("starting cancelling command"));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("Should always come here."));
        Assert.False(statusTrail.Select(x => x.Item2).Contains("finished cancelling command"));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("Exception Should Be here"));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("finished command Normally"));
        Assert.False(result);
        //// (0, "started command")
        //// (1, "finished command Normally")
        //// (2, "Exception Should Be here")
        //// (3, "Should always come here.")
    }

    [Fact]
    public async Task SignalFromTask_T_HandlesCancellation()
    {
        var statusTrail = new List<(int, string)>();
        var position = 0;
        Exception? exception = null;
        var fixture = Signal.FromTask<Unit>(
             async (cts) =>
             {
                 statusTrail.Add((position++, "started command"));
                 await Task.Delay(10000, cts.Token).HandleCancellation(async () =>
                 {
                     // User Handles cancellation.
                     statusTrail.Add((position++, "starting cancelling command"));

                     // dummy cleanup
                     await Task.Delay(5000, CancellationToken.None).ConfigureAwait(false);
                     statusTrail.Add((position++, "finished cancelling command"));
                 }).ConfigureAwait(true);

                 if (!cts.IsCancellationRequested)
                 {
                     statusTrail.Add((position++, "finished command Normally"));
                 }

                 return Unit.Default;
             }).Catch<Unit, Exception>(
            ex =>
            {
                exception = ex;
                statusTrail.Add((position++, "Exception Should Be here"));
                return Observable.Throw<Unit>(ex);
            }).Finally(() => statusTrail.Add((position++, "Should always come here.")));

        var result = false;
        var cancel = fixture.Subscribe(_ => result = true);
        await Task.Delay(500).ConfigureAwait(true);

        Assert.True(statusTrail.Select(x => x.Item2).Contains("started command"));
        cancel.Dispose();

        // Wait 6000 ms to allow execution and cleanup to complete
        await Task.Delay(6000).ConfigureAwait(false);

        Assert.True(statusTrail.Select(x => x.Item2).Contains("starting cancelling command"));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("Should always come here."));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("finished cancelling command"));
        Assert.False(statusTrail.Select(x => x.Item2).Contains("finished command Normally"));
        Assert.False(result);
        //// (0, "started command")
        //// (1, "starting cancelling command")
        //// (3, "Should always come here.")
        //// (2, "finished cancelling command")
    }

    [Fact]
    public async Task SignalFromTask_T_HandlesTokenCancellation()
    {
        var statusTrail = new List<(int, string)>();
        var position = 0;
        Exception? exception = null;
        var fixture = Signal.FromTask<Unit>(
             async (cts) =>
             {
                 statusTrail.Add((position++, "started command"));
                 await Task.Delay(1000, cts.Token).HandleCancellation();
                 _ = Task.Run(async () =>
                 {
                     // Wait for 1s then cancel
                     await Task.Delay(1000);
                     cts.Cancel();
                 });
                 await Task.Delay(5000, cts.Token).HandleCancellation(async () =>
                 {
                     // User Handles cancellation.
                     statusTrail.Add((position++, "starting cancelling command"));

                     // dummy cleanup
                     await Task.Delay(5000, CancellationToken.None).ConfigureAwait(false);
                     statusTrail.Add((position++, "finished cancelling command"));
                 }).ConfigureAwait(true);

                 if (!cts.IsCancellationRequested)
                 {
                     statusTrail.Add((position++, "finished command Normally"));
                 }

                 return Unit.Default;
             }).Catch<Unit, Exception>(
            ex =>
            {
                exception = ex;
                statusTrail.Add((position++, "Exception Should Be here"));
                return Observable.Throw<Unit>(ex);
            }).Finally(() => statusTrail.Add((position++, "Should always come here.")));

        var result = false;
        var cancel = fixture.Subscribe(_ => result = true);
        await Task.Delay(500).ConfigureAwait(true);

        Assert.True(statusTrail.Select(x => x.Item2).Contains("started command"));

        // Wait 8000 ms to allow execution and cleanup to complete
        await Task.Delay(8000).ConfigureAwait(false);

        Assert.True(statusTrail.Select(x => x.Item2).Contains("starting cancelling command"));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("Should always come here."));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("finished cancelling command"));
        Assert.False(statusTrail.Select(x => x.Item2).Contains("finished command Normally"));
        Assert.False(result);
        //// (0, "started command")
        //// (1, "starting cancelling command")
        //// (2, "Should always come here.")
        //// (3, "finished cancelling command")
    }

    [Fact]
    public async Task SignalFromTask_T_HandlesCancellationInBase()
    {
        var statusTrail = new List<(int, string)>();
        var position = 0;
        Exception? exception = null;
        var fixture = Signal.FromTask<Unit>(
             async (cts) =>
             {
                 var ex = new Exception();
                 statusTrail.Add((position++, "started command"));
                 await Task.Delay(10000, cts.Token).ConfigureAwait(true);
                 if (!cts.IsCancellationRequested)
                 {
                     statusTrail.Add((position++, "finished command Normally"));
                 }

                 return Unit.Default;
             }).Catch<Unit, Exception>(
            ex =>
            {
                exception = ex;
                statusTrail.Add((position++, "Exception Should Be here"));
                return Observable.Throw<Unit>(ex);
            }).Finally(() => statusTrail.Add((position++, "Should always come here.")));

        var cancel = fixture.Subscribe();
        await Task.Delay(500).ConfigureAwait(true);
        Assert.True(statusTrail.Select(x => x.Item2).Contains("started command"));
        cancel.Dispose();

        // Wait 5050 ms to allow execution and cleanup to complete
        await Task.Delay(6000).ConfigureAwait(false);

        Assert.False(statusTrail.Select(x => x.Item2).Contains("finished command Normally"));
        Assert.Equal("Should always come here.", statusTrail.Last().Item2);

        //// (0, "started command")
        //// (1, "Should always come here.")
    }

    [Fact]
    public async Task SignalFromTask_T_HandlesCompletion()
    {
        var statusTrail = new List<(int, string)>();
        var position = 0;
        Exception? exception = null;
        var fixture = Signal.FromTask<Unit>(
             async (cts) =>
             {
                 statusTrail.Add((position++, "started command"));
                 await Task.Delay(10000, cts.Token).HandleCancellation(async () =>
                 {
                     // NOT EXPECTED TO ENTER HERE

                     // User Handles cancellation.
                     statusTrail.Add((position++, "starting cancelling command"));

                     // dummy cleanup
                     await Task.Delay(5000, CancellationToken.None).ConfigureAwait(false);
                     statusTrail.Add((position++, "finished cancelling command"));
                 }).ConfigureAwait(true);

                 if (!cts.IsCancellationRequested)
                 {
                     statusTrail.Add((position++, "finished command Normally"));
                 }

                 return Unit.Default;
             }).Catch<Unit, Exception>(
            ex =>
            {
                exception = ex;
                statusTrail.Add((position++, "Exception Should Be here"));
                return Observable.Throw<Unit>(ex);
            }).Finally(() => statusTrail.Add((position++, "Should always come here.")));

        var result = false;
        var cancel = fixture.Subscribe(_ => result = true);
        await Task.Delay(500).ConfigureAwait(true);

        Assert.True(statusTrail.Select(x => x.Item2).Contains("started command"));

        // Wait 11000 ms to allow execution complete
        await Task.Delay(11000).ConfigureAwait(false);

        Assert.False(statusTrail.Select(x => x.Item2).Contains("starting cancelling command"));
        Assert.False(statusTrail.Select(x => x.Item2).Contains("finished cancelling command"));
        Assert.True(statusTrail.Select(x => x.Item2).Contains("finished command Normally"));
        Assert.Equal("Should always come here.", statusTrail.Last().Item2);
        Assert.True(result);
        //// (0, "started command")
        //// (2, "finished command Normally")
        //// (1, "Should always come here.")
    }
}
