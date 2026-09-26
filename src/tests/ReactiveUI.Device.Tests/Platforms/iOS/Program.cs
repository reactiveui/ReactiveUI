// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Device.Tests;

/// <summary>The iOS entry point.</summary>
internal static class Program
{
    /// <summary>Starts UIKit with <see cref="AppDelegate"/>.</summary>
    /// <param name="args">The launch arguments.</param>
    private static void Main(string[] args) => UIApplication.Main(args, null, typeof(AppDelegate));
}
