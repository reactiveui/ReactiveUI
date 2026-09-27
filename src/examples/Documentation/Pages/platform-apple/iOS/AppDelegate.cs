// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;
using ReactiveUI.Builder;
using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The app's UIApplicationDelegate: builds ReactiveUI, wires up state suspension, and shows the shell.</summary>
[Register(nameof(AppDelegate))]
[System.Diagnostics.DebuggerDisplay("AppDelegate")]
public sealed class AppDelegate : UIApplicationDelegate
{
    /// <summary>Keeps UIKit lifecycle callbacks translated into suspend/resume signals for as long as the process lives.</summary>
    private AutoSuspendHelper<AppDelegate>? _autoSuspendHelper;

    /// <inheritdoc/>
    public override UIWindow? Window { get; set; }

    /// <inheritdoc/>
    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
    {
        ReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder();
        _ = builder.WithPlatformModule<PlatformRegistrations>().BuildApp();

        _autoSuspendHelper = new AutoSuspendHelper<AppDelegate>(this);
        _autoSuspendHelper.FinishedLaunching(application, launchOptions!);
        Console.WriteLine($"FinishedLaunching captured {_autoSuspendHelper.LaunchOptions?.Count ?? 0} launch option(s).");

        RxSuspension.SuspensionHost.CreateNewAppState = static () => new LibraryAppState();

        AppSupportJsonSuspensionDriver driver = new("Library");
        RxSuspension.SuspensionHost.SetupDefaultSuspendResume(driver);

        _ = driver.SaveState(new LibraryAppState { LastViewedBook = "Pride and Prejudice" }, LibraryAppStateJsonContext.Default.LibraryAppState)
            .Subscribe(static _ => Console.WriteLine("AppSupportJsonSuspensionDriver saved the app state."));
        _ = driver.LoadState(LibraryAppStateJsonContext.Default.LibraryAppState)
            .Subscribe(
                static state => Console.WriteLine($"AppSupportJsonSuspensionDriver loaded state for {state?.LastViewedBook}."),
                static error => Console.WriteLine($"AppSupportJsonSuspensionDriver had nothing to load yet: {error.Message}."));
        _ = driver.InvalidateState().Subscribe(static _ => Console.WriteLine("AppSupportJsonSuspensionDriver invalidated the saved state."));

        // GetOrientation() returns the device's current rotation by name, such as "Portrait".
        PlatformOperations platformOperations = new();
        string? orientation = platformOperations.GetOrientation();
        Console.WriteLine($"Device orientation: {orientation}.");

        if (OperatingSystem.IsIOSVersionAtLeast(26))
        {
            // iOS 26 deprecates window construction outside a UIWindowScene; this sample targets 15 through 25 and
            // stays on the classic, scene-less API below that ceiling.
            throw new PlatformNotSupportedException("This sample targets iOS 15 through 25.");
        }

        UIWindow window = new(UIScreen.MainScreen.Bounds);
        window.RootViewController = LibraryComposition.CreateRootViewController();
        window.MakeKeyAndVisible();
        Window = window;

        return true;
    }

    /// <inheritdoc/>
    public override void OnActivated(UIApplication application) =>
        _autoSuspendHelper?.OnActivated(application);

    /// <inheritdoc/>
    public override void DidEnterBackground(UIApplication application) =>
        _autoSuspendHelper?.DidEnterBackground(application);

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _autoSuspendHelper?.Dispose();
        }

        base.Dispose(disposing);
    }
}
