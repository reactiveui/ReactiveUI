// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.GitHub;

namespace ReactiveUI.Documentation.GettingStarted;

/// <summary>Wraps a <see cref="Repository"/> the search returns, and can open its page with <see cref="OpenPage"/>.</summary>
[System.Diagnostics.DebuggerDisplay("{FullName}")]
public sealed class RepositoryDetailsViewModel : ReactiveObject, IDisposable
{
    /// <summary>Initializes a new instance of the <see cref="RepositoryDetailsViewModel"/> class.</summary>
    /// <param name="repository">The repository this row shows.</param>
    public RepositoryDetailsViewModel(Repository repository)
    {
        FullName = repository.FullName;
        Description = repository.Description;
        ProjectUrl = new Uri($"https://github.com/{repository.FullName}");

        // ReactiveCommand lets us run logic without exposing the implementation to the view. We take no input and
        // return no output, so both generic parameters are RxVoid, a value that carries no data.
        OpenPage = ReactiveCommand.Create(() => Console.WriteLine($"Opening {ProjectUrl}"));
    }

    /// <summary>Gets the owner and name, such as <c>reactiveui/ReactiveUI</c>.</summary>
    public string FullName { get; }

    /// <summary>Gets the one-line description.</summary>
    public string Description { get; }

    /// <summary>Gets the repository's page.</summary>
    public Uri ProjectUrl { get; }

    /// <summary>Gets the command that opens <see cref="ProjectUrl"/>.</summary>
    public ReactiveCommand<RxVoid, RxVoid> OpenPage { get; }

    /// <inheritdoc/>
    public void Dispose() => OpenPage.Dispose();
}
