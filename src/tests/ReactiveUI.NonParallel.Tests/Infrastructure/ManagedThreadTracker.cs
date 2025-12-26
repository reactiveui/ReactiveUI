// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;

namespace ReactiveUI.Tests.Infrastructure;

/// <summary>
/// Tracks managed threads created during tests to help diagnose thread leaks.
/// </summary>
public static class ManagedThreadTracker
{
    private static readonly ConcurrentDictionary<int, ThreadInfo> _threads = new();

    /// <summary>
    /// Registers a thread for tracking.
    /// </summary>
    /// <param name="thread">The thread to track.</param>
    public static void Register(Thread thread)
    {
        ArgumentNullException.ThrowIfNull(thread);

        if (thread.Name != null)
        {
            _threads.TryAdd(thread.ManagedThreadId, new ThreadInfo(thread.Name, thread.IsBackground, thread.IsAlive));
        }
    }

    /// <summary>
    /// Gets a snapshot of all tracked threads and their current status.
    /// </summary>
    /// <returns>Dictionary of thread ID to thread info.</returns>
    public static Dictionary<int, ThreadInfo> GetSnapshot()
    {
        var snapshot = new Dictionary<int, ThreadInfo>();
        foreach (var kvp in _threads)
        {
            snapshot[kvp.Key] = kvp.Value;
        }

        return snapshot;
    }

    /// <summary>
    /// Clears the tracked threads collection.
    /// </summary>
    public static void Clear() => _threads.Clear();

    /// <summary>
    /// Information about a tracked thread.
    /// </summary>
    /// <param name="Name">The thread name.</param>
    /// <param name="IsBackground">Whether the thread is a background thread.</param>
    /// <param name="WasAlive">Whether the thread was alive when registered.</param>
    public record ThreadInfo(string Name, bool IsBackground, bool WasAlive);
}
