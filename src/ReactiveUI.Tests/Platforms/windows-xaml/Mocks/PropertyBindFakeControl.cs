// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
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
/// A fake view for property binding.
/// </summary>
public class PropertyBindFakeControl : Control
{
    /// <summary>
    /// The null hating string property.
    /// </summary>
    public static readonly DependencyProperty NullHatingStringProperty =
        DependencyProperty.Register("NullHatingString", typeof(string), typeof(PropertyBindFakeControl), new PropertyMetadata(string.Empty));

    /// <summary>
    /// The nullable double property.
    /// </summary>
    public static readonly DependencyProperty NullableDoubleProperty =
        DependencyProperty.Register("NullableDouble", typeof(double?), typeof(PropertyBindFakeControl), new PropertyMetadata(null));

    /// <summary>
    /// The just a double property.
    /// </summary>
    public static readonly DependencyProperty JustADoubleProperty =
        DependencyProperty.Register("JustADouble", typeof(double), typeof(PropertyBindFakeControl), new PropertyMetadata(0.0));

    /// <summary>
    /// Gets or sets the nullable double.
    /// </summary>
    public double? NullableDouble
    {
        get => (double?)GetValue(NullableDoubleProperty);
        set => SetValue(NullableDoubleProperty, value);
    }

    /// <summary>
    /// Gets or sets the just a double.
    /// </summary>
    public double JustADouble
    {
        get => (double)GetValue(JustADoubleProperty);
        set => SetValue(JustADoubleProperty, value);
    }

    /// <summary>
    /// Gets or sets the null hating string.
    /// </summary>
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
