// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

#pragma warning disable SA1300 // Element should begin with upper-case letter

namespace IntegrationTests.iOS
{
    /// <summary>
    /// The main application which contains the entry point to the application.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class Application
    {
        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args">Arguments passed from the command line to the application.</param>
        public static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
