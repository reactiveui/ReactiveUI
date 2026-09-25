// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.GitHub;

/// <summary>The error the GitHub API returns, such as when the caller has run out of requests.</summary>
public sealed class GitHubApiException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="GitHubApiException"/> class.</summary>
    public GitHubApiException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="GitHubApiException"/> class.</summary>
    /// <param name="message">The message the API returned.</param>
    public GitHubApiException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="GitHubApiException"/> class.</summary>
    /// <param name="message">The message the API returned.</param>
    /// <param name="innerException">The error that caused this one.</param>
    public GitHubApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
