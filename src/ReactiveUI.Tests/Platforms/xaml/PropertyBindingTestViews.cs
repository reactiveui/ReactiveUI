// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using Xunit;

namespace ReactiveUI.Tests
{
    public class PropertyBindView : Control, IViewFor<PropertyBindViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(PropertyBindViewModel), typeof(PropertyBindView), new PropertyMetadata(null));

        public TextBox SomeTextBox;
        public TextBox Property2;
        public PropertyBindFakeControl FakeControl;
        public ListBox FakeItemsControl;

        public PropertyBindView()
        {
            SomeTextBox = new TextBox();
            Property2 = new TextBox();
            FakeControl = new PropertyBindFakeControl();
            FakeItemsControl = new ListBox();
        }

        public PropertyBindViewModel ViewModel
        {
            get => (PropertyBindViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (PropertyBindViewModel)value;
        }
    }

    public class PropertyBindFakeControl : Control
    {
        public static readonly DependencyProperty NullHatingStringProperty =
            DependencyProperty.Register("NullHatingString", typeof(string), typeof(PropertyBindFakeControl), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty NullableDoubleProperty =
            DependencyProperty.Register("NullableDouble", typeof(double?), typeof(PropertyBindFakeControl), new PropertyMetadata(null));

        public static readonly DependencyProperty JustADoubleProperty =
            DependencyProperty.Register("JustADouble", typeof(double), typeof(PropertyBindFakeControl), new PropertyMetadata(0.0));

        public double? NullableDouble
        {
            get => (double?)GetValue(NullableDoubleProperty);
            set => SetValue(NullableDoubleProperty, value);
        }

        public double JustADouble
        {
            get => (double)GetValue(JustADoubleProperty);
            set => SetValue(JustADoubleProperty, value);
        }

        public string NullHatingString
        {
            get => (string)GetValue(NullHatingStringProperty);
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "No nulls! I get confused!");
                }

                SetValue(NullHatingStringProperty, value);
            }
        }
    }
}
