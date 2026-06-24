// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.IO;
using System.Windows;
using ReactiveUI.Builder.WpfApp.Models;
using ReactiveUI.Builder.WpfApp.Services;
using ReactiveUI.Builder.WpfApp.ViewModels;
using Splat;

namespace ReactiveUI.Builder.WpfApp;

/// <summary>Interaction logic for App.xaml.</summary>
public partial class App : Application
{
    /// <summary>The small object-cache size configured for ReactiveUI.</summary>
    private const int SmallCacheSize = 100;

    /// <summary>The large object-cache size configured for ReactiveUI.</summary>
    private const int LargeCacheSize = 400;

    /// <summary>The suspension driver that persists and restores the transaction journal.</summary>
    private FileJsonSuspensionDriver? _driver;

    /// <summary>Raises the <see cref="Application.Startup"/> event.</summary>
    /// <param name="e">A <see cref="StartupEventArgs"/> that contains the event data.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // The whole stack is configured through the builder: the WPF platform, automatic view discovery,
        // a typed suspension host, cache sizes, an exception handler, the message bus, and the services.
        _ = RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .WithViewsFromAssembly(typeof(App).Assembly)
            .WithSuspensionHost<TerminalState>()
            .WithCacheSizes(SmallCacheSize, LargeCacheSize)
            .WithExceptionHandler(new LoggingExceptionObserver())
            .WithMessageBus()
            .WithRegistration(static r =>
            {
                r.RegisterLazySingleton<IPaymentProcessor>(static () => new MockPaymentProcessor());
                r.RegisterLazySingleton<IScreen>(static () => new AppBootstrapper());
            })
            .Build();

        RxSuspension.SuspensionHost.CreateNewAppState = static () => new TerminalState();

        var statePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ReactiveUI.Builder.WpfApp",
            "journal.json");
        _ = Directory.CreateDirectory(Path.GetDirectoryName(statePath)!);
        _driver = new(statePath);

        // Restore the persisted journal before the first navigation so the terminal and journal see it.
        RxSuspension.SuspensionHost.AppState =
            (_driver.LoadState().GetAwaiter().GetResult() as TerminalState) ?? new TerminalState();

        var window = new MainWindow();
        MainWindow = window;
        window.Show();
    }

    /// <summary>Raises the <see cref="Application.Exit"/> event and persists the journal.</summary>
    /// <param name="e">An <see cref="ExitEventArgs"/> that contains the event data.</param>
    protected override void OnExit(ExitEventArgs e)
    {
        if (_driver is not null && RxSuspension.SuspensionHost.AppState is TerminalState state)
        {
            _ = _driver.SaveState(state).GetAwaiter().GetResult();
        }

        base.OnExit(e);
    }

    /// <summary>Logs unhandled ReactiveUI exceptions and breaks into the debugger when one is attached.</summary>
    private sealed class LoggingExceptionObserver : IObserver<Exception>
    {
        /// <inheritdoc/>
        public void OnNext(Exception value)
        {
            Trace.TraceError($"[ReactiveUI] Unhandled exception: {value}");
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
