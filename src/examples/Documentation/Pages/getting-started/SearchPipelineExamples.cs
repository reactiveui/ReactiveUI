// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.GitHub;

namespace ReactiveUI.Documentation.GettingStarted;

/// <summary>Shows the compelling example's search pipeline: <c>Calm</c>, <c>Unique</c>, <c>SwitchTo</c> and <c>WitnessOn</c>.</summary>
public static class SearchPipelineExamples
{
    /// <summary><c>Calm</c> waits for a quiet period, and <c>Unique</c> then ignores a trimmed value equal to the one before it.</summary>
    /// <returns>A task that completes once both searches have had a chance to run.</returns>
    public static async Task ThrottleAndDeduplicateSearchInput()
    {
        InMemoryGitHubApi api = new();
        using AppViewModel viewModel = new(api, TimeSpan.FromMilliseconds(50));

        viewModel.SearchTerm = "r";
        viewModel.SearchTerm = "re";
        viewModel.SearchTerm = "react";
        await Task.Delay(150);
        Console.WriteLine(api.RequestCount);

        // Trimmed, this is the same text as the last search, so Unique drops it and no new search runs.
        viewModel.SearchTerm = " react ";
        await Task.Delay(150);
        Console.WriteLine(api.RequestCount);

        // Output:
        // 1
        // 1
    }

    /// <summary><c>SwitchTo</c> follows only the newest search; a slower first search never reaches <see cref="AppViewModel.SearchResults"/>.</summary>
    /// <returns>A task that completes once the newest search has answered.</returns>
    public static async Task SwitchToTheLatestSearch()
    {
        InMemoryGitHubApi api = new() { Latency = TimeSpan.FromMilliseconds(200) };
        using AppViewModel viewModel = new(api, TimeSpan.FromMilliseconds(20));

        viewModel.SearchTerm = "reactiveui";
        await Task.Delay(50);
        viewModel.SearchTerm = "refit";
        await Task.Delay(400);

        Console.WriteLine(viewModel.SearchResults.Count);
        Console.WriteLine(viewModel.SearchResults[0].FullName);

        // Output:
        // 1
        // reactiveui/refit
    }

    /// <summary><see cref="AppViewModel.IsAvailable"/> follows <see cref="AppViewModel.SearchResults"/>: empty until a search answers.</summary>
    /// <returns>A task that completes once the search has answered.</returns>
    public static async Task TrackAvailabilityFromResults()
    {
        InMemoryGitHubApi api = new();
        using AppViewModel viewModel = new(api, TimeSpan.FromMilliseconds(20));
        Console.WriteLine(viewModel.IsAvailable);

        viewModel.SearchTerm = "splat";
        await Task.Delay(100);

        Console.WriteLine(viewModel.IsAvailable);

        // Output:
        // False
        // True
    }

    /// <summary>The output property's <c>ThrownExceptions</c> carries a failed search's error to <see cref="AppViewModel.ErrorMessage"/>.</summary>
    /// <returns>A task that completes once the failed search has been reported.</returns>
    public static async Task ReportSearchErrorsWithThrownExceptions()
    {
        InMemoryGitHubApi api = new() { RemainingRequests = 0 };
        using AppViewModel viewModel = new(api, TimeSpan.FromMilliseconds(20));

        viewModel.SearchTerm = "akavache";
        await Task.Delay(100);

        Console.WriteLine(viewModel.ErrorMessage);

        // Output:
        // API rate limit exceeded.
    }
}
