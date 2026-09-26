// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Infrastructure;

namespace ReactiveUI.Tests;

/// <summary>Tests for the lazily initialized <see cref="RxCacheSize"/> limits.</summary>
[NotInParallel]
public class RxCacheSizeTest
{
    /// <summary>The small cache limit the tests configure.</summary>
    private const int ConfiguredSmallLimit = 128;

    /// <summary>The big cache limit the tests configure.</summary>
    private const int ConfiguredBigLimit = 512;

    /// <summary>Verifies that threads racing on the first read never see an unset limit.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Limits_ConcurrentFirstRead_NeverReturnZero()
    {
        try
        {
            var invalidReads = FirstReadRace.CountInvalidReads(
                RxCacheSize.ResetForTesting,
                static () => (Small: RxCacheSize.SmallCacheLimit, Big: RxCacheSize.BigCacheLimit),
                static limits => limits.Small > 0 && limits.Big > 0,
                FirstReadRace.DefaultRounds);

            await Assert.That(invalidReads).IsEqualTo(0);
        }
        finally
        {
            RxCacheSize.ResetForTesting();
        }
    }

    /// <summary>Verifies that limits supplied before the first read are the ones returned.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Initialize_BeforeFirstRead_IsReturned()
    {
        RxCacheSize.ResetForTesting();
        try
        {
            RxCacheSize.Initialize(ConfiguredSmallLimit, ConfiguredBigLimit);

            await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(ConfiguredSmallLimit);
            await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(ConfiguredBigLimit);
        }
        finally
        {
            RxCacheSize.ResetForTesting();
        }
    }

    /// <summary>Verifies that the first limits published win over later ones.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Initialize_AfterFirstRead_KeepsFirstLimits()
    {
        RxCacheSize.ResetForTesting();
        try
        {
            var small = RxCacheSize.SmallCacheLimit;
            var big = RxCacheSize.BigCacheLimit;

            RxCacheSize.Initialize(ConfiguredSmallLimit, ConfiguredBigLimit);

            await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(small);
            await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(big);
        }
        finally
        {
            RxCacheSize.ResetForTesting();
        }
    }
}
