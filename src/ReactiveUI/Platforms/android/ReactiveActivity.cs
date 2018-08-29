// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// This is an Activity that is both an Activity and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    public class ReactiveActivity<TViewModel> : ReactiveActivity, IViewFor<TViewModel>, ICanActivate
        where TViewModel : class
    {
        private TViewModel _viewModel;

        /// <inheritdoc/>
        public TViewModel ViewModel
        {
            get { return _viewModel; }
            set { this.RaiseAndSetIfChanged(ref _viewModel, value); }
        }

        /// <inheritdoc/>
        object IViewFor.ViewModel
        {
            get { return _viewModel; }
            set { _viewModel = (TViewModel)value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveActivity{TViewModel}"/> class.
        /// </summary>
        protected ReactiveActivity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveActivity{TViewModel}"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="ownership">The ownership.</param>
        protected ReactiveActivity(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }
    }

    /// <summary>
    /// This is an Activity that is both an Activity and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    public class ReactiveActivity : Activity, IReactiveObject, IReactiveNotifyPropertyChanged<ReactiveActivity>, IHandleObservableErrors
    {
        private readonly Subject<Unit> _activated = new Subject<Unit>();
        private readonly Subject<Unit> _deactivated = new Subject<Unit>();


        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add { PropertyChangingEventManager.AddHandler(this, value); }
            remove { PropertyChangingEventManager.RemoveHandler(this, value); }
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { PropertyChangedEventManager.AddHandler(this, value); }
            remove { PropertyChangedEventManager.RemoveHandler(this, value); }
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveActivity>> Changing
        {
            get { return this.GetChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveActivity>> Changed
        {
            get { return this.GetChangedObservable(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveActivity"/> class.
        /// </summary>
        protected ReactiveActivity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveActivity"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="ownership">The ownership.</param>
        protected ReactiveActivity(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change
        /// notifications.</returns>
        public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions
        {
            get => this.GetThrownExceptionsObservable();
        }

        /// <summary>
        /// Gets a signal when the activity is activated.
        /// </summary>
        /// <value>
        /// The activated.
        /// </value>
        public IObservable<Unit> Activated
        {
            get => _activated.AsObservable();
        }

        /// <summary>
        ///  Gets a signal when the activity is deactivated.
        /// </summary>
        /// <value>
        /// The deactivated.
        /// </value>
        public IObservable<Unit> Deactivated
        {
            get { return _deactivated.AsObservable(); }
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

        private readonly Subject<Tuple<int, Result, Intent>> _activityResult = new Subject<Tuple<int, Result, Intent>>();

        /// <summary>
        /// Gets the activity result.
        /// </summary>
        /// <value>
        /// The activity result.
        /// </value>
        public IObservable<Tuple<int, Result, Intent>> ActivityResult
        {
            get { return _activityResult.AsObservable(); }
        }

        /// <inheritdoc/>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            _activityResult.OnNext(Tuple.Create(requestCode, resultCode, data));
        }

        /// <summary>
        /// Starts the activity for result asynchronously.
        /// </summary>
        /// <param name="intent">The intent.</param>
        /// <param name="requestCode">The request code.</param>
        /// <returns>A task with the result and the intent.</returns>
        public Task<Tuple<Result, Intent>> StartActivityForResultAsync(Intent intent, int requestCode)
        {
            // NB: It's important that we set up the subscription *before* we
            // call ActivityForResult
            var ret = ActivityResult
                .Where(x => x.Item1 == requestCode)
                .Select(x => Tuple.Create(x.Item2, x.Item3))
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
        public Task<Tuple<Result, Intent>> StartActivityForResultAsync(Type type, int requestCode)
        {
            // NB: It's important that we set up the subscription *before* we
            // call ActivityForResult
            var ret = ActivityResult
                .Where(x => x.Item1 == requestCode)
                .Select(x => Tuple.Create(x.Item2, x.Item3))
                .FirstAsync()
                .ToTask();

            StartActivityForResult(type, requestCode);
            return ret;
        }
    }
}
