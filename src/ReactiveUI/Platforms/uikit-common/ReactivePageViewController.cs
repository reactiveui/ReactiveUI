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
    public abstract class ReactivePageViewController : UIPageViewController,
        IReactiveNotifyPropertyChanged<ReactivePageViewController>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation)
            : base(style, orientation)
        {
            SetupRxObj();
        }

        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, NSDictionary options)
            : base(style, orientation, options)
        {
            SetupRxObj();
        }

        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, UIPageViewControllerSpineLocation spineLocation)
            : base(style, orientation, spineLocation)
        {
            SetupRxObj();
        }

        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, UIPageViewControllerSpineLocation spineLocation, float interPageSpacing)
            : base(style, orientation, spineLocation, interPageSpacing)
        {
            SetupRxObj();
        }

        protected ReactivePageViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
            SetupRxObj();
        }

        protected ReactivePageViewController(IntPtr handle)
            : base(handle)
        {
            SetupRxObj();
        }

        protected ReactivePageViewController(NSObjectFlag t)
            : base(t)
        {
            SetupRxObj();
        }

        protected ReactivePageViewController(NSCoder coder)
            : base(coder)
        {
            SetupRxObj();
        }

        protected ReactivePageViewController()
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
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePageViewController>> Changing => this.GetChangingObservable();

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePageViewController>> Changed => this.GetChangedObservable();

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

    public abstract class ReactivePageViewController<TViewModel> : ReactivePageViewController, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation)
            : base(style, orientation)
        {
        }

        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, NSDictionary options)
            : base(style, orientation, options)
        {
        }

        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, UIPageViewControllerSpineLocation spineLocation)
            : base(style, orientation, spineLocation)
        {
        }

        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, UIPageViewControllerSpineLocation spineLocation, float interPageSpacing)
            : base(style, orientation, spineLocation, interPageSpacing)
        {
        }

        protected ReactivePageViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
        }

        protected ReactivePageViewController(IntPtr handle)
            : base(handle)
        {
        }

        protected ReactivePageViewController(NSObjectFlag t)
            : base(t)
        {
        }

        protected ReactivePageViewController(NSCoder coder)
            : base(coder)
        {
        }

        protected ReactivePageViewController()
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
