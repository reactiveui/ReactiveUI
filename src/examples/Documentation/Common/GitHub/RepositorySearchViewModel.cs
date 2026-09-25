// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.GitHub;

/// <summary>
/// The view model behind a GitHub repository search box. Typing three or more characters runs a search; the results,
/// the busy flag and the last error are output properties the view binds to.
/// </summary>
[System.Diagnostics.DebuggerDisplay("SearchText = {SearchText}, Results = {Results.Count}")]
public sealed class RepositorySearchViewModel : ReactiveObject, IDisposable
{
    /// <summary>The shortest text that starts a search.</summary>
    private const int MinimumQueryLength = 3;

    /// <summary>The subscriptions the view model owns, disposed with it.</summary>
    private readonly MultipleDisposable _subscriptions = [];

    /// <summary>Backs <see cref="Results"/>.</summary>
    private readonly ObservableAsPropertyHelper<IReadOnlyList<Repository>> _results;

    /// <summary>Backs <see cref="IsSearching"/>.</summary>
    private readonly ObservableAsPropertyHelper<bool> _isSearching;

    /// <summary>Initializes a new instance of the <see cref="RepositorySearchViewModel"/> class.</summary>
    /// <param name="api">The GitHub API to search.</param>
    public RepositorySearchViewModel(IGitHubApi api)
    {
        Search = ReactiveCommand.CreateFromTask<string, IReadOnlyList<Repository>>(api.SearchRepositoriesAsync);

        _subscriptions.Add(this.WhenAnyValue(x => x.SearchText)
            .Select(static text => text.Trim())
            .Where(static text => text.Length >= MinimumQueryLength)
            .InvokeCommand(Search));
        _subscriptions.Add(Search.ThrownExceptions.Subscribe(ex => ErrorMessage = ex.Message));
        _subscriptions.Add(Search.Subscribe(_ => ErrorMessage = string.Empty));

        _results = Search.ToProperty(this, nameof(Results), []);
        _isSearching = Search.IsExecuting.ToProperty(this, nameof(IsSearching));
    }

    /// <summary>Gets the command that searches for a text.</summary>
    public ReactiveCommand<string, IReadOnlyList<Repository>> Search { get; }

    /// <summary>Gets or sets the text in the search box.</summary>
    public string SearchText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets the message from the last failed search, or an empty string.</summary>
    public string ErrorMessage
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets the repositories the last successful search found.</summary>
    public IReadOnlyList<Repository> Results => _results.Value;

    /// <summary>Gets a value indicating whether a search is running.</summary>
    public bool IsSearching => _isSearching.Value;

    /// <inheritdoc/>
    public void Dispose()
    {
        _subscriptions.Dispose();
        _results.Dispose();
        _isSearching.Dispose();
        Search.Dispose();
    }
}
