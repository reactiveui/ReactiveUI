// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;

namespace ReactiveUI.Maui;

/// <summary>
/// This is an MasterDetailPage that is also an <see cref="IViewFor{T}"/>.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="Microsoft.Maui.Controls" />
/// <seealso cref="IViewFor{TViewModel}" />
public class ReactiveMasterDetailPage<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes
        .PublicParameterlessConstructor)]
    TViewModel> : FlyoutPage, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>
    /// The view model bindable property.
    /// </summary>
    public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
        nameof(ViewModel),
        typeof(TViewModel),
        typeof(ReactiveMasterDetailPage<TViewModel>),
        propertyChanged: OnViewModelChanged);

    /// <summary>
    /// Gets or sets the ViewModel to display.
    /// </summary>
    public TViewModel? ViewModel
    {
        get => (TViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }

    /// <inheritdoc/>
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        ViewModel = BindingContext as TViewModel;
    }

    /// <summary>
    /// Updates the binding context when the view model changes.
    /// </summary>
    /// <param name="bindableObject">The bindable object whose property changed.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnViewModelChanged(BindableObject bindableObject, object oldValue, object newValue) =>
        bindableObject.BindingContext = newValue;
}
