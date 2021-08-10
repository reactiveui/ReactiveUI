// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace IntegrationTests.XamarinForms.iOS
{
    /// <summary>
    /// The class which hosts the main entry point to the application.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class AppLaunch
    {
        /// <summary>
        /// The main entry point to the application.
        /// </summary>
        /// <param name="args">Arguments that are passed to the application from the command line.</param>
        internal static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
