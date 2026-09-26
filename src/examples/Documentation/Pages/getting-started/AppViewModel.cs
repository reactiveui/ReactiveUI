// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.GitHub;

namespace ReactiveUI.Documentation.GettingStarted;

/// <summary>
/// The compelling example's search screen. Typing flows through <c>Calm</c>, a trim, <c>Unique</c> and a search, and the
/// results land in an output property the view binds to.
/// </summary>
[System.Diagnostics.DebuggerDisplay("SearchTerm = {SearchTerm}, Results = {SearchResults.Count}")]
public sealed class AppViewModel : ReactiveObject, IDisposable
{
    /// <summary>The GitHub API this screen searches.</summary>
    private readonly IGitHubApi _api;

    /// <summary>The subscriptions the view model owns, disposed with it.</summary>
    private readonly MultipleDisposable _subscriptions = [];

    /// <summary>Backs <see cref="SearchResults"/>.</summary>
    private readonly ObservableAsPropertyHelper<IReadOnlyList<RepositoryDetailsViewModel>> _searchResults;

    /// <summary>Backs <see cref="IsAvailable"/>.</summary>
    private readonly ObservableAsPropertyHelper<bool> _isAvailable;

    /// <summary>Initializes a new instance of the <see cref="AppViewModel"/> class.</summary>
    /// <param name="api">The GitHub API to search.</param>
    /// <param name="throttle">How long to wait after the last keystroke before searching.</param>
    public AppViewModel(IGitHubApi api, TimeSpan throttle)
    {
        _api = api;

        // Calm ignores changes that happen too quickly, so we don't search on every keystroke. We then trim the
        // value and use Unique to skip a value equal to the one before it, and filter out empty strings.
        // Select turns each term into Signal.FromAsync, which runs the search and hands it a CancellationToken.
        // SwitchTo follows only the newest search, cancelling one still running when the user types again, and
        // WitnessOn moves delivery back to the main thread, since Calm runs on a background thread.
        _searchResults = this.WhenAnyValue(x => x.SearchTerm)
            .Calm(throttle)
            .Select(static term => term.Trim())
            .Unique()
            .Where(static term => !string.IsNullOrWhiteSpace(term))
            .Select(term => Signal.FromAsync(token => SearchRepositoriesAsync(term, token)))
            .SwitchTo()
            .WitnessOn(RxSchedulers.MainThreadScheduler)
            .ToProperty(this, nameof(SearchResults), []);

        // We subscribe to the output property's ThrownExceptions, where any error the search throws is marshalled.
        _subscriptions.Add(_searchResults.ThrownExceptions.Subscribe(error => ErrorMessage = error.Message));

        // A helper property for a spinner or a results panel to show if results are available: the latest
        // SearchResults value is not empty.
        _isAvailable = this.WhenAnyValue(x => x.SearchResults)
            .Select(static results => results.Count > 0)
            .ToProperty(this, nameof(IsAvailable));
    }

    /// <summary>Gets or sets the text in the search box.</summary>
    public string SearchTerm
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets the repositories the last search found.</summary>
    public IReadOnlyList<RepositoryDetailsViewModel> SearchResults => _searchResults.Value;

    /// <summary>Gets a value indicating whether search results are available to show.</summary>
    public bool IsAvailable => _isAvailable.Value;

    /// <summary>Gets the message from the last failed search, or an empty string.</summary>
    public string ErrorMessage
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <inheritdoc/>
    public void Dispose()
    {
        _subscriptions.Dispose();
        _searchResults.Dispose();
        _isAvailable.Dispose();
    }

    /// <summary>Searches GitHub and wraps each match in a details view model.</summary>
    /// <param name="term">The text to search for.</param>
    /// <param name="cancellationToken">A token that cancels the search when a newer one starts.</param>
    /// <returns>The matching repositories, wrapped for the view.</returns>
    private async Task<IReadOnlyList<RepositoryDetailsViewModel>> SearchRepositoriesAsync(string term, CancellationToken cancellationToken)
    {
        IReadOnlyList<Repository> repositories = await _api.SearchRepositoriesAsync(term, cancellationToken).ConfigureAwait(false);
        return [.. repositories.Select(static repository => new RepositoryDetailsViewModel(repository))];
    }
}
