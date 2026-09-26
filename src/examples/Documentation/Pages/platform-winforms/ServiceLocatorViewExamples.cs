// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Winforms;
using Splat;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// Shows the two ways to host a view registered only with the service locator: the Unsafe twin of each host, or a
/// <c>MapFromServiceLocator</c> entry that the default hosts read.
/// </summary>
public static class ServiceLocatorViewExamples
{
    /// <summary><see cref="ViewModelControlHostUnsafe"/> also asks the service locator, so it finds a view the default host misses.</summary>
    public static void ViewModelControlHostUnsafeFindsTheView()
    {
        AppLocator.CurrentMutable.Register<IViewFor<ReadingChallengeViewModel>>(static () => new ReadingChallengeView());
        ReadingChallengeViewModel challenge = new(new LibraryShellViewModel(), "Summer reading: 10 books");

        using ViewModelControlHost host = new();
        using ViewModelControlHostUnsafe unsafeHost = new();
        host.ViewModel = challenge;
        unsafeHost.ViewModel = challenge;

        Console.WriteLine(host.CurrentView?.GetType().Name ?? "(no view)");
        Console.WriteLine(unsafeHost.CurrentView?.GetType().Name ?? "(no view)");

        unsafeHost.CurrentView?.Dispose();

        // Output:
        // (no view)
        // ReadingChallengeView
    }

    /// <summary><see cref="RoutedControlHostUnsafe"/> shows the view for a routed view model registered only with the service locator.</summary>
    public static void RoutedControlHostUnsafeFindsTheView()
    {
        AppLocator.CurrentMutable.Register<IViewFor<ReadingChallengeViewModel>>(static () => new ReadingChallengeView());
        RoutingState router = new(Sequencer.Immediate);

        using RoutedControlHostUnsafe host = new() { Router = router };
        using IDisposable navigation = router.Navigate.Execute(new ReadingChallengeViewModel(new LibraryShellViewModel(), "Winter reading: 5 books")).Subscribe();

        Console.WriteLine(host.Controls[0].GetType().Name);

        // Output:
        // ReadingChallengeView
    }

    /// <summary>
    /// <c>MapFromServiceLocator</c> adds the service locator's view to the view locator's <c>Map</c> entries, so the
    /// default <see cref="ViewModelControlHost"/> finds it and the app stays safe to compile ahead of time.
    /// </summary>
    public static void MapFromServiceLocatorLetsTheDefaultHostFindTheView()
    {
        AppLocator.CurrentMutable.Register<IViewFor<ReadingChallengeViewModel>>(static () => new ReadingChallengeView());
        DefaultViewLocator locator = new();
        _ = locator.CreateMappingBuilder().MapFromServiceLocator<ReadingChallengeViewModel, IViewFor<ReadingChallengeViewModel>>();

        using ViewModelControlHost host = new() { ViewLocator = locator };
        host.ViewModel = new ReadingChallengeViewModel(new LibraryShellViewModel(), "Summer reading: 10 books");

        Console.WriteLine(host.CurrentView?.GetType().Name ?? "(no view)");

        host.CurrentView?.Dispose();

        // Output:
        // ReadingChallengeView
    }
}
