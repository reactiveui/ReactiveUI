// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;
using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>Owns the library window and forwards scene lifecycle events to state suspension.</summary>
[Register(nameof(SceneDelegate))]
[System.Diagnostics.DebuggerDisplay("SceneDelegate")]
public sealed class SceneDelegate : UIWindowSceneDelegate
{
    /// <inheritdoc/>
    public override UIWindow? Window { get; set; }

    /// <inheritdoc/>
    public override void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
    {
        if (scene is not UIWindowScene windowScene)
        {
            return;
        }

        Window = new(windowScene)
        {
            RootViewController = LibraryComposition.CreateRootViewController(),
        };
        Window.MakeKeyAndVisible();
    }

    /// <inheritdoc/>
    public override void DidBecomeActive(UIScene scene)
    {
        UIApplication application = UIApplication.SharedApplication;
        if (application.Delegate is AppDelegate appDelegate)
        {
            appDelegate.OnActivated(application);
        }
    }

    /// <inheritdoc/>
    public override void DidEnterBackground(UIScene scene)
    {
        UIApplication application = UIApplication.SharedApplication;
        if (application.Delegate is AppDelegate appDelegate)
        {
            appDelegate.DidEnterBackground(application);
        }
    }
}
