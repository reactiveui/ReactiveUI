// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Documentation.Registration;

/// <summary>
/// Shows how a module registers a view for its view model directly on a Splat resolver, without the reflection
/// that <see cref="ViewLocator"/>'s source generator replaces on most pages of this handbook.
/// </summary>
public static class ViewRegistrationExamples
{
    /// <summary>
    /// <c>RegisterViewForViewModel</c> registers a new view every time one is asked for; a contract picks a second
    /// view of the same view model, such as the compact "now playing" strip.
    /// </summary>
    public static void RegisterAViewForAViewModel()
    {
        using ModernDependencyResolver resolver = new();
        resolver.RegisterViewForViewModel<PlaylistView, PlaylistViewModel>();
        resolver.RegisterViewForViewModel<CompactPlaylistView, PlaylistViewModel>("compact");

        Console.WriteLine(resolver.GetService<IViewFor<PlaylistViewModel>>()?.GetType().Name);
        Console.WriteLine(resolver.GetService<IViewFor<PlaylistViewModel>>("compact")?.GetType().Name);

        // Output:
        // PlaylistView
        // CompactPlaylistView
    }

    /// <summary>
    /// <c>RegisterSingletonViewForViewModel</c> builds the view once and hands back that same instance every time; a
    /// contract works the same way it does for <c>RegisterViewForViewModel</c>.
    /// </summary>
    public static void RegisterASingletonViewForAViewModel()
    {
        using ModernDependencyResolver resolver = new();
        resolver.RegisterSingletonViewForViewModel<SettingsView, SettingsViewModel>();
        resolver.RegisterSingletonViewForViewModel<PrintSettingsView, SettingsViewModel>("print");

        IViewFor<SettingsViewModel>? first = resolver.GetService<IViewFor<SettingsViewModel>>();
        IViewFor<SettingsViewModel>? second = resolver.GetService<IViewFor<SettingsViewModel>>();
        IViewFor<SettingsViewModel>? printView = resolver.GetService<IViewFor<SettingsViewModel>>("print");

        Console.WriteLine(ReferenceEquals(first, second));
        Console.WriteLine(printView?.GetType().Name);

        // Output:
        // True
        // PrintSettingsView
    }
}
