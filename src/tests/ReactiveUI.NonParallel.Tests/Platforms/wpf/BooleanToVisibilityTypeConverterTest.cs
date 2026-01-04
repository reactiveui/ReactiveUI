// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="BooleanToVisibilityTypeConverter"/> in WPF.
/// </summary>
public class BooleanToVisibilityTypeConverterTest
{
    /// <summary>
    /// Tests that FromType and ToType properties are correctly set.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TypeProperties_AreCorrectlySet()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        await Assert.That(converter.FromType).IsEqualTo(typeof(object));
        await Assert.That(converter.ToType).IsEqualTo(typeof(object));
    }

    /// <summary>
    /// Tests that GetAffinityForObjects returns correct affinity.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObjects_ReturnsCorrectAffinity()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var affinity = converter.GetAffinityForObjects();

        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Tests that TryConvert converts true to Visibility.Visible.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_ConvertsTrueToVisible()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(true, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Visible);
    }

    /// <summary>
    /// Tests that TryConvert converts false to Visibility.Collapsed (default).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_ConvertsFalseToCollapsed()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(false, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Collapsed);
    }

    /// <summary>
    /// Tests that TryConvert with Inverse hint inverts the conversion.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithInverseHint_InvertsConversion()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(true, BooleanToVisibilityHint.Inverse, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Collapsed);
    }

    /// <summary>
    /// Tests that TryConvert with UseHidden hint uses Hidden instead of Collapsed (WPF-specific).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithUseHiddenHint_UsesHidden()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(false, BooleanToVisibilityHint.UseHidden, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Hidden);
    }

    /// <summary>
    /// Tests that TryConvert with both Inverse and UseHidden hints works correctly (WPF-specific).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_WithInverseAndUseHidden_UsesHidden()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(
            true,
            BooleanToVisibilityHint.Inverse | BooleanToVisibilityHint.UseHidden,
            out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Hidden);
    }

    /// <summary>
    /// Tests that TryConvert converts Visibility.Visible to false (XOR logic).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_ConvertsVisibleToFalse()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(Visibility.Visible, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(false);
    }

    /// <summary>
    /// Tests that TryConvert converts Visibility.Collapsed to true (XOR logic).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_ConvertsCollapsedToTrue()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(Visibility.Collapsed, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(true);
    }

    /// <summary>
    /// Tests that TryConvert converts Visibility.Hidden to true (WPF-specific, XOR logic).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_ConvertsHiddenToTrue()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(Visibility.Hidden, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(true);
    }

    /// <summary>
    /// Tests that TryConvert with Inverse hint on Visibility to bool inverts the result.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_VisibilityToBoolWithInverse_InvertsResult()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var successVisible = converter.TryConvertTyped(Visibility.Visible, BooleanToVisibilityHint.Inverse, out var resultVisible);
        var successCollapsed = converter.TryConvertTyped(Visibility.Collapsed, BooleanToVisibilityHint.Inverse, out var resultCollapsed);

        await Assert.That(successVisible).IsTrue();
        await Assert.That(successCollapsed).IsTrue();
        await Assert.That(resultVisible).IsEqualTo(true);
        await Assert.That(resultCollapsed).IsEqualTo(false);
    }

    /// <summary>
    /// Tests that TryConvert with non-Visibility, non-bool input defaults to Visible.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_NonVisibilityInput_DefaultsToVisible()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped("some string", null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Visible);
    }

    /// <summary>
    /// Tests that TryConvert with null input defaults to Visible.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_NullInput_DefaultsToVisible()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(null, null, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Visible);
    }

    /// <summary>
    /// Tests that TryConvert with UseHidden hint on true stays Visible.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task TryConvert_TrueWithUseHidden_StaysVisible()
    {
        var converter = new BooleanToVisibilityTypeConverter();

        var success = converter.TryConvertTyped(true, BooleanToVisibilityHint.UseHidden, out var result);

        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Visible);
    }
}
