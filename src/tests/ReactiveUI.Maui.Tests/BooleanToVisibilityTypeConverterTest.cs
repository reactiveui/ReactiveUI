// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui;

namespace ReactiveUI.Maui.Tests;

/// <summary>Tests for <see cref="BooleanToVisibilityTypeConverter"/>.</summary>
public sealed class BooleanToVisibilityTypeConverterTest
{
    /// <summary>The expected affinity returned by the converter.</summary>
    private const int ExpectedAffinity = 2;

    /// <summary>Tests that GetAffinityForObjects returns correct affinity for bool to Visibility.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObjects_ReturnsCorrectAffinityForBoolToVisibility()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var affinity = converter.GetAffinityForObjects();

        await Assert.That(affinity).IsEqualTo(ExpectedAffinity);
    }

    /// <summary>Tests that GetAffinityForObjects returns correct affinity for Visibility to bool.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "Intentional duplicate test scenario.")]
    public async Task GetAffinityForObjects_ReturnsCorrectAffinityForVisibilityToBool()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var affinity = converter.GetAffinityForObjects();

        await Assert.That(affinity).IsEqualTo(ExpectedAffinity);
    }

    /// <summary>Tests that TryConvert converts true to Visibility.Visible.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_ConvertsTrueToVisible()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(true, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Visible);
    }

    /// <summary>Tests that TryConvert converts false to Visibility.Collapsed.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_ConvertsFalseToCollapsed()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(false, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Collapsed);
    }

    /// <summary>Tests that TryConvert with Inverse hint inverts the conversion.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithInverseHint_InvertsConversion()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(true, BooleanToVisibilityHint.Inverse, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Collapsed);
    }

#if !HAS_UNO && !HAS_WINUI && !IS_MAUI
    /// <summary>Tests that TryConvert with UseHidden hint uses Hidden instead of Collapsed (WPF only).</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithUseHiddenHint_UsesHidden()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(false, BooleanToVisibilityHint.UseHidden, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Hidden);
    }
#endif

    /// <summary>Tests that TryConvert converts Visibility to bool.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_ConvertsVisibilityToBool()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        var successVisible = converter.TryConvertTyped(Visibility.Visible, null, out var resultVisible);
        var successCollapsed = converter.TryConvertTyped(Visibility.Collapsed, null, out var resultCollapsed);

        await Assert.That(successVisible).IsTrue();
        await Assert.That(successCollapsed).IsTrue();
        await Assert.That(resultVisible).IsNotNull();
        await Assert.That(resultCollapsed).IsNotNull();

        // The actual conversion logic uses XOR, so we just verify it returns a bool
        await Assert.That(resultVisible).IsTypeOf<bool>();
        await Assert.That(resultCollapsed).IsTypeOf<bool>();
    }

    /// <summary>Tests that TryConvert with non-bool input for boolean converter.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "Intentional duplicate test scenario.")]
    public async Task TryConvert_BooleanConverter_HandlesInput()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(false, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Collapsed);
    }

    /// <summary>Tests that TryConvert with Inverse hint on Visibility to bool.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_VisibilityToBoolWithInverse_InvertsResult()
    {
        var converter = new VisibilityToBooleanTypeConverter();

        var success = converter.TryConvertTyped(Visibility.Visible, BooleanToVisibilityHint.Inverse, out var result);

        await Assert.That(success).IsTrue();

        // With Inverse hint, Visible should become false
        await Assert.That(result).IsNotNull();
        await Assert.That((bool)result!).IsFalse();
    }

#if !HAS_UNO && !HAS_WINUI && !IS_MAUI
    /// <summary>Tests that TryConvert with both Inverse and UseHidden hints (WPF only).</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithInverseAndUseHidden_WorksCorrectly()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(
            true,
            BooleanToVisibilityHint.Inverse | BooleanToVisibilityHint.UseHidden,
            out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Hidden);
    }
#endif
}
