// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.WpfApp.Services;

/// <summary>
/// Cross-process instance counter. Used to determine when the last instance closes.
/// </summary>
public sealed class AppLifetimeCoordinator : IDisposable
{
    /// <summary>
    /// The name of the shared memory-mapped file that holds the running instance count.
    /// </summary>
    private const string MapName = "ReactiveUI.Builder.WpfApp.InstanceCounter";

    /// <summary>
    /// The name of the system-wide mutex used to serialize updates to the instance count.
    /// </summary>
    private const string MutexName = "ReactiveUI.Builder.WpfApp.InstanceMutex";

    /// <summary>
    /// The size, in bytes, of the shared counter (a single 32-bit integer).
    /// </summary>
    private const int CounterByteSize = 4;

    /// <summary>
    /// The maximum time to wait when acquiring the mutex before giving up.
    /// </summary>
    private static readonly TimeSpan LockTimeout = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// The shared memory-mapped file backing the cross-process instance counter.
    /// </summary>
    private readonly MemoryMappedFile _mmf;

    /// <summary>
    /// The named mutex guarding read/modify/write access to the counter.
    /// </summary>
    private readonly Mutex _mutex;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppLifetimeCoordinator"/> class.
    /// </summary>
    public AppLifetimeCoordinator()
    {
        _mutex = new(false, MutexName, out _);

        try
        {
            _mmf = MemoryMappedFile.CreateOrOpen(MapName, CounterByteSize);
        }
        catch
        {
            // Fallback: create a per-user mapping name if needed
            _mmf = MemoryMappedFile.CreateOrOpen(MapName + "." + Environment.UserName, CounterByteSize);
        }
    }

    /// <summary>
    /// Increments the instance count.
    /// </summary>
    /// <returns>The new count after incrementing.</returns>
    public int Increment() => UpdateCount(static c => c + 1);

    /// <summary>
    /// Decrements the instance count.
    /// </summary>
    /// <returns>The new count after decrementing (0 means last instance is closing).</returns>
    public int Decrement() => UpdateCount(static c => Math.Max(0, c - 1));

    /// <inheritdoc />
    public void Dispose()
    {
        _mutex.Dispose();
        _mmf.Dispose();
    }

    /// <summary>
    /// Atomically reads the current instance count, applies the supplied transformation, and writes it back
    /// while holding the cross-process mutex.
    /// </summary>
    /// <param name="updater">A function that maps the current count to the new count.</param>
    /// <returns>The updated instance count.</returns>
    private int UpdateCount(Func<int, int> updater)
    {
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
                catch
                {
                    // ignore
                }
            }
        }
    }
}
