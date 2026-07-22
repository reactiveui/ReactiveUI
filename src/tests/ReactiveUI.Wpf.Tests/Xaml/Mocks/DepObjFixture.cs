// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>A dependency object fixture.</summary>
public class DepObjFixture : FrameworkElement
{
    /// <summary>The test string property.</summary>
    public static readonly DependencyProperty TestStringProperty =
        DependencyProperty.Register(nameof(TestString), typeof(string), typeof(DepObjFixture), new(null));

    /// <summary>Gets or sets the test string.</summary>
    public string TestString
    {
        get => (string)GetValue(TestStringProperty);
        set => SetValue(TestStringProperty, value);
    }
}
