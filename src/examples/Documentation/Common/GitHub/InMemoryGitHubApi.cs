// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.GitHub;

/// <summary>An <see cref="IGitHubApi"/> that answers from a fixed set of repositories, standing in for api.github.com.</summary>
[System.Diagnostics.DebuggerDisplay("Requests = {RequestCount}")]
public sealed class InMemoryGitHubApi : IGitHubApi
{
    /// <summary>The repositories the API knows about.</summary>
    private static readonly Repository[] _repositories =
    [
        new("reactiveui/ReactiveUI", "An advanced, composable, functional reactive model-view-viewmodel framework for all .NET platforms", 8400),
        new("reactiveui/ReactiveUI.Binding.SourceGenerators", "Source-generated bindings for ReactiveUI", 120),
        new("reactiveui/Akavache", "An asynchronous, persistent key-value store", 2500),
        new("reactiveui/refit", "The automatic type-safe REST library for .NET", 8900),
        new("reactiveui/splat", "Makes things cross-platform", 1000),
    ];

    /// <summary>Gets the number of searches the API has answered or refused.</summary>
    public int RequestCount { get; private set; }

    /// <summary>Gets or sets how long each request takes, as a network round trip would.</summary>
    public TimeSpan Latency { get; set; }

    /// <summary>Gets or sets the number of requests left before the API refuses with a rate-limit error.</summary>
    public int RemainingRequests { get; set; } = int.MaxValue;

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Repository>> SearchRepositoriesAsync(string query, CancellationToken cancellationToken)
    {
        RequestCount++;
        await Task.Delay(Latency, cancellationToken).ConfigureAwait(false);
        if (RemainingRequests <= 0)
        {
            throw new GitHubApiException("API rate limit exceeded.");
        }

        RemainingRequests--;

        return
        [
            .. _repositories
                .Where(repository => repository.FullName.Contains(query, StringComparison.OrdinalIgnoreCase)
                                     || repository.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(static repository => repository.Stars),
        ];
    }
}
