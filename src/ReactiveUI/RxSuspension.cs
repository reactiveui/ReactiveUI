// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides access to ReactiveUI suspension functionality.
/// </summary>
/// <remarks>
/// This class provides suspension host functionality for application lifecycle management.
/// Configure using <see cref="Builder.IReactiveUIBuilder.WithSuspensionHost"/> or
/// <see cref="Builder.IReactiveUIBuilder.WithSuspensionHost{TAppState}"/>.
/// </remarks>
public static class RxSuspension
{
    private static ISuspensionHost? _suspensionHost;
    private static int _suspensionHostInitialized; // 0 = false, 1 = true

    /// <summary>
    /// Gets the suspension host for application lifecycle management.
    /// Provides events for process lifetime events, especially on mobile devices.
    /// Auto-initializes with default SuspensionHost if not configured via builder.
    /// </summary>
    public static ISuspensionHost SuspensionHost
    {
        get
        {
            if (Interlocked.CompareExchange(ref _suspensionHostInitialized, 0, 0) == 0)
            {
                InitializeDefaultSuspensionHost();
            }

            return _suspensionHost!;
        }
    }

    /// <summary>
    /// Initializes the suspension host with a custom instance. Called by ReactiveUIBuilder.
    /// </summary>
    /// <param name="suspensionHost">The custom suspension host to use.</param>
    internal static void InitializeSuspensionHost(ISuspensionHost suspensionHost)
    {
        if (Interlocked.CompareExchange(ref _suspensionHostInitialized, 1, 0) == 0)
        {
            _suspensionHost = suspensionHost ?? throw new ArgumentNullException(nameof(suspensionHost));
        }
    }

    /// <summary>
    /// Resets the suspension host state for testing purposes.
    /// </summary>
    /// <remarks>
    /// WARNING: This method should ONLY be used in unit tests to reset state between test runs.
    /// Never call this in production code as it can lead to inconsistent application state.
    /// </remarks>
    internal static void ResetForTesting()
    {
        Interlocked.Exchange(ref _suspensionHostInitialized, 0);
        _suspensionHost = null;
    }

    /// <summary>
    /// Initializes the default suspension host if not already configured.
    /// Creates a new SuspensionHost instance.
    /// </summary>
    private static void InitializeDefaultSuspensionHost()
    {
        if (Interlocked.CompareExchange(ref _suspensionHostInitialized, 1, 0) == 0)
        {
            _suspensionHost = new SuspensionHost();
        }
    }
}
