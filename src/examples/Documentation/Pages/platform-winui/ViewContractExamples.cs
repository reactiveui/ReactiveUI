// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// Shows the contract members every view host adds beyond what <see cref="MainWindow"/> already exercises:
/// <c>ViewContract</c>, <c>ViewContractObservable</c> and the <c>ViewContractObservableProperty</c> dependency
/// property, plus the protected <c>ResolveViewForViewModel</c> a host calls each time its view model or contract
/// changes. Each host below builds its own <see cref="WeatherShell"/> and view locator, so it does not touch the
/// dashboard's own registrations.
/// </summary>
public static class ViewContractExamples
{
    /// <summary>
    /// <c>ResolveViewForViewModel</c> runs once for the empty host and again for each <c>ViewModel</c> change;
    /// <see cref="AnalyticsViewModelViewHost"/> overrides it to record what it resolved. <c>ViewContract</c>
    /// republishes as <c>ViewContractObservable</c>, the same stream <c>ViewContractObservableProperty</c> holds.
    /// </summary>
    public static void ViewModelViewHostResolvesEachViewModelChangeAndRepublishesItsContract()
    {
        DefaultViewLocator locator = new();
        locator.Map<WeatherReading, WeatherReadingRowView>();

        AnalyticsViewModelViewHost host = new() { ViewLocator = locator };
        WeatherReading riverside = new("Riverside", 18.5, isStormy: false);
        WeatherReading highlands = new("Highlands", 9.0, isStormy: true);

        host.ViewModel = riverside;
        host.ViewModel = highlands;

        Console.WriteLine(string.Join(", ", host.ResolvedViews));

        host.ViewContract = "Wide";
        Console.WriteLine(host.ViewContract);

        string? published = null;
        using IDisposable subscription = host.ViewContractObservable.Subscribe(contract => published = contract);
        Console.WriteLine(published);

        bool sameObservable = ReferenceEquals(host.ViewContractObservable, host.GetValue(ViewModelViewHost.ViewContractObservableProperty));
        Console.WriteLine(sameObservable);

        // Output:
        // (nothing), WeatherReadingRowView, WeatherReadingRowView
        // Wide
        // Wide
        // True
    }

    /// <summary><see cref="ViewModelViewHost{TViewModel}"/> adds the same three contract members, typed to one view model.</summary>
    public static void GenericViewModelViewHostResolvesEachViewModelChangeAndRepublishesItsContract()
    {
        DefaultViewLocator locator = new();
        locator.Map<WeatherReading, WeatherReadingRowView>();

        AnalyticsViewModelViewHost<WeatherReading> host = new() { ViewLocator = locator };
        WeatherReading harbor = new("Harbor", 21.0, isStormy: false);
        WeatherReading highlands = new("Highlands", 9.0, isStormy: true);

        host.ViewModel = harbor;
        host.ViewModel = highlands;

        Console.WriteLine(string.Join(", ", host.ResolvedViews));

        host.ViewContract = "Wide";
        Console.WriteLine(host.ViewContract);

        string? published = null;
        using IDisposable subscription = host.ViewContractObservable.Subscribe(contract => published = contract);
        Console.WriteLine(published);

        bool sameObservable = ReferenceEquals(
            host.ViewContractObservable,
            host.GetValue(ViewModelViewHost<WeatherReading>.ViewContractObservableProperty));
        Console.WriteLine(sameObservable);

        // Output:
        // (nothing), WeatherReadingRowView, WeatherReadingRowView
        // Wide
        // Wide
        // True
    }

    /// <summary>Setting <c>ViewContract</c> before the router navigates picks the view mapped to that contract.</summary>
    public static void RoutedViewHostViewContractSelectsTheContractView()
    {
        DefaultViewLocator locator = new();
        locator.Map<SensorMaintenanceViewModel, SensorMaintenanceView>();
        locator.Map<SensorMaintenanceViewModel, SensorMaintenanceCompactView>("Compact");

        WeatherShell defaultShell = new();
        RoutedViewHost defaultHost = new() { ViewLocator = locator, Router = defaultShell.Router };
        _ = defaultShell.Router.Navigate.Execute(new SensorMaintenanceViewModel(defaultShell, "Harbor", "Replace the wind vane")).Subscribe();
        Console.WriteLine(((SensorMaintenanceView)defaultHost.Content).Summary);

        WeatherShell compactShell = new();
        RoutedViewHost compactHost = new() { ViewLocator = locator, Router = compactShell.Router, ViewContract = "Compact" };
        _ = compactShell.Router.Navigate.Execute(new SensorMaintenanceViewModel(compactShell, "Riverside", "Clean the rain gauge")).Subscribe();
        Console.WriteLine(((SensorMaintenanceCompactView)compactHost.Content).Summary);
        Console.WriteLine(compactHost.ViewContract);

        string? published = null;
        using IDisposable subscription = ((IObservable<string?>)compactHost.GetValue(RoutedViewHost.ViewContractObservableProperty))
            .Subscribe(contract => published = contract);
        Console.WriteLine(published);

        // Output:
        // Harbor: Replace the wind vane
        // Riverside
        // Compact
        // Compact
    }

    /// <summary><see cref="RoutedViewHost{TViewModel}"/> adds the same three contract members, typed to one routable view model.</summary>
    public static void GenericRoutedViewHostViewContractSelectsTheContractView()
    {
        DefaultViewLocator locator = new();
        locator.Map<SensorMaintenanceViewModel, SensorMaintenanceView>();
        locator.Map<SensorMaintenanceViewModel, SensorMaintenanceCompactView>("Compact");

        WeatherShell shell = new();
        RoutedViewHost<SensorMaintenanceViewModel> host = new() { ViewLocator = locator, Router = shell.Router, ViewContract = "Compact" };
        _ = shell.Router.Navigate.Execute(new SensorMaintenanceViewModel(shell, "Highlands", "Inspect the anemometer")).Subscribe();

        Console.WriteLine(((SensorMaintenanceCompactView)host.Content).Summary);
        Console.WriteLine(host.ViewContract);

        bool sameObservable = ReferenceEquals(
            host.ViewContractObservable,
            host.GetValue(RoutedViewHost<SensorMaintenanceViewModel>.ViewContractObservableProperty));
        Console.WriteLine(sameObservable);

        // Output:
        // Highlands
        // Compact
        // True
    }
}
