// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

#if !WINFORMS || !WPF
using System;
using System.Reactive.Concurrency;

/// <summary>
/// PlatformRegistrations for the WinForms and WPF platforms are done
/// in their own seperate class. One per platform. As these platforms
/// have been split out into their own NuGet package.
/// 
/// Here we are defining the registrations that will be used when
/// running ReactiveUI for all other NET45 platforms except
/// WINFORMS and WPF. ie. unit test runners, mono and aspnet.
/// </summary>
namespace ReactiveUI
{
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;

            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => DispatcherScheduler.Current);
        }
    }
}
#endif