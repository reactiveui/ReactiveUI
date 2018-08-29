// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;

#if UIKIT
using UIKit;
using NSSplitViewController = UIKit.UISplitViewController;
using NSView = UIKit.UIView;
#else
using AppKit;
#endif

namespace ReactiveUI
{
    public abstract class ReactiveSplitViewController : NSSplitViewController,
    IReactiveNotifyPropertyChanged<ReactiveSplitViewController>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
#if UIKIT
        protected ReactiveSplitViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
            SetupRxObj();
        }

#endif
        protected ReactiveSplitViewController(IntPtr handle)
            : base(handle)
        {
            SetupRxObj();
        }

        protected ReactiveSplitViewController(NSObjectFlag t)
            : base(t)
        {
            SetupRxObj();
        }

        protected ReactiveSplitViewController(NSCoder coder)
            : base(coder)
        {
            SetupRxObj();
        }

        protected ReactiveSplitViewController()
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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveSplitViewController>> Changing => this.GetChangingObservable();

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveSplitViewController>> Changed => this.GetChangedObservable();

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

#if UIKIT
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
#else
        /// <inheritdoc/>
        public override void ViewWillAppear()
        {
            base.ViewWillAppear();
            _activated.OnNext(Unit.Default);
            this.ActivateSubviews(true);
        }

        /// <inheritdoc/>
        public override void ViewDidDisappear()
        {
            base.ViewDidDisappear();
            _deactivated.OnNext(Unit.Default);
            this.ActivateSubviews(false);
        }
#endif
    }

    public abstract class ReactiveSplitViewController<TViewModel> : ReactiveSplitViewController, IViewFor<TViewModel>
        where TViewModel : class
    {
#if UIKIT
        protected ReactiveSplitViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
        }

#endif
        protected ReactiveSplitViewController(IntPtr handle)
            : base(handle)
        {
        }

        protected ReactiveSplitViewController(NSObjectFlag t)
            : base(t)
        {
        }

        protected ReactiveSplitViewController(NSCoder coder)
            : base(coder)
        {
        }

        protected ReactiveSplitViewController()
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
