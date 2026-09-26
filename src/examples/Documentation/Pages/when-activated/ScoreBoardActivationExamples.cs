// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.WhenActivated;

/// <summary>
/// Shows the view side of <c>WhenActivated</c>: a screen that raises its own activation, in the three disposal
/// styles the library offers, plus the built-in fetcher that watches an <see cref="ICanActivate"/> view.
/// </summary>
public static class ScoreBoardActivationExamples
{
    /// <summary>
    /// The function-block style: the block returns the disposables to keep. Here it keeps a subscription that
    /// updates the screen's title from whatever content it is showing.
    /// </summary>
    public static void UpdateTheTitleWithAFunctionBlock()
    {
        LiveScoreFeed feed = new();
        using ScoreBoardViewModel viewModel = new(feed);
        using ScoreBoardScreen screen = new(viewModel);

        Console.WriteLine(screen.GetIsDesignMode());

        using IDisposable subscription = screen.WhenActivated(
            () => [screen.ContentChanged.Subscribe(content => screen.Title = content is ScoreBoardViewModel ? "Score Board" : "Unknown")],
            screen.ContentChanged);

        screen.Show();
        Console.WriteLine(screen.Title);
        feed.Report(15);
        Console.WriteLine(viewModel.CurrentScore);
        screen.Hide();

        // Output:
        // False
        // Score Board
        // 15
    }

    /// <summary>
    /// The disposables-container style: the block adds each disposable to the container the activator hands it.
    /// Here the screen shows the past-games panel instead.
    /// </summary>
    public static void UpdateTheTitleWithADisposablesContainer()
    {
        int[] pastScores = [10, 42, 87];
        using ScoreBoardHistoryViewModel viewModel = new(() => pastScores);
        using ScoreBoardScreen screen = new(viewModel);

        using IDisposable subscription = screen.WhenActivated(
            disposables => disposables.Add(screen.ContentChanged.Subscribe(content => screen.Title = content is ScoreBoardHistoryViewModel ? "History" : "Unknown")),
            screen.ContentChanged);

        screen.Show();
        Console.WriteLine(screen.Title);
        Console.WriteLine(string.Join(", ", viewModel.PastScores));
        screen.Hide();

        // Output:
        // History
        // 10, 42, 87
    }

    /// <summary>
    /// The no-block overload: the screen has no resources of its own, only a content view model to activate and
    /// deactivate with it.
    /// </summary>
    public static void ActivateOnlyTheContentViewModel()
    {
        LiveScoreFeed feed = new();
        using ScoreBoardViewModel viewModel = new(feed);
        using ScoreBoardScreen screen = new(viewModel);

        using IDisposable subscription = screen.WhenActivated(screen.ContentChanged);

        screen.Show();
        feed.Report(64);
        Console.WriteLine(viewModel.CurrentScore);
        screen.Hide();
        Console.WriteLine(feed.IsRunning);

        // Output:
        // 64
        // False
    }

    /// <summary>
    /// <see cref="CanActivateViewFetcher"/> is the fetcher every ReactiveUI app registers by default; it watches
    /// any <see cref="ICanActivate"/> view. Used directly here, outside the automatic pipeline, to see what it does.
    /// </summary>
    public static void InspectTheBuiltInActivationFetcher()
    {
        CanActivateViewFetcher fetcher = new();
        using ScoreBoardScreen screen = new(content: null);

        Console.WriteLine(fetcher.GetAffinityForView(typeof(ScoreBoardScreen)));
        Console.WriteLine(fetcher.GetAffinityForView(typeof(object)));

        List<bool> activationStates = [];
        using IDisposable subscription = fetcher.GetActivationForView(screen).Subscribe(activationStates.Add);

        screen.Show();
        screen.Hide();

        Console.WriteLine(string.Join(", ", activationStates));

        // Output:
        // 10
        // 0
        // True, False
    }
}
