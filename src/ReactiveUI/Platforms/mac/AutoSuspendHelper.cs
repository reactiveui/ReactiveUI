// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AppKit;
using Foundation;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// <para>
    /// AutoSuspend-based App Delegate. To use AutoSuspend with iOS, change your
    /// AppDelegate to inherit from this class, then call:
    /// </para>
    /// <para><c>Locator.Current.GetService.{ISuspensionHost}().SetupDefaultSuspendResume();</c>.</para>
    /// <para>This will fetch your SuspensionHost.</para>
    /// </summary>
    public class AutoSuspendHelper : IEnableLogger, IDisposable
    {
        private readonly Subject<IDisposable> _shouldPersistState = new();
        private readonly Subject<Unit> _isResuming = new();
        private readonly Subject<Unit> _isUnpausing = new();

        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoSuspendHelper"/> class.
        /// </summary>
        /// <param name="appDelegate">The application delegate.</param>
        public AutoSuspendHelper(NSApplicationDelegate appDelegate)
        {
            Reflection.ThrowIfMethodsNotOverloaded(
                "AutoSuspendHelper",
                appDelegate,
                "ApplicationShouldTerminate",
                "DidFinishLaunching",
                "DidResignActive",
                "DidBecomeActive",
                "DidHide");

            RxApp.SuspensionHost.IsLaunchingNew = Observable<Unit>.Never;
            RxApp.SuspensionHost.IsResuming = _isResuming;
            RxApp.SuspensionHost.IsUnpausing = _isUnpausing;
            RxApp.SuspensionHost.ShouldPersistState = _shouldPersistState;

            var untimelyDemise = new Subject<Unit>();
            AppDomain.CurrentDomain.UnhandledException += (o, e) =>
                untimelyDemise.OnNext(Unit.Default);

            RxApp.SuspensionHost.ShouldInvalidateState = untimelyDemise;
        }

        /// <summary>
        /// Applications the should terminate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <returns>The termination reply from the application.</returns>
        public NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
                _shouldPersistState.OnNext(Disposable.Create(() =>
                    sender.ReplyToApplicationShouldTerminate(true))));

            return NSApplicationTerminateReply.Later;
        }

        /// <summary>
        /// Dids the finish launching.
        /// </summary>
        /// <param name="notification">The notification.</param>
#pragma warning disable RCS1163 // Unused parameter.
        public void DidFinishLaunching(NSNotification notification) => _isResuming.OnNext(Unit.Default);

        /// <summary>
        /// Dids the resign active.
        /// </summary>
        /// <param name="notification">The notification.</param>
        public void DidResignActive(NSNotification notification) => _shouldPersistState.OnNext(Disposable.Empty);

        /// <summary>
        /// Dids the become active.
        /// </summary>
        /// <param name="notification">The notification.</param>
        public void DidBecomeActive(NSNotification notification) => _isUnpausing.OnNext(Unit.Default);

        /// <summary>
        /// Dids the hide.
        /// </summary>
        /// <param name="notification">The notification.</param>
        public void DidHide(NSNotification notification) => _shouldPersistState.OnNext(Disposable.Empty);
#pragma warning restore RCS1163 // Unused parameter.

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
                _isResuming?.Dispose();
                _isUnpausing?.Dispose();
                _shouldPersistState?.Dispose();
            }

            _isDisposed = true;
        }
    }
}
