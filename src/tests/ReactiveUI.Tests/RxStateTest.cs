// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Infrastructure;

namespace ReactiveUI.Tests;

/// <summary>Tests for the lazily initialized <see cref="RxState.DefaultExceptionHandler"/>.</summary>
[NotInParallel]
public class RxStateTest
{
    /// <summary>Verifies that threads racing on the first read never see a null handler.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task DefaultExceptionHandler_ConcurrentFirstRead_NeverReturnsNull()
    {
        try
        {
            var invalidReads = FirstReadRace.CountInvalidReads(
                RxState.ResetForTesting,
                static () => RxState.DefaultExceptionHandler,
                static handler => handler is not null,
                FirstReadRace.DefaultRounds);

            await Assert.That(invalidReads).IsEqualTo(0);
        }
        finally
        {
            RxState.ResetForTesting();
        }
    }

    /// <summary>Verifies that a rejected null handler leaves the default handler in place.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task InitializeExceptionHandler_Null_ThrowsAndKeepsDefault()
    {
        RxState.ResetForTesting();
        try
        {
            _ = Assert.Throws<ArgumentNullException>(static () => RxState.InitializeExceptionHandler(null!));

            await Assert.That(RxState.DefaultExceptionHandler).IsNotNull();
        }
        finally
        {
            RxState.ResetForTesting();
        }
    }

    /// <summary>Verifies that a handler supplied before the first read is the one returned.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task InitializeExceptionHandler_BeforeFirstRead_IsReturned()
    {
        RxState.ResetForTesting();
        try
        {
            var handler = new NoOpHandler();

            RxState.InitializeExceptionHandler(handler);

            await Assert.That(RxState.DefaultExceptionHandler).IsSameReferenceAs(handler);
        }
        finally
        {
            RxState.ResetForTesting();
        }
    }

    /// <summary>Verifies that the first handler published wins over a later one.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task InitializeExceptionHandler_AfterFirstRead_KeepsFirstHandler()
    {
        RxState.ResetForTesting();
        try
        {
            var first = RxState.DefaultExceptionHandler;

            RxState.InitializeExceptionHandler(new NoOpHandler());

            await Assert.That(RxState.DefaultExceptionHandler).IsSameReferenceAs(first);
        }
        finally
        {
            RxState.ResetForTesting();
        }
    }

    /// <summary>An exception handler that ignores every notification.</summary>
    private sealed class NoOpHandler : IObserver<Exception>
    {
        /// <inheritdoc/>
        public void OnCompleted()
        {
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
        }

        /// <inheritdoc/>
        public void OnNext(Exception value)
        {
        }
    }
}
