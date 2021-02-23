// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using UIKit;

namespace ReactiveUI
{
    /// <summary>
    /// This is a UIPageViewController that is both an UIPageViewController and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    [SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    public abstract class ReactivePageViewController : UIPageViewController, IReactiveNotifyPropertyChanged<ReactivePageViewController>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        private Subject<Unit> _activated = new();
        private Subject<Unit> _deactivated = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="orientation">The orientation.</param>
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation)
            : base(style, orientation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="orientation">The orientation.</param>
        /// <param name="options">The options.</param>
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, NSDictionary options)
            : base(style, orientation, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="orientation">The orientation.</param>
        /// <param name="spineLocation">The spine location.</param>
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, UIPageViewControllerSpineLocation spineLocation)
            : base(style, orientation, spineLocation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="orientation">The orientation.</param>
        /// <param name="spineLocation">The spine location.</param>
        /// <param name="interPageSpacing">The inter page spacing.</param>
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, UIPageViewControllerSpineLocation spineLocation, float interPageSpacing)
            : base(style, orientation, spineLocation, interPageSpacing)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController"/> class.
        /// </summary>
        /// <param name="nibName">Name of the nib.</param>
        /// <param name="bundle">The bundle.</param>
        protected ReactivePageViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        protected ReactivePageViewController(IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        protected ReactivePageViewController(NSObjectFlag t)
            : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        protected ReactivePageViewController(NSCoder coder)
            : base(coder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController"/> class.
        /// </summary>
        protected ReactivePageViewController()
        {
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc />
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePageViewController>> Changing => this.GetChangingObservable();

        /// <inheritdoc />
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePageViewController>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        /// <inheritdoc/>
        public IObservable<Unit> Activated => _activated.AsObservable();

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated => _deactivated.AsObservable();

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change
        /// notifications.</returns>
        public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

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

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _activated?.Dispose();
                _deactivated?.Dispose();
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// This is a UIPageViewController that is both an UIPageViewController and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    [SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    public abstract class ReactivePageViewController<TViewModel> : ReactivePageViewController, IViewFor<TViewModel>
        where TViewModel : class
    {
        private TViewModel? _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.
        /// </summary>
        /// <param name="style">The view controller transition style.</param>
        /// <param name="orientation">The view controller navigation orientation.</param>
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation)
            : base(style, orientation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.
        /// </summary>
        /// <param name="style">The view controller transition style.</param>
        /// <param name="orientation">The view controller navigation orientation.</param>
        /// <param name="options">The options.</param>
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, NSDictionary options)
            : base(style, orientation, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.
        /// </summary>
        /// <param name="style">The view controller transition style.</param>
        /// <param name="orientation">The view controller navigation orientation.</param>
        /// <param name="spineLocation">The view controller spine location.</param>
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, UIPageViewControllerSpineLocation spineLocation)
            : base(style, orientation, spineLocation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.
        /// </summary>
        /// <param name="style">The view controller transition style.</param>
        /// <param name="orientation">The view controller navigation orientation.</param>
        /// <param name="spineLocation">The view controller spine location.</param>
        /// <param name="interPageSpacing">The spacing between pages.</param>
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, UIPageViewControllerSpineLocation spineLocation, float interPageSpacing)
            : base(style, orientation, spineLocation, interPageSpacing)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.
        /// </summary>
        /// <param name="nibName">The name.</param>
        /// <param name="bundle">The bundle.</param>
        protected ReactivePageViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected ReactivePageViewController(IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.
        /// </summary>
        /// <param name="t">The object flag.</param>
        protected ReactivePageViewController(NSObjectFlag t)
            : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        protected ReactivePageViewController(NSCoder coder)
            : base(coder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.
        /// </summary>
        protected ReactivePageViewController()
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
            get => ViewModel;
            set => ViewModel = (TViewModel)value!;
        }
    }
}
