// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI.Wpf;
using Splat;

namespace ReactiveUI.Builder.WpfApp;

/// <summary>
/// Interaction logic for App.xaml.
/// </summary>
[SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Disposed on application exit in OnExit")]
public partial class App : Application
{
    private Services.WpfAutoSuspendHelper? _autoSuspend;
    private Services.FileJsonSuspensionDriver? _driver;
    private Services.ChatNetworkService? _networkService;

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Application.Startup" /> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Initialize ReactiveUI via the Builder
        var locator = Locator.CurrentMutable;
        var builder = locator.CreateBuilder();
        builder
            .WithCoreServices()
            .WithWpf()
            .WithViewsFromAssembly(typeof(App).Assembly)
            .WithCustomRegistration(r =>
            {
                // Register IScreen implementation as a factory so creation happens after state is loaded
                r.Register<IScreen>(() => new ViewModels.AppBootstrapper());

                // Register MessageBus as a singleton if not already
                if (Locator.Current.GetService<IMessageBus>() is null)
                {
                    r.RegisterConstant<IMessageBus>(MessageBus.Current);
                }
            })
            .Build();

        // Setup Suspension
        RxApp.SuspensionHost.CreateNewAppState = () => new ViewModels.ChatState();

        var statePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ReactiveUI.Builder.WpfApp",
            "state.json");
        Directory.CreateDirectory(Path.GetDirectoryName(statePath)!);

        _driver = new Services.FileJsonSuspensionDriver(statePath);
        _autoSuspend = new Services.WpfAutoSuspendHelper(this, _driver);
        _autoSuspend.OnStartup();

        // Load state from disk (or create new)
        var loaded = _driver.LoadState().Wait();
        RxApp.SuspensionHost.AppState = loaded;

        // Start network service
        _networkService = new Services.ChatNetworkService();
        _networkService.Start();

        // Create and show the shell
        var mainWindow = new MainWindow();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Application.Exit" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs" /> that contains the event data.</param>
    protected override void OnExit(ExitEventArgs e)
    {
        _networkService?.Dispose();
        if (_driver is not null && RxApp.SuspensionHost.AppState is not null)
        {
            _driver.SaveState(RxApp.SuspensionHost.AppState).Wait();
        }

        _autoSuspend?.OnExit();
        base.OnExit(e);
    }
}
