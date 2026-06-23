// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using ReactiveUI.Builder.WpfApp.Models;
using Splat;

namespace ReactiveUI.Builder.WpfApp;

/// <summary>Interaction logic for App.xaml.</summary>
[SuppressMessage(
    "Design",
    "CA1001:Types that own disposable fields should be disposable",
    Justification = "Disposed on application exit in OnExit")]
public partial class App : Application
{
    /// <summary>The small object cache size configured for ReactiveUI.</summary>
    private const int SmallCacheSize = 100;

    /// <summary>The large object cache size configured for ReactiveUI.</summary>
    private const int LargeCacheSize = 400;

    /// <summary>The suspension driver that persists and restores the chat state to a JSON file on disk.</summary>
    private Services.FileJsonSuspensionDriver? _driver;

    /// <summary>The service that relays chat messages and room events between running app instances.</summary>
    private Services.ChatNetworkService? _networkService;

    /// <summary>The coordinator that tracks how many app instances are running so the last one can save state.</summary>
    private Services.AppLifetimeCoordinator? _lifetime;

    /// <summary>Raises the <see cref="E:System.Windows.Application.Startup" /> event.</summary>
    /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Initialize ReactiveUI via the Builder only
        _ = RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .WithViewsFromAssembly(typeof(App).Assembly) // auto-register all IViewFor in this assembly

            ////.RegisterView<MainWindow, ViewModels.AppBootstrapper>()
            ////.RegisterView<Views.ChatRoomView, ViewModels.ChatRoomViewModel>()
            ////.RegisterView<Views.LobbyView, ViewModels.LobbyViewModel>()
            .WithSuspensionHost<ChatState>() // Configure typed suspension host
            .WithCacheSizes(SmallCacheSize, LargeCacheSize) // Customize cache sizes
            .WithExceptionHandler(new LoggingExceptionObserver())
            .WithMessageBus()
            .WithRegistration(static r =>
            {
                // Register IScreen as a singleton so all resolutions share the same Router
                r.RegisterLazySingleton<IScreen>(static () => new ViewModels.AppBootstrapper());

                // Cross-process instance lifetime coordination
                r.RegisterLazySingleton(static () => new Services.AppLifetimeCoordinator());

                // Network service used to broadcast/receive messages across instances
                r.RegisterLazySingleton(static () => new Services.ChatNetworkService());
            })
            .Build();

        // Setup Suspension
        RxSuspension.SuspensionHost.CreateNewAppState = static () => new ChatState();

        var statePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ReactiveUI.Builder.WpfApp",
            "state.json");
        _ = Directory.CreateDirectory(Path.GetDirectoryName(statePath)!);

        _driver = new(statePath);

        // Set an initial state instantly to avoid blocking UI
        RxSuspension.SuspensionHost.AppState = new ChatState();

        // Load persisted state asynchronously and update UI when ready
        _ = _driver
            .LoadState()
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(
                static stateObj =>
                {
                    RxSuspension.SuspensionHost.AppState = stateObj;
                    MessageBus.Current.SendMessage(new ChatStateChanged());
                    Trace.TraceInformation("[App] State loaded");
                },
                static ex => Trace.TraceInformation($"[App] State load failed: {ex.Message}"));

        // Resolve coordinator + network service
        _lifetime = Locator.Current.GetService<Services.AppLifetimeCoordinator>();
        var count = _lifetime?.Increment() ?? 1;
        Trace.TraceInformation($"[App] Instance started. Count={count} Id={Services.AppInstance.Id}");

        _networkService = Locator.Current.GetService<Services.ChatNetworkService>();
        _networkService?.Start(); // starts background receive loop, no UI blocking

        // Resolve AppBootstrapper once and use it for both ViewModel and Router
        var appBoot = (ViewModels.AppBootstrapper)Locator.Current.GetService<IScreen>()!;

        // Create and show the shell
        var mainWindow = new MainWindow { ViewModel = appBoot };

        // Replace RoutedViewHost router to ensure it uses the same singleton instance
        if (mainWindow.Content is RoutedViewHost host)
        {
            host.Router = appBoot.Router;
        }

        MainWindow = mainWindow;
        mainWindow.Show();
    }

    /// <summary>Raises the <see cref="E:System.Windows.Application.Exit" /> event.</summary>
    /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs" /> that contains the event data.</param>
    protected override void OnExit(ExitEventArgs e)
    {
        try
        {
            var remaining = _lifetime?.Decrement() ?? 0;
            Trace.TraceInformation($"[App] Instance exiting. Remaining={remaining} Id={Services.AppInstance.Id}");

            // Only the last instance persists the final state to the central store
            if (remaining == 0 && _driver is not null && RxSuspension.SuspensionHost.AppState is not null)
            {
                _ = _driver.SaveState(RxSuspension.SuspensionHost.AppState).GetAwaiter().GetResult();
            }
        }
        finally
        {
            _networkService?.Dispose();
            base.OnExit(e);
        }
    }

    /// <summary>Logs unhandled ReactiveUI exceptions and breaks into the debugger when attached.</summary>
    private sealed class LoggingExceptionObserver : IObserver<Exception>
    {
        /// <inheritdoc/>
        public void OnNext(Exception value)
        {
            Trace.TraceInformation($"[ReactiveUI] Unhandled exception: {value}");
            if (!Debugger.IsAttached)
            {
                return;
            }

            Debugger.Break();
        }

        /// <inheritdoc/>
        public void OnError(Exception error) => OnNext(error);

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }
    }
}
