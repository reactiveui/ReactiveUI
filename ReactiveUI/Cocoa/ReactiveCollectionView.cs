﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using ReactiveUI;
using Splat;

#if UNIFIED
using CoreGraphics;
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

namespace ReactiveUI
{
    public abstract class ReactiveCollectionView : UICollectionView, 
        IReactiveNotifyPropertyChanged<ReactiveCollectionView>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
    {
#if UNIFIED
        protected ReactiveCollectionView(CGRect frame, UICollectionViewLayout layout) : base(frame, layout) { setupRxObj(); }
#else
        protected ReactiveCollectionView(RectangleF frame, UICollectionViewLayout layout) : base(frame, layout) { setupRxObj(); }
#endif

        protected ReactiveCollectionView(IntPtr handle) : base(handle) { setupRxObj(); }
        protected ReactiveCollectionView(NSObjectFlag t) : base(t) { setupRxObj(); }
        protected ReactiveCollectionView(NSCoder coder) : base(coder) { setupRxObj(); }

        public event PropertyChangingEventHandler PropertyChanging {
            add { PropertyChangingEventManager.AddHandler(this, value); }
            remove { PropertyChangingEventManager.RemoveHandler(this, value); }
        }

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        public event PropertyChangedEventHandler PropertyChanged {
            add { PropertyChangedEventManager.AddHandler(this, value); }
            remove { PropertyChangedEventManager.RemoveHandler(this, value); }
        }

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionView>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionView>> Changed {
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

        Subject<Unit> activated = new Subject<Unit>();
        public IObservable<Unit> Activated { get { return activated; } }
        Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated; } }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            activated.OnNext(Unit.Default);
        }

        public override void RemoveFromSuperview()
        {
            base.RemoveFromSuperview();
            deactivated.OnNext(Unit.Default);
        }

        void ICanForceManualActivation.Activate(bool activate) 
        {
            RxApp.MainThreadScheduler.Schedule(() => 
		        (activate ? activated : deactivated).OnNext(Unit.Default));
        }
    }
}
