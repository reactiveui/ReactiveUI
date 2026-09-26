// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Documentation.Reflection;

/// <summary>
/// Shows the two ways to scan an assembly by reflection and register every <see cref="IViewFor{T}"/> it finds. The
/// builder's own <c>RegisterView&lt;TView, TViewModel&gt;()</c> registers one view at a time without scanning
/// anything, and is the AOT-safe alternative to both.
/// </summary>
public static class RegistrationReflectionExamples
{
    /// <summary>
    /// <see cref="DependencyResolverMixins.RegisterViewsForViewModels"/> walks every type in the assembly, finds the
    /// ones that implement <see cref="IViewFor{T}"/>, and registers each against the view model type it names.
    /// </summary>
    public static void ScanAnAssemblyForViews()
    {
        using ModernDependencyResolver resolver = new();
        resolver.InitializeSplat();

        resolver.RegisterViewsForViewModels(typeof(MusicPlayerScreen).Assembly);

        IViewFor<MusicPlayerViewModel>? view = resolver.GetService<IViewFor<MusicPlayerViewModel>>();
        Console.WriteLine(view?.GetType().Name);

        // Output:
        // MusicPlayerScreen
    }

    /// <summary>
    /// <see cref="IReactiveUIBuilder.WithViewsFromAssembly"/> runs the same scan while the app starts, as part of the
    /// builder chain, instead of calling <see cref="DependencyResolverMixins.RegisterViewsForViewModels"/> directly.
    /// </summary>
    public static void RegisterViewsWhileBuildingTheApp()
    {
        using ModernDependencyResolver resolver = new();
        resolver.InitializeSplat();

        ReactiveUIBuilder builder = resolver.CreateReactiveUIBuilder();
        _ = builder.WithCoreServices().BuildApp();
        _ = builder.WithViewsFromAssembly(typeof(MusicPlayerScreen).Assembly);

        IViewFor<MusicPlayerViewModel>? view = resolver.GetService<IViewFor<MusicPlayerViewModel>>();
        Console.WriteLine(view?.GetType().Name);

        // Output:
        // MusicPlayerScreen
    }
}
