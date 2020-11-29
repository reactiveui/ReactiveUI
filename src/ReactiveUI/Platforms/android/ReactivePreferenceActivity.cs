// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Preferences;
using Android.Runtime;

namespace ReactiveUI
{
    /// <summary>
    /// This is an Activity that is both an Activity and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    [Obsolete("This class is obsoleted in this android platform")]
    public class ReactivePreferenceActivity<TViewModel> : ReactivePreferenceActivity, IViewFor<TViewModel>, ICanActivate
        where TViewModel : class
    {
        private TViewModel? _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePreferenceActivity{TViewModel}"/> class.
        /// </summary>
        protected ReactivePreferenceActivity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePreferenceActivity{TViewModel}"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="ownership">The ownership.</param>
        protected ReactivePreferenceActivity(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        /// <inheritdoc/>
        public TViewModel? ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => _viewModel;
            set => _viewModel = (TViewModel?)value!;
        }
    }

    /// <summary>
    /// This is an Activity that is both an Activity and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    [Obsolete("This class is obsoleted in this android platform")]
    public class ReactivePreferenceActivity : PreferenceActivity, IReactiveObject, IReactiveNotifyPropertyChanged<ReactivePreferenceActivity>, IHandleObservableErrors
    {
        private readonly Subject<Unit> _activated = new Subject<Unit>();
        private readonly Subject<Unit> _deactivated = new Subject<Unit>();
        private readonly Subject<(int requestCode, Result resultCode, Intent? intent)> _activityResult = new Subject<(int requestCode, Result resultCode, Intent? intent)>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePreferenceActivity"/> class.
        /// </summary>
        protected ReactivePreferenceActivity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePreferenceActivity"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        /// <param name="ownership">The ownership.</param>
        protected ReactivePreferenceActivity(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc />
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceActivity>> Changing => this.GetChangingObservable();

        /// <inheritdoc />
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceActivity>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        /// <summary>
        ///  Gets a signal when the activity is activated.
        /// </summary>
        /// <value>
        /// The deactivated.
        /// </value>
        public IObservable<Unit> Activated => _activated.AsObservable();

        /// <summary>
        ///  Gets a signal when the activity is deactivated.
        /// </summary>
        /// <value>
        /// The deactivated.
        /// </value>
        public IObservable<Unit> Deactivated => _deactivated.AsObservable();

        /// <summary>
        ///  Gets a signal with an activity result.
        /// </summary>
        /// <value>
        /// The deactivated.
        /// </value>
        public IObservable<(int requestCode, Result resultCode, Intent? intent)> ActivityResult => _activityResult.AsObservable();

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, re-enables change
        /// notifications.</returns>
        public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

        /// <summary>
        /// Starts the activity for result asynchronously.
        /// </summary>
        /// <param name="intent">The intent.</param>
        /// <param name="requestCode">The request code.</param>
        /// <returns>A task with the result and intent.</returns>
        public Task<(Result resultCode, Intent? intent)> StartActivityForResultAsync(Intent? intent, int requestCode)
        {
            // NB: It's important that we set up the subscription *before* we
            // call ActivityForResult
            var ret = ActivityResult
                .Where(x => x.requestCode == requestCode)
                .Select(x => (x.resultCode, x.intent))
                .FirstAsync()
                .ToTask();

            StartActivityForResult(intent, requestCode);
            return ret;
        }

        /// <summary>
        /// Starts the activity for result asynchronously.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="requestCode">The request code.</param>
        /// <returns>A task with the result and intent.</returns>
        public Task<(Result resultCode, Intent? intent)> StartActivityForResultAsync(Type type, int requestCode)
        {
            // NB: It's important that we set up the subscription *before* we
            // call ActivityForResult
            var ret = ActivityResult
                .Where(x => x.requestCode == requestCode)
                .Select(x => (x.resultCode, x.intent))
                .FirstAsync()
                .ToTask();

            StartActivityForResult(type, requestCode);
            return ret;
        }

        /// <inheritdoc/>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            _activityResult.OnNext((requestCode, resultCode, data));
        }

        /// <inheritdoc/>
        protected override void OnPause()
        {
            base.OnPause();
            _deactivated.OnNext(Unit.Default);
        }

        /// <inheritdoc/>
        protected override void OnResume()
        {
            base.OnResume();
            _activated.OnNext(Unit.Default);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _activated?.Dispose();
                _deactivated?.Dispose();
                _activityResult?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
