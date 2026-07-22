// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using Control = System.Windows.Controls.Control;

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>A property binding view.</summary>
public class PropertyBindView : Control, IViewFor<PropertyBindViewModel>
{
    /// <summary>The view model property.</summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(PropertyBindViewModel), typeof(PropertyBindView), new(null));

    /// <summary>The fake control property.</summary>
    public static readonly DependencyProperty FakeControlProperty =
        DependencyProperty.Register(nameof(FakeControl), typeof(PropertyBindFakeControl), typeof(PropertyBindView), new(null));

    /// <summary>The some text box property.</summary>
    public static readonly DependencyProperty SomeTextBoxProperty =
        DependencyProperty.Register(nameof(SomeTextBox), typeof(TextBox), typeof(PropertyBindView), new(null));

    /// <summary>The property2 property.</summary>
    public static readonly DependencyProperty Property2Property =
        DependencyProperty.Register(nameof(Property2), typeof(TextBox), typeof(PropertyBindView), new(null));

    /// <summary>The fake items control property.</summary>
    public static readonly DependencyProperty FakeItemsControlProperty =
        DependencyProperty.Register(nameof(FakeItemsControl), typeof(ListBox), typeof(PropertyBindView), new(null));

    /// <summary>The combo box selection property.</summary>
    public static readonly DependencyProperty ComboBoxSelectionProperty =
        DependencyProperty.Register(nameof(ComboBoxSelection), typeof(ComboBox), typeof(PropertyBindView), new(null));

    /// <summary>Initializes a new instance of the <see cref="PropertyBindView"/> class.</summary>
    public PropertyBindView()
    {
        SomeTextBox = new();
        Property2 = new();
        FakeControl = new();
        FakeItemsControl = new();
        ComboBoxSelection = new();
    }

    /// <summary>Gets or sets some text box.</summary>
    public TextBox SomeTextBox
    {
        get => (TextBox)GetValue(SomeTextBoxProperty);
        set => SetValue(SomeTextBoxProperty, value);
    }

    /// <summary>Gets or sets the property2.</summary>
    public TextBox Property2
    {
        get => (TextBox)GetValue(Property2Property);
        set => SetValue(Property2Property, value);
    }

    /// <summary>Gets or sets the fake control.</summary>
    public PropertyBindFakeControl FakeControl
    {
        get => (PropertyBindFakeControl)GetValue(FakeControlProperty);
        set => SetValue(FakeControlProperty, value);
    }

    /// <summary>Gets or sets the fake items control.</summary>
    public ListBox FakeItemsControl
    {
        get => (ListBox)GetValue(FakeItemsControlProperty);
        set => SetValue(FakeItemsControlProperty, value);
    }

    /// <summary>Gets or sets the combo box selection.</summary>
    public ComboBox ComboBoxSelection
    {
        get => (ComboBox)GetValue(ComboBoxSelectionProperty);
        set => SetValue(ComboBoxSelectionProperty, value);
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
