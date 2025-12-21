// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// A property binding view.
/// </summary>
public class PropertyBindView : Control, IViewFor<PropertyBindViewModel>
{
    /// <summary>
    /// The view model property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(PropertyBindViewModel), typeof(PropertyBindView), new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBindView"/> class.
    /// </summary>
    public PropertyBindView()
    {
        SomeTextBox = new TextBox();
        Property2 = new TextBox();
        FakeControl = new PropertyBindFakeControl();
        FakeItemsControl = new ListBox();
    }

    /// <summary>
    /// Gets or sets some text box.
    /// </summary>
    public TextBox SomeTextBox { get; set; }

    /// <summary>
    /// Gets or sets the property2.
    /// </summary>
    public TextBox Property2 { get; set; }

    /// <summary>
    /// Gets or sets the fake control.
    /// </summary>
    public PropertyBindFakeControl FakeControl { get; set; }

    /// <summary>
    /// Gets or sets the fake items control.
    /// </summary>
    public ListBox FakeItemsControl { get; set; }

    /// <inheritdoc/>
    public PropertyBindViewModel? ViewModel
    {
        get => (PropertyBindViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (PropertyBindViewModel?)value;
    }
}
