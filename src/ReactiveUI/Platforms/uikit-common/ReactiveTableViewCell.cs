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
using UIKit;

namespace ReactiveUI
{
    /// <summary>
    /// This is a UITableViewCell that is both an UITableViewCell and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    [SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
    public abstract class ReactiveTableViewCell : UITableViewCell, IReactiveNotifyPropertyChanged<ReactiveTableViewCell>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        private Subject<Unit> _activated = new Subject<Unit>();
        private Subject<Unit> _deactivated = new Subject<Unit>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
        /// </summary>
        /// <param name="frame">The frame.</param>
        protected ReactiveTableViewCell(CGRect frame)
            : base(frame)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
        /// </summary>
        /// <param name="t">The object flag.</param>
        protected ReactiveTableViewCell(NSObjectFlag t)
            : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        [SuppressMessage("Redundancy", "CA1801: Redundant parameter", Justification = "Legacy interface")]
        protected ReactiveTableViewCell(NSCoder coder)
            : base(NSObjectFlag.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
        /// </summary>
        protected ReactiveTableViewCell()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
        /// </summary>
        /// <param name="style">The ui table view cell style.</param>
        /// <param name="reuseIdentifier">The reuse identifier.</param>
        protected ReactiveTableViewCell(UITableViewCellStyle style, string reuseIdentifier)
            : base(style, reuseIdentifier)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
        /// </summary>
        /// <param name="style">The ui table view cell style.</param>
        /// <param name="reuseIdentifier">The reuse identifier.</param>
        protected ReactiveTableViewCell(UITableViewCellStyle style, NSString reuseIdentifier)
            : base(style, reuseIdentifier)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected ReactiveTableViewCell(IntPtr handle)
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

        /// <inheritdoc />
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableViewCell>> Changing => this.GetChangingObservable();

        /// <inheritdoc />
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableViewCell>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        /// <inheritdoc/>
        public IObservable<Unit> Activated => _activated.AsObservable();

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated => _deactivated.AsObservable();

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
        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            (newsuper != null ? _activated : _deactivated).OnNext(Unit.Default);
        }

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
    /// This is a UITableViewCell that is both an UITableViewCell and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    [SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
    public abstract class ReactiveTableViewCell<TViewModel> : ReactiveTableViewCell, IViewFor<TViewModel>
        where TViewModel : class
    {
        private TViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
        /// </summary>
        /// <param name="frame">The frame.</param>
        protected ReactiveTableViewCell(CGRect frame)
            : base(frame)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
        /// </summary>
        /// <param name="t">The object flag.</param>
        protected ReactiveTableViewCell(NSObjectFlag t)
            : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        [SuppressMessage("Redundancy", "CA1801: Redundant parameter", Justification = "Legacy interface")]
        protected ReactiveTableViewCell(NSCoder coder)
            : base(NSObjectFlag.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
        /// </summary>
        protected ReactiveTableViewCell()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
        /// </summary>
        /// <param name="style">The ui table view cell style.</param>
        /// <param name="reuseIdentifier">The reuse identifier.</param>
        protected ReactiveTableViewCell(UITableViewCellStyle style, string reuseIdentifier)
            : base(style, reuseIdentifier)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
        /// </summary>
        /// <param name="style">The ui table view cell style.</param>
        /// <param name="reuseIdentifier">The reuse identifier.</param>
        protected ReactiveTableViewCell(UITableViewCellStyle style, NSString reuseIdentifier)
            : base(style, reuseIdentifier)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.
        /// </summary>
        /// <param name="handle">The pointer.</param>
        protected ReactiveTableViewCell(IntPtr handle)
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
