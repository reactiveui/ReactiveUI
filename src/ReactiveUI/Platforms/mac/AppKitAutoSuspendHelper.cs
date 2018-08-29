// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public class AutoSuspendHelper : IEnableLogger
    {
        private readonly Subject<IDisposable> _shouldPersistState = new Subject<IDisposable>();
        private readonly Subject<Unit> _isResuming = new Subject<Unit>();
        private readonly Subject<Unit> _isUnpausing = new Subject<Unit>();

        public AutoSuspendHelper(NSApplicationDelegate appDelegate)
        {
            Reflection.ThrowIfMethodsNotOverloaded("AutoSuspendHelper", appDelegate,
                "ApplicationShouldTerminate", "DidFinishLaunching", "DidResignActive", "DidBecomeActive", "DidHide");

            RxApp.SuspensionHost.IsLaunchingNew = Observable<Unit>.Never;
            RxApp.SuspensionHost.IsResuming = _isResuming;
            RxApp.SuspensionHost.IsUnpausing = _isUnpausing;
            RxApp.SuspensionHost.ShouldPersistState = _shouldPersistState;

            var untimelyDemise = new Subject<Unit>();
            AppDomain.CurrentDomain.UnhandledException += (o, e) =>
                untimelyDemise.OnNext(Unit.Default);

            RxApp.SuspensionHost.ShouldInvalidateState = untimelyDemise;
        }

        public NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
                _shouldPersistState.OnNext(Disposable.Create(() =>
                    sender.ReplyToApplicationShouldTerminate(true))));

            return NSApplicationTerminateReply.Later;
        }

        public void DidFinishLaunching(NSNotification notification)
        {
            _isResuming.OnNext(Unit.Default);
        }

        public void DidResignActive(NSNotification notification)
        {
            _shouldPersistState.OnNext(Disposable.Empty);
        }

        public void DidBecomeActive(NSNotification notification)
        {
            _isUnpausing.OnNext(Unit.Default);
        }

        public void DidHide(NSNotification notification)
        {
            _shouldPersistState.OnNext(Disposable.Empty);
        }
    }
}