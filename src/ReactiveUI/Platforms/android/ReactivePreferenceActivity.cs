// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
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
    public class ReactivePreferenceActivity<TViewModel> : ReactivePreferenceActivity, IViewFor<TViewModel>, ICanActivate
        where TViewModel : class
    {
        private TViewModel _viewModel;

        /// <inheritdoc/>
        public TViewModel ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        /// <inheritdoc/>
        object IViewFor.ViewModel
        {
            get => _viewModel;
            set => _viewModel = (TViewModel)value;
        }

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
    }

    /// <summary>
    /// This is an Activity that is both an Activity and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    public class ReactivePreferenceActivity : PreferenceActivity, IReactiveObject, IReactiveNotifyPropertyChanged<ReactivePreferenceActivity>, IHandleObservableErrors
    {
        private readonly Subject<Unit> _activated = new Subject<Unit>();
        private readonly Subject<Unit> _deactivated = new Subject<Unit>();
        private readonly Subject<Tuple<int, Result, Intent>> _activityResult = new Subject<Tuple<int, Result, Intent>>();

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add => PropertyChangingEventManager.AddHandler(this, value);
            remove => PropertyChangingEventManager.RemoveHandler(this, value);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => PropertyChangedEventManager.AddHandler(this, value);
            remove => PropertyChangedEventManager.RemoveHandler(this, value);
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
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceActivity>> Changing
        {
            get => this.GetChangingObservable();
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceActivity>> Changed
        {
            get => this.GetChangedObservable();
        }

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions
        {
            get => this.GetThrownExceptionsObservable();
        }

        /// <summary>
        ///  Gets a signal when the activity is activated.
        /// </summary>
        /// <value>
        /// The deactivated.
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
            get => _deactivated.AsObservable();
        }

        /// <summary>
        ///  Gets a signal with an activity result.
        /// </summary>
        /// <value>
        /// The deactivated.
        /// </value>
        public IObservable<Tuple<int, Result, Intent>> ActivityResult
        {
            get => _activityResult.AsObservable();
        }

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

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change
        /// notifications.</returns>
        public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

        /// <summary>
        /// Starts the activity for result asynchronously.
        /// </summary>
        /// <param name="intent">The intent.</param>
        /// <param name="requestCode">The request code.</param>
        /// <returns>A task with the result and intent.</returns>
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

        /// <inheritdoc/>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            _activityResult.OnNext(Tuple.Create(requestCode, resultCode, data));
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
    }
}
