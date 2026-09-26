// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.WhenActivated;

/// <summary>Shows forcing activation directly, and plugging a custom <see cref="IActivationForViewFetcher"/> into the app.</summary>
public static class ManualActivationExamples
{
    /// <summary>
    /// A dashboard has one show/hide event for the whole grid, not one per tile, so it forces each tile's activation
    /// state through <see cref="ICanForceManualActivation"/> directly. The tile still raises <see cref="ICanActivate"/>
    /// ticks for whatever is watching them.
    /// </summary>
    public static void ForceATileActiveFromTheDashboard()
    {
        using ScoreBoardTile tile = new();
        List<string> log = [];
        using IDisposable activatedSubscription = tile.Activated.Subscribe(_ => log.Add("Activated"));
        using IDisposable deactivatedSubscription = tile.Deactivated.Subscribe(_ => log.Add("Deactivated"));

        ICanForceManualActivation manualActivation = tile;
        manualActivation.Activate(isActivating: true);
        manualActivation.Activate(isActivating: false);

        Console.WriteLine(string.Join(", ", log));

        // Output:
        // Activated, Deactivated
    }

    /// <summary>
    /// <see cref="LegacyPanelActivationFetcher"/> is registered with the app at start-up, the way a platform package
    /// plugs itself in. Once registered, <c>WhenActivated</c> on <see cref="LegacyScorePanel"/> resolves to it
    /// automatically and activates the content view model in step with the panel's plain events.
    /// </summary>
    public static void ActivateALegacyControlThroughACustomFetcher()
    {
        LegacyScorePanel panel = new();
        LiveScoreFeed feed = new();
        using ScoreBoardViewModel viewModel = new(feed);

        using IDisposable subscription = panel.WhenActivated(Signal.Emit<object?>(viewModel));

        panel.Show();
        feed.Report(30);
        Console.WriteLine(viewModel.CurrentScore);

        panel.Hide();
        Console.WriteLine(feed.IsRunning);

        // Output:
        // 30
        // False
    }
}
