// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI.Builder.WpfApp.Models;
using Splat;

namespace ReactiveUI.Builder.WpfApp;

/// <summary>
/// Interaction logic for App.xaml.
/// </summary>
[SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Disposed on application exit in OnExit")]
public partial class App : Application
{
    private Services.FileJsonSuspensionDriver? _driver;
    private Services.ChatNetworkService? _networkService;
    private Services.AppLifetimeCoordinator? _lifetime;

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Application.Startup" /> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Initialize ReactiveUI via the Builder only
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .WithViewsFromAssembly(typeof(App).Assembly) // auto-register all IViewFor in this assembly
            ////.RegisterView<MainWindow, ViewModels.AppBootstrapper>()
            ////.RegisterView<Views.ChatRoomView, ViewModels.ChatRoomViewModel>()
            ////.RegisterView<Views.LobbyView, ViewModels.LobbyViewModel>()
            .WithRegistration(r =>
            {
                // Register IScreen as a singleton so all resolutions share the same Router
                r.RegisterLazySingleton<IScreen>(() => new ViewModels.AppBootstrapper());

                // Register MessageBus as a singleton if not already
                if (Locator.Current.GetService<IMessageBus>() is null)
                {
                    r.RegisterConstant(MessageBus.Current);
                }

                // Cross-process instance lifetime coordination
                r.RegisterLazySingleton(() => new Services.AppLifetimeCoordinator());

                // Network service used to broadcast/receive messages across instances
                r.RegisterLazySingleton(() => new Services.ChatNetworkService());
            })
            .Build();

        // Setup Suspension
        RxApp.SuspensionHost.CreateNewAppState = () => new ChatState();

        var statePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ReactiveUI.Builder.WpfApp",
            "state.json");
        Directory.CreateDirectory(Path.GetDirectoryName(statePath)!);

        _driver = new Services.FileJsonSuspensionDriver(statePath);

        // Set an initial state instantly to avoid blocking UI
        RxApp.SuspensionHost.AppState = new ChatState();

        // Load persisted state asynchronously and update UI when ready
        _ = _driver
            .LoadState()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(
                stateObj =>
                {
                    RxApp.SuspensionHost.AppState = stateObj;
                    MessageBus.Current.SendMessage(new ChatStateChanged());
                    Trace.WriteLine("[App] State loaded");
                },
                ex => Trace.WriteLine($"[App] State load failed: {ex.Message}"));

        // Resolve coordinator + network service
        _lifetime = Locator.Current.GetService<Services.AppLifetimeCoordinator>();
        var count = _lifetime?.Increment() ?? 1;
        Trace.WriteLine($"[App] Instance started. Count={count} Id={Services.AppInstance.Id}");

        _networkService = Locator.Current.GetService<Services.ChatNetworkService>();
        _networkService?.Start(); // starts background receive loop, no UI blocking

        // Resolve AppBootstrapper once and use it for both ViewModel and Router
        var appBoot = (ViewModels.AppBootstrapper)Locator.Current.GetService<IScreen>()!;

        // Create and show the shell
        var mainWindow = new MainWindow
        {
            ViewModel = appBoot,
        };

        // Replace RoutedViewHost router to ensure it uses the same singleton instance
        if (mainWindow.Content is RoutedViewHost host)
        {
            host.Router = appBoot.Router;
        }

        MainWindow = mainWindow;
        mainWindow.Show();
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Application.Exit" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs" /> that contains the event data.</param>
    protected override void OnExit(ExitEventArgs e)
    {
        try
        {
            var remaining = _lifetime?.Decrement() ?? 0;
            Trace.WriteLine($"[App] Instance exiting. Remaining={remaining} Id={Services.AppInstance.Id}");

            // Only the last instance persists the final state to the central store
            if (remaining == 0 && _driver is not null && RxApp.SuspensionHost.AppState is not null)
            {
                _driver.SaveState(RxApp.SuspensionHost.AppState).Wait();
            }
        }
        finally
        {
            _networkService?.Dispose();
            base.OnExit(e);
        }
    }
}
