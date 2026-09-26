// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.WhenActivated;

/// <summary>Shows <see cref="ViewModelActivator"/> directly, the way a unit test simulates a screen being shown and hidden.</summary>
public static class ScoreBoardViewModelExamples
{
    /// <summary>
    /// <c>Activate</c> counts how many times it has been called without a matching <c>Deactivate</c>; the view model
    /// stays active until every caller has let go. <see cref="ViewModelActivator.Activated"/> and
    /// <see cref="ViewModelActivator.Deactivated"/> tick only on the first activation and the last deactivation.
    /// </summary>
    public static void ActivateAndDeactivateWithARefCount()
    {
        LiveScoreFeed feed = new();
        using ScoreBoardViewModel viewModel = new(feed);

        List<string> log = [];
        using IDisposable activatedSubscription = viewModel.Activator.Activated.Subscribe(_ => log.Add("Activated"));
        using IDisposable deactivatedSubscription = viewModel.Activator.Deactivated.Subscribe(_ => log.Add("Deactivated"));

        _ = viewModel.Activator.Activate();
        _ = viewModel.Activator.Activate();
        feed.Report(87);
        Console.WriteLine(viewModel.CurrentScore);

        viewModel.Activator.Deactivate();
        Console.WriteLine(feed.IsRunning);

        viewModel.Activator.Deactivate();
        Console.WriteLine(feed.IsRunning);
        Console.WriteLine(string.Join(", ", log));

        // Output:
        // 87
        // True
        // False
        // Activated, Deactivated
    }

    /// <summary>
    /// <c>Deactivate(true)</c> forces the view model inactive at once, ignoring the ref count. A screen uses this
    /// when it is torn down and cannot wait for every outstanding activation to be released in turn.
    /// </summary>
    public static void ForceDeactivateRegardlessOfRefCount()
    {
        LiveScoreFeed feed = new();
        ScoreBoardViewModel viewModel = new(feed);

        IDisposable firstActivation = viewModel.Activator.Activate();
        IDisposable secondActivation = viewModel.Activator.Activate();
        Console.WriteLine(feed.IsRunning);

        viewModel.Activator.Deactivate(ignoreRefCount: true);
        Console.WriteLine(feed.IsRunning);

        firstActivation.Dispose();
        secondActivation.Dispose();
        viewModel.Dispose();

        // Output:
        // True
        // False
    }

    /// <summary>The function-block style loads the past scores once, when the panel is first shown.</summary>
    public static void LoadHistoryOnActivation()
    {
        int[] pastScores = [21, 33, 55];
        using ScoreBoardHistoryViewModel viewModel = new(() => pastScores);

        using IDisposable activation = viewModel.Activator.Activate();
        Console.WriteLine(string.Join(", ", viewModel.PastScores));

        // Output:
        // 21, 33, 55
    }
}
