// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.WhenActivated;

/// <summary>A live feed of a game's score. A scoreboard screen starts it when shown and stops it when hidden.</summary>
[System.Diagnostics.DebuggerDisplay("IsRunning = {IsRunning}")]
public sealed class LiveScoreFeed : IDisposable
{
    /// <summary>Ticks each score reported while the feed is running.</summary>
    private readonly Signal<int> _scores = new();

    /// <summary>Gets a value indicating whether the feed is currently running.</summary>
    public bool IsRunning { get; private set; }

    /// <summary>Gets the stream of reported scores.</summary>
    public IObservable<int> Scores => _scores;

    /// <summary>Starts the feed, as a live game does when its scoreboard screen comes on air.</summary>
    public void Start() => IsRunning = true;

    /// <summary>Reports a new score, ignored unless the feed is running.</summary>
    /// <param name="score">The score to report.</param>
    public void Report(int score)
    {
        if (!IsRunning)
        {
            return;
        }

        _scores.OnNext(score);
    }

    /// <summary>Stops the feed, as a live game does once its scoreboard screen goes off air.</summary>
    public void Stop() => IsRunning = false;

    /// <inheritdoc/>
    public void Dispose() => _scores.Dispose();
}
