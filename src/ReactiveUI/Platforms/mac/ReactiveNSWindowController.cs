using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Reactive;
using AppKit;
using Foundation;

namespace ReactiveUI
{
    public class ReactiveWindowController : NSWindowController, IReactiveNotifyPropertyChanged<ReactiveWindowController>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        protected ReactiveWindowController(NSWindow window) : base(window) { setupRxObj(); }
        protected ReactiveWindowController(string windowNibName) : base(windowNibName) { setupRxObj(); }
        protected ReactiveWindowController(string windowNibName, NSObject owner) : base(windowNibName, owner) { setupRxObj(); }
        protected ReactiveWindowController(NSCoder coder) : base(coder) { setupRxObj(); }
        protected ReactiveWindowController(NSObjectFlag t) : base(t) { setupRxObj(); }
        protected ReactiveWindowController(IntPtr handle) : base(handle) { setupRxObj(); }
        protected ReactiveWindowController() { setupRxObj(); }

        public event PropertyChangingEventHandler PropertyChanging;

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            var handler = PropertyChanging;
            if (handler != null) {
                handler(this, args);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, args);
            }
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.         
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveWindowController>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveWindowController>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        void setupRxObj()
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
            return this.suppressChangeNotifications();
        }

        readonly Subject<Unit> activated = new Subject<Unit>();
        public IObservable<Unit> Activated { get { return activated; } }
        readonly Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated; } }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            // subscribe to listen to window closing
            // notification to support (de)activation
            NSNotificationCenter
                .DefaultCenter
                .AddObserver(NSWindow.WillCloseNotification, _ => deactivated.OnNext(Unit.Default), this.Window);

            activated.OnNext(Unit.Default);
        }
    }
}
