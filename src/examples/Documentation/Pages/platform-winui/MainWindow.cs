// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// The dashboard's window. It hosts both flavors of the two view hosts: <see cref="RoutedViewHost"/>/
/// <see cref="ViewModelViewHost"/> for content whose type varies at run time, and the generic
/// <see cref="RoutedViewHost{TViewModel}"/>/<see cref="ViewModelViewHost{TViewModel}"/> for a panel that always shows
/// one known type. All four find views through the generated view lookup. The maintenance panel shows a view added
/// with <c>Map</c>, which that lookup cannot find, so it uses the Unsafe twins.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Weather Station")]
public sealed class MainWindow : Window
{
    /// <summary>The window's router, navigating between the station list and its detail pages.</summary>
    private readonly WeatherShell _shell = new();

    /// <summary>A router dedicated to the alert panel, which only ever shows <see cref="AlertViewModel"/>.</summary>
    private readonly WeatherShell _alertShell = new();

    /// <summary>Hosts whichever page is on top of <see cref="_shell"/>'s stack.</summary>
    private readonly RoutedViewHost _pageHost = new();

    /// <summary>A router dedicated to the maintenance panel.</summary>
    private readonly WeatherShell _maintenanceShell = new();

    /// <summary>Hosts the alert panel through the generic host.</summary>
    private readonly RoutedViewHost<AlertViewModel> _alertHost = new();

    /// <summary>Shows one reading directly, without going through the router.</summary>
    private readonly ViewModelViewHost _quickGlanceHost = new();

    /// <summary>Shows one reading directly through the generic host.</summary>
    private readonly ViewModelViewHost<WeatherReading> _compactGlanceHost = new();

    /// <summary>Hosts the maintenance visit on top of <see cref="_maintenanceShell"/>'s stack.</summary>
    private readonly RoutedViewHostUnsafe _maintenancePageHost = new();

    /// <summary>Shows the next maintenance visit directly.</summary>
    private readonly ViewModelViewHostUnsafe _nextVisitHost = new();

    /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
    /// <param name="readings">The stations to show.</param>
    public MainWindow(IReadOnlyList<WeatherReading> readings)
    {
        Title = "Weather Station";

        _pageHost.SetValue(RoutedViewHost.RouterProperty, _shell.Router);
        _pageHost.DefaultContent = new TextBlock { Text = "Loading stations..." };

        AlertViewModel alert = new(_alertShell);
        _alertHost.Router = _alertShell.Router;
        _alertHost.DefaultContent = new TextBlock { Text = "(no alert yet)" };
        _ = _alertShell.Router.Navigate.Execute(alert).Subscribe();

        _quickGlanceHost.SetValue(ViewModelViewHost.ViewModelProperty, readings[0]);
        _quickGlanceHost.ContractFallbackByPass = false;
        _quickGlanceHost.DefaultContent = new TextBlock { Text = "(no reading)" };

        _compactGlanceHost.ViewModel = readings[^1];
        _compactGlanceHost.ContractFallbackByPass = true;
        _compactGlanceHost.DefaultContent = new TextBlock { Text = "(no reading)" };

        StackPanel layout = new();
        layout.Children.Add(_alertHost);
        layout.Children.Add(_pageHost);
        layout.Children.Add(_quickGlanceHost);
        layout.Children.Add(_compactGlanceHost);
        layout.Children.Add(CreateMaintenancePanel());
        Content = layout;

        StationListPageViewModel list = new(_shell, readings);
        _ = _shell.Router.Navigate.Execute(list).Subscribe();

        AlertPanel = alert;
        StationList = list;
    }

    /// <summary>Gets the alert panel's view model, so a caller (such as the <c>--smoke</c> run) can drive it.</summary>
    public AlertViewModel AlertPanel { get; }

    /// <summary>Gets the station list's view model, so a caller can navigate it.</summary>
    public StationListPageViewModel StationList { get; }

    /// <summary>Gets the live view <see cref="_pageHost"/> resolved for the page on top of the router, once it has one.</summary>
    public StationListPageView? CurrentStationListView => _pageHost.Content as StationListPageView;

    /// <summary>Gets what the maintenance panel's routed host shows.</summary>
    public object? MaintenancePage => _maintenancePageHost.Content;

    /// <summary>Gets what the maintenance panel's view model host shows.</summary>
    public object? NextVisit => _nextVisitHost.Content;

    /// <summary>
    /// Builds the maintenance panel. <see cref="SensorMaintenanceView"/> is added to the view locator with <c>Map</c>,
    /// which the generated view lookup does not read, so the panel uses the Unsafe twins of both hosts.
    /// </summary>
    /// <returns>The panel holding both hosts.</returns>
    private StackPanel CreateMaintenancePanel()
    {
        _maintenancePageHost.Router = _maintenanceShell.Router;
        _maintenancePageHost.DefaultContent = new TextBlock { Text = "(no visit booked)" };
        SensorMaintenanceViewModel visit = new SensorMaintenanceViewModel(_maintenanceShell, "Harbor", "Replace the wind vane");
        _ = _maintenanceShell.Router.Navigate.Execute(visit).Subscribe();

        _nextVisitHost.DefaultContent = new TextBlock { Text = "(no visit booked)" };
        _nextVisitHost.ViewModel = new SensorMaintenanceViewModel(_maintenanceShell, "Riverside", "Clean the rain gauge");

        StackPanel panel = new StackPanel();
        panel.Children.Add(_maintenancePageHost);
        panel.Children.Add(_nextVisitHost);
        return panel;
    }
}
