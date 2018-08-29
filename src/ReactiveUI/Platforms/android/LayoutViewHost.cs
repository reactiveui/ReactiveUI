// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Views;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Interface that defines a layout view host.
    /// </summary>
    public interface ILayoutViewHost
    {
        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        View View { get; }
    }

    /// <summary>
    /// Default methods for <see cref="ILayoutViewHost"/>.
    /// </summary>
    public static class ViewMixins
    {
        internal const int ViewHostTag = -4222;

        /// <summary>
        /// Gets the ViewHost associated with a given View by accessing the
        /// Tag of the View.
        /// </summary>
        /// <typeparam name="T">The layout view host type.</typeparam>
        /// <param name="this">The view.</param>
        /// <returns>The view host.</returns>
        public static T GetViewHost<T>(this View @this)
            where T : ILayoutViewHost
        {
            var tagData = @this.GetTag(ViewHostTag);
            if (tagData != null)
            {
                return tagData.ToNetObject<T>();
            }

            return default(T);
        }

        /// <summary>
        /// Gets the ViewHost associated with a given View by accessing the
        /// Tag of the View.
        /// </summary>
        /// <param name="this">The view.</param>
        /// <returns>The view host.</returns>
        public static ILayoutViewHost GetViewHost(this View @this)
        {
            var tagData = @this.GetTag(ViewHostTag);
            if (tagData != null)
            {
                return tagData.ToNetObject<ILayoutViewHost>();
            }

            return null;
        }
    }

    /// <summary>
    /// A class that implements the Android ViewHolder pattern. Use it along
    /// with GetViewHost.
    /// </summary>
    public abstract class LayoutViewHost : ILayoutViewHost, IEnableLogger
    {
        private View _view;

        /// <inheritdoc/>
        public View View
        {
            get => _view;

            set
            {
                if (_view == value)
                {
                    return;
                }

                _view = value;
                _view.SetTag(ViewMixins.ViewHostTag, this.ToJavaObject());
            }
        }

        public static implicit operator View(LayoutViewHost @this)
        {
            return @this.View;
        }

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
            View = inflater.Inflate(layoutId, parent, attachToRoot);

            if (performAutoWireup)
            {
                this.WireUpControls();
            }
        }
    }

    /// <summary>
    /// A class that implements the Android ViewHolder pattern with a
    /// ViewModel. Use it along with GetViewHost.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    public abstract class ReactiveViewHost<TViewModel> : LayoutViewHost, IViewFor<TViewModel>, IReactiveNotifyPropertyChanged<ReactiveViewHost<TViewModel>>, IReactiveObject
        where TViewModel : class, IReactiveObject
    {
        private TViewModel _viewModel;

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
            SetupRxObj();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveViewHost{TViewModel}"/> class.
        /// </summary>
        protected ReactiveViewHost()
        {
            SetupRxObj();
        }

        /// <inheritdoc/>
        public TViewModel ViewModel
        {
            get { return _viewModel; }
            set { this.RaiseAndSetIfChanged(ref _viewModel, value); }
        }

        /// <inheritdoc/>
        object IViewFor.ViewModel
        {
            get { return _viewModel; }
            set { _viewModel = (TViewModel)value; }
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add { PropertyChangingEventManager.AddHandler(this, value); }
            remove { PropertyChangingEventManager.RemoveHandler(this, value); }
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { PropertyChangedEventManager.AddHandler(this, value); }
            remove { PropertyChangedEventManager.RemoveHandler(this, value); }
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewHost<TViewModel>>> Changing
        {
            get { return this.GetChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewHost<TViewModel>>> Changed
        {
            get { return this.GetChangedObservable(); }
        }

        /// <summary>
        /// All public properties.
        /// </summary>
        [IgnoreDataMember]
        protected Lazy<PropertyInfo[]> allPublicProperties;

        /// <summary>
        /// Gets the thrown exceptions.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions
        {
            get { return this.GetThrownExceptionsObservable(); }
        }

        [OnDeserialized]
        private void SetupRxObj(StreamingContext sc)
        {
            SetupRxObj();
        }

        private void SetupRxObj()
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
            return IReactiveObjectExtensions.SuppressChangeNotifications(this);
        }

        /// <summary>
        /// Gets a value indicating if change notifications are enabled.
        /// </summary>
        /// <returns>A value indicating if change notifications are on or off.</returns>
        public bool AreChangeNotificationsEnabled()
        {
            return IReactiveObjectExtensions.AreChangeNotificationsEnabled(this);
        }
    }
}
