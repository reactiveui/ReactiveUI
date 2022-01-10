// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Splat;

namespace ReactiveUI;

/// <summary>
/// Class for helping with Auto Suspending.
/// Auto Suspender helpers will assist with saving out the application state
/// when the application closes or activates.
/// </summary>
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
    public TimeSpan IdleTimeout { get; set; }
}