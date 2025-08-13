// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;

namespace ReactiveUI.Maui;

/// <summary>
/// This is an <see cref="EntryCell"/> that is also an <see cref="IViewFor{T}"/>.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="EntryCell" />
/// <seealso cref="IViewFor{TViewModel}" />
#if NET6_0_OR_GREATER
[RequiresDynamicCode("ReactiveEntryCell uses methods that require dynamic code generation")]
[RequiresUnreferencedCode("ReactiveEntryCell uses methods that may require unreferenced code")]
#endif
public partial class ReactiveEntryCell<TViewModel> : EntryCell, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>
    /// The view model bindable property.
    /// </summary>
    public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
     nameof(ViewModel),
     typeof(TViewModel),
     typeof(ReactiveEntryCell<TViewModel>),
     default(TViewModel),
     BindingMode.OneWay,
     propertyChanged: OnViewModelChanged);

    /// <inheritdoc/>
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

    private static void OnViewModelChanged(BindableObject bindableObject, object oldValue, object newValue) => bindableObject.BindingContext = newValue;
}
