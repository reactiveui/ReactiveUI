﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Android.Support.V7.Widget;
using Android.Views;

namespace ReactiveUI.AndroidSupport
{
    /// <summary>
    /// A <see cref="RecyclerView.ViewHolder"/> implementation that binds to a reactive view model.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public class ReactiveRecyclerViewViewHolder<TViewModel> : RecyclerView.ViewHolder, ILayoutViewHost, IViewFor<TViewModel>, IReactiveNotifyPropertyChanged<ReactiveRecyclerViewViewHolder<TViewModel>>, IReactiveObject
            where TViewModel : class, IReactiveObject
    {
        /// <summary>
        /// Gets all public accessible properties.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401: Field should be private", Justification = "Legacy reasons")]
        [SuppressMessage("Design", "CA1051: Do not declare visible instance fields", Justification = "Legacy reasons")]
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1306: Field should start with a lower case letter", Justification = "Legacy reasons")]
        [IgnoreDataMember]
        protected Lazy<PropertyInfo[]> AllPublicProperties = null!;

        private TViewModel? _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveRecyclerViewViewHolder{TViewModel}"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        protected ReactiveRecyclerViewViewHolder(View view)
            : base(view)
        {
            SetupRxObj();

            Selected = Observable.FromEvent<EventHandler, int>(
                                eventHandler =>
                                {
                                    void Handler(object sender, EventArgs e) => eventHandler(AdapterPosition);
                                    return Handler;
                                },
                                h => view.Click += h,
                                h => view.Click -= h);

            LongClicked = Observable.FromEvent<EventHandler<View.LongClickEventArgs>, int>(
                                eventHandler =>
                                {
                                    void Handler(object sender, View.LongClickEventArgs e) => eventHandler(AdapterPosition);
                                    return Handler;
                                },
                                h => view.LongClick += h,
                                h => view.LongClick -= h);

            SelectedWithViewModel = Observable.FromEvent<EventHandler, TViewModel?>(
                                eventHandler =>
                                {
                                    void Handler(object sender, EventArgs e) => eventHandler(ViewModel);
                                    return Handler;
                                },
                                h => view.Click += h,
                                h => view.Click -= h);

            LongClickedWithViewModel = Observable.FromEvent<EventHandler<View.LongClickEventArgs>, TViewModel?>(
                                eventHandler =>
                                {
                                    void Handler(object sender, View.LongClickEventArgs e) => eventHandler(ViewModel);
                                    return Handler;
                                },
                                h => view.LongClick += h,
                                h => view.LongClick -= h);
        }

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging = null!;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged = null!;

        /// <summary>
        /// Gets an observable that signals that this ViewHolder has been selected.
        ///
        /// The <see cref="int"/> is the position of this ViewHolder in the <see cref="RecyclerView"/>
        /// and corresponds to the <see cref="RecyclerView.ViewHolder.AdapterPosition"/> property.
        /// </summary>
        public IObservable<int> Selected { get; }

        /// <summary>
        /// Gets an observable that signals that this ViewHolder has been selected.
        ///
        /// The <see cref="IObservable{TViewModel}"/> is the ViewModel of this ViewHolder in the <see cref="RecyclerView"/>.
        /// </summary>
        public IObservable<TViewModel?> SelectedWithViewModel { get; }

        /// <summary>
        /// Gets an observable that signals that this ViewHolder has been long-clicked.
        ///
        /// The <see cref="int"/> is the position of this ViewHolder in the <see cref="RecyclerView"/>
        /// and corresponds to the <see cref="RecyclerView.ViewHolder.AdapterPosition"/> property.
        /// </summary>
        public IObservable<int> LongClicked { get; }

        /// <summary>
        /// Gets an observable that signals that this ViewHolder has been long-clicked.
        ///
        /// The <see cref="IObservable{TViewModel}"/> is the ViewModel of this ViewHolder in the <see cref="RecyclerView"/>.
        /// </summary>
        public IObservable<TViewModel?> LongClickedWithViewModel { get; }

        /// <summary>
        /// Gets the current view being shown.
        /// </summary>
        public View View => ItemView;

        /// <inheritdoc/>
        public TViewModel? ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        /// <summary>
        /// Gets an observable which signals when exceptions are thrown.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel?)value;
        }

        /// <inheritdoc/>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveRecyclerViewViewHolder<TViewModel>>> Changing => this.GetChangingObservable();

        /// <inheritdoc/>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveRecyclerViewViewHolder<TViewModel>>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        public IDisposable SuppressChangeNotifications()
        {
            return IReactiveObjectExtensions.SuppressChangeNotifications(this);
        }

        /// <summary>
        /// Gets if change notifications via the INotifyPropertyChanged interface are being sent.
        /// </summary>
        /// <returns>A value indicating whether change notifications are enabled or not.</returns>
        public bool AreChangeNotificationsEnabled()
        {
            return IReactiveObjectExtensions.AreChangeNotificationsEnabled(this);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChanging?.Invoke(this, args);
        }

        /// <inheritdoc/>
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        [OnDeserialized]
        private void SetupRxObj(StreamingContext sc)
        {
            SetupRxObj();
        }

        private void SetupRxObj()
        {
            AllPublicProperties = new Lazy<PropertyInfo[]>(() =>
                GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray());
        }
    }
}
