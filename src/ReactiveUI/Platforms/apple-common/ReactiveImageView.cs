﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CoreGraphics;
using Foundation;

#if UIKIT
using UIKit;
using NSImage = UIKit.UIImage;
using NSImageView = UIKit.UIImageView;
using NSView = UIKit.UIView;
#else
using AppKit;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// This is an  ImageView that is both and ImageView and has a ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    [SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
    public abstract class ReactiveImageView : NSImageView, IReactiveNotifyPropertyChanged<ReactiveImageView>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
    {
        private Subject<Unit> _activated = new Subject<Unit>();
        private Subject<Unit> _deactivated = new Subject<Unit>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView"/> class.
        /// </summary>
        protected ReactiveImageView()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView"/> class.
        /// </summary>
        /// <param name="frame">The frame.</param>
        protected ReactiveImageView(CGRect frame)
            : base(frame)
        {
        }

#if UIKIT
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView"/> class.
        /// </summary>
        /// <param name="image">The image.</param>
        protected ReactiveImageView(NSImage image)
            : base(image)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView"/> class.
        /// </summary>
        /// <param name="t">The flag.</param>
        protected ReactiveImageView(NSObjectFlag t)
            : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView"/> class.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="highlightedImage">The higlighted image.</param>
        protected ReactiveImageView(NSImage image, NSImage highlightedImage)
            : base(image, highlightedImage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        protected ReactiveImageView(NSCoder coder)
            : base(coder)
        {
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected ReactiveImageView(IntPtr handle)
            : base(handle)
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
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        /// <inheritdoc/>
#pragma warning disable CS0108 // member hides inherited member
        public IObservable<Unit> Activated => _activated.AsObservable();
#pragma warning restore CS0108

        /// <inheritdoc/>
#pragma warning disable CS0108 // member hides inherited member
        public IObservable<Unit> Deactivated => _deactivated.AsObservable();
#pragma warning restore CS0108

        /// <inheritdoc />
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveImageView>> Changing => this.GetChangingObservable();

        /// <inheritdoc />
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveImageView>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

        /// <inheritdoc/>
        public IDisposable SuppressChangeNotifications()
        {
            return IReactiveObjectExtensions.SuppressChangeNotifications(this);
        }

#if UIKIT
        /// <inheritdoc/>
        public override void WillMoveToSuperview(NSView newsuper)
#else
        /// <inheritdoc/>
        public override void ViewWillMoveToSuperview(NSView newsuper)
#endif
        {
#if UIKIT
            base.WillMoveToSuperview(newsuper);
#else
            base.ViewWillMoveToSuperview(newsuper);
#endif
            (newsuper != null ? _activated : _deactivated).OnNext(Unit.Default);
        }

        /// <inheritdoc/>
        void ICanForceManualActivation.Activate(bool activate)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
                (activate ? _activated : _deactivated).OnNext(Unit.Default));
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
    /// This is an  ImageView that is both and ImageView and has a ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    [SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
    public abstract class ReactiveImageView<TViewModel> : ReactiveImageView, IViewFor<TViewModel>
        where TViewModel : class
    {
        private TViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.
        /// </summary>
        protected ReactiveImageView()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.
        /// </summary>
        /// <param name="frame">The frame.</param>
        protected ReactiveImageView(CGRect frame)
            : base(frame)
        {
        }

#if UIKIT
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.
        /// </summary>
        /// <param name="image">The image.</param>
        protected ReactiveImageView(NSImage image)
            : base(image)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.
        /// </summary>
        /// <param name="t">The object flag.</param>
        protected ReactiveImageView(NSObjectFlag t)
            : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="highlightedImage">The highlighted image.</param>
        protected ReactiveImageView(NSImage image, NSImage highlightedImage)
            : base(image, highlightedImage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        protected ReactiveImageView(NSCoder coder)
            : base(coder)
        {
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveImageView{TViewModel}"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected ReactiveImageView(IntPtr handle)
            : base(handle)
        {
        }

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
