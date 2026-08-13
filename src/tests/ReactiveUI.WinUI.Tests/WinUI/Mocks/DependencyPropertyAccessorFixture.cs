// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;

namespace ReactiveUI.Tests.WinUI.Mocks;

/// <summary>A dependency object exposing its dependency property through a static property rather than a field.</summary>
/// <remarks>
/// Both shapes occur in the wild, and the property lookup is tried before the field lookup, so this fixture pins
/// the property-based branch.
/// </remarks>
public class DependencyPropertyAccessorFixture : DependencyObject
{
    /// <summary>Gets the dependency property backing <see cref="Caption"/>.</summary>
    public static DependencyProperty CaptionProperty { get; } =
        DependencyProperty.Register(nameof(Caption), typeof(string), typeof(DependencyPropertyAccessorFixture), new(null));

    /// <summary>Gets the dependency property accessor left unset, so no property can be fetched from it.</summary>
    public static DependencyProperty UnsetAccessorProperty => null!;

    /// <summary>Gets or sets the observed caption.</summary>
    public string? Caption
    {
        get => (string?)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }
}
