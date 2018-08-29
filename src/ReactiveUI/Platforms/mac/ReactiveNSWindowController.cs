// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Subjects;
using AppKit;
using Foundation;

namespace ReactiveUI
{
    public class ReactiveWindowController : NSWindowController, IReactiveNotifyPropertyChanged<ReactiveWindowController>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        protected ReactiveWindowController(NSWindow window)
            : base(window)
        {
            SetupRxObj();
        }

        protected ReactiveWindowController(string windowNibName)
            : base(windowNibName)
        {
            SetupRxObj();
        }

        protected ReactiveWindowController(string windowNibName, NSObject owner)
            : base(windowNibName, owner)
        {
            SetupRxObj();
        }

        protected ReactiveWindowController(NSCoder coder)
            : base(coder)
        {
            SetupRxObj();
        }

        protected ReactiveWindowController(NSObjectFlag t)
            : base(t)
        {
            SetupRxObj();
        }

        protected ReactiveWindowController(IntPtr handle)
            : base(handle)
        {
            SetupRxObj();
        }

        protected ReactiveWindowController()
        {
            SetupRxObj();
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveWindowController>> Changing
        {
            get { return this.GetChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveWindowController>> Changed
        {
            get { return this.GetChangedObservable(); }
        }

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions
        {
            get { return this.GetThrownExceptionsObservable(); }
        }

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

        private readonly Subject<Unit> _activated = new Subject<Unit>();

        /// <inheritdoc/>
        public IObservable<Unit> Activated
        {
            get { return _activated; }
        }

        private readonly Subject<Unit> _deactivated = new Subject<Unit>();

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated
        {
            get { return _deactivated; }
        }

        /// <inheritdoc/>
        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            // subscribe to listen to window closing
            // notification to support (de)activation
            NSNotificationCenter
                .DefaultCenter
                .AddObserver(NSWindow.WillCloseNotification, _ => _deactivated.OnNext(Unit.Default), Window);

            _activated.OnNext(Unit.Default);
        }
    }
}
