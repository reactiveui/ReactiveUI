using System;
using ReactiveUI;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Reflection;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Linq;
using System.Threading;
using System.Reactive.Disposables;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Drawing;
using Splat;
using System.Reactive;

#if UIKIT
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NSImageView = MonoTouch.UIKit.UIImageView;
using NSImage = MonoTouch.UIKit.UIImage;
using NSView = MonoTouch.UIKit.UIView;
#else
using MonoMac.AppKit;
#endif


namespace ReactiveUI.Cocoa
{
    public abstract class ReactiveImageView : NSImageView, IReactiveNotifyPropertyChanged<ReactiveImageView>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        public ReactiveImageView(RectangleF frame) : base(frame) { }
        public ReactiveImageView(IntPtr handle) : base(handle) { }
        public ReactiveImageView() { }

#if UIKIT
        public ReactiveImageView(NSImage image) : base(image) { }
        public ReactiveImageView(NSObjectFlag t) : base(t) { }
        public ReactiveImageView(NSImage image, NSImage highlightedImage) : base(image, highlightedImage) { }
        public ReactiveImageView(NSCoder coder) : base(coder) { }
#endif

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
        public IObservable<IObservedChange<ReactiveImageView, object>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IObservedChange<ReactiveImageView, object>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IDisposable SuppressChangeNotifications() {
            return this.suppressChangeNotifications();
        }

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }
        
        Subject<Unit> activated = new Subject<Unit>();
        public IObservable<Unit> Activated { get { return activated; } }
        Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated; } }

#if UIKIT
        public override void WillMoveToSuperview(NSView newsuper)
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

