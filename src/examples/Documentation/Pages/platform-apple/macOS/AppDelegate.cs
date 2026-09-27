// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AppKit;
using Foundation;
using ReactiveUI.Builder;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>The app's NSApplicationDelegate: builds ReactiveUI, wires up state suspension, and shows the window.</summary>
[Register(nameof(AppDelegate))]
[System.Diagnostics.DebuggerDisplay("AppDelegate")]
public sealed class AppDelegate : NSApplicationDelegate
{
    /// <summary>Keeps AppKit lifecycle notifications translated into suspend/resume signals for as long as the process lives.</summary>
    private AutoSuspendHelper<AppDelegate>? _autoSuspendHelper;

    /// <summary>Keeps the window controller alive for as long as the app runs.</summary>
    private MainWindowController? _mainWindowController;

    /// <inheritdoc/>
    public override void DidFinishLaunching(NSNotification notification)
    {
        ReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder();
        _ = builder.WithPlatformModule<PlatformRegistrations>().BuildApp();

        _autoSuspendHelper = new AutoSuspendHelper<AppDelegate>(this);
        _autoSuspendHelper.DidFinishLaunching(notification);
        RxSuspension.SuspensionHost.CreateNewAppState = static () => new LibraryAppState();

        AppSupportJsonSuspensionDriver driver = new();
        RxSuspension.SuspensionHost.SetupDefaultSuspendResume(driver);

        _ = driver.SaveState(new LibraryAppState { LastViewedBook = "Dune" }, LibraryAppStateJsonContext.Default.LibraryAppState)
            .Subscribe(static _ => Console.WriteLine("AppSupportJsonSuspensionDriver saved the app state."));
        _ = driver.LoadState(LibraryAppStateJsonContext.Default.LibraryAppState)
            .Subscribe(
                static state => Console.WriteLine($"AppSupportJsonSuspensionDriver loaded state for {state?.LastViewedBook}."),
                static error => Console.WriteLine($"AppSupportJsonSuspensionDriver had nothing to load yet: {error.Message}."));
        _ = driver.InvalidateState().Subscribe(static _ => Console.WriteLine("AppSupportJsonSuspensionDriver invalidated the saved state."));

        PlatformOperations platformOperations = new();
        string? orientation = platformOperations.GetOrientation();
        Console.WriteLine($"Device orientation reported by AppKit: {orientation ?? "(none; orientation is a UIKit concept)"}.");

        _mainWindowController = new MainWindowController();
        _mainWindowController.ShowWindow(this);
    }

    /// <inheritdoc/>
    public override void DidBecomeActive(NSNotification notification) =>
        _autoSuspendHelper?.DidBecomeActive(notification);

    /// <inheritdoc/>
    public override void DidResignActive(NSNotification notification) =>
        _autoSuspendHelper?.DidResignActive(notification);

    /// <inheritdoc/>
    public override void DidHide(NSNotification notification) =>
        _autoSuspendHelper?.DidHide(notification);

    /// <inheritdoc/>
    public override NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender) =>
        _autoSuspendHelper?.ApplicationShouldTerminate(sender) ?? NSApplicationTerminateReply.Now;

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _autoSuspendHelper?.Dispose();
            _mainWindowController?.Dispose();
        }

        base.Dispose(disposing);
    }
}
