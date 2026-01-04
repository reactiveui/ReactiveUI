// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides configurable cache size limits for ReactiveUI's internal caching mechanisms.
/// These values can be configured via <see cref="Builder.IReactiveUIBuilder.WithCacheSizes"/> or will auto-initialize with platform-specific defaults.
/// </summary>
public static class RxCacheSize
{
#if ANDROID || IOS
    private const int DefaultSmallCacheLimit = 32;
    private const int DefaultBigCacheLimit = 64;
#else
    private const int DefaultSmallCacheLimit = 64;
    private const int DefaultBigCacheLimit = 256;
#endif

    private static int _smallCacheLimit;
    private static int _bigCacheLimit;
    private static int _initialized; // 0 = false, 1 = true

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

    /// <summary>
    /// Gets the big cache limit used for internal memoizing caches.
    /// Default: 64 (mobile platforms) or 256 (desktop platforms).
    /// </summary>
    public static int BigCacheLimit
    {
        get
        {
            EnsureInitialized();
            return _bigCacheLimit;
        }
    }

    /// <summary>
    /// Initializes the cache size limits. Called by ReactiveUIBuilder.
    /// </summary>
    /// <param name="smallCacheLimit">The small cache limit to use.</param>
    /// <param name="bigCacheLimit">The big cache limit to use.</param>
    internal static void Initialize(int smallCacheLimit, int bigCacheLimit)
    {
        if (Interlocked.CompareExchange(ref _initialized, 1, 0) == 0)
        {
            _smallCacheLimit = smallCacheLimit;
            _bigCacheLimit = bigCacheLimit;
        }
    }

    /// <summary>
    /// Ensures cache sizes are initialized with platform defaults if not already configured.
    /// </summary>
    private static void EnsureInitialized()
    {
        if (Interlocked.CompareExchange(ref _initialized, 0, 0) == 0)
        {
            Initialize(DefaultSmallCacheLimit, DefaultBigCacheLimit);
        }
    }
}
