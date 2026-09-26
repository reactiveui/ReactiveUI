// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Infrastructure;

namespace ReactiveUI.Tests;

/// <summary>Tests for the lazily initialized <see cref="RxSuspension.SuspensionHost"/>.</summary>
[NotInParallel]
public class RxSuspensionTest
{
    /// <summary>Verifies that threads racing on the first read never see a null host.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task SuspensionHost_ConcurrentFirstRead_NeverReturnsNull()
    {
        try
        {
            var invalidReads = FirstReadRace.CountInvalidReads(
                RxSuspension.ResetForTesting,
                static () => RxSuspension.SuspensionHost,
                static host => host is not null,
                FirstReadRace.DefaultRounds);

            await Assert.That(invalidReads).IsEqualTo(0);
        }
        finally
        {
            RxSuspension.ResetForTesting();
        }
    }

    /// <summary>Verifies that a rejected null host leaves the default host in place.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task InitializeSuspensionHost_Null_ThrowsAndKeepsDefault()
    {
        RxSuspension.ResetForTesting();
        try
        {
            _ = Assert.Throws<ArgumentNullException>(static () => RxSuspension.InitializeSuspensionHost(null!));

            await Assert.That(RxSuspension.SuspensionHost).IsNotNull();
        }
        finally
        {
            RxSuspension.ResetForTesting();
        }
    }

    /// <summary>Verifies that a host supplied before the first read is the one returned.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task InitializeSuspensionHost_BeforeFirstRead_IsReturned()
    {
        RxSuspension.ResetForTesting();
        try
        {
            using var host = new SuspensionHost();

            RxSuspension.InitializeSuspensionHost(host);

            await Assert.That(RxSuspension.SuspensionHost).IsSameReferenceAs(host);
        }
        finally
        {
            RxSuspension.ResetForTesting();
        }
    }

    /// <summary>Verifies that the first host published wins over a later one.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task InitializeSuspensionHost_AfterFirstRead_KeepsFirstHost()
    {
        RxSuspension.ResetForTesting();
        try
        {
            var first = RxSuspension.SuspensionHost;
            using var later = new SuspensionHost();

            RxSuspension.InitializeSuspensionHost(later);

            await Assert.That(RxSuspension.SuspensionHost).IsSameReferenceAs(first);
        }
        finally
        {
            RxSuspension.ResetForTesting();
        }
    }
}
