// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// Application entry point. Configures ReactiveUI for WinUI, registers the dashboard's views, then shows the main
/// window — unless started with <c>--smoke</c>, in which case it drives the dashboard itself, prints what it
/// observed, and exits.
/// </summary>
[System.Diagnostics.DebuggerDisplay("WeatherStationApp")]
public sealed class WeatherStationApp : Application
{
    /// <summary>The stations the dashboard starts with.</summary>
    private static readonly IReadOnlyList<WeatherReading> SeedReadings =
    [
        new WeatherReading("Riverside", 18.5, isStormy: false),
        new WeatherReading("Highlands", 9.0, isStormy: true),
        new WeatherReading("Harbor", 21.0, isStormy: false),
    ];

    /// <summary>Whether the process should drive itself and exit rather than stay open.</summary>
    private readonly bool _smoke;

    /// <summary>The window shown once ReactiveUI and the views are configured.</summary>
    private MainWindow? _mainWindow;

    /// <summary>Initializes a new instance of the <see cref="WeatherStationApp"/> class.</summary>
    /// <param name="args">The process's command-line arguments.</param>
    public WeatherStationApp(string[] args) => _smoke = Array.IndexOf(args, "--smoke") >= 0;

    /// <inheritdoc/>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        ConfigureReactiveUI();
        WinUIStartupExamples.ShowTheIndividualExtensions();

        _mainWindow = new MainWindow(SeedReadings);

        if (_smoke)
        {
            RunSmokeTest(_mainWindow);
            return;
        }

        _mainWindow.Activate();
    }

    /// <summary>Registers ReactiveUI's WinUI services and the dashboard's own views.</summary>
    private static void ConfigureReactiveUI()
    {
        _ = RxAppBuilder.CreateReactiveUIBuilder()
            .WithWinUI()
            .RegisterView<StationListPageView, StationListPageViewModel>()
            .RegisterView<StationDetailPageView, StationDetailPageViewModel>()
            .RegisterView<AlertView, AlertViewModel>()
            .BuildApp();

        // WeatherReading is a plain class, not an IReactiveObject, so RegisterView (which requires IReactiveObject)
        // cannot register it; it is registered with Splat directly instead.
        Locator.CurrentMutable.Register(static () => new WeatherReadingRowView(), typeof(IViewFor<WeatherReading>));
    }

    /// <summary>Drives the dashboard the way a user would, prints what happened, then closes the window and exits.</summary>
    /// <param name="mainWindow">The window to drive.</param>
    /// <remarks>
    /// <c>Application.Start</c> only begins pumping its dispatcher queue once <see cref="OnLaunched"/> returns, so
    /// activation-driven work (view <c>Loading</c>, <c>WhenActivated</c>, <see cref="AutoDataTemplateBindingHook"/>)
    /// has not run yet at the point this method is called. Each step below is posted through
    /// <see cref="PumpThen"/> so the dispatcher gets turns to run that work between steps, the way a person
    /// clicking through the app, one action at a time, would let it catch up.
    /// </remarks>
    private static void RunSmokeTest(MainWindow mainWindow)
    {
        mainWindow.Activate();
        DispatcherQueue queue = mainWindow.DispatcherQueue;
        PumpThen(queue, 10, () => DescribeStationList(mainWindow, queue));
    }

    /// <summary>Reports what activating the station list actually did, once the dispatcher has had a chance to load it.</summary>
    /// <param name="mainWindow">The window being driven.</param>
    /// <param name="queue">The window's dispatcher queue.</param>
    private static void DescribeStationList(MainWindow mainWindow, DispatcherQueue queue)
    {
        StationListPageView? stationListView = mainWindow.CurrentStationListView;
        if (stationListView is not null)
        {
            ActivationForViewFetcherExamples.DescribeActivation(stationListView);
            Console.WriteLine($"AutoDataTemplateBindingHook assigned a template: {stationListView.ReadingsList.ItemTemplate is not null}");
        }
        else
        {
            Console.WriteLine("Station list view not resolved yet.");
        }

        _ = mainWindow.StationList.OpenDetail.Execute(mainWindow.StationList.Readings[1]).Subscribe();
        Console.WriteLine($"Navigated to: {mainWindow.StationList.HostScreen.Router.NavigationStack[^1].UrlPathSegment}");

        mainWindow.AlertPanel.Message = "Storm warning: Highlands";
        Console.WriteLine($"Alert: {mainWindow.AlertPanel.Message}");

        PumpThen(queue, 10, () => FinishSmokeTest(mainWindow));
    }

    /// <summary>Closes the window and exits the process once every posted step has had its turn.</summary>
    /// <param name="mainWindow">The window to close.</param>
    private static void FinishSmokeTest(MainWindow mainWindow)
    {
        Console.WriteLine("Closing window.");
        mainWindow.Close();
        Environment.Exit(0);
    }

    /// <summary>Posts <paramref name="continuation"/> after <paramref name="hops"/> low-priority round trips through the queue.</summary>
    /// <param name="queue">The dispatcher queue to post through.</param>
    /// <param name="hops">How many low-priority turns to give the queue before running <paramref name="continuation"/>.</param>
    /// <param name="continuation">The work to run once every hop has had its turn.</param>
    private static void PumpThen(DispatcherQueue queue, int hops, Action continuation)
    {
        if (hops <= 0)
        {
            continuation();
            return;
        }

        _ = queue.TryEnqueue(DispatcherQueuePriority.Low, () => PumpThen(queue, hops - 1, continuation));
    }
}
