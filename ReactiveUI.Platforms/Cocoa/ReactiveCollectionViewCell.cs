using System;
using System.Drawing;
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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Splat;

namespace ReactiveUI.Cocoa
{
    public abstract class ReactiveCollectionViewCell : UICollectionViewCell, IReactiveNotifyPropertyChanged, IHandleObservableErrors, IReactiveObjectExtension
    {
        public ReactiveCollectionViewCell(IntPtr handle) : base (handle) { setupRxObj(); }
        public ReactiveCollectionViewCell(NSObjectFlag t) : base (t) { setupRxObj(); }
        public ReactiveCollectionViewCell(NSCoder coder) : base (NSObjectFlag.Empty) { setupRxObj(); }
        public ReactiveCollectionViewCell() : base() { setupRxObj(); }
        public ReactiveCollectionViewCell(RectangleF frame) : base (frame) { setupRxObj(); }

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

        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        [OnDeserialized]
        void setupRxObj(StreamingContext sc) { setupRxObj(); }

        void setupRxObj()
        {
            this.setupReactiveExtension();
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
    }
}
