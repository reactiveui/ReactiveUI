// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

namespace ReactiveUI;

/// <summary>
/// Wires WPF <see cref="Application"/> lifecycle events into <see cref="RxApp.SuspensionHost"/> so application state can be persisted automatically.
/// </summary>
/// <remarks>
/// <para>
/// Create a single instance of this helper in <see cref="Application.OnStartup(StartupEventArgs)"/> to forward the
/// <c>Startup</c>, <c>Activated</c>, <c>Deactivated</c>, and <c>Exit</c> events. Combine it with <see cref="SuspensionHostExtensions.SetupDefaultSuspendResume(ISuspensionHost, ISuspensionDriver?)"/>
/// to flush <see cref="ISuspensionHost.AppState"/> to disk whenever the window loses focus for longer than <see cref="IdleTimeout"/> or when the
/// process exits.
/// </para>
/// <para>
/// Example usage:
/// <code language="csharp">
/// <![CDATA[
/// public partial class App : Application
/// {
///     private AutoSuspendHelper? _autoSuspendHelper;
///
///     protected override void OnStartup(StartupEventArgs e)
///     {
///         base.OnStartup(e);
///         _autoSuspendHelper = new AutoSuspendHelper(this)
///         {
///             IdleTimeout = TimeSpan.FromSeconds(10)
///         };
///
///         RxApp.SuspensionHost.CreateNewAppState = () => new ShellState();
///         RxApp.SuspensionHost.SetupDefaultSuspendResume(new FileSuspensionDriver(LocalAppDataProvider.Resolve()));
///     }
/// }
/// ]]>
/// </code>
/// </para>
/// </remarks>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("AutoSuspendHelper uses RxApp.SuspensionHost and TaskpoolScheduler which require dynamic code generation")]
[RequiresUnreferencedCode("AutoSuspendHelper uses RxApp.SuspensionHost and TaskpoolScheduler which may require unreferenced code")]
#endif
public class AutoSuspendHelper : IEnableLogger
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoSuspendHelper"/> class.
    /// </summary>
    /// <param name="app">The application.</param>
    public AutoSuspendHelper(Application app)
    {
        IdleTimeout = TimeSpan.FromSeconds(15.0);

        RxApp.SuspensionHost.IsLaunchingNew =
            Observable.FromEvent<StartupEventHandler, Unit>(
                                                            eventHandler =>
                                                            {
                                                                void Handler(object sender, StartupEventArgs e) => eventHandler(Unit.Default);
                                                                return Handler;
                                                            },
                                                            x => app.Startup += x,
                                                            x => app.Startup -= x);

        RxApp.SuspensionHost.IsUnpausing =
            Observable.FromEvent<EventHandler, Unit>(
                                                     eventHandler => (_, _) => eventHandler(Unit.Default),
                                                     x => app.Activated += x,
                                                     x => app.Activated -= x);

        RxApp.SuspensionHost.IsResuming = Observable<Unit>.Never;

        // NB: No way to tell OS that we need time to suspend, we have to
        // do it in-process
        var deactivated = Observable.FromEvent<EventHandler, Unit>(
                                                                   eventHandler => (_, _) => eventHandler(Unit.Default),
                                                                   x => app.Deactivated += x,
                                                                   x => app.Deactivated -= x);

        var exit = Observable.FromEvent<ExitEventHandler, IDisposable>(
                                                                       eventHandler =>
                                                                       {
                                                                           void Handler(object sender, ExitEventArgs e) => eventHandler(Disposable.Empty);
                                                                           return Handler;
                                                                       },
                                                                       x => app.Exit += x,
                                                                       x => app.Exit -= x);

        RxApp.SuspensionHost.ShouldPersistState = exit.Merge(
                                                             deactivated
                                                                 .SelectMany(_ => Observable.Timer(IdleTimeout, RxApp.TaskpoolScheduler))
                                                                 .TakeUntil(RxApp.SuspensionHost.IsUnpausing)
                                                                 .Repeat()
                                                                 .Select(_ => Disposable.Empty));

        var untimelyDeath = new Subject<Unit>();
        AppDomain.CurrentDomain.UnhandledException += (_, _) => untimelyDeath.OnNext(Unit.Default);
        RxApp.SuspensionHost.ShouldInvalidateState = untimelyDeath;
    }

    /// <summary>
    /// Gets or sets the time out before the Auto Suspension happens.
    /// </summary>
    /// <remarks>
    /// Determines how long the helper waits after the app is deactivated before emitting <see cref="ISuspensionHost.ShouldPersistState"/>
    /// (unless the window is reactivated sooner). Shorter durations trade battery for durability.
    /// </remarks>
    public TimeSpan IdleTimeout { get; set; }
}
