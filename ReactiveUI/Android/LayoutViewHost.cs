using System;
using System.Runtime.Serialization;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Reactive.Disposables;
using System.Diagnostics.Contracts;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Android.Content;
using Splat;
using Android.Views;
using Android.App;

namespace ReactiveUI
{
    public interface ILayoutViewHost
    {
        View View { get; }
    }

    public static class ViewMixins
    {
        internal const int viewHostTag = -4222;

        /// <summary>
        /// Gets the ViewHost associated with a given View by accessing the
        /// Tag of the View
        /// </summary>
        /// <returns>The view host.</returns>
        public static T GetViewHost<T>(this View This) where T : ILayoutViewHost
        {
            var tagData = This.GetTag(viewHostTag);
            if (tagData != null) return tagData.ToNetObject<T>();

            return default(T);
        }
                
        /// <summary>
        /// Gets the ViewHost associated with a given View by accessing the
        /// Tag of the View
        /// </summary>
        /// <returns>The view host.</returns>
        public static ILayoutViewHost GetViewHost(this View This)
        {
            var tagData = This.GetTag(viewHostTag);
            if (tagData != null) return tagData.ToNetObject<ILayoutViewHost>();

            return null;
        }
    }

    /// <summary>
    /// A class that implements the Android ViewHolder pattern. Use it along 
    /// with GetViewHost.
    /// </summary>
    public abstract class LayoutViewHost : ILayoutViewHost, IEnableLogger
    {
        View view;
        public View View {
            get { return view; }
            set {
                if (view == value) return;
                view = value;
                view.SetTag(ViewMixins.viewHostTag, this.ToJavaObject());
            }
        }

        public static implicit operator View(LayoutViewHost This)
        {
            return This.View;
        }

        protected LayoutViewHost()
        {
        }

        protected LayoutViewHost(Context ctx, int layoutId, ViewGroup parent, bool attachToRoot = false, bool performAutoWireup = true)
        {
            var inflater = LayoutInflater.FromContext(ctx);
            View = inflater.Inflate(layoutId, parent, attachToRoot);

            if (performAutoWireup) this.WireUpControls();
        }
    }

    /// <summary>
    /// A class that implements the Android ViewHolder pattern with a 
    /// ViewModel. Use it along with GetViewHost.
    /// </summary>
    public abstract class ReactiveViewHost<TViewModel> : LayoutViewHost, IViewFor<TViewModel>, IReactiveNotifyPropertyChanged<ReactiveViewHost<TViewModel>>, IReactiveObject
        where TViewModel : class, IReactiveObject
    {
        protected ReactiveViewHost(Context ctx, int layoutId, ViewGroup parent, bool attachToRoot = false, bool performAutoWireup = true)
            : base(ctx, layoutId, parent, attachToRoot, performAutoWireup)
        {
            setupRxObj();
        }

        protected ReactiveViewHost()
        {
            setupRxObj();
        }

        TViewModel _ViewModel;
        public TViewModel ViewModel {
            get { return _ViewModel; }
            set { this.RaiseAndSetIfChanged(ref _ViewModel, value); }
        }

        object IViewFor.ViewModel {
            get { return _ViewModel; }
            set { _ViewModel = (TViewModel)value; }
        }

        public event PropertyChangingEventHandler PropertyChanging {
            add { PropertyChangingEventManager.AddHandler(this, value); }
            remove { PropertyChangingEventManager.RemoveHandler(this, value); }
        }

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) {
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
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewHost<TViewModel>>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewHost<TViewModel>>> Changed {
            get { return this.getChangedObservable(); }
        }

        [IgnoreDataMember]
        protected Lazy<PropertyInfo[]> allPublicProperties;

        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        [OnDeserialized]
        void setupRxObj(StreamingContext sc) { setupRxObj(); }

        void setupRxObj()
        {
            allPublicProperties = new Lazy<PropertyInfo[]>(() =>
                GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray());
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

        public bool AreChangeNotificationsEnabled()
        {
            return this.areChangeNotificationsEnabled();
        }
    }
}

