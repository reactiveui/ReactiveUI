// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.NUnit.UI;

namespace ReactiveUI.Tests_iOS
{
    /// <summary>
    /// The UIApplicationDelegate for the application. This class is responsible for launching the
    /// User Interface of the application, as well as listening (and optionally responding) to
    /// application events from iOS.
    /// </summary>
    [Register ("UnitTestAppDelegate")]
    public partial class UnitTestAppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        /// <summary>
        /// The application window.
        /// </summary>
        UIWindow window;

        /// <summary>
        /// The touch runner that hosts the test runner UI.
        /// </summary>
        TouchRunner runner;

        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //

        /// <summary>
        /// Called when the application has finished launching.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="options">The launch options.</param>
        /// <returns><see langword="true"/> if the launch was handled successfully; otherwise, <see langword="false"/>.</returns>
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // create a new window instance based on the screen size
            window = new UIWindow(UIScreen.MainScreen.Bounds);
            runner = new TouchRunner(window);

            // register every tests included in the main application/assembly
            runner.Add(System.Reflection.Assembly.GetExecutingAssembly());

            window.RootViewController = new UINavigationController(runner.GetViewController());
            
            // make the window visible
            window.MakeKeyAndVisible();
            
            return true;
        }
    }
}

