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
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value), "No nulls! I get confused!");
                }

                SetValue(NullHatingStringProperty, value);
            }
        }
    }
}
