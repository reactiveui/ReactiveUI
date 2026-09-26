// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// Shows <c>ReactiveUI.Maui.Registrations</c>, the module <c>WithMaui</c> loads. It registers MAUI's
/// activation fetcher and its two <see cref="IBindingTypeConverter"/>s into an <see cref="IRegistrar"/> that forwards
/// to a fresh Splat resolver, so this example never touches the app's real registrations. <c>ReactiveUI.Maui</c> and
/// plain <c>ReactiveUI</c> each have a type named <c>Registrations</c>, so the platform module keeps its full name here.
/// </summary>
public static class RegistrationsExamples
{
    /// <summary>Registering the module fills a resolver with the two services MAUI binding needs.</summary>
    public static void RegisterPlatformServices()
    {
        ModernDependencyResolver resolver = new();
        DependencyResolverRegistrar registrar = new(resolver);

        new ReactiveUI.Maui.Registrations().Register(registrar);

        Console.WriteLine(resolver.GetService<IActivationForViewFetcher>()?.GetType().FullName);
        Console.WriteLine(resolver.GetServices<IBindingTypeConverter>().Count());

        // Output:
        // ReactiveUI.Maui.ActivationForViewFetcher
        // 2
    }
}
