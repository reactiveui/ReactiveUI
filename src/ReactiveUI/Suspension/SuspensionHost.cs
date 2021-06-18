// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveUI
{
    /// <summary>
    /// A internal state setup by other classes for the different suspension state of a application.
    /// The user does not implement themselves but is often setup via the AutoSuspendHelper class.
    /// </summary>
    internal class SuspensionHost : ReactiveObject, ISuspensionHost, IDisposable
    {
        private readonly ReplaySubject<IObservable<Unit>> _isLaunchingNew = new(1);

        private readonly ReplaySubject<IObservable<Unit>> _isResuming = new(1);

        private readonly ReplaySubject<IObservable<Unit>> _isUnpausing = new(1);

        private readonly ReplaySubject<IObservable<IDisposable>> _shouldPersistState = new(1);

        private readonly ReplaySubject<IObservable<Unit>> _shouldInvalidateState = new(1);

        private object? _appState;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuspensionHost"/> class.
        /// </summary>
        public SuspensionHost()
        {
#if COCOA
            const string? message = "Your AppDelegate class needs to use AutoSuspendHelper";
#elif ANDROID
            const string? message = "You need to create an App class and use AutoSuspendHelper";
#else
            const string? message = "Your App class needs to use AutoSuspendHelper";
#endif

            IsLaunchingNew = IsResuming = IsUnpausing = ShouldInvalidateState =
                                                            Observable.Throw<Unit>(new Exception(message));

            ShouldPersistState = Observable.Throw<IDisposable>(new Exception(message));
        }

        /// <summary>
        /// Gets or sets a observable which notifies when the application is resuming.
        /// </summary>
        public IObservable<Unit> IsResuming // TODO: Create Test
        {
            get => _isResuming.Switch();
            set => _isResuming.OnNext(value);
        }

        /// <summary>
        /// Gets or sets a observable which notifies when the application is un-pausing.
        /// </summary>
        public IObservable<Unit> IsUnpausing // TODO: Create Test
        {
            get => _isUnpausing.Switch();
            set => _isUnpausing.OnNext(value);
        }

        /// <summary>
        /// Gets or sets a observable which notifies when the application should persist its state.
        /// </summary>
        public IObservable<IDisposable> ShouldPersistState // TODO: Create Test
        {
            get => _shouldPersistState.Switch();
            set => _shouldPersistState.OnNext(value);
        }

        /// <summary>
        /// Gets or sets a observable which notifies when a application is launching new.
        /// </summary>
        public IObservable<Unit> IsLaunchingNew // TODO: Create Test
        {
            get => _isLaunchingNew.Switch();
            set => _isLaunchingNew.OnNext(value);
        }

        /// <summary>
        /// Gets or sets a observable which notifies when the application state should be invalidated.
        /// </summary>
        public IObservable<Unit> ShouldInvalidateState // TODO: Create Test
        {
            get => _shouldInvalidateState.Switch();
            set => _shouldInvalidateState.OnNext(value);
        }

        /// <summary>
        /// Gets or sets a Func which will generate a fresh application state.
        /// </summary>
        public Func<object>? CreateNewAppState { get; set; }

        /// <summary>
        /// Gets or sets the application state that will be used when suspending and resuming the class.
        /// </summary>
        public object? AppState
        {
            get => _appState;
            set => this.RaiseAndSetIfChanged(ref _appState, value);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isLaunchingNew.Dispose();
                _isResuming.Dispose();
                _isUnpausing.Dispose();
                _shouldPersistState.Dispose();
                _shouldInvalidateState.Dispose();
            }
        }
    }
}
