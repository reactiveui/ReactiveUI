// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

namespace ReactiveUI.Tests.Xaml
{
    public class PropertyBindView : Control, IViewFor<PropertyBindViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(PropertyBindViewModel), typeof(PropertyBindView), new PropertyMetadata(null));

        public PropertyBindView()
        {
            SomeTextBox = new TextBox();
            Property2 = new TextBox();
            FakeControl = new PropertyBindFakeControl();
            FakeItemsControl = new ListBox();
        }

        public TextBox SomeTextBox { get; set; }

        public TextBox Property2 { get; set; }

        public PropertyBindFakeControl FakeControl { get; set; }

        public ListBox FakeItemsControl { get; set; }

        public PropertyBindViewModel? ViewModel
        {
            get => (PropertyBindViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (PropertyBindViewModel?)value;
        }
    }
}
