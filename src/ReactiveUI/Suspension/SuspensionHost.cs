// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveUI
{
    internal class SuspensionHost : ReactiveObject, ISuspensionHost
    {
        private readonly ReplaySubject<IObservable<Unit>> _isLaunchingNew = new ReplaySubject<IObservable<Unit>>(1);

        public IObservable<Unit> IsLaunchingNew
        {
            get => _isLaunchingNew.Switch();
            set => _isLaunchingNew.OnNext(value);
        }

        private readonly ReplaySubject<IObservable<Unit>> _isResuming = new ReplaySubject<IObservable<Unit>>(1);

        public IObservable<Unit> IsResuming
        {
            get => _isResuming.Switch();
            set => _isResuming.OnNext(value);
        }

        private readonly ReplaySubject<IObservable<Unit>> _isUnpausing = new ReplaySubject<IObservable<Unit>>(1);

        public IObservable<Unit> IsUnpausing
        {
            get => _isUnpausing.Switch();
            set => _isUnpausing.OnNext(value);
        }

        private readonly ReplaySubject<IObservable<IDisposable>> _shouldPersistState = new ReplaySubject<IObservable<IDisposable>>(1);

        public IObservable<IDisposable> ShouldPersistState
        {
            get => _shouldPersistState.Switch();
            set => _shouldPersistState.OnNext(value);
        }

        private readonly ReplaySubject<IObservable<Unit>> _shouldInvalidateState = new ReplaySubject<IObservable<Unit>>(1);

        public IObservable<Unit> ShouldInvalidateState
        {
            get => _shouldInvalidateState.Switch();
            set => _shouldInvalidateState.OnNext(value);
        }

        /// <summary>
        ///
        /// </summary>
        public Func<object> CreateNewAppState { get; set; }

        private object _appState;

        /// <summary>
        ///
        /// </summary>
        public object AppState
        {
            get => _appState;
            set => this.RaiseAndSetIfChanged(ref _appState, value);
        }

        public SuspensionHost()
        {
#if COCOA
            var message = "Your AppDelegate class needs to use AutoSuspendHelper";
#elif ANDROID
            var message = "You need to create an App class and use AutoSuspendHelper";
#else
            var message = "Your App class needs to use AutoSuspendHelper";
#endif

            IsLaunchingNew = IsResuming = IsUnpausing = ShouldInvalidateState =
                Observable.Throw<Unit>(new Exception(message));

            ShouldPersistState = Observable.Throw<IDisposable>(new Exception(message));
        }
    }
}
