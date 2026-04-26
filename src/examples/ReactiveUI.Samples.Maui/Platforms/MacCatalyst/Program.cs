// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace ReactiveUI.Samples.Maui;

/// <summary>
/// Mac Catalyst application entry point.
/// </summary>
public static class Program
{
    /// <summary>
    /// Starts the Mac Catalyst application.
    /// </summary>
    /// <param name="args">The application arguments.</param>
    public static void Main(string[] args) => UIApplication.Main(args, null, typeof(AppDelegate));
}
