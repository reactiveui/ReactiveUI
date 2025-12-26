// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using TUnit.Core;

namespace ReactiveUI.Tests;

/// <summary>
/// Assembly-level hooks for diagnostics and tracking test execution.
/// </summary>
public class AssemblyHooks
{
    /// <summary>
    /// Called before any tests in this assembly start.
    /// </summary>
    [Before(HookType.Assembly)]
    public static void AssemblySetup()
    {
        Console.WriteLine($"[ASSEMBLY] ReactiveUI.Tests - Starting at {DateTime.Now:HH:mm:ss.fff}");
        Console.WriteLine($"[ASSEMBLY] Process ID: {Environment.ProcessId}, Threads: {Process.GetCurrentProcess().Threads.Count}");
    }

    /// <summary>
    /// Called after all tests in this assembly complete.
    /// THIS IS THE LAST TEST ASSEMBLY - if process doesn't exit after this, we have a thread leak!
    /// </summary>
    [After(HookType.Assembly)]
    public static void AssemblyTeardown()
    {
        var process = Process.GetCurrentProcess();
        Console.WriteLine($"[ASSEMBLY] ReactiveUI.Tests - FINAL ASSEMBLY ENDING at {DateTime.Now:HH:mm:ss.fff}");
        Console.WriteLine($"[ASSEMBLY] Active threads BEFORE cleanup: {process.Threads.Count}");

        // List all threads with their states
        Console.WriteLine("[ASSEMBLY] Thread details:");
        foreach (ProcessThread thread in process.Threads)
        {
            Console.WriteLine($"  Thread {thread.Id}: State={thread.ThreadState}, Priority={thread.PriorityLevel}, StartTime={thread.StartTime:HH:mm:ss}");
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
        Console.WriteLine("[ASSEMBLY] If process doesn't exit now, we have a foreground thread or resource leak!");
    }
}
