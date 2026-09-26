// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>Shows what powers <c>WhenActivated</c> on a WinUI view: <see cref="WinUI.Registrations"/> registers <see cref="WinUI.ActivationForViewFetcher"/>.</summary>
public static class ActivationForViewFetcherExamples
{
    /// <summary>Asks the fetcher directly, the way a view host asks it internally before subscribing to a view's activation.</summary>
    /// <param name="view">A live view to ask about.</param>
    public static void DescribeActivation(StationListPageView view)
    {
        // Qualified with the WinUI. namespace prefix on purpose: ReactiveUI.Registrations exists as an unrelated
        // core type, and this code's own namespace nests under ReactiveUI, so an unqualified "ActivationForViewFetcher"
        // would still compile if a same-named core type ever existed, silently binding to the wrong one.
        WinUI.ActivationForViewFetcher fetcher = new();

        int affinity = fetcher.GetAffinityForView(view.GetType());
        Console.WriteLine($"Affinity for {nameof(StationListPageView)}: {affinity}");

        using IDisposable subscription = fetcher.GetActivationForView(view)
            .Subscribe(static isActive => Console.WriteLine($"Activated: {isActive}"));
    }
}
