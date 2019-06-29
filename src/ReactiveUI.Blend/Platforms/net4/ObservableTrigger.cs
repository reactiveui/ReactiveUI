﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace ReactiveUI.Blend
{
    /// <summary>
    /// A blend based trigger which will be activated when a IObservable triggers.
    /// </summary>
    public class ObservableTrigger : TriggerBase<FrameworkElement>
    {
        /// <summary>
        /// The dependency property registration for the Observable property.
        /// </summary>
        public static readonly DependencyProperty ObservableProperty =
            DependencyProperty.Register("Observable", typeof(IObservable<object>), typeof(ObservableTrigger), new PropertyMetadata(OnObservableChanged));

        private IDisposable _watcher;

        /// <summary>
        /// Gets or sets the observable which will activate the trigger.
        /// </summary>
        public IObservable<object> Observable
        {
            get => (IObservable<object>)GetValue(ObservableProperty);
            set => SetValue(ObservableProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to resubscribe the trigger if there is a error when running the IObservable.
        /// </summary>
        public bool AutoResubscribeOnError { get; set; }

        /// <summary>
        /// Called when [observable changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void OnObservableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ObservableTrigger triggerItem))
            {
                throw new ArgumentException("Sender must be of type " + nameof(ObservableTrigger), nameof(sender));
            }

            if (triggerItem._watcher != null)
            {
                triggerItem._watcher.Dispose();
                triggerItem._watcher = null;
            }

            triggerItem._watcher = ((IObservable<object>)e.NewValue).ObserveOn(RxApp.MainThreadScheduler).Subscribe(
                x => triggerItem.InvokeActions(x),
                ex =>
                {
                    if (!triggerItem.AutoResubscribeOnError)
                    {
                        return;
                    }

                    OnObservableChanged(triggerItem, e);
                });
        }
    }
}
