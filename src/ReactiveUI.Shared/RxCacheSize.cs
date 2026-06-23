// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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

    /// <summary>The configured small cache limit.</summary>
    private static int _smallCacheLimit;

    /// <summary>The configured big cache limit.</summary>
    private static int _bigCacheLimit;

    /// <summary>Tracks whether initialization has occurred; 0 means uninitialized, 1 means initialized.</summary>
    private static int _initialized;

    /// <summary>
    /// Gets the small cache limit used for internal memoizing caches.
    /// Default: 32 (mobile platforms) or 64 (desktop platforms).
    /// </summary>
    public static int SmallCacheLimit
    {
        get
        {
            EnsureInitialized();
            return _smallCacheLimit;
        }
    }

    /// <summary>Gets the big cache limit used for internal memoizing caches. Default: 64 (mobile platforms) or 256 (desktop platforms).</summary>
    public static int BigCacheLimit
    {
        get
        {
            EnsureInitialized();
            return _bigCacheLimit;
        }
    }

    /// <summary>Initializes the cache size limits. Called by ReactiveUIBuilder.</summary>
    /// <param name="smallCacheLimit">The small cache limit to use.</param>
    /// <param name="bigCacheLimit">The big cache limit to use.</param>
    internal static void Initialize(int smallCacheLimit, int bigCacheLimit)
    {
        if (Interlocked.CompareExchange(ref _initialized, 1, 0) != 0)
        {
            return;
        }

        _smallCacheLimit = smallCacheLimit;
        _bigCacheLimit = bigCacheLimit;
    }

    /// <summary>Resets the cache size state for testing purposes.</summary>
    /// <remarks>
    /// WARNING: This method should ONLY be used in unit tests to reset state between test runs.
    /// Never call this in production code as it can lead to inconsistent application state.
    /// </remarks>
    internal static void ResetForTesting()
    {
        _ = Interlocked.Exchange(ref _initialized, 0);
        _smallCacheLimit = 0;
        _bigCacheLimit = 0;
    }

    /// <summary>Ensures cache sizes are initialized with platform defaults if not already configured.</summary>
    private static void EnsureInitialized()
    {
        if (Interlocked.CompareExchange(ref _initialized, 0, 0) != 0)
        {
            return;
        }

        Initialize(DefaultSmallCacheLimit, DefaultBigCacheLimit);
    }
}
