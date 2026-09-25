// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.GitHub;

namespace ReactiveUI.Documentation.ViewModels;

/// <summary>Shows a search box that queries GitHub as the user types, built from <c>WhenAnyValue</c>, a command and output properties.</summary>
public static class RepositorySearchExamples
{
    /// <summary>Short text is ignored; three or more characters run a search whose results become an output property.</summary>
    /// <returns>A task that completes when the search has answered.</returns>
    public static async Task SearchAsTheUserTypes()
    {
        InMemoryGitHubApi api = new();
        using RepositorySearchViewModel viewModel = new(api);
        var answered = viewModel.Search.FirstAsync().ToTask();

        viewModel.SearchText = "pl";
        Console.WriteLine(api.RequestCount);

        viewModel.SearchText = "platform";
        await answered;

        Console.WriteLine(api.RequestCount);
        foreach (var repository in viewModel.Results)
        {
            Console.WriteLine($"{repository.FullName} ({repository.Stars})");
        }

        // Output:
        // 0
        // 1
        // reactiveui/ReactiveUI (8400)
        // reactiveui/splat (1000)
    }
}
