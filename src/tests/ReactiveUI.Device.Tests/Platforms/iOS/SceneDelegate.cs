// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;
using UIKit;

namespace ReactiveUI.Device.Tests;

/// <summary>Creates the app's window when UIKit connects the scene, then starts the test run.</summary>
[Register(nameof(SceneDelegate))]
public sealed class SceneDelegate : UIResponder, IUIWindowSceneDelegate
{
    /// <summary>Gets or sets the scene's window.</summary>
    [Export("window")]
    public UIWindow? Window { get; set; }

    /// <summary>Creates the window with an empty root controller and starts the tests.</summary>
    /// <param name="scene">The connecting scene.</param>
    /// <param name="session">The scene session.</param>
    /// <param name="connectionOptions">The connection options.</param>
    [Export("scene:willConnectToSession:options:")]
    public void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
    {
        if (scene is not UIWindowScene windowScene)
        {
            return;
        }

        Window = new(windowScene) { RootViewController = new() };
        Window.MakeKeyAndVisible();
        IosTestHost.Start(Window);
    }
}
