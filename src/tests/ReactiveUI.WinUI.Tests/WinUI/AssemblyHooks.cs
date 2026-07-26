// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

[assembly: NotInParallel]

namespace ReactiveUI.WinUI.Tests;

/// <summary>Controls the lifetime of the WinUI application used by this test assembly.</summary>
public static class AssemblyHooks
{
    /// <summary>Stops the WinUI application after all tests complete.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [After(Assembly)]
    public static Task AssemblyCleanup() => WinUITestExecutor.StopAsync();
}
