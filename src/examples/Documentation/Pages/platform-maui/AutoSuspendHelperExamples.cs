// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// Shows <see cref="AutoSuspendHelper"/>, which relays a MAUI <c>Application</c>'s lifecycle callbacks to
/// <see cref="RxSuspension.SuspensionHost"/> so a state driver can save and restore view models across a suspend.
/// </summary>
public static class AutoSuspendHelperExamples
{
    /// <summary>Each lifecycle method raises the matching <see cref="RxSuspension.SuspensionHost"/> signal.</summary>
    public static void LifecycleMethodsRelayToTheSuspensionHost()
    {
        List<string> events = [];
        using AutoSuspendHelper helper = new();
        using IDisposable launchSubscription = RxSuspension.SuspensionHost.IsLaunchingNew!.Subscribe(_ => events.Add("launching"));
        using IDisposable startSubscription = RxSuspension.SuspensionHost.IsUnpausing!.Subscribe(_ => events.Add("starting"));
        using IDisposable resumeSubscription = RxSuspension.SuspensionHost.IsResuming!.Subscribe(_ => events.Add("resuming"));
        using IDisposable sleepSubscription = RxSuspension.SuspensionHost.ShouldPersistState!.Subscribe(_ => events.Add("sleeping"));

        helper.OnCreate();
        helper.OnStart();
        helper.OnResume();
        helper.OnSleep();

        Console.WriteLine(string.Join(",", events));

        // Output:
        // launching,starting,resuming,sleeping
    }

    /// <summary><see cref="AutoSuspendHelper.UntimelyDemise"/> fires when the app domain reports an unhandled exception, so a driver can mark the saved state as stale.</summary>
    public static void UntimelyDemiseIsAStaticSignal()
    {
        bool fired = false;
        using IDisposable subscription = AutoSuspendHelper.UntimelyDemise.Subscribe(_ => fired = true);

        AutoSuspendHelper.UntimelyDemise.OnNext(RxVoid.Default);

        Console.WriteLine(fired);

        // Output:
        // True
    }
}
