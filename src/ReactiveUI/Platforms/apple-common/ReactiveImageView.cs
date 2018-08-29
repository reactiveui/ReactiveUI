// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
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
    public abstract class ReactiveImageView : NSImageView, IReactiveNotifyPropertyChanged<ReactiveImageView>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
    {
        protected ReactiveImageView()
        {
        }

        protected ReactiveImageView(CGRect frame)
            : base(frame)
        {
        }

#if UIKIT
        protected ReactiveImageView(NSImage image)
            : base(image)
        {
        }

        protected ReactiveImageView(NSObjectFlag t)
            : base(t)
        {
        }

        protected ReactiveImageView(NSImage image, NSImage highlightedImage)
            : base(image, highlightedImage)
        {
        }

        protected ReactiveImageView(NSCoder coder)
            : base(coder)
        {
        }
#endif

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveImageView>> Changing => this.GetChangingObservable();

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveImageView>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        public IDisposable SuppressChangeNotifications()
        {
            return IReactiveObjectExtensions.SuppressChangeNotifications(this);
        }

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        private Subject<Unit> _activated = new Subject<Unit>();

        /// <inheritdoc/>
        public IObservable<Unit> Activated => _activated.AsObservable();

        private Subject<Unit> _deactivated = new Subject<Unit>();

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated => _deactivated.AsObservable();

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
    }

    public abstract class ReactiveImageView<TViewModel> : ReactiveImageView, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactiveImageView()
        {
        }

        protected ReactiveImageView(CGRect frame)
            : base(frame)
        {
        }

#if UIKIT
        protected ReactiveImageView(NSImage image)
            : base(image)
        {
        }

        protected ReactiveImageView(NSObjectFlag t)
            : base(t)
        {
        }

        protected ReactiveImageView(NSImage image, NSImage highlightedImage)
            : base(image, highlightedImage)
        {
        }

        protected ReactiveImageView(NSCoder coder)
            : base(coder)
        {
        }
#endif

        protected ReactiveImageView(IntPtr handle)
            : base(handle)
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
