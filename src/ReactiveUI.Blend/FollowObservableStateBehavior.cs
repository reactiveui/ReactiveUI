// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive.Linq;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
#endif

namespace ReactiveUI.Blend
{
#if NETFX_CORE
    public class FollowObservableStateBehavior : Behavior<Control>
#else
    public class FollowObservableStateBehavior : Behavior<FrameworkElement>
#endif
    {
        public IObservable<string> StateObservable
        {
            get => (IObservable<string>)GetValue(StateObservableProperty);
            set => SetValue(StateObservableProperty, value);
        }

        public static readonly DependencyProperty StateObservableProperty =
            DependencyProperty.Register("StateObservable", typeof(IObservable<string>), typeof(FollowObservableStateBehavior), new PropertyMetadata(null, OnStateObservableChanged));

#if NETFX_CORE
        public Control TargetObject
        {
            get { return (Control)GetValue(TargetObjectProperty); }
            set { SetValue(TargetObjectProperty, value); }
        }

        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register("TargetObject", typeof(Control), typeof(FollowObservableStateBehavior), new PropertyMetadata(null));
#else
        public FrameworkElement TargetObject
        {
            get => (FrameworkElement)GetValue(TargetObjectProperty);
            set => SetValue(TargetObjectProperty, value);
        }

        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register("TargetObject", typeof(FrameworkElement), typeof(FollowObservableStateBehavior), new PropertyMetadata(null));
#endif

        public bool AutoResubscribeOnError { get; set; }

        private IDisposable _watcher;

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }

            base.OnDetaching();
        }

        protected static void OnStateObservableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var @this = (FollowObservableStateBehavior)sender;
            if (@this._watcher != null)
            {
                @this._watcher.Dispose();
                @this._watcher = null;
            }

            @this._watcher = ((IObservable<string>)e.NewValue).ObserveOn(RxApp.MainThreadScheduler).Subscribe(
                x =>
                {
                    var target = @this.TargetObject ?? @this.AssociatedObject;
#if NETFX_CORE
                    VisualStateManager.GoToState(target, x, true);
#else
                    if (target is Control)
                    {
                        VisualStateManager.GoToState(target, x, true);
                    }
                    else
                    {
                        VisualStateManager.GoToElementState(target, x, true);
                    }
#endif
                },
                ex =>
                {
                    if (!@this.AutoResubscribeOnError)
                    {
                        return;
                    }

                    OnStateObservableChanged(@this, e);
                });
        }
    }
}
