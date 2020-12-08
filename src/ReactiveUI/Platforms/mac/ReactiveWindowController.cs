// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Subjects;
using AppKit;
using Foundation;

namespace ReactiveUI
{
    /// <summary>
    /// This is a NSWindowController that is both a NSWindowController and has ReactiveObject powers
    /// (i.e. you can call RaiseAndSetIfChanged).
    /// </summary>
    public class ReactiveWindowController : NSWindowController, IReactiveNotifyPropertyChanged<ReactiveWindowController>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        private readonly Subject<Unit> _activated = new();
        private readonly Subject<Unit> _deactivated = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveWindowController"/> class.
        /// </summary>
        /// <param name="window">The window.</param>
        protected ReactiveWindowController(NSWindow window)
            : base(window)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveWindowController"/> class.
        /// </summary>
        /// <param name="windowNibName">Name of the window nib.</param>
        protected ReactiveWindowController(string windowNibName)
            : base(windowNibName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveWindowController"/> class.
        /// </summary>
        /// <param name="windowNibName">Name of the window nib.</param>
        /// <param name="owner">The owner.</param>
        protected ReactiveWindowController(string windowNibName, NSObject owner)
            : base(windowNibName, owner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveWindowController"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        protected ReactiveWindowController(NSCoder coder)
            : base(coder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveWindowController"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        protected ReactiveWindowController(NSObjectFlag t)
            : base(t)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveWindowController"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        protected ReactiveWindowController(IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveWindowController"/> class.
        /// </summary>
        protected ReactiveWindowController()
        {
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc />
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveWindowController>> Changing => this.GetChangingObservable();

        /// <inheritdoc />
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveWindowController>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        /// <inheritdoc/>
        public IObservable<Unit> Activated => _activated;

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated => _deactivated;

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            var handler = PropertyChanging;
            if (handler is not null)
            {
                handler(this, args);
            }
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            if (handler is not null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change
        /// notifications.</returns>
        public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

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
}
