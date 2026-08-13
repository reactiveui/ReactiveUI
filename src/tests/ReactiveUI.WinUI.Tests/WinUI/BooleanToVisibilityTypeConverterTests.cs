// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Tests for <see cref="BooleanToVisibilityTypeConverter"/>.</summary>
/// <remarks>
/// WinUI has no hidden-but-laid-out visibility, so <see cref="BooleanToVisibilityHint.UseHidden"/> has nothing to
/// select here and a false value collapses either way.
/// </remarks>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class BooleanToVisibilityTypeConverterTests
{
    /// <summary>A hint value of a type the converter does not recognise.</summary>
    private const string UnrelatedHint = "not a hint";

    /// <summary>Verifies the converter advertises the pair of types it converts between.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TypeProperties_DescribeABooleanToVisibilityConversion()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        using (Assert.Multiple())
        {
            await Assert.That(converter.FromType).IsEqualTo(typeof(bool));
            await Assert.That(converter.ToType).IsEqualTo(typeof(Visibility));
        }
    }

    /// <summary>Verifies the converter registers as a built-in conversion rather than a preferred one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObjects_ReturnsTheBuiltInConverterAffinity()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        await Assert.That(converter.GetAffinityForObjects()).IsEqualTo(BindingAffinity.DefaultInternalTypeConverter);
    }

    /// <summary>Verifies the conversion honours the hint the binding supplies.</summary>
    /// <param name="value">The boolean being converted.</param>
    /// <param name="hint">The conversion hint supplied by the binding.</param>
    /// <param name="expected">The visibility the conversion is expected to produce.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(true, BooleanToVisibilityHint.None, Visibility.Visible)]
    [Arguments(false, BooleanToVisibilityHint.None, Visibility.Collapsed)]
    [Arguments(true, BooleanToVisibilityHint.Inverse, Visibility.Collapsed)]
    [Arguments(false, BooleanToVisibilityHint.Inverse, Visibility.Visible)]
    [Arguments(true, BooleanToVisibilityHint.UseHidden, Visibility.Visible)]
    [Arguments(false, BooleanToVisibilityHint.UseHidden, Visibility.Collapsed)]
    [Arguments(false, BooleanToVisibilityHint.Inverse | BooleanToVisibilityHint.UseHidden, Visibility.Visible)]
    [Arguments(true, null, Visibility.Visible)]
    [Arguments(false, null, Visibility.Collapsed)]
    [Arguments(true, UnrelatedHint, Visibility.Visible)]
    [Arguments(false, UnrelatedHint, Visibility.Collapsed)]
    public async Task TryConvert_AppliesTheHint(bool value, object? hint, Visibility expected)
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var converted = converter.TryConvert(value, hint, out var result);

        using (Assert.Multiple())
        {
            await Assert.That(converted).IsTrue();
            await Assert.That(result).IsEqualTo(expected);
        }
    }
}
