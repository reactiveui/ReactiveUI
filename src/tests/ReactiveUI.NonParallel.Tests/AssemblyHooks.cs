// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
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
    /// </summary>
    [After(HookType.Assembly)]
    public static void AssemblyTeardown()
    {
        var process = Process.GetCurrentProcess();
        Console.WriteLine($"[ASSEMBLY] ReactiveUI.NonParallel.Tests - Ending at {DateTime.Now:HH:mm:ss.fff}");
        Console.WriteLine($"[ASSEMBLY] Active threads: {process.Threads.Count}");

        // List all threads with their states
        Console.WriteLine("[ASSEMBLY] Thread details:");
        foreach (ProcessThread thread in process.Threads)
        {
            Console.WriteLine($"  Thread {thread.Id}: State={thread.ThreadState}, Priority={thread.PriorityLevel}");
        }

        // Force garbage collection to clean up any finalizable objects
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Console.WriteLine($"[ASSEMBLY] After GC - Active threads: {Process.GetCurrentProcess().Threads.Count}");
    }
}
