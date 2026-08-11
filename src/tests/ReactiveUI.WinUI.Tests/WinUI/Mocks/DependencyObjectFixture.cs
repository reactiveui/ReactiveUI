// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;

namespace ReactiveUI.Tests.WinUI.Mocks;

/// <summary>A dependency object exposing its dependency property through the conventional static field.</summary>
public class DependencyObjectFixture : DependencyObject
{
    /// <summary>The dependency property backing <see cref="TestString"/>.</summary>
    public static readonly DependencyProperty TestStringProperty =
        DependencyProperty.Register(nameof(TestString), typeof(string), typeof(DependencyObjectFixture), new(null));

    /// <summary>The dependency property field left unset, so no property can be fetched from it.</summary>
    public static readonly DependencyProperty UnsetFieldProperty = null!;

    /// <summary>Gets or sets the observed string.</summary>
    public string? TestString
    {
        get => (string?)GetValue(TestStringProperty);
        set => SetValue(TestStringProperty, value);
    }
}
