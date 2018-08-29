// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using UIKit;

namespace ReactiveUI
{
    public abstract class ReactiveNavigationController : UINavigationController,
    IReactiveNotifyPropertyChanged<ReactiveNavigationController>, IHandleObservableErrors, IReactiveObject, ICanActivate, IActivatable
    {
        protected ReactiveNavigationController(UIViewController rootViewController)
            : base(rootViewController)
        {
            SetupRxObj();
        }

        protected ReactiveNavigationController(Type navigationBarType, Type toolbarType)
            : base(navigationBarType, toolbarType)
        {
            SetupRxObj();
        }

        protected ReactiveNavigationController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
            SetupRxObj();
        }

        protected ReactiveNavigationController(IntPtr handle)
            : base(handle)
        {
            SetupRxObj();
        }

        protected ReactiveNavigationController(NSObjectFlag t)
            : base(t)
        {
            SetupRxObj();
        }

        protected ReactiveNavigationController(NSCoder coder)
            : base(coder)
        {
            SetupRxObj();
        }

        protected ReactiveNavigationController()
        {
            SetupRxObj();
        }

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveNavigationController>> Changing => this.GetChangingObservable();

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveNavigationController>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        private void SetupRxObj()
        {
        }

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change
        /// notifications.</returns>
        public IDisposable SuppressChangeNotifications()
        {
            return IReactiveObjectExtensions.SuppressChangeNotifications(this);
        }

        private Subject<Unit> _activated = new Subject<Unit>();

        /// <inheritdoc/>
        public IObservable<Unit> Activated => _activated.AsObservable();

        private Subject<Unit> _deactivated = new Subject<Unit>();

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated => _deactivated.AsObservable();

        /// <inheritdoc/>
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _activated.OnNext(Unit.Default);
            this.ActivateSubviews(true);
        }

        /// <inheritdoc/>
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _deactivated.OnNext(Unit.Default);
            this.ActivateSubviews(false);
        }
    }

    public abstract class ReactiveNavigationController<TViewModel> : ReactiveNavigationController, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactiveNavigationController(UIViewController rootViewController)
            : base(rootViewController)
        {
        }

        protected ReactiveNavigationController(Type navigationBarType, Type toolbarType)
            : base(navigationBarType, toolbarType)
        {
        }

        protected ReactiveNavigationController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
        }

        protected ReactiveNavigationController(IntPtr handle)
            : base(handle)
        {
        }

        protected ReactiveNavigationController(NSObjectFlag t)
            : base(t)
        {
        }

        protected ReactiveNavigationController(NSCoder coder)
            : base(coder)
        {
        }

        protected ReactiveNavigationController()
        {
        }

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
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }
    }
}
