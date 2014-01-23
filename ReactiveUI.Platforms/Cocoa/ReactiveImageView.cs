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

#if UIKIT
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NSImageView = MonoTouch.UIKit.UIImageView;
using NSImage = MonoTouch.UIKit.UIImage;
#else
using MonoMac.AppKit;
#endif


namespace ReactiveUI.Cocoa
{
    public abstract class ReactiveImageView : NSImageView, IReactiveNotifyPropertyChanged, IHandleObservableErrors, IReactiveObjectExtension
    {
        public ReactiveImageView(RectangleF frame) : base(frame) { this.setupReactiveExtension(); }
        public ReactiveImageView(IntPtr handle) : base(handle) { this.setupReactiveExtension(); }
        public ReactiveImageView() { this.setupReactiveExtension(); }

#if UIKIT
        public ReactiveImageView(NSImage image) : base(image) { this.setupReactiveExtension(); }
        public ReactiveImageView(NSObjectFlag t) : base(t) { this.setupReactiveExtension(); }
        public ReactiveImageView(NSImage image, NSImage highlightedImage) : base(image, highlightedImage) { this.setupReactiveExtension(); }
        public ReactiveImageView(NSCoder coder) : base(coder) { this.setupReactiveExtension(); }
#endif

        [field:IgnoreDataMember]
        public event PropertyChangingEventHandler PropertyChanging;

        void IReactiveObjectExtension.RaisePropertyChanging(PropertyChangingEventArgs args) {
            var handler = PropertyChanging;
            if (handler != null) {
                handler(this, args);
            }
        }

        [field:IgnoreDataMember]
        public event PropertyChangedEventHandler PropertyChanged;

        void IReactiveObjectExtension.RaisePropertyChanged(PropertyChangedEventArgs args) {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, args);
            }
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.         
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IObservedChange<object, object>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IObservedChange<object, object>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IDisposable SuppressChangeNotifications() {
            return this.suppressChangeNotifications();
        }

        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        [OnDeserialized]
        void setupRxObj(StreamingContext sc) { this.setupReactiveExtension(); }
    }
}

