// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.WhenActivated;

/// <summary>
/// The scoreboard screen's view model: it starts the live score feed when the screen is shown and stops it when the
/// screen is hidden, so nothing keeps polling once nobody can see it.
/// </summary>
[System.Diagnostics.DebuggerDisplay("CurrentScore = {CurrentScore}")]
public sealed class ScoreBoardViewModel : ReactiveObject, IActivatableViewModel, IDisposable
{
    /// <summary>The live feed this screen watches while shown.</summary>
    private readonly LiveScoreFeed _feed;

    /// <summary>Initializes a new instance of the <see cref="ScoreBoardViewModel"/> class.</summary>
    /// <param name="feed">The live feed to start and stop with the screen.</param>
    public ScoreBoardViewModel(LiveScoreFeed feed)
    {
        _feed = feed;

        // The disposables container style: add each disposable to the container the activator hands the block.
        this.WhenActivated(disposables =>
        {
            _feed.Start();
            disposables.Add(new ActionDisposable(_feed.Stop));
            disposables.Add(_feed.Scores.Subscribe(score => CurrentScore = score));
        });
    }

    /// <inheritdoc/>
    public ViewModelActivator Activator { get; } = new();

    /// <summary>Gets the most recent score reported while the screen is shown.</summary>
    public int CurrentScore
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    public void Dispose() => Activator.Dispose();
}
