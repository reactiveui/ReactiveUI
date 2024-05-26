// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui.Plugins.Popup;

/// <summary>
/// Base Popup page for that implements <see cref="IViewFor"/>.
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
public abstract class ReactivePopupPage<TViewModel> : ReactivePopupPage, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>
    /// The view model property.
    /// </summary>
    public static new readonly BindableProperty ViewModelProperty = BindableProperty.Create(
         nameof(ViewModel),
         typeof(TViewModel),
         typeof(ReactivePopupPage<TViewModel>),
         default(TViewModel),
         BindingMode.OneWay,
         propertyChanged: OnViewModelChanged);

    /// <summary>
    /// Gets or sets the ViewModel to display.
    /// </summary>
    public new TViewModel? ViewModel
    {
        get => (TViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        ViewModel = BindingContext as TViewModel;
    }
}
