// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Device.Tests;

/// <summary>Starts ReactiveUI once for the whole run, the way an AndroidX app starts it.</summary>
public static class AndroidAssemblyHooks
{
    /// <summary>Gets the suspension helper the app registers, as an app does in its <c>Application.OnCreate</c>.</summary>
    internal static AutoSuspendHelper SuspendHelper { get; private set; } = null!;

    /// <summary>Builds ReactiveUI with the AndroidX platform module on the main thread and registers the suspension helper.</summary>
    /// <returns>A task that completes once ReactiveUI is built.</returns>
    /// <remarks>
    /// The mode detector reports a normal app rather than a unit test runner, so the platform registrations take the
    /// same path they take in a shipped app.
    /// </remarks>
    [Before(Assembly)]
    public static Task StartReactiveUI()
    {
        ModeDetector.OverrideModeDetector(AppModeDetector.Instance);
        return MainThread.RunAsync(static () =>
        {
            _ = RxAppBuilder.CreateReactiveUIBuilder().WithAndroidX().BuildApp();
            SuspendHelper = new((Application)ActivityLauncher.TargetContext.ApplicationContext!);
        });
    }

    /// <summary>Disposes the suspension helper after the last test.</summary>
    [After(Assembly)]
    public static void StopReactiveUI() => SuspendHelper?.Dispose();

    /// <summary>A mode detector that reports a normal app.</summary>
    private sealed class AppModeDetector : IModeDetector
    {
        /// <summary>Gets the shared instance.</summary>
        public static AppModeDetector Instance { get; } = new();

        /// <inheritdoc/>
        public bool? InUnitTestRunner() => false;
    }
}
