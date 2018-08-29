// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Preferences;
using Android.Runtime;

namespace ReactiveUI
{
    /// <summary>
    /// This is a PreferenceFragment that is both an Activity and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    public class ReactivePreferenceFragment<TViewModel> : ReactivePreferenceFragment, IViewFor<TViewModel>, ICanActivate
        where TViewModel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePreferenceFragment{TViewModel}"/> class.
        /// </summary>
        protected ReactivePreferenceFragment()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePreferenceFragment{TViewModel}"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="ownership">The ownership.</param>
        protected ReactivePreferenceFragment(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

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
    }

    /// <summary>
    /// This is a PreferenceFragment that is both an Activity and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    public class ReactivePreferenceFragment : PreferenceFragment, IReactiveNotifyPropertyChanged<ReactivePreferenceFragment>, IReactiveObject, IHandleObservableErrors
    {
        private readonly Subject<Unit> _activated = new Subject<Unit>();
        private readonly Subject<Unit> _deactivated = new Subject<Unit>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePreferenceFragment"/> class.
        /// </summary>
        protected ReactivePreferenceFragment()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePreferenceFragment"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="ownership">The ownership.</param>
        protected ReactivePreferenceFragment(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add => PropertyChangingEventManager.AddHandler(this, value);
            remove => PropertyChangingEventManager.RemoveHandler(this, value);
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

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceFragment>> Changing
        {
            get => this.GetChangingObservable();
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePreferenceFragment>> Changed
        {
            get => this.GetChangedObservable();
        }

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions
        {
            get => this.GetThrownExceptionsObservable();
        }

        /// <summary>
        /// Gets a signal when the fragment is activated.
        /// </summary>
        public IObservable<Unit> Activated
        {
            get => _activated.AsObservable();
        }

        /// <summary>
        /// Gets a signal when the fragment is deactivated.
        /// </summary>
        public IObservable<Unit> Deactivated
        {
            get => _deactivated.AsObservable();
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
        public override void OnPause()
        {
            base.OnPause();
            _deactivated.OnNext(Unit.Default);
        }

        /// <inheritdoc/>
        public override void OnResume()
        {
            base.OnResume();
            _activated.OnNext(Unit.Default);
        }
    }
}
