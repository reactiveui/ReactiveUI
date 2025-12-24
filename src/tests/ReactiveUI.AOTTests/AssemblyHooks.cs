// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Core;

namespace ReactiveUI.AOTTests;

/// <summary>
/// Assembly-level hooks for diagnostics and tracking test execution.
/// </summary>
public class AssemblyHooks
{
    private static int _testCount;

    /// <summary>
    /// Called before any tests in this assembly start.
    /// </summary>
    [Before(HookType.Assembly)]
    public static void AssemblySetup()
    {
        _testCount = 0;
        Console.WriteLine($"[ASSEMBLY] ReactiveUI.AOTTests - Starting test session at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
        Console.WriteLine($"[ASSEMBLY] Process ID: {Environment.ProcessId}");
        Console.WriteLine($"[ASSEMBLY] Thread ID: {Environment.CurrentManagedThreadId}");
    }

    /// <summary>
    /// Called after all tests in this assembly complete.
    /// </summary>
    [After(HookType.Assembly)]
    public static void AssemblyTeardown()
    {
        Console.WriteLine($"[ASSEMBLY] ReactiveUI.AOTTests - Ending test session at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
        Console.WriteLine($"[ASSEMBLY] Total tests executed: {_testCount}");
        Console.WriteLine($"[ASSEMBLY] Active threads: {System.Diagnostics.Process.GetCurrentProcess().Threads.Count}");
    }

    /// <summary>
    /// Called before each test starts.
    /// </summary>
    [Before(HookType.Test)]
    public void BeforeTest()
    {
        Interlocked.Increment(ref _testCount);
        Console.WriteLine($"[TEST #{_testCount} START] at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
    }

    /// <summary>
    /// Called after each test completes.
    /// </summary>
    [After(HookType.Test)]
    public void AfterTest()
    {
        Console.WriteLine($"[TEST #{_testCount} END] at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
    }
}
