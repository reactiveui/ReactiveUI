// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AppKit;
using Foundation;

namespace IntegrationTests.Mac
{
    /// <summary>
    /// The main application delegate.
    /// </summary>
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        /// <inheritdoc />
        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
        }

        /// <inheritdoc />
        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

        /// <inheritdoc />
        [Export("applicationShouldTerminateAfterLastWindowClosed:")]
        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }
    }
}
