using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Android.Content;
using Android.Views;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// interface for Layout View Host
    /// </summary>
    public interface ILayoutViewHost
    {
        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>The view.</value>
        View View { get; }
    }

    /// <summary>
    /// View Mixins
    /// </summary>
    public static class ViewMixins
    {
        internal const int viewHostTag = -4222;

        /// <summary>
        /// Gets the ViewHost associated with a given View by accessing the Tag of the View
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="This">The this.</param>
        /// <returns>The view host.</returns>
        public static T GetViewHost<T>(this View This) where T : ILayoutViewHost
        {
            var tagData = This.GetTag(viewHostTag);
            if (tagData != null) return tagData.ToNetObject<T>();

            return default(T);
        }

        /// <summary>
        /// Gets the ViewHost associated with a given View by accessing the Tag of the View
        /// </summary>
        /// <param name="This">The this.</param>
        /// <returns>The view host.</returns>
        public static ILayoutViewHost GetViewHost(this View This)
        {
            var tagData = This.GetTag(viewHostTag);
            if (tagData != null) return tagData.ToNetObject<ILayoutViewHost>();

            return null;
        }
    }

    /// <summary>
    /// A class that implements the Android ViewHolder pattern. Use it along with GetViewHost.
    /// </summary>
    public abstract class LayoutViewHost : ILayoutViewHost, IEnableLogger
    {
        private View view;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutViewHost"/> class.
        /// </summary>
        protected LayoutViewHost()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutViewHost"/> class.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="layoutId">The layout identifier.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="attachToRoot">if set to <c>true</c> [attach to root].</param>
        /// <param name="performAutoWireup">if set to <c>true</c> [perform automatic wireup].</param>
        protected LayoutViewHost(Context ctx, int layoutId, ViewGroup parent, bool attachToRoot = false, bool performAutoWireup = true)
        {
            var inflater = LayoutInflater.FromContext(ctx);
            this.View = inflater.Inflate(layoutId, parent, attachToRoot);

            if (performAutoWireup) this.WireUpControls();
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>The view.</value>
        public View View
        {
            get
            {
                return this.view;
            }

            set
            {
                if (this.view == value) return;
                this.view = value;
                this.view.SetTag(ViewMixins.viewHostTag, this.ToJavaObject());
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="LayoutViewHost"/> to <see cref="View"/>.
        /// </summary>
        /// <param name="This">The this.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator View(LayoutViewHost This)
        {
            return This.View;
        }
    }

    /// <summary>
    /// A class that implements the Android ViewHolder pattern with a ViewModel. Use it along with GetViewHost.
    /// </summary>
    public abstract class ReactiveViewHost<TViewModel> : LayoutViewHost, IViewFor<TViewModel>, IReactiveNotifyPropertyChanged<ReactiveViewHost<TViewModel>>, IReactiveObject
        where TViewModel : class, IReactiveObject
    {
        /// <summary>
        /// All public properties
        /// </summary>
        [IgnoreDataMember]
        protected Lazy<PropertyInfo[]> allPublicProperties;

        private TViewModel _ViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveViewHost{TViewModel}"/> class.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="layoutId">The layout identifier.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="attachToRoot">if set to <c>true</c> [attach to root].</param>
        /// <param name="performAutoWireup">if set to <c>true</c> [perform automatic wireup].</param>
        protected ReactiveViewHost(Context ctx, int layoutId, ViewGroup parent, bool attachToRoot = false, bool performAutoWireup = true)
                            : base(ctx, layoutId, parent, attachToRoot, performAutoWireup)
        {
            setupRxObj();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveViewHost{TViewModel}"/> class.
        /// </summary>
        protected ReactiveViewHost()
        {
            setupRxObj();
        }

        /// <summary>
        /// To be added.
        /// </summary>
        /// <remarks>To be added.</remarks>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { PropertyChangedEventManager.AddHandler(this, value); }
            remove { PropertyChangedEventManager.RemoveHandler(this, value); }
        }

        /// <summary>
        /// Occurs when [property changing].
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add { PropertyChangingEventManager.AddHandler(this, value); }
            remove { PropertyChangingEventManager.RemoveHandler(this, value); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewHost<TViewModel>>> Changed
        {
            get { return this.getChangedObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to be changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewHost<TViewModel>>> Changing
        {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Gets the thrown exceptions.
        /// </summary>
        /// <value>The thrown exceptions.</value>
        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        /// <summary>
        /// The ViewModel corresponding to this specific View. This should be a DependencyProperty if
        /// you're using XAML.
        /// </summary>
        public TViewModel ViewModel
        {
            get { return this._ViewModel; }
            set { this.RaiseAndSetIfChanged(ref this._ViewModel, value); }
        }

        object IViewFor.ViewModel
        {
            get { return this._ViewModel; }
            set { this._ViewModel = (TViewModel)value; }
        }

        /// <summary>
        /// Ares the change notifications enabled.
        /// </summary>
        /// <returns></returns>
        public bool AreChangeNotificationsEnabled()
        {
            return this.areChangeNotificationsEnabled();
        }

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <summary>
        /// When this method is called, an object will not fire change notifications (neither
        /// traditional nor Observable notifications) until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change notifications.</returns>
        public IDisposable SuppressChangeNotifications()
        {
            return this.suppressChangeNotifications();
        }

        [OnDeserialized]
        private void setupRxObj(StreamingContext sc)
        { setupRxObj(); }

        private void setupRxObj()
        {
            this.allPublicProperties = new Lazy<PropertyInfo[]>(() =>
                GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray());
        }
    }
}