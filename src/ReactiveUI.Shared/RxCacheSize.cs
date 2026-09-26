// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// Provides configurable cache size limits for ReactiveUI's internal caching mechanisms.
/// These values can be configured via <see cref="IReactiveUIBuilder.WithCacheSizes"/> or will auto-initialize with platform-specific defaults.
/// </summary>
public static class RxCacheSize
{
#if ANDROID || IOS
    /// <summary>Default small cache limit for mobile platforms.</summary>
    private const int DefaultSmallCacheLimit = 32;

    /// <summary>Default big cache limit for mobile platforms.</summary>
    private const int DefaultBigCacheLimit = 64;
#else
    /// <summary>Default small cache limit for desktop platforms.</summary>
    private const int DefaultSmallCacheLimit = 64;

    /// <summary>Default big cache limit for desktop platforms.</summary>
    private const int DefaultBigCacheLimit = 256;
#endif

    /// <summary>The published cache limits, or null until they are published.</summary>
    /// <remarks>
    /// Both limits travel in one immutable object, and the field is its own initialization flag. The limits are
    /// published once with an atomic compare-and-exchange, so a reader sees either null or both limits, and the
    /// first limits published win.
    /// </remarks>
    private static CacheLimits? _limits;

    /// <summary>
    /// Gets the small cache limit used for internal memoizing caches.
    /// Default: 32 (mobile platforms) or 64 (desktop platforms).
    /// </summary>
    public static int SmallCacheLimit => Limits.Small;

    /// <summary>Gets the big cache limit used for internal memoizing caches. Default: 64 (mobile platforms) or 256 (desktop platforms).</summary>
    public static int BigCacheLimit => Limits.Big;

    /// <summary>Gets the published cache limits, publishing the platform defaults first if none are published.</summary>
    private static CacheLimits Limits => Volatile.Read(ref _limits) ?? InitializeDefaults();

    /// <summary>Initializes the cache size limits. Called by ReactiveUIBuilder.</summary>
    /// <param name="smallCacheLimit">The small cache limit to use.</param>
    /// <param name="bigCacheLimit">The big cache limit to use.</param>
    internal static void Initialize(int smallCacheLimit, int bigCacheLimit) =>
        _ = Interlocked.CompareExchange(ref _limits, new(smallCacheLimit, bigCacheLimit), null);

    /// <summary>Resets the cache size state for testing purposes.</summary>
    /// <remarks>
    /// WARNING: This method should ONLY be used in unit tests to reset state between test runs.
    /// Never call this in production code as it can lead to inconsistent application state.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ResetForTesting() => Volatile.Write(ref _limits, null);

    /// <summary>Publishes the platform default limits if no limits have been published yet.</summary>
    /// <returns>The published limits, which are another thread's limits when that thread published first.</returns>
    private static CacheLimits InitializeDefaults()
    {
        var defaults = new CacheLimits(DefaultSmallCacheLimit, DefaultBigCacheLimit);
        return Interlocked.CompareExchange(ref _limits, defaults, null) ?? defaults;
    }

    /// <summary>An immutable pair of cache limits, published together.</summary>
    /// <param name="small">The small cache limit.</param>
    /// <param name="big">The big cache limit.</param>
    private sealed class CacheLimits(int small, int big)
    {
        /// <summary>Gets the small cache limit.</summary>
        public int Small { get; } = small;

        /// <summary>Gets the big cache limit.</summary>
        public int Big { get; } = big;
    }
}
