// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace ReactiveUI.Blend;

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

    private IDisposable? _watcher;

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
    /// Gets or sets the scheduler to use for observing changes.
    /// If null, uses RxSchedulers.MainThreadScheduler. This property is primarily for testing purposes.
    /// </summary>
    public IScheduler? SchedulerOverride { get; set; }

    /// <summary>
    /// Internal method for testing purposes that calls OnObservableChanged.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    internal static void InternalOnObservableChangedForTesting(DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
        OnObservableChanged(sender, e);

    /// <summary>
    /// Called when [observable changed].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected static void OnObservableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        ArgumentExceptionHelper.ThrowIfNotOfType<ObservableTrigger>(sender, nameof(sender));
        var triggerItem = (ObservableTrigger)sender;

        if (triggerItem._watcher is not null)
        {
            triggerItem._watcher.Dispose();
            triggerItem._watcher = null;
        }

        if (e == default)
        {
            throw new ArgumentNullException(nameof(e));
        }

        var newValue = (IObservable<object>)e.NewValue;
        var scheduler = triggerItem.SchedulerOverride ?? RxSchedulers.MainThreadScheduler;
        triggerItem._watcher = newValue.ObserveOn(scheduler).Subscribe(
         triggerItem.InvokeActions,
         _ =>
         {
             if (!triggerItem.AutoResubscribeOnError)
             {
                 return;
             }

             OnObservableChanged(triggerItem, e);
         });
    }
}
