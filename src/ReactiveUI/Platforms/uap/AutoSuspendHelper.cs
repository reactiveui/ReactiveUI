// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Splat;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ReactiveUI
{
    /// <summary>
    /// AutoSuspend-based Application. To use AutoSuspend with WinRT, change your
    /// Application to inherit from this class, then call:
    /// Locator.Current.GetService.&lt;ISuspensionHost&gt;().SetupDefaultSuspendResume();
    /// This will register your suspension host.
    /// </summary>
    public class AutoSuspendHelper : IEnableLogger, IDisposable
    {
        private readonly ReplaySubject<IActivatedEventArgs> _activated = new (1);

        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoSuspendHelper"/> class.
        /// </summary>
        /// <param name="app">The application.</param>
        public AutoSuspendHelper(Application app)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            Reflection.ThrowIfMethodsNotOverloaded("AutoSuspendHelper", app, "OnLaunched");

            var launchNew = new[] { ApplicationExecutionState.ClosedByUser, ApplicationExecutionState.NotRunning, };
            RxApp.SuspensionHost.IsLaunchingNew = _activated
                .Where(x => launchNew.Contains(x.PreviousExecutionState))
                .Select(_ => Unit.Default);

            RxApp.SuspensionHost.IsResuming = _activated
                .Where(x => x.PreviousExecutionState == ApplicationExecutionState.Terminated)
                .Select(_ => Unit.Default);

            var unpausing = new[] { ApplicationExecutionState.Suspended, ApplicationExecutionState.Running, };
            RxApp.SuspensionHost.IsUnpausing = _activated
                .Where(x => unpausing.Contains(x.PreviousExecutionState))
                .Select(_ => Unit.Default);

            var shouldPersistState = new Subject<SuspendingEventArgs>();
            app.Suspending += (_, e) => shouldPersistState.OnNext(e);
            RxApp.SuspensionHost.ShouldPersistState =
                shouldPersistState.Select(x =>
                {
                    var deferral = x.SuspendingOperation.GetDeferral();
                    return Disposable.Create(deferral.Complete);
                });

            var shouldInvalidateState = new Subject<Unit>();
            app.UnhandledException += (_, __) => shouldInvalidateState.OnNext(Unit.Default);
            RxApp.SuspensionHost.ShouldInvalidateState = shouldInvalidateState;
        }

        /// <summary>
        /// Raises the applications Launch event.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        public void OnLaunched(IActivatedEventArgs args) => _activated.OnNext(args);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of resources inside the class.
        /// </summary>
        /// <param name="isDisposing">If we are disposing managed resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                _activated?.Dispose();
            }

            _isDisposed = true;
        }
    }
}
