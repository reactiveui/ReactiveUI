// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.GitHub;

/// <summary>The part of the GitHub REST API the repository search screen calls.</summary>
public interface IGitHubApi
{
    /// <summary>Searches public repositories by name and description.</summary>
    /// <param name="query">The text to search for.</param>
    /// <param name="cancellationToken">A token that cancels the request.</param>
    /// <returns>The matching repositories, most starred first.</returns>
    /// <exception cref="GitHubApiException">The API refused the request.</exception>
    Task<IReadOnlyList<Repository>> SearchRepositoriesAsync(string query, CancellationToken cancellationToken);
}
