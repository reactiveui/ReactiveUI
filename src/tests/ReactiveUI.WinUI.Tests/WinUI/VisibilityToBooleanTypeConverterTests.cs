// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Tests for <see cref="VisibilityToBooleanTypeConverter"/>.</summary>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class VisibilityToBooleanTypeConverterTests
{
    /// <summary>A hint value of a type the converter does not recognise.</summary>
    private const string UnrelatedHint = "not a hint";

    /// <summary>Verifies the converter advertises the pair of types it converts between.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TypeProperties_DescribeAVisibilityToBooleanConversion()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        using (Assert.Multiple())
        {
            await Assert.That(converter.FromType).IsEqualTo(typeof(Visibility));
            await Assert.That(converter.ToType).IsEqualTo(typeof(bool));
        }
    }

    /// <summary>Verifies the converter registers as a built-in conversion rather than a preferred one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObjects_ReturnsTheBuiltInConverterAffinity()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        await Assert.That(converter.GetAffinityForObjects()).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies the conversion honours the hint the binding supplies.</summary>
    /// <param name="value">The visibility being converted.</param>
    /// <param name="hint">The conversion hint supplied by the binding.</param>
    /// <param name="expected">The boolean the conversion is expected to produce.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(Visibility.Visible, BooleanToVisibilityHint.None, true)]
    [Arguments(Visibility.Collapsed, BooleanToVisibilityHint.None, false)]
    [Arguments(Visibility.Visible, BooleanToVisibilityHint.Inverse, false)]
    [Arguments(Visibility.Collapsed, BooleanToVisibilityHint.Inverse, true)]
    [Arguments(Visibility.Visible, null, true)]
    [Arguments(Visibility.Collapsed, null, false)]
    [Arguments(Visibility.Visible, UnrelatedHint, true)]
    [Arguments(Visibility.Collapsed, UnrelatedHint, false)]
    public async Task TryConvert_AppliesTheHint(Visibility value, object? hint, bool expected)
    {
        var converter = new VisibilityToBooleanTypeConverter();

        var converted = converter.TryConvert(value, hint, out var result);

        using (Assert.Multiple())
        {
            await Assert.That(converted).IsTrue();
            await Assert.That(result).IsEqualTo(expected);
        }
    }
}
