// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive.Concurrency;

/// <summary>
/// **** THIS FILE IS EXCLUDED FROM WINFORM AND WPF BUILDS ****
///
/// PlatformRegistrations for the WinForms and WPF platforms are done
/// in their own separate class. One per platform. As these platforms
/// have been split out into their own NuGet package. You can find
/// their implementation within the project of the platform.
/// 
/// Here we are defining the PlatformRegistrations when
/// running ReactiveUI for all other NET45 platforms except
/// WINFORMS and WPF. ie. unit test runners, mono and aspnet.
///
/// **** THIS FILE IS EXCLUDED FROM WINFORM AND WPF BUILDS ****
/// </summary>
namespace ReactiveUI
{
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {

            registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));

            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;

            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => DispatcherScheduler.Current);
        }
    }
}