// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AppKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The macOS entry point.</summary>
internal static class Program
{
    /// <summary>
    /// Starts AppKit directly instead of calling <c>NSApplication.Main</c>, because this app builds its window and
    /// menu in code and has no storyboard to wire <see cref="AppDelegate"/> up as <c>NSApplication.Delegate</c>.
    /// </summary>
    /// <param name="args">The process arguments. Unused: this app takes no command-line configuration.</param>
    private static void Main(string[] args)
    {
        _ = args;
        NSApplication.Init();
        NSApplication.SharedApplication.Delegate = new AppDelegate();
        NSApplication.Main([]);
    }
}
