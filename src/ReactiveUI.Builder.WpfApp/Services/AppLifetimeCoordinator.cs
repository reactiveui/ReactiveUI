// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO.MemoryMappedFiles;
using System.Threading;

namespace ReactiveUI.Builder.WpfApp.Services;

/// <summary>
/// Cross-process instance counter. Used to determine when the last instance closes.
/// </summary>
public sealed class AppLifetimeCoordinator : IDisposable
{
    private const string MapName = "ReactiveUI.Builder.WpfApp.InstanceCounter";
    private const string MutexName = "ReactiveUI.Builder.WpfApp.InstanceMutex";

    private static readonly TimeSpan LockTimeout = TimeSpan.FromMilliseconds(500);

    private readonly MemoryMappedFile _mmf;
    private readonly Mutex _mutex;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppLifetimeCoordinator"/> class.
    /// </summary>
    public AppLifetimeCoordinator()
    {
        _mutex = new Mutex(false, MutexName, out var _);

        try
        {
            _mmf = MemoryMappedFile.CreateOrOpen(MapName, capacity: 4);
        }
        catch
        {
            // Fallback: create a per-user mapping name if needed
            _mmf = MemoryMappedFile.CreateOrOpen(MapName + "." + Environment.UserName, capacity: 4);
        }
    }

    /// <summary>
    /// Increments the instance count.
    /// </summary>
    /// <returns>The new count after incrementing.</returns>
    public int Increment()
    {
        return UpdateCount(static c => c + 1);
    }

    /// <summary>
    /// Decrements the instance count.
    /// </summary>
    /// <returns>The new count after decrementing (0 means last instance is closing).</returns>
    public int Decrement()
    {
        return UpdateCount(static c => Math.Max(0, c - 1));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _mutex.Dispose();
        _mmf.Dispose();
    }

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
            var current = 0;
            view.Read(0, out current);
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
