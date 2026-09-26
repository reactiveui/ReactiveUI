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
/// Provides access to the application's suspension host for managing process lifecycle events, such as application
/// suspension and resumption. This class enables integration with platform-specific lifecycle management, particularly
/// on mobile devices.
/// </summary>
/// <remarks>The suspension host is automatically initialized with a default implementation if not configured
/// explicitly. This class is intended for use by application infrastructure and advanced scenarios that require direct
/// access to lifecycle events. Most applications interact with the suspension host through higher-level APIs.</remarks>
public static class RxSuspension
{
#if NET9_0_OR_GREATER
    /// <summary>Serializes publishing the suspension host, so only one default host is ever created.</summary>
    private static readonly Lock _gate = new();
#else
    /// <summary>Serializes publishing the suspension host, so only one default host is ever created.</summary>
    private static readonly object _gate = new();
#endif

    /// <summary>The published suspension host, or null until one is published.</summary>
    /// <remarks>
    /// The field is its own initialization flag. A host is published once, under <see cref="_gate"/>, so a
    /// reader sees either null or a fully constructed host, and the first host published wins.
    /// </remarks>
    private static ISuspensionHost? _suspensionHost;

    /// <summary>
    /// Gets the suspension host for application lifecycle management.
    /// Provides events for process lifetime events, especially on mobile devices.
    /// Auto-initializes with default SuspensionHost if not configured via builder.
    /// </summary>
    public static ISuspensionHost SuspensionHost =>
        Volatile.Read(ref _suspensionHost) ?? InitializeDefaultSuspensionHost();

    /// <summary>Initializes the suspension host with a custom instance. Called by ReactiveUIBuilder.</summary>
    /// <param name="suspensionHost">The custom suspension host to use.</param>
    /// <exception cref="ArgumentNullException"><paramref name="suspensionHost"/> is null.</exception>
    internal static void InitializeSuspensionHost(ISuspensionHost suspensionHost)
    {
        ArgumentExceptionHelper.ThrowIfNull(suspensionHost);

        lock (_gate)
        {
            if (_suspensionHost is null)
            {
                Volatile.Write(ref _suspensionHost, suspensionHost);
            }
        }
    }

    /// <summary>Resets the suspension host state for testing purposes.</summary>
    /// <remarks>
    /// WARNING: This method should ONLY be used in unit tests to reset state between test runs.
    /// Never call this in production code as it can lead to inconsistent application state.
    /// </remarks>
    internal static void ResetForTesting()
    {
        lock (_gate)
        {
            Volatile.Write(ref _suspensionHost, null);
        }
    }

    /// <summary>Publishes a new default suspension host if no host has been published yet.</summary>
    /// <returns>The published host, which is another thread's host when that thread published first.</returns>
    private static ISuspensionHost InitializeDefaultSuspensionHost()
    {
        lock (_gate)
        {
            if (_suspensionHost is { } published)
            {
                return published;
            }

            var host = new SuspensionHost();
            Volatile.Write(ref _suspensionHost, host);
            return host;
        }
    }
}
