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
using UIKit;

namespace ReactiveUI
{
    /// <summary>
    /// This is a TableView that is both an TableView and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    public abstract class ReactiveTableView : UITableView, IReactiveNotifyPropertyChanged<ReactiveTableView>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
    {
        private Subject<Unit> _activated = new Subject<Unit>();
        private Subject<Unit> _deactivated = new Subject<Unit>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableView"/> class.
        /// </summary>
        protected ReactiveTableView()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableView"/> class.
        /// </summary>
        /// <param name="t">The object flag.</param>
        protected ReactiveTableView(NSObjectFlag t)
            : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableView"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        protected ReactiveTableView(NSCoder coder)
            : base(coder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableView"/> class.
        /// </summary>
        /// <param name="frame">The frame.</param>
        protected ReactiveTableView(CGRect frame)
            : base(frame)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableView"/> class.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <param name="style">The table view style.</param>
        protected ReactiveTableView(CGRect frame, UITableViewStyle style)
            : base(frame, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableView"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected ReactiveTableView(IntPtr handle)
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

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableView>> Changing => this.GetChangingObservable();

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableView>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        /// <inheritdoc/>
        public IObservable<Unit> Activated => _activated.AsObservable();

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated => _deactivated.AsObservable();

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

        /// <inheritdoc/>
        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            (newsuper != null ? _activated : _deactivated).OnNext(Unit.Default);
        }

        /// <inheritdoc/>
        void ICanForceManualActivation.Activate(bool activate)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
                (activate ? _activated : _deactivated).OnNext(Unit.Default));
        }
    }

    /// <summary>
    /// This is a TableView that is both an TableView and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    public abstract class ReactiveTableView<TViewModel> : ReactiveTableView, IViewFor<TViewModel>
        where TViewModel : class
    {
        private TViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableView{TViewModel}"/> class.
        /// </summary>
        protected ReactiveTableView()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableView{TViewModel}"/> class.
        /// </summary>
        /// <param name="t">The object flag.</param>
        protected ReactiveTableView(NSObjectFlag t)
            : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableView{TViewModel}"/> class.
        /// </summary>
        /// <param name="coder">The pointer.</param>
        protected ReactiveTableView(NSCoder coder)
            : base(coder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableView{TViewModel}"/> class.
        /// </summary>
        /// <param name="frame">The frame.</param>
        protected ReactiveTableView(CGRect frame)
            : base(frame)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableView{TViewModel}"/> class.
        /// </summary>
        /// <param name="frame">The frmae.</param>
        /// <param name="style">The ui view style.</param>
        protected ReactiveTableView(CGRect frame, UITableViewStyle style)
            : base(frame, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableView{TViewModel}"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected ReactiveTableView(IntPtr handle)
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
