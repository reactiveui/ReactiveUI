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
using NSView = UIKit.UIView;
#else
using AppKit;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// This is an View that is both an NSView and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    public class ReactiveView : NSView, IReactiveNotifyPropertyChanged<ReactiveView>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
    {
        protected ReactiveView()
        {
        }

        protected ReactiveView(NSCoder c)
            : base(c)
        {
        }

        protected ReactiveView(NSObjectFlag f)
            : base(f)
        {
        }

        protected ReactiveView(IntPtr handle)
            : base(handle)
        {
        }

        protected ReactiveView(CGRect frame)
            : base(frame)
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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveView>> Changing => this.GetChangingObservable();

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveView>> Changed => this.GetChangedObservable();

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
            // Xamarin throws ArgumentNullException if newsuper is null
            if (newsuper != null)
            {
                base.ViewWillMoveToSuperview(newsuper);
            }
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

    public abstract class ReactiveView<TViewModel> : ReactiveView, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactiveView()
        {
        }

        protected ReactiveView(NSCoder c)
            : base(c)
        {
        }

        protected ReactiveView(NSObjectFlag f)
            : base(f)
        {
        }

        protected ReactiveView(IntPtr handle)
            : base(handle)
        {
        }

        protected ReactiveView(CGRect frame)
            : base(frame)
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
