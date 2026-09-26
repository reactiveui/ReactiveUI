// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Documentation.Registration;

/// <summary>
/// Shows how a feature splits its registrations into an <see cref="IWantsToRegisterStuff"/> of its own. The playlist
/// app has one for the library, one for the player and one for settings, so each feature owns the services it needs
/// without the others knowing about them.
/// </summary>
public static class ModuleExamples
{
    /// <summary>A module's <c>Register</c> method receives the registrar and adds only the services its feature owns.</summary>
    public static void RegisterOneModule()
    {
        using ModernDependencyResolver resolver = new();
        DependencyResolverRegistrar registrar = new(resolver);
        TrackLibraryModule libraryModule = new();

        libraryModule.Register(registrar);

        ITrackLibrary? library = resolver.GetService<ITrackLibrary>();

        Console.WriteLine(library?.Titles.Count);

        // Output:
        // 3
    }

    /// <summary>Every feature module registers against the same resolver, so the app assembles from independent parts.</summary>
    public static void RegisterEveryFeatureModule()
    {
        using ModernDependencyResolver resolver = new();
        DependencyResolverRegistrar registrar = new(resolver);
        IWantsToRegisterStuff[] modules = [new TrackLibraryModule(), new PlayerModule(), new SettingsModule()];

        foreach (IWantsToRegisterStuff module in modules)
        {
            module.Register(registrar);
        }

        Console.WriteLine(resolver.GetService<ITrackLibrary>()?.Titles.Count);
        Console.WriteLine(resolver.GetService<IPlaybackEngine>() is not null);
        Console.WriteLine(resolver.GetService<ISettingsStore>()?.Volume);

        // Output:
        // 3
        // True
        // 80
    }
}
