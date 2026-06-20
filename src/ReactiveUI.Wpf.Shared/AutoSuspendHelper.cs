// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Wires WPF <see cref="Application"/> lifecycle events into <see cref="RxSuspension.SuspensionHost"/> so application state can be persisted automatically.</summary>
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
///     protected override void OnStartup(StartupEventArgs e)
///     {
///         base.OnStartup(e);
///         _autoSuspendHelper = new AutoSuspendHelper(this)
///         {
///             IdleTimeout = TimeSpan.FromSeconds(10)
///         };
///         RxSuspension.SuspensionHost.CreateNewAppState = () => new ShellState();
///         RxSuspension.SuspensionHost.SetupDefaultSuspendResume(new FileSuspensionDriver(LocalAppDataProvider.Resolve()));
///     }
/// }
/// ]]>
/// </code>
/// </para>
/// </remarks>
public class AutoSuspendHelper : IEnableLogger
{
    /// <summary>Initializes a new instance of the <see cref="AutoSuspendHelper"/> class.</summary>
    /// <param name="app">The application.</param>
    public AutoSuspendHelper(Application app)
    {
        IdleTimeout = TimeSpan.FromSeconds(15.0);

        var isUnpausing = new FromEventObservable<RxVoid>(onNext =>
        {
            void Handler(object? sender, EventArgs e) => onNext(RxVoid.Default);
            app.Activated += Handler;
            return new ActionDisposable(() => app.Activated -= Handler);
        });

        RxSuspension.SuspensionHost.IsLaunchingNew = new FromEventObservable<RxVoid>(onNext =>
        {
            void Handler(object sender, StartupEventArgs e) => onNext(RxVoid.Default);
            app.Startup += Handler;
            return new ActionDisposable(() => app.Startup -= Handler);
        });

        RxSuspension.SuspensionHost.IsUnpausing = isUnpausing;
        RxSuspension.SuspensionHost.IsResuming = Signal.Silent<RxVoid>();

        // NB: No way to tell OS that we need time to suspend, we have to
        // do it in-process
        var deactivated = new FromEventObservable<RxVoid>(onNext =>
        {
            void Handler(object? sender, EventArgs e) => onNext(RxVoid.Default);
            app.Deactivated += Handler;
            return new ActionDisposable(() => app.Deactivated -= Handler);
        });

        var exit = new FromEventObservable<IDisposable>(onNext =>
        {
            void Handler(object sender, ExitEventArgs e) => onNext(EmptyDisposable.Instance);
            app.Exit += Handler;
            return new ActionDisposable(() => app.Exit -= Handler);
        });

        // Persist when the app exits, or when it stays deactivated for the idle timeout without being reactivated
        // (a reset-on-reactivate idle timer), replacing the SelectMany(Timer)+TakeUntil+Repeat+Merge pipeline.
        RxSuspension.SuspensionHost.ShouldPersistState = Signal.Blend<IDisposable>(
            exit,
            new IdleTimeoutObservable(deactivated, isUnpausing, () => IdleTimeout, RxSchedulers.TaskpoolScheduler));

        var untimelyDeath = new Signal<RxVoid>();
        AppDomain.CurrentDomain.UnhandledException += (_, _) => untimelyDeath.OnNext(RxVoid.Default);
        RxSuspension.SuspensionHost.ShouldInvalidateState = untimelyDeath;
    }

    /// <summary>Gets or sets the time out before the Auto Suspension happens.</summary>
    /// <remarks>
    /// Determines how long the helper waits after the app is deactivated before emitting <see cref="ISuspensionHost.ShouldPersistState"/>
    /// (unless the window is reactivated sooner). Shorter durations trade battery for durability.
    /// </remarks>
    public TimeSpan IdleTimeout { get; set; }
}
