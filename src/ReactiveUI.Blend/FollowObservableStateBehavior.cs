// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
#endif

#pragma warning disable SA1201 // A field should not follow a property - macro if statements make this hard

namespace ReactiveUI.Blend;

/// <summary>
/// Behavior that tracks the state of an observable.
/// </summary>
#if NETFX_CORE
public class FollowObservableStateBehavior : Behavior<Control>
#else
public class FollowObservableStateBehavior : Behavior<FrameworkElement>
#endif
{
    private IDisposable? _watcher;

    /// <summary>
    /// Gets or sets the state observable.
    /// </summary>
    public IObservable<string> StateObservable
    {
        get => (IObservable<string>)GetValue(StateObservableProperty);
        set => SetValue(StateObservableProperty, value);
    }

    /// <summary>
    /// The state observable dependency property.
    /// </summary>
    public static readonly DependencyProperty StateObservableProperty =
        DependencyProperty.Register("StateObservable", typeof(IObservable<string>), typeof(FollowObservableStateBehavior), new PropertyMetadata(null, OnStateObservableChanged));

#if NETFX_CORE
    /// <summary>
    /// Gets or sets the target object.
    /// </summary>
    public Control TargetObject
    {
        get => (Control)GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

    /// <summary>
    /// Gets or sets the target object.
    /// </summary>
    public static readonly DependencyProperty TargetObjectProperty =
        DependencyProperty.Register("TargetObject", typeof(Control), typeof(FollowObservableStateBehavior), new PropertyMetadata(null));
#else
    /// <summary>
    /// Gets or sets the target object.
    /// </summary>
    public FrameworkElement TargetObject
    {
        get => (FrameworkElement)GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

    /// <summary>
    /// The target object dependency property.
    /// </summary>
    public static readonly DependencyProperty TargetObjectProperty =
        DependencyProperty.Register("TargetObject", typeof(FrameworkElement), typeof(FollowObservableStateBehavior), new PropertyMetadata(null));
#endif

    /// <summary>
    /// Gets or sets a value indicating whether [automatic resubscribe on error].
    /// </summary>
    public bool AutoResubscribeOnError { get; set; }

    /// <summary>
    /// Called when [state observable changed].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected static void OnStateObservableChanged(DependencyObject? sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not FollowObservableStateBehavior item)
        {
            throw new ArgumentException("Sender must be of type " + nameof(FollowObservableStateBehavior), nameof(sender));
        }

        if (item._watcher is not null)
        {
            item._watcher.Dispose();
            item._watcher = null;
        }

        if (e == default)
        {
            throw new ArgumentNullException(nameof(e));
        }

        var newValue = (IObservable<string>)e.NewValue;
        if (newValue is null)
        {
            throw new ArgumentNullException(nameof(e.NewValue));
        }

        item._watcher = newValue.ObserveOn(RxApp.MainThreadScheduler).Subscribe(
            x =>
            {
                var target = item.TargetObject ?? item.AssociatedObject;
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
            _ =>
            {
                if (!item.AutoResubscribeOnError)
                {
                    return;
                }

                OnStateObservableChanged(item, e);
            });
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        if (_watcher is not null)
        {
            _watcher.Dispose();
            _watcher = null;
        }

        base.OnDetaching();
    }
}
