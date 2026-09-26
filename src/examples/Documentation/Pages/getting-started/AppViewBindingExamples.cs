// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.GitHub;

namespace ReactiveUI.Documentation.GettingStarted;

/// <summary>
/// Shows the compelling example's view bindings: a two-way <c>Bind</c> for the search box, and <c>OneWayBind</c> for the
/// results list and the availability label.
/// </summary>
public static class AppViewBindingExamples
{
    /// <summary>Typing into the search box flows to the view model, and the results and availability flow back to the view.</summary>
    /// <returns>A task that completes once the search has answered.</returns>
    public static async Task BindTheSearchBoxAndResultsList()
    {
        InMemoryGitHubApi api = new();
        using AppViewModel viewModel = new(api, TimeSpan.FromMilliseconds(20));
        AppView view = new() { ViewModel = viewModel };

        using IReactiveBinding<AppView, BindingChange> searchBinding = view.Bind(viewModel, x => x.SearchTerm, v => v.SearchBox.Text);
        using IReactiveBinding<AppView, IReadOnlyList<RepositoryDetailsViewModel>> resultsBinding =
            view.OneWayBind(viewModel, x => x.SearchResults, v => v.ResultList.Items);
        using IReactiveBinding<AppView, bool> availableBinding =
            view.OneWayBind(viewModel, x => x.IsAvailable, v => v.AvailableLabel.IsVisible);

        view.SearchBox.Text = "akavache";
        Console.WriteLine(viewModel.SearchTerm);

        await Task.Delay(100);

        Console.WriteLine(view.ResultList.Items.Count);
        Console.WriteLine(view.AvailableLabel.IsVisible);

        // Output:
        // akavache
        // 1
        // True
    }
}
