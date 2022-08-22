// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AppKit;

namespace IntegrationTests.Mac
{
    /// <summary>
    /// The class which hosts the main entry point for the application.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class MainClass
    {
        /// <summary>
        /// Executes the application.
        /// </summary>
        /// <param name="args">Arguments passed to the appliation on the command line.</param>
        public static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.Main(args);
        }
    }
}
