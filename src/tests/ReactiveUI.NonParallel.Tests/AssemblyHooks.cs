// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using ReactiveUI.Tests.Infrastructure;
using TUnit.Core;

namespace ReactiveUI.Tests.Core;

/// <summary>
/// Assembly-level hooks for diagnostics and tracking test execution.
/// </summary>
public static class AssemblyHooks
{
    /// <summary>
    /// Called before any tests in this assembly start.
    /// </summary>
    [Before(HookType.Assembly)]
    public static void AssemblySetup()
    {
        Console.WriteLine($"[ASSEMBLY] ReactiveUI.NonParallel.Tests - Starting at {DateTime.Now:HH:mm:ss.fff}");
        Console.WriteLine($"[ASSEMBLY] Process ID: {Environment.ProcessId}, Threads: {Process.GetCurrentProcess().Threads.Count}");
    }

    /// <summary>
    /// Called after all tests in this assembly complete.
    /// THIS IS THE LAST TEST ASSEMBLY (on Windows) - if process doesn't exit after this, we have a thread leak.
    /// </summary>
    [After(HookType.Assembly)]
    public static void AssemblyTeardown()
    {
        var process = Process.GetCurrentProcess();
        Console.WriteLine($"[ASSEMBLY] ReactiveUI.NonParallel.Tests - FINAL ASSEMBLY ENDING at {DateTime.Now:HH:mm:ss.fff}");
        Console.WriteLine($"[ASSEMBLY] Active threads BEFORE cleanup: {process.Threads.Count}");

        // List all threads with their states
        Console.WriteLine("[ASSEMBLY] Thread details:");
        foreach (ProcessThread thread in process.Threads)
        {
#if WINDOWS
            Console.WriteLine($"  Thread {thread.Id}: State={thread.ThreadState}, Priority={thread.PriorityLevel}, StartTime={thread.StartTime:HH:mm:ss}");
#else
            // PriorityLevel and StartTime are not supported on macOS/Linux
            Console.WriteLine($"  Thread {thread.Id}: State={thread.ThreadState}");
#endif
        }

        // Force garbage collection to clean up any finalizable objects
        Console.WriteLine("[ASSEMBLY] Running GC.Collect()...");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Console.WriteLine($"[ASSEMBLY] Active threads AFTER GC: {Process.GetCurrentProcess().Threads.Count}");

        // Give thread pool a chance to shut down
        Console.WriteLine("[ASSEMBLY] Waiting for ThreadPool to drain...");
        Thread.Sleep(1000);

        Console.WriteLine($"[ASSEMBLY] Active threads AFTER wait: {Process.GetCurrentProcess().Threads.Count}");

        // Show tracked managed threads with their names
        var trackedThreads = ManagedThreadTracker.GetSnapshot();
        if (trackedThreads.Count > 0)
        {
            Console.WriteLine($"[ASSEMBLY] Tracked managed threads ({trackedThreads.Count}):");
            foreach (var thread in trackedThreads)
            {
                Console.WriteLine($"  ManagedThreadId {thread.Key}: Name='{thread.Value.Name}', IsBackground={thread.Value.IsBackground}");
            }
        }

        // Last resort: Check for any known test threads that should be background threads
        // This helps diagnose which threads are preventing exit
        // Arbitrary threshold - normal process has ~8-10 system threads
        var finalThreadCount = Process.GetCurrentProcess().Threads.Count;
        if (finalThreadCount > 10)
        {
            Console.WriteLine($"[ASSEMBLY] WARNING: {finalThreadCount} threads still active!");
            Console.WriteLine("[ASSEMBLY] Suspected test fixture cleanup issue - check WpfActiveContentFixture disposal");
        }

        Console.WriteLine("[ASSEMBLY] If process doesn't exit now, we have a foreground thread or resource leak!");
    }
}
