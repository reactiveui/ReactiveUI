// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.WhenActivated;

/// <summary>The past-games panel: it reloads the list of past scores each time the screen is shown.</summary>
[System.Diagnostics.DebuggerDisplay("PastScores.Count = {PastScores.Count}")]
public sealed class ScoreBoardHistoryViewModel : ReactiveObject, IActivatableViewModel, IDisposable
{
    /// <summary>Reads the past scores; a real screen would call a database or a web service instead.</summary>
    private readonly Func<IReadOnlyList<int>> _loadPastScores;

    /// <summary>Initializes a new instance of the <see cref="ScoreBoardHistoryViewModel"/> class.</summary>
    /// <param name="loadPastScores">Reads the past scores to show.</param>
    public ScoreBoardHistoryViewModel(Func<IReadOnlyList<int>> loadPastScores)
    {
        _loadPastScores = loadPastScores;

        // The function-block style: the block returns the disposables to keep; a load with nothing to
        // subscribe to has none.
        this.WhenActivated(() =>
        {
            PastScores = _loadPastScores();
            return [];
        });
    }

    /// <inheritdoc/>
    public ViewModelActivator Activator { get; } = new();

    /// <summary>Gets the scores from past games, freshest first.</summary>
    public IReadOnlyList<int> PastScores
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    /// <inheritdoc/>
    public void Dispose() => Activator.Dispose();
}
