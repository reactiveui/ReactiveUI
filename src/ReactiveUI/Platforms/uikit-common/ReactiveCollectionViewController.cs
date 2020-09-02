﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
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
    /// This is a UICollectionViewController that is both an UICollectionViewController and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    [SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    public abstract class ReactiveCollectionViewController : UICollectionViewController,
        IReactiveNotifyPropertyChanged<ReactiveCollectionViewController>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        private Subject<Unit> _activated = new Subject<Unit>();
        private Subject<Unit> _deactivated = new Subject<Unit>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewController"/> class.
        /// </summary>
        /// <param name="withLayout">The ui collection view layout.</param>
        protected ReactiveCollectionViewController(UICollectionViewLayout withLayout)
            : base(withLayout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewController"/> class.
        /// </summary>
        /// <param name="nibName">The name.</param>
        /// <param name="bundle">The bundle.</param>
        protected ReactiveCollectionViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewController"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected ReactiveCollectionViewController(IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewController"/> class.
        /// </summary>
        /// <param name="t">The object flag.</param>
        protected ReactiveCollectionViewController(NSObjectFlag t)
            : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewController"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        protected ReactiveCollectionViewController(NSCoder coder)
            : base(coder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewController"/> class.
        /// </summary>
        protected ReactiveCollectionViewController()
        {
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc />
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewController>> Changing => this.GetChangingObservable();

        /// <inheritdoc />
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewController>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        /// <inheritdoc/>
        public IObservable<Unit> Activated => _activated.AsObservable();

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated => _deactivated.AsObservable();

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChanging?.Invoke(this, args);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
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
    /// This is a UICollectionViewController that is both an UICollectionViewController and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    [SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    public abstract class ReactiveCollectionViewController<TViewModel> : ReactiveCollectionViewController, IViewFor<TViewModel>
        where TViewModel : class
    {
        private TViewModel? _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewController{TViewModel}"/> class.
        /// </summary>
        /// <param name="withLayout">The ui collection view layout.</param>
        protected ReactiveCollectionViewController(UICollectionViewLayout withLayout)
            : base(withLayout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewController{TViewModel}"/> class.
        /// </summary>
        /// <param name="nibName">The name.</param>
        /// <param name="bundle">The bundle.</param>
        protected ReactiveCollectionViewController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewController{TViewModel}"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected ReactiveCollectionViewController(IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewController{TViewModel}"/> class.
        /// </summary>
        /// <param name="t">The object flag.</param>
        protected ReactiveCollectionViewController(NSObjectFlag t)
            : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewController{TViewModel}"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        protected ReactiveCollectionViewController(NSCoder coder)
            : base(coder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveCollectionViewController{TViewModel}"/> class.
        /// </summary>
        protected ReactiveCollectionViewController()
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
