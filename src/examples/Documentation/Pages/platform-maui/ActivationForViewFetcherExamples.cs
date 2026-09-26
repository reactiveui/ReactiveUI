// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;
using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// Shows <see cref="ActivationForViewFetcher"/>, which turns a MAUI <c>Page</c>, <c>View</c> or <c>Cell</c>'s own
/// appearing, loaded and unloaded events into the activation signal <c>WhenActivated</c> relies on.
/// <see cref="Registrations"/> already registers this fetcher for every view in the app; this example builds one
/// directly to show its own two members.
/// </summary>
public static class ActivationForViewFetcherExamples
{
    /// <summary>A <c>Page</c>, <c>View</c> or <c>Cell</c> gets the strongest affinity; an unrelated type gets none.</summary>
    public static void RankViewTypes()
    {
        ActivationForViewFetcher fetcher = new();

        Console.WriteLine(fetcher.GetAffinityForView(typeof(RecipeListPage)));
        Console.WriteLine(fetcher.GetAffinityForView(typeof(RecipeSummaryView)));
        Console.WriteLine(fetcher.GetAffinityForView(typeof(Label)));
        Console.WriteLine(fetcher.GetAffinityForView(typeof(string)));

        // Output:
        // 10
        // 10
        // 10
        // 0
    }

    /// <summary>
    /// <c>GetActivationForView</c> raises <see langword="true"/> when the page appears and <see langword="false"/> when
    /// it disappears; a running app raises these, so this example only shows the subscription staying open with no
    /// running app to raise them.
    /// </summary>
    public static void ObservingActivationNeedsARunningApp()
    {
        ActivationForViewFetcher fetcher = new();
        RecipeListPage page = new();
        List<bool> states = [];

        using IDisposable subscription = fetcher.GetActivationForView(page).Subscribe(states.Add);

        Console.WriteLine(states.Count);

        // Output:
        // 0
    }
}
