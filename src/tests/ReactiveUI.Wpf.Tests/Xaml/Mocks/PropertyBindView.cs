// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using PropertyMetadata = System.Windows.PropertyMetadata;

namespace ReactiveUI.Tests.Xaml.Mocks;

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
    /// The fake control property.
    /// </summary>
    public static readonly DependencyProperty FakeControlProperty =
        DependencyProperty.Register("FakeControl", typeof(PropertyBindFakeControl), typeof(PropertyBindView), new PropertyMetadata(null));

    /// <summary>
    /// The some text box property.
    /// </summary>
    public static readonly DependencyProperty SomeTextBoxProperty =
        DependencyProperty.Register("SomeTextBox", typeof(TextBox), typeof(PropertyBindView), new PropertyMetadata(null));

    /// <summary>
    /// The property2 property.
    /// </summary>
    public static readonly DependencyProperty Property2Property =
        DependencyProperty.Register("Property2", typeof(TextBox), typeof(PropertyBindView), new PropertyMetadata(null));

    /// <summary>
    /// The fake items control property.
    /// </summary>
    public static readonly DependencyProperty FakeItemsControlProperty =
        DependencyProperty.Register("FakeItemsControl", typeof(ListBox), typeof(PropertyBindView), new PropertyMetadata(null));

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
    public TextBox SomeTextBox
    {
        get => (TextBox)GetValue(SomeTextBoxProperty);
        set => SetValue(SomeTextBoxProperty, value);
    }

    /// <summary>
    /// Gets or sets the property2.
    /// </summary>
    public TextBox Property2
    {
        get => (TextBox)GetValue(Property2Property);
        set => SetValue(Property2Property, value);
    }

    /// <summary>
    /// Gets or sets the fake control.
    /// </summary>
    public PropertyBindFakeControl FakeControl
    {
        get => (PropertyBindFakeControl)GetValue(FakeControlProperty);
        set => SetValue(FakeControlProperty, value);
    }

    /// <summary>
    /// Gets or sets the fake items control.
    /// </summary>
    public ListBox FakeItemsControl
    {
        get => (ListBox)GetValue(FakeItemsControlProperty);
        set => SetValue(FakeItemsControlProperty, value);
    }

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
