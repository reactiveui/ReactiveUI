using System;
using System.ComponentModel;
using System.Drawing;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace ReactiveUI.Cocoa
{
    public class ReactiveControl : UIControl, IReactiveNotifyPropertyChanged, IHandleObservableErrors, IReactiveObjectExtension, ICanActivate
    {
        protected ReactiveControl() : base()
        {
            this.setupReactiveExtension();
        }

        protected ReactiveControl(NSCoder c) : base(c)
        {
            this.setupReactiveExtension();
        }

        protected ReactiveControl(NSObjectFlag f) : base(f)
        {
            this.setupReactiveExtension();
        }

        protected ReactiveControl(IntPtr handle) : base(handle)
        {
            this.setupReactiveExtension();
        }

        protected ReactiveControl(RectangleF size) : base(size)
        {
            this.setupReactiveExtension();
        }

        public event PropertyChangingEventHandler PropertyChanging;

        void IReactiveObjectExtension.RaisePropertyChanging(PropertyChangingEventArgs args) 
        {
            var handler = PropertyChanging;
            if (handler != null) {
                handler(this, args);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void IReactiveObjectExtension.RaisePropertyChanged(PropertyChangedEventArgs args) 
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
        public IObservable<IObservedChange<object, object>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IObservedChange<object, object>> Changed {
            get { return this.getChangedObservable(); }
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

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        Subject<Unit> activated = new Subject<Unit>();
        public IObservable<Unit> Activated { get { return activated; } }
        Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated; } }

        #if UIKIT
        public override void WillMoveToSuperview(UIView newsuper)
        #else
        public override void ViewWillMoveToSuperview(NSView newsuper)
        #endif
        {
            #if UIKIT
            base.WillMoveToSuperview(newsuper);
            #else
            base.ViewWillMoveToSuperview(newsuper);
            #endif
            RxApp.MainThreadScheduler.Schedule(() => (newsuper != null ? activated : deactivated).OnNext(Unit.Default));
        }
    }
}