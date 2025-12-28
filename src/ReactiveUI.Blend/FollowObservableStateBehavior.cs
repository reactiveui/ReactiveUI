// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

using System.Reactive.Concurrency;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace ReactiveUI.Blend;

/// <summary>
/// Behavior that tracks the state of an observable.
/// </summary>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("OnStateObservableChanged uses methods that require dynamic code generation")]
[RequiresUnreferencedCode("OnStateObservableChanged uses methods that may require unreferenced code")]
#endif
public class FollowObservableStateBehavior : Behavior<FrameworkElement>
{
    /// <summary>
    /// The state observable dependency property.
    /// </summary>
    public static readonly DependencyProperty StateObservableProperty =
        DependencyProperty.Register("StateObservable", typeof(IObservable<string>), typeof(FollowObservableStateBehavior), new PropertyMetadata(null, OnStateObservableChanged));

    /// <summary>
    /// The target object dependency property.
    /// </summary>
    public static readonly DependencyProperty TargetObjectProperty =
        DependencyProperty.Register("TargetObject", typeof(FrameworkElement), typeof(FollowObservableStateBehavior), new PropertyMetadata(null));

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
    /// Gets or sets the target object.
    /// </summary>
    public FrameworkElement TargetObject
    {
        get => (FrameworkElement)GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic resubscribe on error].
    /// </summary>
    public bool AutoResubscribeOnError { get; set; }

    /// <summary>
    /// Gets or sets the scheduler to use for observing state changes.
    /// If null, uses RxSchedulers.MainThreadScheduler. This property is primarily for testing purposes.
    /// </summary>
    public IScheduler? SchedulerOverride { get; set; }

    /// <summary>
    /// Internal method for testing purposes that calls OnStateObservableChanged.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("InternalOnStateObservableChangedForTesting uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("InternalOnStateObservableChangedForTesting uses methods that may require unreferenced code")]
#endif
    internal static void InternalOnStateObservableChangedForTesting(DependencyObject? sender, DependencyPropertyChangedEventArgs e) =>
        OnStateObservableChanged(sender, e);

    /// <summary>
    /// Called when [state observable changed].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("OnStateObservableChanged uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("OnStateObservableChanged uses methods that may require unreferenced code")]
#endif
    protected static void OnStateObservableChanged(DependencyObject? sender, DependencyPropertyChangedEventArgs e)
    {
        ArgumentExceptionHelper.ThrowIfNotOfType<FollowObservableStateBehavior>(sender);
        var item = (FollowObservableStateBehavior)sender;

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
        var scheduler = item.SchedulerOverride ?? RxSchedulers.MainThreadScheduler;
        item._watcher = newValue.ObserveOn(scheduler).Subscribe(
            x =>
            {
                var target = item.TargetObject ?? item.AssociatedObject;
                if (target is Control)
                {
                    VisualStateManager.GoToState(target, x, true);
                }
                else
                {
                    VisualStateManager.GoToElementState(target, x, true);
                }
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
