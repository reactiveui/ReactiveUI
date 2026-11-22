// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Documentation.Registration;

/// <summary>
/// Shows compile-time view registration via <c>[RegisterView]</c> and <c>RegisterGeneratedViews()</c>, replacing
/// reflection-based assembly scanning with trim- and Native AOT-safe generated code.
/// </summary>
public static class GeneratedViewRegistrationExamples
{
    /// <summary>
    /// Calls <c>RegisterGeneratedViews()</c> on a <see cref="IReactiveUIBuilder"/> backed by a fresh resolver and
    /// resolves both the default view and the contracted singleton view.
    /// </summary>
    public static void RegisterViewsWithSourceGenerator()
    {
        using ModernDependencyResolver resolver = new();
        ReactiveUIBuilder builder = new(resolver, resolver);
        _ = builder.RegisterGeneratedViews().BuildApp();

        IViewFor<GeneratedPlaylistViewModel>? defaultView = resolver.GetService<IViewFor<GeneratedPlaylistViewModel>>();
        IViewFor<GeneratedPlaylistViewModel>? firstCompact = resolver.GetService<IViewFor<GeneratedPlaylistViewModel>>("compact");
        IViewFor<GeneratedPlaylistViewModel>? secondCompact = resolver.GetService<IViewFor<GeneratedPlaylistViewModel>>("compact");

        Console.WriteLine(defaultView?.GetType().Name);
        Console.WriteLine(firstCompact?.GetType().Name);
        Console.WriteLine(ReferenceEquals(firstCompact, secondCompact));

        // Output:
        // GeneratedPlaylistView
        // GeneratedCompactPlaylistView
        // True
    }
}
