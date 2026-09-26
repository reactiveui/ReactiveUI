// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// The dashboard's window. It hosts both flavors of the two view hosts: the reflection-based
/// <see cref="RoutedViewHost"/>/<see cref="ViewModelViewHost"/> for pages whose type varies at runtime, and the
/// AOT-safe generic <see cref="RoutedViewHost{TViewModel}"/>/<see cref="ViewModelViewHost{TViewModel}"/> for a
/// panel that always shows one known type.
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

    /// <summary>Hosts the alert panel through the generic, AOT-safe host.</summary>
    private readonly RoutedViewHost<AlertViewModel> _alertHost = new();

    /// <summary>Shows one reading directly, without going through the router, via the reflection-based host.</summary>
    private readonly ViewModelViewHost _quickGlanceHost = new();

    /// <summary>Shows one reading directly through the generic, AOT-safe host.</summary>
    private readonly ViewModelViewHost<WeatherReading> _compactGlanceHost = new();

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

        _compactGlanceHost.ViewModel = readings[^1];
        _compactGlanceHost.ContractFallbackByPass = true;

        StackPanel layout = new();
        layout.Children.Add(_alertHost);
        layout.Children.Add(_pageHost);
        layout.Children.Add(_quickGlanceHost);
        layout.Children.Add(_compactGlanceHost);
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
}
