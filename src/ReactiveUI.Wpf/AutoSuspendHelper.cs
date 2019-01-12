// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using Splat;

namespace ReactiveUI
{
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
                Observable.FromEventPattern<StartupEventHandler, StartupEventArgs>(
                    x => app.Startup += x, x => app.Startup -= x)
                    .Select(_ => Unit.Default);

            RxApp.SuspensionHost.IsUnpausing =
                Observable.FromEventPattern<EventHandler, EventArgs>(
                    x => app.Activated += x, x => app.Activated -= x)
                    .Select(_ => Unit.Default);

            RxApp.SuspensionHost.IsResuming = Observable<Unit>.Never;

            // NB: No way to tell OS that we need time to suspend, we have to
            // do it in-process
            var deactivated = Observable.FromEventPattern<EventHandler, EventArgs>(
                x => app.Deactivated += x, x => app.Deactivated -= x);

            var exit = Observable.FromEventPattern<ExitEventHandler, ExitEventArgs>(
                x => app.Exit += x, x => app.Exit -= x);

            RxApp.SuspensionHost.ShouldPersistState = Observable.Merge(
                exit.Select(_ => Disposable.Empty),
                deactivated
                    .SelectMany(_ => Observable.Timer(IdleTimeout, RxApp.TaskpoolScheduler))
                    .TakeUntil(RxApp.SuspensionHost.IsUnpausing)
                    .Repeat()
                    .Select(_ => Disposable.Empty));

            var untimelyDeath = new Subject<Unit>();
            AppDomain.CurrentDomain.UnhandledException += (o, e) => untimelyDeath.OnNext(Unit.Default);
            RxApp.SuspensionHost.ShouldInvalidateState = untimelyDeath;
        }

        /// <summary>
        /// Gets or sets the time out before the Auto Suspension happens.
        /// </summary>
        public TimeSpan IdleTimeout { get; set; }
    }
}
