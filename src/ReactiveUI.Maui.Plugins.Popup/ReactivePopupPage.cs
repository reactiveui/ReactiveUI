// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Mopups.Pages;

namespace ReactiveUI.Maui.Plugins.Popup;

/// <summary>
/// Base Popup page for that implements <see cref="IViewFor"/>.
/// </summary>
public abstract class ReactivePopupPage : PopupPage, IViewFor
{
    /// <summary>
    /// The view model property.
    /// </summary>
    public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
        nameof(ViewModel),
        typeof(object),
        typeof(IViewFor<object>),
        default,
        BindingMode.OneWay,
        propertyChanged: OnViewModelChanged);

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePopupPage"/> class.
    /// </summary>
    protected ReactivePopupPage() => BackgroundClick =
            Observable.FromEvent<EventHandler, Unit>(
                    handler =>
                    {
                        void EventHandler(object? sender, EventArgs args) => handler(Unit.Default);
                        return EventHandler;
                    },
                    x => BackgroundClicked += x,
                    x => BackgroundClicked -= x)
                .Select(_ => Unit.Default);

    /// <summary>
    /// Gets or sets the background click observable signal.
    /// </summary>
    /// <value>The background click.</value>
    public IObservable<Unit> BackgroundClick { get; protected set; }

    /// <summary>
    /// Gets or sets the ViewModel to display.
    /// </summary>
    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Gets the control binding disposable.
    /// </summary>
    protected CompositeDisposable ControlBindings { get; } = [];

    /// <summary>
    /// Called when [view model changed].
    /// </summary>
    /// <param name="bindableObject">The bindable object.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    protected static void OnViewModelChanged(BindableObject bindableObject, object oldValue, object newValue)
    {
        ArgumentNullException.ThrowIfNull(bindableObject);
        bindableObject.BindingContext = newValue;
    }

    /// <inheritdoc/>
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        ViewModel = BindingContext;
    }
}
