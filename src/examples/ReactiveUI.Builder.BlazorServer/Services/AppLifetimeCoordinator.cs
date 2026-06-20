// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.IO.MemoryMappedFiles;

namespace ReactiveUI.Builder.BlazorServer.Services;

/// <summary>Cross-process instance counter. Used to determine when the last instance closes.</summary>
public sealed class AppLifetimeCoordinator : IDisposable
{
    /// <summary>The name of the shared memory-mapped file used to store the instance count.</summary>
    private const string MapName = "ReactiveUI.Builder.BlazorServer.InstanceCounter";

    /// <summary>The name of the named mutex used to synchronize access to the instance count.</summary>
    private const string MutexName = "ReactiveUI.Builder.BlazorServer.InstanceMutex";

    /// <summary>The size, in bytes, of the memory-mapped region holding the 32-bit instance count.</summary>
    private const int CounterSizeBytes = 4;

    /// <summary>The maximum time to wait when acquiring the mutex before proceeding.</summary>
    private static readonly TimeSpan LockTimeout = TimeSpan.FromMilliseconds(500);

    /// <summary>The memory-mapped file backing the cross-process instance count, or <see langword="null"/> when unavailable.</summary>
    private readonly MemoryMappedFile? _mmf;

    /// <summary>The named mutex guarding access to the shared instance count.</summary>
    private readonly Mutex _mutex;

    /// <summary>Initializes a new instance of the <see cref="AppLifetimeCoordinator"/> class.</summary>
    public AppLifetimeCoordinator()
    {
        _mutex = new(false, MutexName, out _);

        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        try
        {
            _mmf = MemoryMappedFile.CreateOrOpen(MapName, CounterSizeBytes);
        }
        catch
        {
            // Fallback: create a per-user mapping name if needed
            _mmf = MemoryMappedFile.CreateOrOpen(MapName + "." + Environment.UserName, CounterSizeBytes);
        }
    }

    /// <summary>Increments the instance count.</summary>
    /// <returns>The new count after incrementing.</returns>
    public int Increment() => UpdateCount(static c => c + 1);

    /// <summary>Decrements the instance count.</summary>
    /// <returns>The new count after decrementing (0 means last instance is closing).</returns>
    public int Decrement() => UpdateCount(static c => Math.Max(0, c - 1));

    /// <inheritdoc />
    public void Dispose()
    {
        _mutex.Dispose();
        _mmf?.Dispose();
    }

    /// <summary>Atomically reads, transforms, and writes the shared instance count under the mutex.</summary>
    /// <param name="updater">A function that maps the current count to the new count.</param>
    /// <returns>The updated count, or 0 when the memory-mapped file is unavailable.</returns>
    private int UpdateCount(Func<int, int> updater)
    {
        if (_mmf is null)
        {
            return 0;
        }

        var locked = false;
        try
        {
            try
            {
                locked = _mutex.WaitOne(LockTimeout);
            }
            catch (AbandonedMutexException)
            {
                // Consider it acquired in abandoned state
                locked = true;
            }

            using var view = _mmf.CreateViewAccessor(0, 4, MemoryMappedFileAccess.ReadWrite);
            view.Read(0, out int current);
            var updated = updater(current);
            view.Write(0, updated);
            view.Flush();
            return updated;
        }
        finally
        {
            if (locked)
            {
                try
                {
                    _mutex.ReleaseMutex();
                }
                catch (ApplicationException ex)
                {
                    // The mutex was not held by the current thread (e.g. acquired in an abandoned state).
                    Trace.TraceWarning($"[Lifetime] Failed to release mutex: {ex.Message}");
                }
                catch (ObjectDisposedException ex)
                {
                    // The coordinator is being disposed concurrently; nothing further to release.
                    Trace.TraceWarning($"[Lifetime] Mutex already disposed during release: {ex.Message}");
                }
            }
        }
    }
}
