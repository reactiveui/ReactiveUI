// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;

namespace ReactiveUI.Blend
{
    /// <summary>
    /// A blend based trigger which will be activated when a IObservable triggers.
    /// </summary>
    public class ObservableTrigger : TriggerBase<FrameworkElement>
    {
        /// <summary>
        /// Gets or sets the observable which will activate the trigger.
        /// </summary>
        public IObservable<object> Observable
        {
            get => (IObservable<object>)GetValue(ObservableProperty);
            set => SetValue(ObservableProperty, value);
        }

        /// <summary>
        /// The dependency property registration for the Observable property.
        /// </summary>
        public static readonly DependencyProperty ObservableProperty =
            DependencyProperty.Register("Observable", typeof(IObservable<object>), typeof(ObservableTrigger), new PropertyMetadata(OnObservableChanged));

        /// <summary>
        /// Gets or set if we should resubscribe the trigger if there is a error when running the IObservable.
        /// </summary>
        public bool AutoResubscribeOnError { get; set; }

        private IDisposable _watcher;

        /// <summary>
        /// Called when [observable changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void OnObservableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ObservableTrigger This = (ObservableTrigger)sender;
            if (This._watcher != null)
            {
                This._watcher.Dispose();
                This._watcher = null;
            }

            This._watcher = ((IObservable<object>)e.NewValue).ObserveOn(RxApp.MainThreadScheduler).Subscribe(
                x => This.InvokeActions(x),
                ex =>
                {
                    if (!This.AutoResubscribeOnError)
                    {
                        return;
                    }

                    OnObservableChanged(This, e);
                });
        }
    }
}
