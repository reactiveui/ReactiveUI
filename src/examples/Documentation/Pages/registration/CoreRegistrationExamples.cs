// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Documentation.Registration;

/// <summary>
/// <see cref="Registrations"/> and <see cref="PlatformRegistrations"/> are the same <see cref="IWantsToRegisterStuff"/>
/// modules that <c>WithCoreServices</c> and <c>WithPlatformServices</c> run for you when an app starts. They are shown
/// here, against a resolver of their own, so you can see what they add without starting a whole app.
/// </summary>
public static class CoreRegistrationExamples
{
    /// <summary><c>Registrations</c> adds the default view locator and activation fetcher that every ReactiveUI app needs.</summary>
    public static void SeeWhatRegistrationsAdds()
    {
        using ModernDependencyResolver resolver = new();
        DependencyResolverRegistrar registrar = new(resolver);
        Registrations coreRegistrations = new();

        coreRegistrations.Register(registrar);

        Console.WriteLine(resolver.GetService<IViewLocator>()?.GetType().Name);
        Console.WriteLine(resolver.GetService<IActivationForViewFetcher>()?.GetType().Name);

        // Output:
        // DefaultViewLocator
        // CanActivateViewFetcher
    }

    /// <summary>
    /// <c>PlatformRegistrations</c> sets the schedulers <see cref="RxSchedulers"/> exposes. A module that runs this
    /// itself, outside of app start-up, restores the previous scheduler afterwards so it does not change behaviour
    /// for the rest of the process.
    /// </summary>
    public static void SeeWhatPlatformRegistrationsAdds()
    {
        ISequencer previousMainThreadScheduler = RxSchedulers.MainThreadScheduler;
        using ModernDependencyResolver resolver = new();
        DependencyResolverRegistrar registrar = new(resolver);
        PlatformRegistrations platformRegistrations = new();

        platformRegistrations.Register(registrar);

        Console.WriteLine(RxSchedulers.MainThreadScheduler.GetType().Name);
        Console.WriteLine(RxSchedulers.TaskpoolScheduler.GetType().Name);

        RxSchedulers.MainThreadScheduler = previousMainThreadScheduler;

        // Output:
        // TaskPoolSequencer
        // TaskPoolSequencer
    }
}
