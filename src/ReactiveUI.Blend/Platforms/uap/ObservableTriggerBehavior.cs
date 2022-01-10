// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace ReactiveUI.Blend;

/// <summary>
/// Behavior that response to triggered observables.
/// </summary>
[ContentProperty(Name = "Actions")]
public sealed class ObservableTriggerBehavior : Behavior<DependencyObject>, IDisposable
{
    /// <summary>
    /// The observable dependency property.
    /// </summary>
    public static readonly DependencyProperty ObservableProperty =
        DependencyProperty.Register("Observable", typeof(IObservable<object>), typeof(ObservableTriggerBehavior), new PropertyMetadata(null, OnObservableChanged));

    /// <summary>
    /// The actions dependency property.
    /// </summary>
    public static readonly DependencyProperty ActionsProperty =
        DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(ObservableTriggerBehavior), new PropertyMetadata(null));

    /// <summary>
    /// The source object dependency property.
    /// </summary>
    public static readonly DependencyProperty SourceObjectProperty =
        DependencyProperty.Register("SourceObject", typeof(object), typeof(ObservableTriggerBehavior), new PropertyMetadata(null, OnSourceObjectChanged));

    private object? _resolvedSource;
    private SerialDisposable _watcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableTriggerBehavior"/> class.
    /// </summary>
    public ObservableTriggerBehavior()
    {
        _watcher = new SerialDisposable();
        _watcher.Disposable = Disposable.Empty;
    }

    /// <summary>
    /// Gets the actions.
    /// </summary>
    public ActionCollection Actions
    {
        get
        {
            var actionCollection = (ActionCollection)GetValue(ObservableTriggerBehavior.ActionsProperty);

            if (actionCollection is null)
            {
                actionCollection = new ActionCollection();
                SetValue(ObservableTriggerBehavior.ActionsProperty, actionCollection);
            }

            return actionCollection;
        }
    }

    /// <summary>
    /// Gets or sets the source object.
    /// </summary>
    public object SourceObject
    {
        get => GetValue(ObservableTriggerBehavior.SourceObjectProperty);
        set => SetValue(ObservableTriggerBehavior.SourceObjectProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic resubscribe on error].
    /// </summary>
    public bool AutoResubscribeOnError { get; set; }

    /// <summary>
    /// Gets or sets the observable.
    /// </summary>
    public IObservable<object> Observable
    {
        get => (IObservable<object>)GetValue(ObservableProperty);
        set => SetValue(ObservableProperty, value);
    }

    /// <inheritdoc />
    public void Dispose() => _watcher?.Dispose();

    /// <inheritdoc/>
    protected override void OnAttached()
    {
        base.OnAttached();
        SetResolvedSource(ComputeResolvedSource());
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        SetResolvedSource(null);
        base.OnDetaching();

        _watcher.Dispose();
    }

    private static void OnObservableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var @this = (ObservableTriggerBehavior)sender;

        @this._watcher.Disposable = ((IObservable<object>)e.NewValue).ObserveOn(RxApp.MainThreadScheduler).Subscribe(
         x => Interaction.ExecuteActions(@this._resolvedSource, @this.Actions, x),
         ex =>
         {
             if (!@this.AutoResubscribeOnError)
             {
                 return;
             }

             OnObservableChanged(@this, e);
         });
    }

    private static void OnSourceObjectChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        var observableTriggerBehavior = (ObservableTriggerBehavior)dependencyObject;

        observableTriggerBehavior.SetResolvedSource(observableTriggerBehavior.ComputeResolvedSource());
    }

    private void SetResolvedSource(object? newSource)
    {
        if (AssociatedObject is null || _resolvedSource == newSource)
        {
            return;
        }

        _resolvedSource = newSource;
    }

    private object? ComputeResolvedSource()
    {
        if (ReadLocalValue(ObservableTriggerBehavior.SourceObjectProperty) != DependencyProperty.UnsetValue)
        {
            return SourceObject;
        }

        return AssociatedObject;
    }
}
