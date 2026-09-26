// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Documentation.Registration;

/// <summary>
/// Shows the three ways an <see cref="IRegistrar"/> can hand back a service, through a
/// <see cref="DependencyResolverRegistrar"/> that forwards every call to a Splat resolver. Each example builds its
/// own <see cref="ModernDependencyResolver"/>, so the registrations in one example never leak into another.
/// </summary>
public static class RegistrarExamples
{
    /// <summary><c>Register</c> calls its factory again every time the service is asked for, so each instance differs.</summary>
    public static void RegisterMakesANewInstanceEveryTime()
    {
        using ModernDependencyResolver resolver = new();
        DependencyResolverRegistrar registrar = new(resolver);
        registrar.Register<IPlaybackEngine>(static () => new PlaybackEngine());

        IPlaybackEngine? first = resolver.GetService<IPlaybackEngine>();
        IPlaybackEngine? second = resolver.GetService<IPlaybackEngine>();

        Console.WriteLine(first?.InstanceId != second?.InstanceId);

        // Output:
        // True
    }

    /// <summary><c>RegisterConstant</c> builds the service once and hands back that same instance every time.</summary>
    public static void RegisterConstantSharesOneInstance()
    {
        using ModernDependencyResolver resolver = new();
        DependencyResolverRegistrar registrar = new(resolver);
        registrar.RegisterConstant<ISettingsStore>(static () => new SettingsStore());

        ISettingsStore? first = resolver.GetService<ISettingsStore>();
        ISettingsStore? second = resolver.GetService<ISettingsStore>();

        Console.WriteLine(ReferenceEquals(first, second));

        // Output:
        // True
    }

    /// <summary><c>RegisterLazySingleton</c> shares one instance too, but waits for the first request before building it.</summary>
    public static void RegisterLazySingletonWaitsForTheFirstRequest()
    {
        using ModernDependencyResolver resolver = new();
        DependencyResolverRegistrar registrar = new(resolver);
        int timesBuilt = 0;
        registrar.RegisterLazySingleton<ITrackLibrary>(() =>
        {
            timesBuilt++;
            return new TrackLibrary();
        });

        Console.WriteLine(timesBuilt);

        _ = resolver.GetService<ITrackLibrary>();
        _ = resolver.GetService<ITrackLibrary>();

        Console.WriteLine(timesBuilt);

        // Output:
        // 0
        // 1
    }

    /// <summary>
    /// A contract picks out one of several registrations of the same service type. <c>Register</c>,
    /// <c>RegisterConstant</c> and <c>RegisterLazySingleton</c> all take an optional contract for this.
    /// </summary>
    public static void RegisterWithAContract()
    {
        using ModernDependencyResolver resolver = new();
        DependencyResolverRegistrar registrar = new(resolver);
        registrar.Register<IPlaybackEngine>(static () => new PlaybackEngine(), "preview");
        registrar.RegisterConstant<IPlatformOperations>(static () => new MobileOrientationOperations(), "mobile");
        registrar.RegisterConstant<IPlatformOperations>(static () => new DesktopOrientationOperations(), "desktop");

        Console.WriteLine(resolver.GetService<IPlaybackEngine>("preview") is not null);
        Console.WriteLine(resolver.GetService<IPlatformOperations>("mobile")?.GetOrientation());
        Console.WriteLine(resolver.GetService<IPlatformOperations>("desktop") is DesktopOrientationOperations);

        // Output:
        // True
        // Portrait
        // True
    }
}
