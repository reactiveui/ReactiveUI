// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Runtime;
using ReactiveUI.Builder;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>The app's <see cref="Application"/> subclass: registers ReactiveUI's AndroidX platform services and
/// wires up automatic state suspension before any activity runs.</summary>
[Application]
[System.Diagnostics.DebuggerDisplay("TimetableApplication")]
public sealed class TimetableApplication : Application
{
    /// <summary>Keeps activity lifecycle callbacks translated into suspend/resume signals for as long as the process lives.</summary>
    private AutoSuspendHelper? _autoSuspendHelper;

    /// <summary>Initializes a new instance of the <see cref="TimetableApplication"/> class.</summary>
    /// <param name="handle">The JNI handle supplied by the Android runtime.</param>
    /// <param name="ownership">The ownership of <paramref name="handle"/>.</param>
    public TimetableApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    /// <inheritdoc/>
    public override void OnCreate()
    {
        base.OnCreate();

        IReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder();
        _ = builder.WithAndroidX().BuildApp();

        TimetableLog.Info($"ReactiveUI is running on {AndroidXReactiveUIBuilderExtensions.AndroidXMainThreadScheduler.GetType().Name}.");

        // WithAndroidXScheduler() alone suits a host that already registered platform services elsewhere (for
        // example a shared multi-platform composition root) and only needs ReactiveUI's main-thread scheduler
        // pointed at AndroidX's looper, without repeating WithAndroidX()'s full platform-module registration.
        IReactiveUIBuilder schedulerOnlyBuilder = RxAppBuilder.CreateReactiveUIBuilder().WithAndroidXScheduler();
        TimetableLog.Info($"WithAndroidXScheduler configured a {schedulerOnlyBuilder.GetType().Name} to run on "
            + $"{AndroidXReactiveUIBuilderExtensions.AndroidXMainThreadScheduler.GetType().Name}.");

        _autoSuspendHelper = new AutoSuspendHelper(this);
        _ = AutoSuspendHelper.UntimelyDemise.Subscribe(static _ => TimetableLog.Info("AutoSuspendHelper reported an untimely demise."));
        RxSuspension.SuspensionHost.CreateNewAppState = static () => new TimetableAppState();

        BundleSuspensionDriver driver = new();
        RxSuspension.SuspensionHost.SetupDefaultSuspendResume(driver);

        TimetableLog.Info("TimetableApplication started; AutoSuspendHelper and BundleSuspensionDriver are wired up.");
        TimetableLog.Info($"AutoSuspendHelper's latest bundle is currently {(AutoSuspendHelper.LatestBundle is null ? "unset" : "set")}.");

        // SaveState<T>(T, JsonTypeInfo<T>) and LoadState<T>(JsonTypeInfo<T>) are the trim- and AOT-safe overloads,
        // through the source-generated TimetableAppStateJsonContext. The untyped LoadState()/SaveState<T>(T)
        // overloads use reflection-based serialization instead, which this AOT page project cannot call.
        _ = driver.SaveState(new TimetableAppState { LastViewedSubject = "Mathematics" }, TimetableAppStateJsonContext.Default.TimetableAppState)
            .Subscribe(static _ => TimetableLog.Info("BundleSuspensionDriver saved the app state."));
        _ = driver.LoadState(TimetableAppStateJsonContext.Default.TimetableAppState)
            .Subscribe(
                static state => TimetableLog.Info($"BundleSuspensionDriver loaded state for {state?.LastViewedSubject}."),
                static error => TimetableLog.Info($"BundleSuspensionDriver had nothing to load yet: {error.Message}"));
        _ = driver.InvalidateState().Subscribe(static _ => TimetableLog.Info("BundleSuspensionDriver invalidated the saved state."));
    }

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
