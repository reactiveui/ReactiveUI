// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using PropertyMetadata = System.Windows.PropertyMetadata;

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>
/// A derived dependency object.
/// </summary>
public class DerivedDepObjFixture : DepObjFixture
{
    /// <summary>
    /// Another test string property.
    /// </summary>
    public static readonly DependencyProperty AnotherTestStringProperty =
        DependencyProperty.Register("AnotherTestString", typeof(string), typeof(DerivedDepObjFixture), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets another test string.
    /// </summary>
    public string AnotherTestString
    {
        get => (string)GetValue(AnotherTestStringProperty);
        set => SetValue(AnotherTestStringProperty, value);
    }
}
